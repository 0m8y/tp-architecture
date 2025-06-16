using GestionHotel.Infrastructure.Data;
using GestionHotel.Infrastructure.Repositories;
using GestionHotel.Apis.Endpoints.Booking;
using GestionHotel.Apis.Endpoints.Clients;
using GestionHotel.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;
using GestionHotel.Application.UseCases.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SampleInjectionInterface, SampleInjectionImplementation>();
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<CreateClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapBookingsEndpoints();
app.MapClientsEndpoints();
app.Run();
