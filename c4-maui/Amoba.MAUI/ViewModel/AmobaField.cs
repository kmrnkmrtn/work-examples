using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amoba.MAUI.ViewModel
{
    public class AmobaField : ViewModelBase
    {
        private string _sign = string.Empty;
        private string _color = "Black";
        private bool _enabled = true;
        public int X { get; set; }

        public int Y { get; set; }
        public string Text
        {
            get { return _sign; }
            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }



        public Tuple<int, int>? Coord { get; set; }

        public int LinearIndex { get; set; }

        public DelegateCommand? StepCommand { get; set; }

        public bool IsEnabled 
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
