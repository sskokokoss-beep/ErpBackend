var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// กำหนดรายละเอียด OpenApiDoc ให้ชัดเจน
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ErpBackend API",
        Version = "v1"
    });
});

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

// เปิดใช้งาน Swagger เสมอ
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // ใช้ Relative path เพื่อป้องกันปัญหาเกี่ยวกับ Domain บน Render
    c.SwaggerEndpoint("./v1/swagger.json", "ErpBackend v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

app.Run();
