using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RaceDay.Api.Data;
using RaceDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RaceDay API", Version = "v1", Description = "REST API for race events, enrolments and results." });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml"), true);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header, Description = "Enter a valid JWT token." });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
});
builder.Services.AddDbContext<RaceDayContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("RaceDay")));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
if (key.Length < 32) throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
    o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidIssuer = jwt["Issuer"], ValidateAudience = true, ValidAudience = jwt["Audience"], ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30), RoleClaimType = System.Security.Claims.ClaimTypes.Role };
});
builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { name = "RaceDay API", swagger = "/swagger" })).WithTags("Health").WithSummary("API health check");
app.Run();
public partial class Program { }
