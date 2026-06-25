using Microsoft.EntityFrameworkCore;
using ZarzadzanieWydatkami.Shared;

namespace ZarzadzanieWydatkami.Server
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Ta właściwość reprezentuje naszą tabelę w bazie danych
        public DbSet<WydatekDto> Wydatki { get; set; }
    }
}