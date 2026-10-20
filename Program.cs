var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Supabase Config
var supabaseUrl = builder.Configuration["Supabase:Url"] 
                  ?? builder.Configuration["Supabase__Url"];
var supabaseKey = builder.Configuration["Supabase:Key"] 
                  ?? builder.Configuration["Supabase__Key"];

if (!string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(supabaseKey))
{
    builder.Services.AddScoped<Supabase.Client>(_ => 
        new Supabase.Client(supabaseUrl, supabaseKey));
}

var app = builder.Build();

// Configure HTTP pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ErpBackend v1");
    c.RoutePrefix = string.Empty; // กำหนดให้เปิดหน้าแรก (Root URL) แล้วเจอ Swagger เลย
});

app.UseAuthorization();
app.MapControllers();

app.Run();
