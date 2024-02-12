using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PotyogosAmoba.Model.Persistance
{
    public class PotyogosAmobaTable
    {
        public enum Player { NoPlayer, PlayerO, PlayerX }
        
        #region Fields

        private Int32 _size; // tábla méret
        private Player[,] _fieldValues = null!; // mezőértékek
        private Player _currentPlayer; // következő játékos
        private TimeSpan _playerXtime = new TimeSpan(0,0,0);
        private TimeSpan _playerOtime = new TimeSpan(0,0,0);
        private Int32 _stepNumber = 0;
        

        public PotyogosAmobaTable(Int32 size)
        {
            _size = size;
            _currentPlayer = Player.PlayerO;
            GenerateTable();
        }

        #endregion

        #region Properties

        public Int32 Size { get { return _size; }  set { _size = value; } }
        public Int32 StepNumber { get { return _stepNumber; }  set { _stepNumber = value; } }
        public Player[,] Table { get { return _fieldValues; }   set { _fieldValues = value;  }  }

        public Player CurrentPlayer {  get { return _currentPlayer; }  set {  _currentPlayer = value; } }

        public TimeSpan XTime { get { return _playerXtime; }  set { _playerXtime = value; } }
        public TimeSpan OTime { get { return _playerOtime; }  set { _playerOtime = value;} }
        #endregion
        #region public methods
        public void SetSign(Int32 x, Int32 y, Int32 sign)
        {
            if (x < 0 || x >= Table.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(x), "X is out of range!");
            if (y < 0 || y >= Table.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(y), "O is out of range!");
            if (sign < 0 || sign > 2)
                throw new ArgumentOutOfRangeException(nameof(sign), "There is no such sign. Either X,O or noplayer is accepted. ");
            if (!CanPlace(x, y))
                throw new InvalidOperationException("Can not place sign!");
            Table[x, y] = (Player)sign;
        }
        #endregion

        #region Private methods
        private bool CanPlace(Int32 x, Int32 y)
        {
            Int32 i = Table.GetLength(0) - 1;
            while (Table[i, y] != Player.NoPlayer && i > 0)
            {
                i--;
            }
            if (i >= 0 && Table[i, y] == Player.NoPlayer) return true;
            return false;
        }
        private void GenerateTable()
        {
            Table = new Player[_size, _size];
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Table[i, j] = Player.NoPlayer;
                }
            }
        }
        
        #endregion
    }
}
