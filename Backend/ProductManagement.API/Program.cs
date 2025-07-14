using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Implementations;
using ProductManagement.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}
app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
