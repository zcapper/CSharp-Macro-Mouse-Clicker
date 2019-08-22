using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using MouseKeyboardActivityMonitor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MouseActionViewModel MouseActions { get; set; }
        public string autosaveFileName = "autosave.json";

        MouseHookListener M;

        public MainWindow()
        {
            InitializeComponent();

            MouseActions = new MouseActionViewModel();

            if (File.Exists(autosaveFileName))
            {
                var autoload = File.ReadAllText(autosaveFileName);
                var actions = JsonConvert.DeserializeObject<JArray>(autoload);
                foreach (var action in actions)
                {
                    var xPosition = (int)action["XPosition"];
                    var yPosition = (int)action["YPosition"];
                    var cooldown = (TimeSpan)action["Cooldown"];
                    var button = (int)action["Button"];
                    MouseActions.Actions.Add(new MouseAction(xPosition, yPosition, cooldown, button));
                }
            }

            MouseKeyboardActivityMonitor.WinApi.GlobalHooker h = new MouseKeyboardActivityMonitor.WinApi.GlobalHooker();
            
            M = new MouseHookListener(h);

            M.MouseClick += (object s, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Middle && MouseActions.IsRunning == false)
                {
                    MouseActions.Actions.Add(new MouseAction(
                e.X,
                e.Y
                ));
                }
            };

            App.Current.Exit += (object sender, ExitEventArgs e) =>
            {
                MouseActions.IsStopRequested = true;
                M?.Stop();

                WriteToJsonFile<Collection<MouseAction>>(autosaveFileName, MouseActions.Actions);
            };

            M.Start();
        }

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
        
        private void ClearButton(object sender, RoutedEventArgs e) { MouseActions.Actions.Clear(); }

        private void RunButton(object sender, RoutedEventArgs e) { MouseActions.RunActions(); }

        private void StopButton(object sender, RoutedEventArgs e) { MouseActions.IsStopRequested = true; }
    }
}
