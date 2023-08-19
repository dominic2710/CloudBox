using CloudBox.Api.Entities;
using CloudBox.Api.Functions;
using CloudBox.Api.Functions.Photo;
using CloudBox.Api.Functions.User;
using CloudBox.Api.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue; // Allow large file uploads
});

builder.Services.AddDbContext<CloudBoxContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserFunction, UserFunction>();
builder.Services.AddScoped<IPhotoFunction, PhotoFunction>();
builder.Services.AddScoped<UserOperator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.WithOrigins("http://localhost:4200");
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

//app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
