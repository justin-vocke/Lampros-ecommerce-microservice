using Lampros.Services.EmailAPI.Data;
using Lampros.Services.EmailAPI.Extension;
using Lampros.Services.EmailAPI.Messaging;
using Lampros.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EmailDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmailDbConnection"));
});

var optionsBuilder = new DbContextOptionsBuilder<EmailDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("EmailDbConnection"));
builder.Services.AddSingleton(new EmailService(optionsBuilder.Options));
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
