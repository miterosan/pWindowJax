﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using Gma.System.MouseKeyHook;
using PInvoke;

namespace pWindowJax
{
    internal class MainController : Form
    {
        private readonly IKeyboardMouseEvents keyboardMouseEvents = Hook.GlobalEvents();
        private readonly NotifyIcon notifyIcon;

        public MainController()
        {
            keyboardMouseEvents.KeyDown += keyDown;
            keyboardMouseEvents.KeyUp += keyUp;

            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("pWindowJax.icon.ico")),
                Text = "pWindowJax",
                Visible = true,

                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Quit", delegate { Application.Exit(); })
                })
            };

            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Visible = false;
        }

        bool ctrlPressed;
        bool altPressed;
        bool winPressed;
        bool shiftPressed;

        void keyUp(object sender, KeyEventArgs e)
        {
            ctrlPressed &= e.KeyValue != 162;
            altPressed &= e.KeyValue != 164;
            winPressed &= e.KeyValue != 91;
            shiftPressed &= e.KeyValue != 0xA0;

            if (currentOperation == WindowJaxOperation.WindowReposition && !ctrlPressed && !winPressed)
                currentOperation = null;

            if (currentOperation == WindowJaxOperation.WindowResize && ((!altPressed && !winPressed) || (!ctrlPressed && !winPressed && !shiftPressed)))
                currentOperation = null;
        }

        void keyDown(object sender, KeyEventArgs e)
        {
            ctrlPressed |= e.KeyValue == 162;
            altPressed |= e.KeyValue == 164;
            winPressed |= e.KeyValue == 91;
            shiftPressed |= e.KeyValue == 0xA0;

            // a operation is already in process.
            if (currentOperation != null)
                return;

            if (ctrlPressed && winPressed)
                startOp(WindowJaxOperation.WindowReposition);
            if ((altPressed && winPressed) || (ctrlPressed && winPressed && shiftPressed))
                startOp(WindowJaxOperation.WindowResize);
        }

        /// <summary>
        /// Position of the window when move/resize operation began.
        /// </summary>
        Point initialPosition;

        /// <summary>
        /// Size of the window when move/resize operation began.
        /// </summary>
        RECT windowSize;

        WindowJaxOperation? currentOperation;

        void startOp(WindowJaxOperation operation)
        {
            currentOperation = operation;

            //Make sure the window underneath the cursor is foregrounded.
            POINT p = User32.GetCursorPos();

            // Get the window the user is hovering his cursor over.
            IntPtr windowHandle = User32.GetAncestor(User32.WindowFromPoint(p), User32.GetAncestorFlags.GA_ROOT);

            // ensure that the target window is the foreground window
            if (windowHandle != IntPtr.Zero && windowHandle != User32.GetForegroundWindow())
            {
                //first set the main form window as active...
                makeWindowActive(Handle);

                // then the tagets window
                makeWindowActive(windowHandle);
            }

            var info = new User32.WINDOWINFO();
            User32.GetWindowInfo(windowHandle, ref info);

            initialPosition = Cursor.Position;
            windowSize = info.rcWindow;

            new Thread(t =>
            {
                Point lastCursorPosition = Point.Empty;

                while (currentOperation != null)
                {
                    if (Cursor.Position == lastCursorPosition)
                    {
                        Thread.Sleep(16);
                        continue;
                    }

                    lastCursorPosition = Cursor.Position;

                    int x = windowSize.left;
                    int y = windowSize.top;
                    int width = windowSize.right - windowSize.left;
                    int height = windowSize.bottom - windowSize.top;

                    if (currentOperation == WindowJaxOperation.WindowResize)
                    {
                        width = width + (lastCursorPosition.X - initialPosition.X);
                        height = height + (lastCursorPosition.Y - initialPosition.Y);
                    }

                    if (currentOperation == WindowJaxOperation.WindowReposition)
                    {
                        x = x + (lastCursorPosition.X - initialPosition.X);
                        y = y + (lastCursorPosition.Y - initialPosition.Y);
                    }

                    User32.SetWindowPos(windowHandle, IntPtr.Zero, x, y, width, height, 0);
                }
            }).Start();
        }

        private void makeWindowActive(IntPtr windowHandle)
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