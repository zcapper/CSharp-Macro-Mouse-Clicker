using System;

namespace Clicker
{
    /// <summary>
    /// Represents a mouse click at a given position on the screen. 
    /// </summary>
    public partial class MouseAction
    {
        /// <summary>
        /// Horizonal position of mouse cursor.
        /// </summary>
        public int XPosition { get; set; }

        /// <summary>
        /// Vertical position of mouse cursor.
        /// </summary>
        public int YPosition { get; set; }

        /// <summary>
        /// Time to wait after performing click.
        /// </summary>
        public TimeSpan Cooldown { get; set; }

        /// <summary>
        /// The mouse button to use when simulating a click.
        /// </summary>
        public ClickType Button { get; set; }


        /// <summary>
        /// Creates a new MouseAction with given position and default cooldown time.
        /// </summary>
        /// <param name="xPosition">Horizonal position of mouse cursor.</param>
        /// <param name="yPosition">Vertical position of mouse cursor.</param>
        public MouseAction(int xPosition, int yPosition)
          : this(xPosition, yPosition, new TimeSpan(0, 0, 10))
        {
        }


        /// <summary>
        /// Creates a new MouseAction with given position and cooldown time.
        /// </summary>
        /// <param name="xPosition">Horizonal position of mouse cursor</param>
        /// <param name="yPosition">Vertical position of mouse cursor</param>
        /// <param name="cooldown">Time to wait after performing action</param>
        public MouseAction(int xPosition, int yPosition, TimeSpan cooldown, int? button = 0)
        {
            XPosition = xPosition;
            YPosition = yPosition;
            Cooldown = cooldown;
            switch (button)
            {
                case 1:
                    Button = ClickType.MiddleClick;
                    break;
                case 2:
                    Button = ClickType.RightClick;
                    break;
                default:
                    Button = ClickType.LeftClick;
                    break;
            }
        }


        /// <summary>
        /// Simulates this mouse action.
        /// </summary>
        public void RunClick()
        {
            MouseClickHelper.SetCursorPos(XPosition, YPosition);

            MouseClickHelper.Click(XPosition, YPosition, Button, true);

            System.Threading.Thread.Sleep(1);

            MouseClickHelper.Click(XPosition, YPosition, Button, false);
        }


        /// <summary>
        /// Pauses thread for duration of Cooldown.
        /// </summary>
        public void RunCooldown()
        {
            System.Threading.Thread.Sleep(Cooldown);
        }


        /// <summary>
        /// Pauses thread for duration of Cooldown or until the specified bool is true.
        /// </summary>
        /// <param name="earlyStopSignal">The timer will monitor this value and stop when it equals true.</param>
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
