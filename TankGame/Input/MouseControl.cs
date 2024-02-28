using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using static TankGame.Input.MouseControl;
using System.ComponentModel;
using System.Reflection;

namespace TankGame.Input
{
    internal delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    internal class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(WindowsHookTypes hookType,
            HookProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// Mouuse event info
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEHOOKSTRUCT
    {
        public POINT pt; // The x and y coordinates in screen coordinates
        public int hwnd; // Handle to the window that'll receive the mouse message
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    /// <summary>
    /// Mouuse event info
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEHOOKSTRUCTEX 
    {
        public POINT pt; // The x and y coordinates in screen coordinates
        public int hwnd; // Handle to the window that'll receive the mouse message
        public int wHitTestCode;
        public int dwExtraInfo;
        public IntPtr mouseData; 
    }

    
    /// <summary>
    /// Mouse event
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    //internal struct MSLLHOOKSTRUCT
    //{
    //    public POINT pt; // The x and y coordinates in screen coordinates. 
    //    public int mouseData; // The mouse wheel and button info.
    //    public int flags;
    //    public int time; // Specifies the time stamp for this message. 
    //    public IntPtr dwExtraInfo;
    //}

    /// <summary>
    ///     The structure contains information about a low-level keyboard input event.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT
    {
        public int vkCode; // Specifies a virtual-key code
        public int scanCode; // Specifies a hardware scan code for the key
        public int flags;
        public int time; // Specifies the time stamp for this message
        public int dwExtraInfo;
    }

    internal enum ButtonState
    {
        Pressed,
        Released
    }

    internal class MouseControl : IDisposable
    {
        bool _mouseCaptured;
        internal bool MouseCaptured
        {
            get
            {
                return this._mouseCaptured;
            }
        }

        private bool disposedValue;

        HookProc _globalMouseHookCallback;
        IntPtr _hGlobalMouseHook;
        IntPtr _hookID;

        ButtonState _leftButtonState;
        ButtonState _rightButtonState;
        bool _startCoordsValid = false;
        int _startX;
        int _startY;

        object _cummalativeLock = new object();
        int _cummalativeXDiff = 0;
        int _cummalativeYDiff = 0;

        public MouseControl()
        {
            _mouseCaptured = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                if (_mouseCaptured)
                {
                    ReleaseMouse();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        ~MouseControl()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void GetMouseMovement(out int dx, out int dy)
        {
            lock(_cummalativeLock)
            {
                dx = _cummalativeXDiff;
                dy = _cummalativeYDiff;
                _cummalativeXDiff = 0;
                _cummalativeYDiff = 0;
            }
        }

        public void CaptureMouse()
        {
            if (!_mouseCaptured)
            {
                Cursor.Hide();
                _startX = Cursor.Position.X;
                _startY = Cursor.Position.Y;
                _startCoordsValid = true;
                lock (_cummalativeLock)
                {
                    _startCoordsValid = false;
                    _cummalativeXDiff = 0;
                    _cummalativeYDiff = 0;
                }
                SetUpHook();
            }
            _mouseCaptured = true;
        }

        public void ReleaseMouse()
        {
            if (_mouseCaptured)
            {
                Cursor.Show();
                ClearHook();
            }
            _mouseCaptured = false;
        }
        private int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (MouseMessageType.WM_LBUTTONDOWN == (MouseMessageType)wParam)
            {
                // Do something.
                var mhs = Marshal.PtrToStructure<MOUSEHOOKSTRUCT>(lParam);
                Console.WriteLine($"point: {mhs.pt.x} {mhs.pt.y}");
            }
            return NativeMethods.CallNextHookEx(this._hookID, nCode, wParam, lParam);
        }

        private void SetUpHook()
        {
            // Create an instance of HookProc.
            _globalMouseHookCallback = LowLevelMouseProc;

            _hGlobalMouseHook = NativeMethods.SetWindowsHookEx(
                WindowsHookTypes.WH_MOUSE_LL,
                _globalMouseHookCallback,
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                0);

            if (_hGlobalMouseHook == IntPtr.Zero)
            {
                throw new Win32Exception("Unable to set MouseHook");
            }

            System.Diagnostics.Debug.WriteLine($"Mouse intercepted");
        }

        private void ClearHook()
        {
            if (_hGlobalMouseHook != IntPtr.Zero)
            {
                // Unhook the low-level mouse hook
                if (!NativeMethods.UnhookWindowsHookEx(_hGlobalMouseHook))
                    throw new Win32Exception("Unable to clear MouseHook");

                _hGlobalMouseHook = IntPtr.Zero;
            }
            System.Diagnostics.Debug.WriteLine($"Mouse interception cleared");
        }

        public int LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                // Get the mouse WM from the wParam parameter
                MouseMessageType wmMouse = (MouseMessageType)wParam;
                if (wmMouse == MouseMessageType.WM_LBUTTONDOWN && _leftButtonState == ButtonState.Released)
                {
                    System.Diagnostics.Debug.WriteLine($"Left button pressed");
                    _leftButtonState = ButtonState.Pressed;
                }
                if (wmMouse == MouseMessageType.WM_LBUTTONUP && _leftButtonState == ButtonState.Pressed)
                {
                    System.Diagnostics.Debug.WriteLine($"Left button released");
                    _leftButtonState = ButtonState.Released;
                }

                if (wmMouse == MouseMessageType.WM_RBUTTONDOWN && _rightButtonState == ButtonState.Released)
                {
                    _rightButtonState = ButtonState.Pressed;
                }
                if (wmMouse == MouseMessageType.WM_RBUTTONUP && _rightButtonState == ButtonState.Pressed)
                {
                    _rightButtonState = ButtonState.Released;
                }
                if (wmMouse == MouseMessageType.WM_MOUSEMOVE)
                {
                    var mhs = Marshal.PtrToStructure<MOUSEHOOKSTRUCT>(lParam);
                    lock (_cummalativeLock)
                    {
                        if (_startCoordsValid == false)
                        {
                            _startX = mhs.pt.x;
                            _startY = mhs.pt.y;
                            _startCoordsValid = true;
                            System.Diagnostics.Debug.WriteLine($"Start point: {mhs.pt.x} {mhs.pt.y}");
                        }
                        else
                        {
                            int dx = mhs.pt.x - _startX;
                            int dy = _startY - mhs.pt.y;
                            _cummalativeXDiff += dx;
                            _cummalativeYDiff += dy;
                            System.Diagnostics.Debug.WriteLine($"st: ({_startX}, {_startY}) curr:({mhs.pt.x}, {mhs.pt.y}) diff ({dx}, {dy})");
                        }
                    }
                }
            }
            // Call this to actually move the mouse
            // return NativeMethods.CallNextHookEx(_hGlobalMouseHook, nCode, wParam, lParam);
            return -1;
        }
    }
}
