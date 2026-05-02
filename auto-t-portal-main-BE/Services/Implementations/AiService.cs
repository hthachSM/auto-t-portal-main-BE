using System.Text;
using System.Text.Json;
using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Models.DTOs.AI;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace auto_t_portal_main_BE.Services.Implementations;

public class AiService(AppDbContext db, IConfiguration config, IHttpClientFactory httpFactory) : IAiService
{
    private const string ClaudeModel = "claude-sonnet-4-20250514";
    private const string AnthropicApiUrl = "https://api.anthropic.com/v1/messages";

    // ─── Semantic Search ────────────────────────────────────────────────────

    public async Task<SemanticSearchResponse> SemanticSearchAsync(SemanticSearchRequest request)
    {
        // Lấy toàn bộ xe available
        var cars = await db.Cars
            .Include(c => c.Images)
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        if (!cars.Any())
            return new SemanticSearchResponse
            {
                Answer = "Hiện tại không có xe nào trong hệ thống.",
                RelatedCars = []
            };

        // Build context cho Claude
        var carContext = string.Join("\n", cars.Select(c =>
            $"[ID:{c.Id}] {c.Make} {c.Model} {c.Year} - " +
            $"Giá: {c.Price:N0}đ - " +
            $"Tình trạng: {(c.Condition == Models.Entities.CarCondition.New ? "Mới" : "Đã qua sử dụng")} - " +
            $"Km: {c.Mileage:N0} - " +
            $"Nhiên liệu: {c.FuelType ?? "N/A"} - " +
            $"Hộp số: {c.Transmission ?? "N/A"} - " +
            $"Số chỗ: {c.SeatingCapacity} - " +
            $"Màu: {c.Color ?? "N/A"} - " +
            $"Trạng thái: {c.Status}"));

        var systemPrompt = """
            Bạn là chuyên gia tư vấn xe hơi của Auto T - hệ thống thương mại điện tử ô tô.
            Nhiệm vụ của bạn là:
            1. Phân tích yêu cầu của khách hàng
            2. Tìm ra các xe phù hợp nhất từ danh sách có sẵn
            3. Tư vấn ngắn gọn, thân thiện bằng tiếng Việt
            4. Trả về JSON với format sau (KHÔNG có markdown, KHÔNG có backtick):
            {
              "answer": "Câu trả lời tư vấn cho khách",
              "carIds": ["id1", "id2", "id3"]
            }
            Chỉ chọn tối đa 5 xe phù hợp nhất. Nếu không có xe phù hợp, trả carIds = [].
            """;

        var userMessage = $"""
            Danh sách xe hiện có:
            {carContext}

            Yêu cầu của khách hàng: {request.Query}
            """;

        var aiResponse = await CallClaudeAsync(systemPrompt, userMessage);

        // Parse response
        try
        {
            var parsed = JsonSerializer.Deserialize<JsonElement>(aiResponse);
            var answer = parsed.GetProperty("answer").GetString() ?? aiResponse;
            var carIds = parsed.GetProperty("carIds")
                .EnumerateArray()
                .Select(e => e.GetString())
                .Where(s => s != null)
                .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToList();

            var matchedCars = cars
                .Where(c => carIds.Contains(c.Id))
                .Select((c, i) => new CarSearchResult
                {
                    Id = c.Id,
                    Title = $"{c.Make} {c.Model} {c.Year}",
                    Price = c.Price,
                    Condition = c.Condition.ToString(),
                    Status = c.Status.ToString(),
                    PrimaryImage = c.Images.FirstOrDefault(img => img.IsPrimary)?.Url
                        ?? c.Images.FirstOrDefault()?.Url,
                    FuelType = c.FuelType,
                    Transmission = c.Transmission,
                    SeatingCapacity = c.SeatingCapacity,
                    SimilarityScore = 1.0 - (i * 0.1) // Điểm giảm dần theo thứ tự AI sắp xếp
                })
                .ToList();

            return new SemanticSearchResponse
            {
                Answer = answer,
                RelatedCars = matchedCars
            };
        }
        catch
        {
            return new SemanticSearchResponse
            {
                Answer = aiResponse,
                RelatedCars = []
            };
        }
    }

    // ─── Trade-in Pricing ────────────────────────────────────────────────────

