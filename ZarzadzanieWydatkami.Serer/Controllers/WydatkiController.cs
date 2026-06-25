using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZarzadzanieWydatkami.Shared;

namespace ZarzadzanieWydatkami.Server.Controllers
{
    // Te atrybuty mówią, że to jest API i ścieżka to np. localhost/wydatki
    [ApiController]
    [Route("api/[controller]")]
    public class WydatkiController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Wstrzykujemy naszą bazę danych przez konstruktor
        public WydatkiController(AppDbContext context)
        {
            _context = context;
        }

        // Metoda GET - pobiera wszystkie wydatki z bazy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WydatekDto>>> PobierzWydatki()
        {
            return await _context.Wydatki.ToListAsync();
        }

        // Metoda POST - dodaje nowy wydatek do bazy
        [HttpPost]
        public async Task<ActionResult<WydatekDto>> DodajWydatek(WydatekDto wydatek)
        {
            _context.Wydatki.Add(wydatek);
            await _context.SaveChangesAsync(); // Asynchroniczny zapis do bazy
            return Ok(wydatek);
        }

        // Metoda PUT - aktualizuje istniejący wydatek w bazie
        [HttpPut("{id}")]
        public async Task<IActionResult> AktualizujWydatek(int id, WydatekDto wydatek)
        {
            if (id != wydatek.Id)
            {
                return BadRequest();
            }

            _context.Entry(wydatek).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}