﻿using System.ComponentModel;

public class TestClassExclude : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}