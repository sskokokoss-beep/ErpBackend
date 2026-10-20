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

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // กำหนด Path เต็มให้ตรงกับที่ไฟล์ swagger.json ถูกสร้างขึ้นจริง
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ErpBackend v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

app.Run();
