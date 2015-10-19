using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;

namespace API_Functions
{
  static class WinAPIs
  {
    // IMPORTING DLLS
    #region
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(String sClassName, String sAppName);
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nWndState);
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpWndPlc);
    [DllImport("user32.dll")]
    public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidht, int nHeight, bool bRepaint);
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pos);
    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);
    [DllImport("user32.dll")]
    public static extern bool BringWindowToTop(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int width, int height, uint flags);
    [DllImport("user32.dll")]
    public static extern long SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    public static extern long SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    #endregion
  }

  public struct WindowPlacement
  {
    public int lenght;
    public int flags;
    public int nWndState; // sometimes called showCmd.
    public Point ptMinPos;
    public Point ptMaxPos;
    public Rectangle rcNormalRectangle;
  };

 // [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int left;    // xCoor of upper left corner.
    public int top;     // yCoor of upper left corner.
    public int right;   // xCoor of lower right corner.
    public int bottom;  // yCoor of lower right corner.
  };
}
