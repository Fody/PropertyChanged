﻿using System.ComponentModel;

namespace SmokeTest
{
    public class BaseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string text1)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text1));
        }
    }
}
