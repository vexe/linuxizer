using System.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace API_Functions
{
  public enum WndState { NORMAL = 1, MIN = 2, MAX = 3 }
  public class WindowsFunctions
  {
    IntPtr hWnd;

    // CONSTRUCTOR(S):
    #region
    public WindowsFunctions(IntPtr hWnd)
    {
      this.hWnd = hWnd;
    }
    public WindowsFunctions() { }
    #endregion

    // METHODS:
    #region
    public void ChangeWindowState(WndState state)
    {
      IntPtr fgWnd = WinAPIs.GetForegroundWindow();
      WindowPlacement wp = new WindowPlacement();
      WinAPIs.GetWindowPlacement(fgWnd, ref wp);
      WinAPIs.ShowWindowAsync(fgWnd, (int)state);
    }
    public void MoveWindow_ToNewPosAndSize(Point newPosition, Size newSize)
    {
      IntPtr hWnd = WinAPIs.GetForegroundWindow();
      WinAPIs.MoveWindow(hWnd, newPosition.X, newPosition.Y, newSize.Width, newSize.Height, true); // if last param isn't true, moving will mess the window up!
    }
    public void MoveWindow_AfterMouse()
    {
      // 1- get a handle to the foreground window.
      // 2- set the mouse pos to the window's center.
      // 3- let the window move with the mouse in a loop, such that:
      // 	win(x) = mouse(x) - win(width)/2   
      //  win(y) = mouse(y) - win(height)/2
      // This is because the origin (point of rendering) of the window, is at its top-left corner and NOT its center!
      // Loop ends when the user clicks the left mouse button.

      // 1- 
      IntPtr hWnd = WinAPIs.GetForegroundWindow();
      //IntPtr hWnd = WinAPIs.FindWindow(null, "Form1");

      // 2- Then:
      // first we need to get the x, y to the center of the window.
      // to do this, we have to know the width/height of the window.
      // to do this, we could use GetWindowRect which will give us the coords of the bottom right and upper left corners of the window,
      // with some math, we could deduce the width/height of the window.
      // after we do that, we simply set the x, y coords of the mouse to that center.
      RECT wndRect = new RECT();
      WinAPIs.GetWindowRect(hWnd, out wndRect);
      int wndWidth = wndRect.right - wndRect.left;
      int wndHeight = wndRect.bottom - wndRect.top; // cuz the more you go down, the more y value increases.
      Point wndCenter = new Point(wndWidth / 2, wndHeight / 2); // this is the center of the window relative to itself.
      WinAPIs.ClientToScreen(hWnd, out wndCenter); // this will make its center relative to the screen coords.
      WinAPIs.SetCursorPos(wndCenter.X, wndCenter.Y);

      // 3- Moving :)))
      while (true)
      {
        Point cursorPos = new Point();
        WinAPIs.GetCursorPos(out cursorPos);
        int xOffset = cursorPos.X - wndWidth / 2;
        int yOffset = cursorPos.Y - wndHeight / 2;
        WinAPIs.MoveWindow(hWnd, xOffset, yOffset, wndWidth, wndHeight, true);
        Thread.Sleep(25);
      }
    }
    public void AlwaysOnTop(IntPtr hWnd, bool ontop)
    {
      IntPtr HWND_TOPMOST = new IntPtr(-1);
      IntPtr HWND_NOTOPMOST = new IntPtr(-2);
      const UInt32 SWP_SHOWWINDOW = 0x0040;
      const UInt32 NOSIZE = 0x0001;
      const UInt32 NOMOVE = 0x0002;
      WinAPIs.SetWindowPos(hWnd, ontop ? HWND_TOPMOST : HWND_NOTOPMOST, 0, 0, 0, 0, SWP_SHOWWINDOW | NOSIZE | NOMOVE);
    }
    public void SetOpacity(IntPtr hWnd, byte opacity)
    { 
    const int GWL_EXSTYLE = -20;
    const int WS_EX_LAYERED = 0x80000;
    const int LWA_ALPHA = 0x2;
    const int LWA_COLORKEY = 0x1;

    WinAPIs.SetWindowLong(hWnd, GWL_EXSTYLE, WinAPIs.GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_LAYERED);
    WinAPIs.SetLayeredWindowAttributes(hWnd, 0, opacity, LWA_ALPHA);
    }
    #endregion
  }
}
