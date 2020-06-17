using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker
{
    public class Settings
    {
        public bool Autorun { get; set; }
        public string AutosaveFilename { get; set; }
        public string SettingsFilename { get; set; }
        public TimeSpan DefaultCooldown { get; set; }
        

        public Settings()
        {
            AutosaveFilename = "autosave.json";
            SettingsFilename = "settings.json";
            DefaultCooldown = TimeSpan.FromSeconds(2);
        }
    }
}
