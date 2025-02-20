var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", builder =>
    {
        builder.WithOrigins("https://localhost:5001") // Adjust to your client URL
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Use configuration values anywhere via dependency injection
string secretEmail = builder.Configuration["email"];
string secretPassword = builder.Configuration["password"];
string secretSmtpServer = builder.Configuration["smtp-server"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



// In the middleware section:
app.UseCors("AllowBlazorClient");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
