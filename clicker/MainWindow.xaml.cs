using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        public Settings Settings { get; set; }
        private RuntimeSettings RuntimeSettings;
        MouseHookListener M;

        public MainWindow()
        {
            InitializeComponent();

            Settings = new Settings();
            RuntimeSettings = new RuntimeSettings();

            if (File.Exists(Settings.SettingsFilename))
            {
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Settings.SettingsFilename));
                AutorunCheckbox.IsChecked = Settings.Autorun;
            }

            MouseActions = new MouseActionViewModel(Settings, RuntimeSettings);

            RuntimeSettings.StepChanged += (object sender, EventArgs args) => {
                if (RuntimeSettings.Step < MouseActions.Actions.Count)
                {
                    App.Current?.Dispatcher.Invoke(() =>
                    {
                        this.DataGrid.SelectedIndex = RuntimeSettings.Step;
                        this.DataGrid.ScrollIntoView(this.DataGrid.Items.GetItemAt(RuntimeSettings.Step));
                        this.TextBox.Text ="" + RuntimeSettings.Step;
                    });
                }
            };

            if (File.Exists(Settings.AutosaveFilename))
            {
                var autoload = File.ReadAllText(Settings.AutosaveFilename);
                var actions = JsonConvert.DeserializeObject<JArray>(autoload);
                foreach (var action in actions)
                {
                    var xPosition = (int)action["XPosition"];
                    var yPosition = (int)action["YPosition"];
                    var cooldown = (TimeSpan)action["Cooldown"];
                    var type = (ActionType)(int)action["Type"];
                    var clickType = (ClickType)(int)action["Button"];
                    var text = (string)action["Text"];
                    MouseActions.Actions.Add(new Action(MouseActions.Actions.Count,xPosition, yPosition, cooldown, clickType, type, text));
                }

                if (Settings.Autorun) MouseActions.RunActions();
            }

            MouseKeyboardActivityMonitor.WinApi.GlobalHooker h = new MouseKeyboardActivityMonitor.WinApi.GlobalHooker();

            M = new MouseHookListener(h);

            M.MouseClick += (object s, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                {
                    if (MouseActions.IsRunning == false)
                    {
                        MouseActions.Actions.Add(new Action(MouseActions.Actions.Count,
                            e.X,
                            e.Y,
                            Settings.DefaultCooldown
                        ));                        
                    }
                    else {
                        if (RuntimeSettings.Pause)
                        {
                            RuntimeSettings.Step += 1;
                            MouseActions.Actions.Insert(RuntimeSettings.Step, new Action(MouseActions.Actions.Count,
                                e.X,
                                e.Y,
                                Settings.DefaultCooldown
                            ));
                        }
                    }
                }
            };

            M.MouseDoubleClick += (object s, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    RuntimeSettings.Pause = !RuntimeSettings.Pause;
                    PausedCheckbox.IsChecked = RuntimeSettings.Pause;
                }
            };

           App.Current.Exit += (object sender, ExitEventArgs e) =>
            {
                MouseActions.IsStopRequested = true;
                M?.Stop();

                WriteToJsonFile<Collection<Action>>(Settings.AutosaveFilename, MouseActions.Actions);
                WriteToJsonFile<Settings>(Settings.SettingsFilename, Settings);
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

        private void ClearButton(object sender, RoutedEventArgs e) { MouseActions.Actions.Clear(); TextBox.Text = ""; }

        private void RunButton(object sender, RoutedEventArgs e) { RuntimeSettings.Step = 0; MouseActions.RunActions(); }

        private void ResetButton(object sender, RoutedEventArgs e) { 
            RuntimeSettings.Step = 0; 
            this.DataGrid.SelectedIndex = RuntimeSettings.Step;
            this.DataGrid.ScrollIntoView(this.DataGrid.Items.GetItemAt(RuntimeSettings.Step));
        }

        private void StopButton(object sender, RoutedEventArgs e) { MouseActions.IsStopRequested = true; }

        private void GoToStepButton(object sender, RoutedEventArgs e) { 
            try {
                RuntimeSettings.Step = Int32.Parse(TextBox.Text);
                if (!MouseActions.IsRunning) {
                    MouseActions.RunActions();
                }
            } catch (Exception exception) {
                TextBox.Text = exception.Message;
            }
        }

        private void AutorunCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Settings.Autorun = AutorunCheckbox.IsChecked ?? false;
        }
        private void PausedCheckbox_Click(object sender, RoutedEventArgs e)
        {
            RuntimeSettings.Pause = PausedCheckbox.IsChecked ?? false;
        }

        private void TextBox_KeyPress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "[0-9]"))
                e.Handled = false;
            else e.Handled = true;
        }

    }
}
