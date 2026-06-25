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

        public MainViewModel()
        {
            // Zabezpieczenie certyfikatu SSL dla lokalnego serwera
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            // WAŻNE: Upewnij się, że port 7265 zgadza się z Twoim serwerem!
            _httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7265/") };
        }

        // ==========================================
        // TUTAJ JEST METODA, KTÓREJ CI BRAKOWAŁO:
        // ==========================================
        [RelayCommand]
        private async Task PobierzWydatkiAsync()
        {
            try
            {
                var dane = await _httpClient.GetFromJsonAsync<List<WydatekDto>>("api/Wydatki");
                if (dane != null)
                {
                    Wydatki = new ObservableCollection<WydatekDto>(dane);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd połączenia z serwerem: {ex.Message}");
            }
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
                }
            }
        }

        [RelayCommand]
        private async Task ZapiszWydatekAsync()
        {
            if (WybranyWydatek != null)
            {
                await _httpClient.PutAsJsonAsync($"api/Wydatki/{WybranyWydatek.Id}", WybranyWydatek);
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