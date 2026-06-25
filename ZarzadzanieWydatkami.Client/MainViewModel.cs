using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZarzadzanieWydatkami.Shared;

namespace ZarzadzanieWydatkami.Client
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private ObservableCollection<WydatekDto> _wydatki = new();

        [ObservableProperty]
        private WydatekDto? _wybranyWydatek;

        [ObservableProperty]
        private ObservableCollection<string> _kategorie = new(new[] { "Jedzenie", "Transport", "Rozrywka", "Dom", "Inne" });

        [ObservableProperty]
        private string _nowaKategoriaTekst = "";

        [ObservableProperty]
        private decimal _budzetMiesieczny = 3000.00m;

        [ObservableProperty]
        private decimal _sumaWydatkow;

        [ObservableProperty]
        private decimal _pozostaloZBudzetu;

        private WydatekDto? _ostatnioUsunietyWydatek;

        [ObservableProperty]
        private bool _czyMoznaCofnac;

        public MainViewModel()
        {
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true };
            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7265/") };
        }

        // === NOWOŚĆ: Automatyczne przeliczanie przy zmianie kwoty budżetu ===
        // CommunityToolkit.Mvvm sam wykryje tę metodę i uruchomi ją, gdy zmienisz wpis w polu budżetu
        partial void OnBudzetMiesiecznyChanged(decimal value)
        {
            PrzeliczPodsumowanie();
        }

        private void PrzeliczPodsumowanie()
        {
            var wydatkiWMiesiacu = Wydatki.Where(w => w.Data.Month == DateTime.Now.Month && w.Data.Year == DateTime.Now.Year).ToList();
            SumaWydatkow = KalkulatorWydatkow.ObliczSume(wydatkiWMiesiacu);
            PozostaloZBudzetu = BudzetMiesieczny - SumaWydatkow;
        }

        [RelayCommand]
        private async Task PobierzWydatkiAsync()
        {
            try
            {
                var dane = await _httpClient.GetFromJsonAsync<List<WydatekDto>>("api/Wydatki");
                if (dane != null)
                {
                    Wydatki = new ObservableCollection<WydatekDto>(dane);
                    PrzeliczPodsumowanie();
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show($"Błąd: {ex.Message}"); }
        }

        [RelayCommand]
        private async Task DodajWydatekAsync()
        {
            var nowyWydatek = new WydatekDto { Tytul = "Nowy wydatek", Kwota = 0, Kategoria = "Inne", Data = DateTime.Now };
            var odpowiedz = await _httpClient.PostAsJsonAsync("api/Wydatki", nowyWydatek);
            if (odpowiedz.IsSuccessStatusCode)
            {
                var utworzony = await odpowiedz.Content.ReadFromJsonAsync<WydatekDto>();
                if (utworzony != null)
                {
                    Wydatki.Add(utworzony);
                    PrzeliczPodsumowanie();
                }
            }
        }

        // === TUTAJ NAPRAWILIŚMY BŁĄD OPÓŹNIENIA ===
        [RelayCommand]
        private async Task ZapiszWydatekAsync()
        {
            if (WybranyWydatek != null)
            {
                // 1. NAJPIERW natychmiast przeliczamy lokalnie na ekranie.
                // Użytkownik widzi zmianę w tej samej milisekundzie, w której kliknął Enter!
                PrzeliczPodsumowanie();

                // 2. POTEM (w tle) serwer zapisuje to sobie w bazie danych SQLite.
                await _httpClient.PutAsJsonAsync($"api/Wydatki/{WybranyWydatek.Id}", WybranyWydatek);
            }
        }

        [RelayCommand]
        private async Task UsunWydatekAsync()
        {
            if (WybranyWydatek != null)
            {
                _ostatnioUsunietyWydatek = new WydatekDto
                {
                    Tytul = WybranyWydatek.Tytul,
                    Kwota = WybranyWydatek.Kwota,
                    Kategoria = WybranyWydatek.Kategoria,
                    Data = WybranyWydatek.Data
                };
                CzyMoznaCofnac = true;

                // Przy usuwaniu też robimy natychmiastowe przeliczenie na ekranie
                var wydatekDoUsuniecia = WybranyWydatek;
                Wydatki.Remove(wydatekDoUsuniecia);
                PrzeliczPodsumowanie();

                // Serwer mieli to w tle
                await _httpClient.DeleteAsync($"api/Wydatki/{wydatekDoUsuniecia.Id}");
            }
        }

        [RelayCommand]
        private async Task CofnijUsuniecieAsync()
        {
            if (_ostatnioUsunietyWydatek != null)
            {
                var odpowiedz = await _httpClient.PostAsJsonAsync("api/Wydatki", _ostatnioUsunietyWydatek);
                if (odpowiedz.IsSuccessStatusCode)
                {
                    var odzyskany = await odpowiedz.Content.ReadFromJsonAsync<WydatekDto>();
                    if (odzyskany != null)
                    {
                        Wydatki.Add(odzyskany);
                        PrzeliczPodsumowanie();
                    }
                }

                _ostatnioUsunietyWydatek = null;
                CzyMoznaCofnac = false;
            }
        }

        [RelayCommand]
        private void DodajKategorie()
        {
            if (!string.IsNullOrWhiteSpace(NowaKategoriaTekst) && !Kategorie.Contains(NowaKategoriaTekst))
            {
                Kategorie.Add(NowaKategoriaTekst);
                NowaKategoriaTekst = string.Empty;
            }
        }
    }
}