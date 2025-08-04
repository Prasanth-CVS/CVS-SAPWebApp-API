using CvsServiceLayer.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<HanaDbHelper>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HttpClient for SapService
builder.Services.AddHttpClient<SapService>(client =>
{
    client.BaseAddress = new Uri("https://10.10.40.105:50000/b1s/v1/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Disable SSL cert validation (only for dev)
});

// ✅ Register SapService itself in DI
//builder.Services.AddScoped<SapService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

