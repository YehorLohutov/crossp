using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Users;
using Projects;
using Files;
using Ads;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
            builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
        );
});

IConfigurationSection tokenOptions = configuration.GetSection("TokenOptions");
string validIssuer = tokenOptions.GetSection("Issuer").Value;
string validAudience = tokenOptions.GetSection("Audience").Value;
string key = tokenOptions.GetSection("Key").Value;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = validIssuer,
            ValidateAudience = true,
            ValidAudience = validAudience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuerSigningKey = true,
        };
    });

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddUsersServices(configuration);
builder.Services.AddProjectsServices(configuration);
builder.Services.AddFilesServices(configuration);
builder.Services.AddAdsServices(configuration);

builder.Services.AddApiVersioning(o =>
{
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
}
);

builder.Services.AddMediator(config =>
{    
    config.AddUsersMediatorConsumers();
    config.AddProjectsMediatorConsumers();
    config.AddFilesMediatorConsumers();
    config.AddAdsMediatorConsumers();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(
    //c => c.SwaggerEndpoint("/swagger/1.0/swagger.json", "API 1.0")

);
//}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();