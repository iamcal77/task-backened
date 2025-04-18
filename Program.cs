using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyWebAPI.Data;
using MyWebAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");




// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SmartOrderDb")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",               // Local development
                "https://noones-03tu.onrender.com"     // Frontend URL without trailing slash
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourapp",
            ValidAudience = "yourapp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hdfdydgww8w8eywvyewpkspowqksowdwudgwydgfwydfwydewddv"))
        };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"));


// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

});

// Register services
builder.Services.AddSingleton<HistoryService>();
builder.Services.AddSingleton<QueryService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Enable controller-based routes

app.Run();
