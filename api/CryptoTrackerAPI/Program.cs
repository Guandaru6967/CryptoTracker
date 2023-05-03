using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using  CryptoTrackerAPI.Data;
using Microsoft.AspNetCore.ResponseCompression;
using  CryptoTrackerAPI.Controllers;
using Microsoft.Owin.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CryptoTrackerDB>(options=>options.UseInMemoryDatabase("customerdb"));

builder.Services.AddControllers();
builder.Services.AddSignalRCore().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<MarketService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>

    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Appsettings:Token").Value))
            , ValidateIssuer = false,
            ValidateAudience = false
        };
        }
    );*/
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
               
               options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddCors(options =>
{
               options.AddPolicy("AllowAllOrigins",
                   builder =>
                   {
                                  builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                   });
});
builder.Services.AddResponseCompression(options=>options.MimeTypes=ResponseCompressionDefaults.MimeTypes.Concat(new []{"application/octet-stream"}));
builder.Services.AddAuthentication();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");

app.MapHub<MarketHub>("/markets",options=>options.Transports=Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);
app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
