using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. เพิ่ม Services พื้นฐาน
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. ตั้งค่า Swagger Generator ให้ระบุข้อมูลและเวอร์ชันของ API ชัดเจน (แก้ปัญหาไฟล์ JSON ไม่สมบูรณ์)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ErpBackend API", 
        Version = "v1" 
    });
});

// 3. ตั้งค่ารองรับ Reverse Proxy ของ Render.com (แก้ปัญหา HTTPS -> HTTP)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 4. ตั้งค่า Supabase Config
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

// 5. เปิดใช้งาน Forwarded Headers (ต้องวางไว้ก่อน Routing หรือ Swagger เสมอ)
app.UseForwardedHeaders();

// 6. เปิดใช้งาน Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // ใช้ Absolute Path ที่ขึ้นต้นด้วย / เพื่อป้องกันปัญหา Path ซ้อนกันบน Render
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ErpBackend v1");
    
    // พิมพ์ /swagger ท้าย URL เพื่อเข้าดู UI (หากต้องการให้อยู่หน้าแรกเลย ให้เปลี่ยนเป็น string.Empty)
    c.RoutePrefix = "swagger"; 
});

app.UseAuthorization();
app.MapControllers();

app.Run();
