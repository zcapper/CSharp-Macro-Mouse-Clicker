using System.Windows;
using MouseKeyboardActivityMonitor;

namespace Clicker
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MouseActionViewModel MouseActions { get; set; }

    MouseHookListener M;


    public MainWindow()
    {
      InitializeComponent();

      MouseActions = new MouseActionViewModel();

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
        MouseActions.IsStopRequested = false;
        M?.Stop();
      };

      M.Start();
    }

    private void ClearButton(object sender, RoutedEventArgs e) { MouseActions.Actions.Clear(); }

    private void RunButton(object sender, RoutedEventArgs e) { MouseActions.RunActions(); }

    private void StopButton(object sender, RoutedEventArgs e) { MouseActions.IsStopRequested = true; }
  }
}
