
using PotyogosAmoba.Model.Model;
using PotyogosAmoba.Model.Persistance;
using Amoba.MAUI.ViewModel;
using Amoba.MAUI.Persistance;


namespace Amoba.MAUI
{
    public partial class App : Application, IDisposable
    {
        private const string SuspendedGameSavePath = "SuspendedGame";
        private AppShell _appShell;
        private PotyogosAmobaModel _model;
        private AmobaViewModel _view;
        private IStore _amobaStore;
        public App()
        {
            InitializeComponent();

            _amobaStore = new AmobaStore();
            _model = new PotyogosAmobaModel(10, new PotyogosAmobaFileDataAccess());
            _view = new AmobaViewModel(_model);

            _appShell = new AppShell(_amobaStore, _model, _view)
            {
                BindingContext = _view
            };
            MainPage = _appShell;
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Created += (s, e) =>
            {
                _model = new PotyogosAmobaModel(30, new PotyogosAmobaFileDataAccess());
                
            };

            window.Activated += (s, e) =>
            {
                if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                    return;

                Task.Run(async () =>
                {
                    try
                    {
                        await _model.LoadGameAsync(SuspendedGameSavePath);

                    }
                    catch
                    {
                    }
                });
            };

            window.Stopped += (s, e) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _model.SaveGameAsync(SuspendedGameSavePath);
                    }
                    catch
                    {
                    }
                });
            };

            return window;
        }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.Dispose();
                GC.SuppressFinalize(this);

            }
        }
    }
}