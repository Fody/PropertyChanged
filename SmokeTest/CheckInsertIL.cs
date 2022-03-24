using System;
using System.ComponentModel;

namespace SmokeTest
{
    public class CheckInsertIL : INotifyPropertyChanged
    {
        private string _stringProperty = "Hello the World";
        public string StringProperty 
        { 
            get => _stringProperty;
            set => _stringProperty = "Bonjour le monde";
        }

        private int _intBeforeValue = 0;
        public int CheckIntBeforeValue => _intBeforeValue;
        private int _intAfterValue = 0;
        public int CheckIntAfterValue => _intAfterValue;

        private int _intProperty = 256;
        public int IntProperty 
        {
            get => _intProperty;
            set
            {
                _intBeforeValue = int.MaxValue;
                _intProperty = value;
                _intAfterValue = -_intProperty;
            }
        }

        private BoxBase _box;
        public BoxBase Box
        {
            get => _box;
            set
            {
                _box = value;
                _box.A = 9;
                _box.B = 10;
                _box.C = 11;
            }
        }

        public CheckInsertIL()
        {
            _box = new BoxInterne();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}