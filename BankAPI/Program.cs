using BankAPI.DAL;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;
using BankAPI.Utilities;
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://example.com",
                                              "http://www.contoso.com");
                      });
});

// Add services to the container.

ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bank System API Documentation",
        Version = "v1",
        Description = "API for bank system that will add new user with PIN, balance inquiry, withdraw cash, deposit and transfer money to different user",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ayisi Oliveros",
            Email = "ayisioliveros@gmail.com",
            Url = new Uri("https://github.com/ayisioliveros05/BankAPI")
        }
    });
});
builder.Services.AddDbContext<BankAPIDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("BankAPIConnection")));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        var prefix = string.IsNullOrEmpty(x.RoutePrefix) ? "." : "..";
        x.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "Banking API doc");
    });
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
