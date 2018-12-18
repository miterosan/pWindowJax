using PInvoke;
using System;
using System.Drawing;

namespace pWindowJax
{
    public static class WindowsWindowHelper
    {
        public static IntPtr GetWindowAt(POINT p)
        {
            return User32.GetAncestor(User32.WindowFromPoint(p), User32.GetAncestorFlags.GA_ROOT);
        }

        public static Rectangle GetWindowRect(IntPtr windowHandle)
        {
            var info = new User32.WINDOWINFO();

            User32.GetWindowInfo(windowHandle, ref info);

            var rect = info.rcWindow;

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        public static void UpdateWindowRect(IntPtr windowHandle, Rectangle newRectangle)
        {
            User32.SetWindowPos(windowHandle, IntPtr.Zero, newRectangle.X, newRectangle.Y, newRectangle.Width, newRectangle.Height, 0);
        }

        /// <summary>
        /// Moves the <paramref name="windowHandle"/> into the foreground.
        /// </summary>
        /// <param name="windowHandle">The window that is supposed to be in the foreground</param>
        /// <param name="baseWindowHandle">The current application window. (Is used for switching the foreground temporarily.)</param>
        public static void EnsureWindowIsInForeground(IntPtr windowHandle, IntPtr baseWindowHandle)
        {
            if (windowHandle != User32.GetForegroundWindow())
            {
                MakeWindowActive(baseWindowHandle);
                MakeWindowActive(windowHandle);
            }
        }

        private static void MakeWindowActive(IntPtr windowHandle)
        {
            int cursorWindowThreadId = User32.GetWindowThreadProcessId(windowHandle, out int o1);
            int foregroundWindowThreadId = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out int o2);

            if (foregroundWindowThreadId != cursorWindowThreadId)
                User32.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, true);

            User32.SetForegroundWindow(windowHandle);

            if (foregroundWindowThreadId != cursorWindowThreadId)
                User32.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, false);
        }
    }
}
