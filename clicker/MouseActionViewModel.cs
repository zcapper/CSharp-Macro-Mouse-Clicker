using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

namespace Clicker
{
    public class MouseActionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Action> Actions { get; set; }
        private Settings Settings;
        private RuntimeSettings RuntimeSettings;

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

        /// <summary>
        /// Whether a mouse macro is currently running.
        /// </summary>
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

        /// <summary>
        /// Flag used to stop running thread simulating mouse macro.
        /// </summary>
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

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRequestStop"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MouseActionViewModel(Settings Settings, RuntimeSettings RuntimeSettings)
        {
            Actions = new ObservableCollection<Action>();

            IsRunning = false;

            Actions.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRunOrClear"));
            };

            this.Settings = Settings;
            this.RuntimeSettings = RuntimeSettings;
        }

        public void RunActions()
        {
            IsRunning = true;

            Thread t = new Thread(() =>
            {
                do
                {
                    RuntimeSettings.Step = 0;
                    while (RuntimeSettings.Step < Actions.Count && !IsStopRequested)
                    {
                        Action ma = Actions[RuntimeSettings.Step];
                        if (!RuntimeSettings.Pause)
                        {                            
                            if (ma.Type == ActionType.Click) ma.RunClick();
                            if (ma.Type == ActionType.Type) ma.RunType();

                            if (!IsStopRequested) { ma.RunCooldown(ref _isStopRequested); }
                            RuntimeSettings.Step++;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    }

                    App.Current?.Dispatcher.Invoke(() =>
                    {
                        IsRunning = false;

                        IsStopRequested = false;
                    });

                } while (Settings.Autorun);
            });

            t.Start();
        }
    }
}
