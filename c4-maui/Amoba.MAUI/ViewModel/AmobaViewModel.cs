using PotyogosAmoba.Model.Model;
using PotyogosAmoba.Model.Persistance;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static PotyogosAmoba.Model.Persistance.PotyogosAmobaTable;

namespace Amoba.MAUI.ViewModel
{
    public class AmobaViewModel : ViewModelBase
    {
        #region Fields
        private PotyogosAmobaModel _model;
        private TableSizeViewModel _tableSizeViewModel;

        #endregion

        #region Properties


        public PotyogosAmobaModel Model
        {
            get => _model;
            private set => _model = value;
        }

        public int Size
        {
            get => _model.Table.Size;
            private set
            {
                OnPropertyChanged();
            }
            
        }

        public TableSizeViewModel TableSizeViewModel
        {
            get => _tableSizeViewModel;
            set
            {
                _tableSizeViewModel = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TableSizeViewModel> TableSizes
        {
            get;
            set;
        }
        public RowDefinitionCollection RowSize
        {
            get => new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(GridLength.Star), _model.Table.Size).ToArray());
        }

        public ColumnDefinitionCollection ColSize
        {
            get => new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), _model.Table.Size).ToArray());
        }
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }

        public DelegateCommand ExitCommand { get; private set; }

        public ObservableCollection<AmobaField> Fields { get; set; }

        public string PlayerXTime { get { return Model.Table.XTime.ToString("g"); } }
        public string PlayerOTime { get { return Model.Table.OTime.ToString("g"); } }




        #endregion

        #region Events

        public event EventHandler<int>? NewGame;
        public event EventHandler<GameWonEventArgs>? GameWon;
        //public event EventHandler<PotyogosAmobaEventArgs>? GameOver;
        public event EventHandler? LoadGameEvent;
        public event EventHandler? SaveGameEvent;
        public event EventHandler? ExitGame;
        #endregion

        #region Constructor
        public AmobaViewModel(PotyogosAmobaModel model)
        {
            _tableSizeViewModel = new TableSizeViewModel();
            _model = model;
            _model.GameAdvanced += OnGameAdvanced;
            _model.GameWon += OnGameWon;
            _model.GameOver += OnGameOver;
            OnPropertyChanged(nameof(RowSize));
            OnPropertyChanged(nameof(ColSize));
            OnPropertyChanged(nameof(PlayerXTime));
            OnPropertyChanged(nameof(PlayerOTime));
            


            Fields = new ObservableCollection<AmobaField>();

            NewGameCommand = new DelegateCommand(param => OnNewGame(Int32.Parse(param!.ToString()!)));
            PauseCommand = new DelegateCommand(param => OnPause());
            LoadCommand = new DelegateCommand(param => OnLoadGame());
            SaveCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());


            TableSizes = new ObservableCollection<TableSizeViewModel>
            {
                new TableSizeViewModel { TableSize = 10 },
                new TableSizeViewModel { TableSize = 20 },
                new TableSizeViewModel { TableSize = 30 }
            };
            TableSizeViewModel = TableSizes[1];

            CreateTable();
            
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            if (_model.IsGameOver) { return; }
            SaveGameEvent?.Invoke(this, EventArgs.Empty);

        }

        public void LoadGame()
        {
            if (_model != null)
            {
                RemoveTable();

            }
            CreateTable();
            RefreshTable();
        }

        private void OnLoadGame()
        {

            LoadGameEvent?.Invoke(this, EventArgs.Empty);
                   

        }

        private void OnPause()
        {
            if (_model.IsGameOver) return;
            if (_model.Table.Size < 10) return;
            if(_model.Table.XTime == TimeSpan.Zero && _model.Table.OTime == TimeSpan.Zero) return;
            _model.Pause();
            
            
        }

        private void DisableFields()
        {
            foreach (var field in Fields)
            {
                field.IsEnabled = false;
            }
        }


        private void OnGameOver(object? sender, PotyogosAmobaEventArgs e)
        {
            DisableFields();
           // GameOver?.Invoke(this,e);
        }

        private void OnGameWon(object? sender, GameWonEventArgs e)
        {

            DisableFields();    
            foreach (var field in Fields)
            {

                if (field.Coord == null) continue;
                for(int i=0; i<4; ++i)
                {
                    
                    if (field.Coord.Item1 == e.WinX[i] && field.Coord.Item2 == e.WinY[i]) 
                    {
                        field.Color = "Green";
                    }

                }

            }
            

            GameWon?.Invoke(this, e);
        }

        private void OnGameAdvanced(object? sender, PotyogosAmobaEventArgs e)
        {
            OnPropertyChanged(nameof(PlayerXTime));
            OnPropertyChanged(nameof(PlayerOTime));
        }


        #endregion

        #region Private methods

        private void RefreshTable()
        {
            foreach (AmobaField field in Fields)
            {
                if (field.Coord == null) continue;
                field.Text = _model.GameTable[field.Coord.Item1, field.Coord.Item2] == Player.NoPlayer ? String.Empty : _model.GameTable[field.Coord.Item1, field.Coord.Item2] == Player.PlayerX ? "X" : "O";
            }
            OnPropertyChanged(nameof(PlayerOTime));
            OnPropertyChanged(nameof(PlayerXTime));
        }

        public void CreateTable()
        {
            for (int i = 0; i < _model.Table.Size; i++)
            {
                for (int j = 0; j < _model.Table.Size; j++)
                {
                    Fields.Add(new AmobaField()
                    {
                        Color = "Purple",
                        Text = string.Empty,
                        Coord = new Tuple<int,int> (i,j),
                        X = i,
                        Y = j,
                        LinearIndex = i * 100 + j,
                        StepCommand = new DelegateCommand(param => Step(((Tuple<int, int>)param!)))
                });
                }
            }
            OnPropertyChanged(nameof(RowSize));
            OnPropertyChanged(nameof(ColSize));
            OnPropertyChanged(nameof(PlayerOTime));
            OnPropertyChanged(nameof(PlayerXTime));
            RefreshTable();
            
        }

        private void Step(Tuple<int,int> tuple)
        {
            if(_model.IsPaused)
            {
                OnPause();
            }
            Model.StepGame(tuple.Item1, tuple.Item2);
            
            RefreshTable();
        }
        #endregion

        #region Event methods

        private void OnNewGame(int size)
        {
            RemoveTable();
            NewGame?.Invoke(this, size);
            _model.GameOver += OnGameOver;
            CreateTable();
        }

        public void RemoveTable()
        {
            Fields.Clear();
            RefreshTable();
        }

      
        #endregion
    }
}
