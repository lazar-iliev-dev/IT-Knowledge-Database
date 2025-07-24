using KnowledgeApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database + Dependency Injection
builder.Services.AddDbContext<KnowledgeContext>(options =>
    options.UseSqlite("Data Source=knowledge.db"));

// CORS for React-Frontend
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()
));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// DB erzeugen, wenn nicht vorhanden
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KnowledgeContext>();
    
    // Achtung: Nur für Debug – Production ➜ db.Migrate() automatisch mit Logging kapseln
    db.Database.Migrate();
}

// Middleware
// Configure the HTTP request pipeline.
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(); // → läuft unter /swagger
app.UseAuthorization();
app.MapControllers();

app.Run();
