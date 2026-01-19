using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipments.Api.Data;
using Shipments.Api.Models;
using Shipments.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShipmentsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IShipmentService, ShipmentService>();

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ShipmentsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
