using System;

namespace Clicker
{
  /// <summary>
  /// Represents a mouse click at a given position on the screen. 
  /// </summary>
  public partial class MouseAction
  {
    public int XPos { get; set; }

    public int YPos { get; set; }

    public TimeSpan Cooldown { get; set; }


    /// <summary>
    /// Creates a new MouseAction with given position and default cooldown time.
    /// </summary>
    /// <param name="xpos">Horizonal position of mouse cursor</param>
    /// <param name="ypos">Vertical position of mouse cursor</param>
    public MouseAction(int xpos, int ypos) 
      : this(xpos, ypos, new TimeSpan(0, 0, 10))
    {
    }


    /// <summary>
    /// Creates a new MouseAction with given position and cooldown time.
    /// </summary>
    /// <param name="xpos">Horizonal position of mouse cursor</param>
    /// <param name="ypos">Vertical position of mouse cursor</param>
    /// <param name="cooldown">Time to wait after performing action</param>
    public MouseAction(int xpos, int ypos, TimeSpan cooldown)
    {
      XPos = xpos;
      YPos = ypos;
      Cooldown = cooldown;
    }


    /// <summary>
    /// Plays mouse action.
    /// </summary>
    public void Click()
    {
      MouseClickHelper.SetCursorPos(XPos, YPos);

      MouseClickHelper.mouse_event(
        MouseClickHelper.MOUSEEVENTF_LEFTDOWN,
        XPos,
        YPos,
        0,
        0);

      System.Threading.Thread.Sleep(1);

      MouseClickHelper.mouse_event(
        MouseClickHelper.MOUSEEVENTF_LEFTUP,
        XPos,
        YPos,
        0,
        0);
    }


    /// <summary>
    /// Pauses thread for duration of Cooldown.
    /// </summary>
    public void RunCooldown()
    {
      System.Threading.Thread.Sleep(Cooldown);
    }


    /// <summary>
    /// Pauses thread for duration of Cooldown or until signal is true.
    /// </summary>
    /// <param name="earlyStopSignal"></param>
    public void RunCooldown(ref bool earlyStopSignal)
    {
      TimeSpan t = Cooldown;
      const int MAX_SLEEP_INTERVAL = 1000;

      while (t.TotalMilliseconds > 0 && !earlyStopSignal)
      {
        if (t.TotalMilliseconds > MAX_SLEEP_INTERVAL)
        {
          System.Threading.Thread.Sleep(MAX_SLEEP_INTERVAL);
          t -= new TimeSpan(MAX_SLEEP_INTERVAL * TimeSpan.TicksPerMillisecond);
        }
        else
        {
          System.Threading.Thread.Sleep(t);
          return;
        }
      }
    }
  }
}
