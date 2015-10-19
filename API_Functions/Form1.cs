using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace API_Functions
{
  public partial class Form1 : Form
  {
    // DATA MEMBERS:
    #region
    WindowsFunctions wFunc;
    Hotkeys hotkeys;
    IntPtr thiswindow;
    fsModifier[] modifiers = { fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,

                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt,

                               fsModifier.Ctrl_Alt,

                               fsModifier.Ctrl_Alt,

                               fsModifier.Ctrl_Alt,
                               fsModifier.Ctrl_Alt
                             };

    Keys[] keys = { Keys.X, // For maximizing. 
                    Keys.M,  // For minimizing.
                    Keys.N,  // For nomral state.

                    Keys.Right,
                    Keys.Left,
                    Keys.Up,
                    Keys.Down,
                    Keys.PageUp,
                    Keys.PageDown,
                    Keys.Home,
                    Keys.End,
                    Keys.Clear, // numpad 5 with numlock off.

                    Keys.F,  // for window mouse-follow movement.

                    Keys.T,

                    Keys.Add,
                    Keys.Subtract
                  }; 

    #endregion

    // CONSTRUCTOR:
    #region
    public Form1()
    {
      InitializeComponent();
      WindowState = FormWindowState.Minimized;
      ShowInTaskbar = false;
      thiswindow = this.Handle;
      hotkeys = new Hotkeys(thiswindow, modifiers, keys);
      hotkeys.RegisterHotkeys();
      
    }
    #endregion

    // EVENTS:
    #region
    private void Form1_Load(object sender, EventArgs e)
    {
      
    }
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      WinAPIs.UnregisterHotKey(thiswindow, 1);
    }

    // this is the event responsible for sending messages to a window...
    int moveCommandToggle = 0;
    int ontopCounterToggle = 0;
    byte opacity = 255;
    IntPtr previousOntopWindow = new IntPtr(0);
    Thread mover = null;
    protected override void WndProc(ref Message m)
    {
      wFunc = new WindowsFunctions();
      
      if (m.Msg == 0x0312) // this will detect if a registered key has been pressed.
      {
        // exotic way of getting the key (without modifiers)
        // Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);

        int keyID = m.WParam.ToInt32();

        // MAXIMIZE, MINIMIZE and NORMAL states.
        if (keyID >= 0 && keyID <= 2)
        #region
        {
          WndState wState;
          switch(keyID)
          {
            case 0: wState = WndState.MAX;  break; 
            case 1: wState = WndState.MIN;  break;
            default: wState = WndState.NORMAL; break;
          }
          wFunc.ChangeWindowState(wState);
        }
        #endregion

        // PLACING THE WINDOW TO THE EDGES OF THE SCREEN.
        else if (keyID >= 3 && keyID <= 11) 
        #region
        {
          Point newPosition = new Point();
          Size newSize = new Size();
          switch (keyID)
          {
            case 3:
              newPosition.X = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newPosition.Y = 0;
              newSize.Width = newPosition.X; // width/2
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height;
              break;
            case 4: 
              newPosition.X = 0;
              newPosition.Y = 0; 
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width / 2; 
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height;
              break;
            case 5: 
              newPosition.X = 0;
              newPosition.Y = 0;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
              break;
            case 6: 
              newPosition.X = 0; 
              newPosition.Y = Screen.PrimaryScreen.WorkingArea.Height / 2;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
              break;
            case 7: // page up (numpad 9 without numlock)
              newPosition.X = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newPosition.Y = 0;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
              break;
            case 8: // page down (numpad 3 without numlock)
              newPosition.X = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newPosition.Y = Screen.PrimaryScreen.WorkingArea.Height / 2;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
              break;
            case 9: // home (numpad 7 without numlock)
              newPosition.X = 0;
              newPosition.Y = 0;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
              break;
            case 10: // end (numpad 1 without numlock)
              newPosition.X = 0;
              newPosition.Y = Screen.PrimaryScreen.WorkingArea.Height / 2;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
              break;
            case 11: // clear (numpad 5 without numlock)
              newPosition.X = Screen.PrimaryScreen.WorkingArea.Width / 4;
              newPosition.Y = 0;
              newSize.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
              newSize.Height = Screen.PrimaryScreen.WorkingArea.Height;
              break;
          }
          wFunc.MoveWindow_ToNewPosAndSize(newPosition, newSize);
        }
        #endregion

        // WINDOW MOVING, FOLLOWING THE MOUSE.
        else if (keyID == 12)
        #region
        {
          if (moveCommandToggle++ % 2 == 0)
          {
            mover = new Thread(() => wFunc.MoveWindow_AfterMouse());
            mover.Start();
          }
          else mover.Abort();
        }
        #endregion

        // ALWAYS ON TOP.
        else if (keyID == 13)
        #region
        {
          IntPtr fgWind = WinAPIs.GetForegroundWindow();
          if (previousOntopWindow == (IntPtr)0)
          {
            wFunc.AlwaysOnTop(fgWind, true);
            previousOntopWindow = fgWind;
          }
          else if (previousOntopWindow == fgWind)
          {
            wFunc.AlwaysOnTop(fgWind, false);
            previousOntopWindow = (IntPtr)0;
          }
          else 
          {
            wFunc.AlwaysOnTop(fgWind, true);
            wFunc.AlwaysOnTop(previousOntopWindow, false);
            previousOntopWindow = fgWind;
          }
        }
        #endregion

        // OPACITY
        else if (keyID == 14 || keyID == 15)
        {
          byte step = 15;
          if (keyID == 14 && opacity < 255) opacity += step;
          else if (opacity > 50 && keyID == 15) opacity -= step;
            wFunc.SetOpacity(WinAPIs.GetForegroundWindow(), opacity);
        }
      }
      base.WndProc(ref m);
    }
    #endregion
  }
}
