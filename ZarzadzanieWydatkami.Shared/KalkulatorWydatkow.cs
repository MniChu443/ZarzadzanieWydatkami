namespace ZarzadzanieWydatkami.Shared
{
    public static class KalkulatorWydatkow
    {
        // Metoda sumuje kwoty wszystkich wydatków
        public static decimal ObliczSume(List<WydatekDto> wydatki)
        {
            if (wydatki == null) return 0;
            return wydatki.Sum(w => w.Kwota);
        }

        // Metoda sprawdza, czy wydatek jest poprawny (cena nie może być ujemna)
        public static bool CzyWydatekPrzekraczaBudzet(WydatekDto wydatek, decimal limit)
        {
            if (wydatek == null) return false;
            return wydatek.Kwota > limit;
        }
    }
}