using PotyogosAmoba.Model.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PotyogosAmoba.Model.Persistance.PotyogosAmobaTable;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

namespace PotyogosAmoba.Model.Model
{

    public class PotyogosAmobaModel : IDisposable
    {
        #region Fields
        private readonly IPotyogosAmobaDataAccess _dataAccess = null!;
        private PotyogosAmobaTable _table = null!;
        private bool _paused = true;
        private bool _gameOver = false;
        private System.Timers.Timer _timer = new System.Timers.Timer();
        #endregion


        #region Properties
        public PotyogosAmobaTable Table { get { return _table; } private set { _table = value; } }
        public Player[,] GameTable { get { return _table.Table; } private set { } }

        public bool IsGameOver { get { return _gameOver; } private set { } }

        public bool IsPaused { get { return _paused; } private set { } }
        public System.Timers.Timer Timer { get { return _timer; } private set { _timer = value; } }


        #endregion
        #region Events

        public event EventHandler<PotyogosAmobaEventArgs>? FieldChanged = null!;
        public event EventHandler<PotyogosAmobaEventArgs>? GameAdvanced = null!;
        public event EventHandler<GameWonEventArgs>? GameWon = null!;
        public event EventHandler<PotyogosAmobaEventArgs>? GameOver = null!;
        #endregion
        #region Constructors
        public PotyogosAmobaModel(Int32 size, IPotyogosAmobaDataAccess dataAccess, ISynchronizeInvoke? sync = null)
        {
            //_table = new PotyogosAmobaTable(size);
            _dataAccess = dataAccess;
            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Tick;
            NewGame(size);
            _timer.SynchronizingObject = sync;
        }
        #endregion

        #region Public game methods

        public void Pause()
        {
            _paused = !_paused;
            // timert itt állítani
            if (_paused) { _timer.Stop(); }
            else { _timer.Start(); }

        }

        public void StartTimer()
        {
            if (!_paused) return;
            _timer.Start();
            _paused = false;           
        }

        public void StopTimer()
        {
            if (_paused) return;
            _timer.Stop();
            _paused = true;
        }

        public void NewGame(Int32 size)
        {
            _gameOver = false;
            _paused = true;
            _table = new PotyogosAmobaTable(size);

            _timer.Start();
        }

        private void Timer_Tick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            AdvanceTime();
        }

        public void StepGame(Int32 x, Int32 y)
        {
            if (!CanPlace(x, y)) return;
            if (x < 0 || x >= GameTable.GetLength(0)) // ellenőrizzük a tartományt
                throw new ArgumentOutOfRangeException(nameof(x), "Bad column index.");
            if (y < 0 || y >= GameTable.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(y), "Bad row index.");

            if (!CanPlace(x, y))
                throw new InvalidOperationException("Can not place sign at the given field!");
            _paused = false;



            Int32[] WhereToPlace = PlaceSign(x, y);


            OnFieldChanged(WhereToPlace[0], WhereToPlace[1], Table.CurrentPlayer); // jelezzük egy eseménykiváltással, hogy változott a mező

            Table.CurrentPlayer = Table.CurrentPlayer == Player.PlayerO ? Player.PlayerX : Player.PlayerO;


            CheckGame();
        }



