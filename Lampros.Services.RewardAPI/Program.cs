
using Lampros.Services.RewardAPI.Data;
using Lampros.Services.RewardAPI.Extension;
using Lampros.Services.RewardAPI.Messaging;
using Lampros.Services.RewardAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RewardDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RewardDbConnection"));
});

var optionsBuilder = new DbContextOptionsBuilder<RewardDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("RewardDbConnection"));
builder.Services.AddSingleton(new RewardService(optionsBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseAzureServiceBusConsumer();
app.Run();
