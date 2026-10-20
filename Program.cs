using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// 1. เพิ่ม Services พื้นฐาน
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. ตั้งค่า รองรับ Reverse Proxy (Render.com HTTPS)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 3. Supabase Config
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

// 4. เปิดใช้งาน Forwarded Headers (ต้องวางก่อน Swagger และ Routing)
app.UseForwardedHeaders();

// 5. เปิดใช้งาน Swagger บน Production และตั้งให้แสดงที่ Root URL
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ErpBackend v1");
    c.RoutePrefix = string.Empty; // เปิดเว็บมาเจอหน้า Swagger UI ทันที
});

app.UseAuthorization();
app.MapControllers();

app.Run();
