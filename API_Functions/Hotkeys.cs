using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace API_Functions
{
  enum fsModifier
  {
    Alt = 0x0001,
    Ctrl = 0x0002,
    Shift = 0x0004,
    Window = 0x0008,
    Ctrl_Alt = 0x0001 + 0x0002,
    Ctrl_Shift = 0x0002 + 0x0004,
    Shift_Alt = 0x0004 + 0x0001
  }

  class Hotkeys
  {
    // DATA MEMBERS:
    #region
    IntPtr hWnd;
    Keys[] keys;
    fsModifier[] modifiers;
    #endregion

    // CONSTRUCTORS:
    #region
    public Hotkeys(IntPtr hWnd, fsModifier[] modifiers, Keys[] keys)
    {
      this.hWnd = hWnd;
      this.modifiers = modifiers;
      this.keys = keys;
    }
    #endregion

    // METHODS:
    #region
    public void RegisterHotkeys()
    {
      for (int i = 0; i < keys.Length; i++)
      {
        WinAPIs.RegisterHotKey(hWnd, i, (uint)modifiers[i], (uint)keys[i]);
      }
    }
    public void UnregisterHotkeys()
    {
      for (int i = 0; i < keys.Length; i++)
      {
        WinAPIs.UnregisterHotKey(hWnd, i);
      }
    }
    #endregion
  }
}
