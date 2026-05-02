using auto_t_portal_main_BE.Data;
using auto_t_portal_main_BE.Helpers;
using auto_t_portal_main_BE.Services.Implementations;
using auto_t_portal_main_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Thêm vào phần DI (sau builder.Services.AddScoped<IAuthService, AuthService>())
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<VnPayHelper>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Thêm vào phần app pipeline (sau app.UseHttpsRedirection())
app.UseStaticFiles();
app.Run();