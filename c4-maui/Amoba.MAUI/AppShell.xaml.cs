
using PotyogosAmoba.Model.Model;
using PotyogosAmoba.Model.Persistance;
using Amoba.MAUI.ViewModel;
using static PotyogosAmoba.Model.Persistance.PotyogosAmobaTable;

using Amoba.MAUI.View;

namespace Amoba.MAUI
{
    public partial class AppShell : Shell 
    {

        private PotyogosAmobaModel _model;
        private AmobaViewModel _view;

        private readonly IStore _store;
        private readonly StoredGameBrowserModel _storedModel;
        private readonly StoredGameBrowserViewModel _storedViewModel;
        public AppShell(IStore amobaStore, PotyogosAmobaModel model,
        AmobaViewModel view)
        {
            InitializeComponent();


            _store = amobaStore;
            _model = model;
            _view = view;

            _storedModel = new StoredGameBrowserModel(_store);
            _storedViewModel = new StoredGameBrowserViewModel(_storedModel);
            _storedViewModel.GameLoading += AppOnGameLoading;
            _storedViewModel.GameSaving += AppOnGameSaving;

            _view.ExitGame += AppOnExitGame;
            _view.NewGame += OnNewGame;
            _model.GameWon += AppOnGameWon;
            _model.GameOver += AppOnGameOver;
            _view.LoadGameEvent += AppOnLoadGame;
            _view.SaveGameEvent += AppOnSaveGame;




        }

        private async void AppOnGameOver(object? sender, PotyogosAmobaEventArgs e)
        {
            await DisplayAlert("Amoba játék",
                    "Döntetlen Játék!",

                    "OK");
        }

        private async void AppOnGameSaving(object? sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync();

            try
            {
                await _model.SaveGameAsync(FileSystem.AppDataDirectory + @"/" + e.Name);
                await DisplayAlert("Amoba játék", "Sikeres mentés.", "OK");
            }
            catch
            {
                await DisplayAlert("Amoba játék", "Sikertelen mentés.", "OK");
            }
        }

        private async void AppOnGameLoading(object? sender, StoredGameEventArgs e)
        {
            await Navigation.PopAsync();

            try
            {
                await _model.LoadGameAsync(FileSystem.AppDataDirectory + @"/" + e.Name);
                _view.RemoveTable();
                _view.CreateTable();
                await Navigation.PopAsync();
                await DisplayAlert("Amoba játék ", " Sikeres betöltés.", "OK");

            }
            catch
            {
                await DisplayAlert("Amoba játék", "Sikertelen betöltés.", "OK");
            }
        }

        private async void AppOnSaveGame(object? sender, EventArgs e)
        {
            await _storedModel.UpdateAsync();
            await Navigation.PushAsync(new SaveGamePage
            {
                BindingContext = _storedViewModel
            });
        }

        private async void AppOnLoadGame(object? sender, EventArgs e)
        {
            
            await _storedModel.UpdateAsync();
            await Navigation.PushAsync(new LoadGamePage
            {
                BindingContext = _storedViewModel
            });
        }

        private async void AppOnExitGame(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage
            {
                BindingContext = _view
            });
        }

        private async void AppOnGameWon(object? sender, GameWonEventArgs e)
        {
            if (e.Player == Player.PlayerO)
            {
                await DisplayAlert("Amoba játék",
                    "Az 'O' játékos nyert!",

                    "OK");
            }
            else if (e.Player == Player.PlayerX)
            {
                await DisplayAlert("Amoba játék",
                   "Az 'X' játékos nyert!",

                   "OK");
            }
        }

        private void OnNewGame(object? sender, int size)
        {

            _model.GameWon -= AppOnGameWon;
            _model.NewGame(size);
            _model.GameWon += AppOnGameWon;
            
            



        }
    }
}