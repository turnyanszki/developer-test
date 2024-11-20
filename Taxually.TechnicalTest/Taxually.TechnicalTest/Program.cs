using Serilog;
using Taxually.TechnicalTest.Exceptions;
using Taxually.TechnicalTest.External;
using Taxually.TechnicalTest.Services;
using Taxually.TechnicalTest.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidators();
builder.Services.AddTaxuallyServices();
builder.Services.AddExternalConnectors();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Host.UseSerilog((ctx, lc) =>

    lc
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")

);

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

app.UseExceptionHandler();

app.Run();
