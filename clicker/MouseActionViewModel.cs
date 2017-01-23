using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

namespace Clicker
{
  public class MouseActionViewModel : INotifyPropertyChanged
  {
    public ObservableCollection<MouseAction> Actions { get; set; }

    public bool CanRunOrClear
    {
      get
      {
        return Actions.Count != 0 && !IsRunning;
      }
    }

    public bool CanRequestStop
    {
      get
      {
        return IsRunning && !IsStopRequested;
      }
    }

    bool _isRunning = false;

    public bool IsRunning
    {
      get
      {
        return _isRunning;
      }
      private set
      {
        if (_isRunning != value)
        {
          _isRunning = value;

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRunOrClear"));

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRequestStop"));

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
        }
      }
    }

    bool _isStopRequested = false;

    public bool IsStopRequested
    {
      get
      {
        return _isStopRequested;
      }
      set
      {
        if (_isStopRequested != value)
        {
          _isStopRequested = value;

          StopRequested?.Invoke(this, new EventArgs());

          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRequestStop"));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    event EventHandler StopRequested;

    public MouseActionViewModel()
    {
      Actions = new ObservableCollection<MouseAction>();

      IsRunning = false;

      Actions.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRunOrClear"));
      };
    }


    public void RunActions()
    {
      IsRunning = true;

      Thread t = new Thread(() =>
      {
        while (!IsStopRequested)
        {
          foreach (MouseAction ma in Actions)
          {
            if (!IsStopRequested) { ma.Click(); }

            if (!IsStopRequested) { ma.RunCooldown(ref _isStopRequested); }
          }
        }

        App.Current.Dispatcher.Invoke(() => 
        {
          IsRunning = false;

          IsStopRequested = false;
        });
      });

      t.Start();
    }
  }
}
