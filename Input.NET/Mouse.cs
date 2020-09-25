using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Input.NET
{
    /// <summary>
    ///  https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mouse_event
    /// </summary>
    public class Mouse
    {
        private const int INPUT_MOUSE = 0x0;
        private const int MOUSEEVENTF_MOVE = 0x1;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        public enum MouseButtons
        {
            LeftDown = 0x2,
            LeftUp = 0x4,
            RightDown = 0x8,
            RightUp = 0x10,
            MiddleDown = 0x20,
            MiddleUp = 0x40,
            Absolute = 0x8000,
            Wheel = 0x800
        }
        public enum MouseKeys
        {
            Left = -1,
            Right = -2,
            Middle = -3
        }
        public enum ScrollDirection
        {
            Up = 120,
            Down = -120
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int smIndex);
        private const int SM_SWAPBUTTON = 23;
        [DllImport("user32.dll")]
        private static extern int SendInput(int cInputs, ref INPUT pInputs, int cbSize);

        private struct INPUT
        {
            public int dwType;
            public MOUSEKEYBDHARDWAREINPUT mkhi;
        }

        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MouseInput mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        private struct MouseInput
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public static bool IsLeftHanded => GetSystemMetrics(SM_SWAPBUTTON) == 1;

        public static void SendButton(MouseButtons mButton)
        {
            INPUT input = new INPUT();
            input.dwType = INPUT_MOUSE;
            input.mkhi.mi = new MouseInput();
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            input.mkhi.mi.dwFlags = (int)mButton;
            input.mkhi.mi.dx = 0;
            input.mkhi.mi.dy = 0;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }

        public static void PressButton(MouseKeys mKey, int Delay = 0)
        {
            ButtonDown(mKey);
            if (Delay > 0)
                System.Threading.Thread.Sleep(Delay);
            ButtonUp(mKey);
        }

        public static void ButtonDown(MouseKeys mKey)
        {
            switch (mKey)
            {
                case MouseKeys.Left:
                    {
                        SendButton(MouseButtons.LeftDown);
                        break;
                    }

                case MouseKeys.Right:
                    {
                        SendButton(MouseButtons.RightDown);
                        break;
                    }

                case MouseKeys.Middle:
                    {
                        SendButton(MouseButtons.MiddleDown);
                        break;
                    }
            }
        }

        public static void ButtonUp(MouseKeys mKey)
        {
            switch (mKey)
            {
                case MouseKeys.Left:
                    {
                        SendButton(MouseButtons.LeftUp);
                        break;
                    }

                case MouseKeys.Right:
                    {
                        SendButton(MouseButtons.RightUp);
                        break;
                    }

                case MouseKeys.Middle:
                    {
                        SendButton(MouseButtons.MiddleUp);
                        break;
                    }
            }
        }

        public static void Move(int X, int Y)
        {
            INPUT input = new INPUT();
            input.dwType = INPUT_MOUSE;
            input.mkhi.mi = new MouseInput();
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            input.mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE + MOUSEEVENTF_MOVE;
            input.mkhi.mi.dx = (int)(X * ((double)ushort.MaxValue / Screen.PrimaryScreen.Bounds.Width));
            input.mkhi.mi.dy = (int)(Y * ((double)ushort.MaxValue / Screen.PrimaryScreen.Bounds.Height));
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }

        public static void MoveRelative(int X, int Y)
        {
            INPUT input = new INPUT();
            input.dwType = INPUT_MOUSE;
            input.mkhi.mi = new MouseInput();
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            input.mkhi.mi.dwFlags = MOUSEEVENTF_MOVE;
            input.mkhi.mi.dx = X;
            input.mkhi.mi.dy = Y;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }


        public static void Scroll(ScrollDirection Direction)
        {
            INPUT input = new INPUT();
            input.dwType = INPUT_MOUSE;
            input.mkhi.mi = new MouseInput();
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            input.mkhi.mi.dwFlags = (int)MouseButtons.Wheel;
            input.mkhi.mi.mouseData = (int)Direction;
            input.mkhi.mi.dx = 0;
            input.mkhi.mi.dy = 0;
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }
    }
}
