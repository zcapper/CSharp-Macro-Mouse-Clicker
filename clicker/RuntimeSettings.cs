using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Clicker
{
    
    public class RuntimeSettings
    {
        private int _step;
        public bool Pause { get; set; }
        public int Step {
            get
            {
                return _step;                
            }
            set
            {
                _step = value;
                OnStepChanged(EventArgs.Empty);
            }
        }

        public delegate void StepChangedEventHandler(object source, EventArgs args);
        public event EventHandler StepChanged;

        public RuntimeSettings()
        {
            Pause = false;
            _step = 0;
        }

        protected virtual void OnStepChanged(EventArgs e)
        {
            StepChanged?.Invoke(this, e);
        }
    }
}
