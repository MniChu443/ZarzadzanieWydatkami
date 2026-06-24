using System;

namespace ZarzadzanieWydatkami.Shared
{
    public class WydatekDto
    {
        public int Id { get; set; }
        public string Tytul { get; set; } = string.Empty;
        public decimal Kwota { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Kategoria { get; set; } = string.Empty;
    }
}