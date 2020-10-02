namespace Clicker
{
  public partial class Action
  {
    /// <summary>
    /// Helper functions for simulating mouse clicks through the Windows API.
    /// </summary>
    static class MouseClickHelper
    {
      [System.Runtime.InteropServices.DllImport("user32.dll")]
      static public extern bool SetCursorPos(int x, int y);

      [System.Runtime.InteropServices.DllImport("user32.dll")]
      private static extern void Mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

      public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
      public const int MOUSEEVENTF_LEFTUP = 0x0004;
      public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
      public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
      public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
      public const int MOUSEEVENTF_RIGHTUP = 0x0010;


      /// <summary>
      /// Simulates a mouse button press or release at the specified position.
      /// </summary>
      /// <param name="xPos">Horizonal position of mouse cursor.</param>
      /// <param name="yPos">Vertical position of mouse cursor.</param>
      /// <param name="ct">Mouse button.</param>
      /// <param name="mouseDown">Simulates mouse press event if true, otherwise mouse release.</param>
      public static void Click(int xPos, int yPos, ActionType ct, bool mouseDown)
      {
        Mouse_event(
          mouseDown ? MouseClickDownDwFlags(ct) : MouseClickUpDwFlags(ct),
          xPos,
          yPos,
          0,
          0
          );
      }


      /// <summary>
      /// Returns bit flags required to simulate mouse button press for specified button.
      /// </summary>
      /// <param name="ct">Mouse button.</param>
      /// <returns></returns>
      public static int MouseClickDownDwFlags(ActionType ct)
      {
        if (ct == ActionType.LeftClick) { return MOUSEEVENTF_LEFTDOWN; }
        if (ct == ActionType.MiddleClick) { return MOUSEEVENTF_MIDDLEDOWN; }
        if (ct == ActionType.RightClick) { return MOUSEEVENTF_RIGHTDOWN; }
        return 0x0000;
      }


      /// <summary>
      /// Returns bit flags required to simulate mouse button release for specified button.
      /// </summary>
      /// <param name="ct">Mouse button.</param>
      /// <returns></returns>
      public static int MouseClickUpDwFlags(ActionType ct)
      {
        if (ct == ActionType.LeftClick) { return MOUSEEVENTF_LEFTUP; }
        if (ct == ActionType.MiddleClick) { return MOUSEEVENTF_MIDDLEUP; }
        if (ct == ActionType.RightClick) { return MOUSEEVENTF_RIGHTUP; }
        return 0x0000;
      }
    }
  }
}