    public async Task<TradeInResponse> EstimateTradeInAsync(TradeInRequest request)
    {
        // Lấy dữ liệu xe cùng hãng/model trong DB để tham khảo
        var similarCars = await db.Cars
            .Where(c => c.Make == request.Make &&
                        c.Model == request.Model &&
                        !c.IsDeleted)
            .Select(c => new { c.Year, c.Price, c.Mileage, c.Condition })
            .ToListAsync();

        var marketContext = similarCars.Any()
            ? string.Join("\n", similarCars.Select(c =>
                $"- Năm {c.Year}, {c.Mileage:N0}km, " +
                $"{(c.Condition == Models.Entities.CarCondition.New ? "Mới" : "Cũ")}: " +
                $"{c.Price:N0}đ"))
            : "Chưa có dữ liệu thị trường cho dòng xe này.";

        var systemPrompt = """
            Bạn là chuyên gia định giá xe cũ tại Việt Nam.
            Hãy ước tính giá xe dựa trên thông tin cung cấp.
            Trả về JSON (KHÔNG có markdown, KHÔNG có backtick):
            {
              "estimatedMinPrice": 500000000,
              "estimatedMaxPrice": 600000000,
              "suggestedPrice": 550000000,
              "explanation": "Giải thích ngắn gọn bằng tiếng Việt",
              "factors": ["Yếu tố 1", "Yếu tố 2", "Yếu tố 3"]
            }
            Tất cả giá trị là số nguyên (VND), không có dấu phẩy hay chữ.
            """;

        var userMessage = $"""
            Thông tin xe cần định giá:
            - Hãng: {request.Make}
            - Dòng xe: {request.Model}
            - Năm sản xuất: {request.Year}
            - Km đã đi: {request.Mileage:N0}
            - Tình trạng: {request.Condition ?? "Đã qua sử dụng"}
            - Màu sắc: {request.Color ?? "Không rõ"}

            Dữ liệu thị trường tham khảo từ hệ thống:
            {marketContext}
            """;

        var aiResponse = await CallClaudeAsync(systemPrompt, userMessage);

        try
        {
            var parsed = JsonSerializer.Deserialize<JsonElement>(aiResponse);
            return new TradeInResponse
            {
                EstimatedMinPrice = parsed.GetProperty("estimatedMinPrice").GetDecimal(),
                EstimatedMaxPrice = parsed.GetProperty("estimatedMaxPrice").GetDecimal(),
                SuggestedPrice = parsed.GetProperty("suggestedPrice").GetDecimal(),
                Explanation = parsed.GetProperty("explanation").GetString() ?? "",
                Factors = parsed.GetProperty("factors")
                    .EnumerateArray()
                    .Select(e => e.GetString() ?? "")
                    .ToList()
            };
        }
        catch
        {
            return new TradeInResponse
            {
                EstimatedMinPrice = 0,
                EstimatedMaxPrice = 0,
                SuggestedPrice = 0,
                Explanation = "Không thể ước tính giá lúc này. Vui lòng liên hệ showroom.",
                Factors = []
            };
        }
    }

    // ─── Generate Embeddings (batch job) ────────────────────────────────────

    public async Task GenerateEmbeddingsForAllCarsAsync()
    {
        var cars = await db.Cars
            .Where(c => c.EmbeddingJson == null && !c.IsDeleted)
            .ToListAsync();

        foreach (var car in cars)
        {
            var text = $"{car.Make} {car.Model} {car.Year} " +
                       $"{car.FuelType} {car.Transmission} " +
                       $"{car.SeatingCapacity} chỗ {car.Color} " +
                       $"{car.Description}";

            // Lưu text representation đơn giản thay vì vector thực
            car.EmbeddingJson = JsonSerializer.Serialize(new { text, updatedAt = DateTime.UtcNow });
            car.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    // ─── Private: Gọi Claude API ─────────────────────────────────────────────

    private async Task<string> CallClaudeAsync(string systemPrompt, string userMessage)
    {
        var apiKey = config["Claude:ApiKey"]
            ?? throw new InvalidOperationException("Claude API Key chưa được cấu hình.");

        var client = httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var body = new
        {
            model = ClaudeModel,
            max_tokens = 1024,
            system = systemPrompt,
            messages = new[]
            {
                new { role = "user", content = userMessage }
            }
        };

        var response = await client.PostAsync(
            AnthropicApiUrl,
            new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Claude API lỗi: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);
        return result
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? "";
    }
}
