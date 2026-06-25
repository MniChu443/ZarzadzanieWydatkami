using Microsoft.EntityFrameworkCore;
using ZarzadzanieWydatkami.Server;
using ZarzadzanieWydatkami.Shared;

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja bazy danych SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Włączenie kontrolerów i Swaggera
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Generowanie Danych 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Automatycznie sprawdza i aplikuje migracje (tworzy plik bazy, jeśli go nie ma)
    context.Database.Migrate();

    // Jeśli w bazie danych nie ma jeszcze ŻADNYCH wydatków (baza jest czysta)
    if (!context.Wydatki.Any())
    {
        // Dodajemy gotowy zestaw danych startowych
        context.Wydatki.AddRange(
            new WydatekDto { Tytul = "Zakupy spożywcze ", Kwota = 154.20m, Kategoria = "Jedzenie", Data = DateTime.Now.AddDays(-2) },
            new WydatekDto { Tytul = "Tankowanie paliwa", Kwota = 230.00m, Kategoria = "Transport", Data = DateTime.Now.AddDays(-1) },
            new WydatekDto { Tytul = "Bilet do kina", Kwota = 38.00m, Kategoria = "Rozrywka", Data = DateTime.Now },
            new WydatekDto { Tytul = "Czynsz za mieszkanie", Kwota = 450.00m, Kategoria = "Dom", Data = DateTime.Now.AddDays(-5) }
        );

        // Zapisujemy te gotowe dane na stałe do bazy SQLite
        context.SaveChanges();
    }
}

// 3. Konfiguracja Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();