        public async Task LoadGameAsync(String path)
        {

            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            _table = await _dataAccess.LoadAsync(path);
            _paused = true;


        }
        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, _table);
        }
        #endregion

        #region Private game methods
        private bool CanPlace(Int32 x, Int32 y)
        {
            Int32 i = GameTable.GetLength(0) - 1;
            while (GameTable[i, y] != Player.NoPlayer && i > 0)
            {
                i--;
            }
            if (i >= 0 && GameTable[i, y] == Player.NoPlayer) return true;
            return false;
        }
        private Int32[] PlaceSign(Int32 x, Int32 y)
        {
            Int32[] result = { -1, -1 };
            Debug.WriteLine(x + " " + y);

            Int32 i = GameTable.GetLength(0) - 1;
            while (GameTable[i, y] != Player.NoPlayer && i > 0)
            {
                i--;
            }

            GameTable[i, y] = Table.CurrentPlayer;
            ++Table.StepNumber;
            Debug.WriteLine(i + " " + y);
            result[0] = i;
            result[1] = y;
            return result;

        }

        private void CheckGame()
        {
            Player won = Player.NoPlayer;
            Int32[] xs = new Int32[4];
            Int32[] ys = new Int32[4];

            for (int i = 0; i < GameTable.GetLength(0); ++i) // sorok ellenőrzése
                for (int j = 0; j < GameTable.GetLength(1) - 3; ++j)
                {
                    if (GameTable[i, j] != Player.NoPlayer && GameTable[i, j] == GameTable[i, j + 1] &&
                        GameTable[i, j] == GameTable[i, j + 2] && GameTable[i, j] == GameTable[i, j + 3])
                    {
                        won = GameTable[i, j];
                        for (int k = 0; k < 4; k++)
                        {
                            xs[k] = i;
                            ys[k] = j + k;
                        }


                    }
                }

            for (int j = 0; j < GameTable.GetLength(1); ++j) // oszlopok ellenőrzése
                for (int i = 0; i < GameTable.GetLength(0) - 3; ++i)
                {
                    if (GameTable[i, j] != Player.NoPlayer && GameTable[i, j] == GameTable[i + 1, j] &&
                        GameTable[i, j] == GameTable[i + 2, j] && GameTable[i, j] == GameTable[i + 3, j])
                    {
                        won = GameTable[i, j];
                        for (int k = 0; k < 4; k++)
                        {
                            xs[k] = i + k;
                            ys[k] = j;
                        }


                    }
                }

            for (int i = 0; i < GameTable.GetLength(0) - 3; ++i) // Jobbra-lefele átlók ellenőrzése
                for (int j = 0; j < GameTable.GetLength(1) - 3; ++j)
                {
                    if (GameTable[i, j] != Player.NoPlayer &&
                        GameTable[i, j] == GameTable[i + 1, j + 1] && GameTable[i, j] == GameTable[i + 2, j + 2]
                         && GameTable[i, j] == GameTable[i + 3, j + 3])
                    {
                        won = GameTable[i, j];
                        for (int k = 0; k < 4; k++)
                        {
                            xs[k] = i + k;
                            ys[k] = j + k;
                        }
                    }
                }

            for (int i = 2; i < GameTable.GetLength(0); ++i) // Jobbra-felfele átlók ellenőrzése
                for (int j = 0; j < GameTable.GetLength(1) - 3; ++j)
                {
                    if (GameTable[i, j] != Player.NoPlayer &&
                        GameTable[i, j] == GameTable[i - 1, j + 1] && GameTable[i, j] == GameTable[i - 2, j + 2]
                        && GameTable[i, j] == GameTable[i - 3, j + 3])
                    {
                        won = GameTable[i, j];
                        for (int k = 0; k < 4; k++)
                        {
                            xs[k] = i - k;
                            ys[k] = j + k;
                        }
                    }

                }

            if (won != Player.NoPlayer) // ha valaki győzött
            {
                Debug.WriteLine("Won"); OnGameWon(won, xs, ys); // esemény kiváltása
            }
            else if (Table.StepNumber == GameTable.Length) // döntetlen játék
            {
                Debug.WriteLine("Draw"); OnGameOver(); // esemény kiváltása
            }
        }
        #endregion

        #region Event triggers
        private void OnFieldChanged(Int32 x, Int32 y, Player player)
        {
            FieldChanged?.Invoke(this, new PotyogosAmobaEventArgs(x, y, player));
        }

        public void AdvanceTime()
        {

            if (_paused) return;
            if (Table.CurrentPlayer == Player.PlayerO)
            {
                Table.OTime += new TimeSpan(0, 0, 1);

            }
            else
            {
                Table.XTime += new TimeSpan(0, 0, 1);

            }

            OnGameAdvanced();


        }

        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new PotyogosAmobaEventArgs(1, 1, Player.NoPlayer));
        }

        private void OnGameWon(Player player, Int32[] xs, Int32[] ys)
        {
            _gameOver = true;
            _paused = true;
            GameWon?.Invoke(this, new GameWonEventArgs(player, xs, ys));
        }

        private void OnGameOver()
        {
            _gameOver = true;
            _paused = true;
            GameOver?.Invoke(this, new PotyogosAmobaEventArgs(1, 1, Player.NoPlayer));
        }

        public void Dispose()
        {
            if(Timer != null)
            {
                Timer.Dispose();
                GC.SuppressFinalize(this);

            }
        }
        #endregion

    }
}
