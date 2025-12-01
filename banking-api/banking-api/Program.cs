using Banking.Application.Interfaces; // Namespace de la interfaz
using Banking.Application.Mappings;
using Banking.Application.Services;   // Namespace de la implementación
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Persistence.Contexts;
using Banking.Infrastructure.Persistence.Repositories;
using banking_api.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Dbcontext

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add Repositories

builder.Services.AddScoped<ITransactionBankRepository, TransactionBankRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Auto Mapper

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Add Services Application

builder.Services.AddScoped<ITransactionBankService, TransactionBankService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPdfService, PdfService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. DEFINIR LA POLÍTICA CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // <--- La URL de tu Angular
              .AllowAnyMethod()                     // Permite GET, POST, PUT, DELETE
              .AllowAnyHeader();                    // Permite Content-Type, Authorization, etc.
    });
});

// 1. REGISTRAR EL MANEJADOR (Antes de builder.Build())
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
