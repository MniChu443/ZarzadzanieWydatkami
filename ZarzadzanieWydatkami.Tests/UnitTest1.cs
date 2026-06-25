using Xunit;
using ZarzadzanieWydatkami.Shared;

namespace ZarzadzanieWydatkami.Tests
{
    public class OdwzorowanieTestow
    {
        [Fact]
        public void Test1_ObliczSume_DlaPustejListy_PowinienZwrocicZero()
        {
            // Arrange (Przygotowanie)
            var lista = new List<WydatekDto>();

            // Act (Działanie)
            var wynik = KalkulatorWydatkow.ObliczSume(lista);

            // Assert (Sprawdzenie)
            Assert.Equal(0, wynik);
        }

        [Fact]
        public void Test2_ObliczSume_DlaKilkuWydatkow_PowinienZwrocicPoprawnaSume()
        {
            // Arrange
            var lista = new List<WydatekDto>
            {
                new WydatekDto { Kwota = 50 },
                new WydatekDto { Kwota = 120.50m }
            };

            // Act
            var wynik = KalkulatorWydatkow.ObliczSume(lista);

            // Assert
            Assert.Equal(170.50m, wynik);
        }

        [Fact]
        public void Test3_CzyWydatekPrzekraczaBudzet_PowinienZwrocicTrue_GdyPrzekracza()
        {
            // Arrange
            var wydatek = new WydatekDto { Kwota = 500 };
            decimal limit = 400;

            // Act
            var wynik = KalkulatorWydatkow.CzyWydatekPrzekraczaBudzet(wydatek, limit);

            // Assert
            Assert.True(wynik);
        }
    }
}