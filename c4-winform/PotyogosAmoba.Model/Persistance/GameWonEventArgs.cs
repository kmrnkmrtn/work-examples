using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PotyogosAmoba.Model.Persistance.PotyogosAmobaTable;


namespace PotyogosAmoba.Model.Persistance
{ 
    public class GameWonEventArgs : EventArgs
        {
            //WinnerFieldCoordinates
            private Int32[] _winX;
            private Int32[] _winO;
            private Player _player;

            public Int32[] WinX { get { return _winX; } }
            public Int32[] WinY { get { return _winO; } }
            public Player Player { get { return _player; } } 
        public GameWonEventArgs(Player player, Int32[] xs, Int32[] ys) 
            {
                this._player = player;
                _winX = xs;
                _winO = ys;
            }

        }
}

