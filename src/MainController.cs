using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using PInvoke;
using System.Collections.Generic;

namespace pWindowJax
{
    internal class MainController : Form
    {
        private readonly NotifyIcon notifyIcon;
        private readonly GlobalKeyCombinationWatcher<WindowJaxOperation> watcher = new GlobalKeyCombinationWatcher<WindowJaxOperation>();

        public MainController()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("pWindowJax.icon.ico")),
                Text = "pWindowJax",
                Visible = true,

                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Info", delegate { new InfoBox().Show(); }),
                    new MenuItem("Quit", delegate { Application.Exit(); })
                })
            };

            watcher.ActionChanged += actionChanged;

            watcher.KeyCombinations.Add(new HashSet<Keys>
            {
                Keys.LControlKey, Keys.LWin
            }, WindowJaxOperation.WindowReposition);

            watcher.KeyCombinations.Add(new HashSet<Keys>
            {
                Keys.LMenu, Keys.LWin
            }, WindowJaxOperation.WindowResize);

            watcher.KeyCombinations.Add(new HashSet<Keys>
            {
                Keys.LWin, Keys.LShiftKey, Keys.LControlKey
            }, WindowJaxOperation.WindowResize);

            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Visible = false;
        }

        void actionChanged()
        {
            if (watcher.CurrentAction == currentOperation)
                return;

            // end the current operation if the combination is released
            if (watcher.CurrentAction == WindowJaxOperation.Idle || currentOperation != WindowJaxOperation.Idle)
            {
                currentOperation = WindowJaxOperation.Idle;
                return;
            }

            startOp(watcher.CurrentAction);
        }

        /// <summary>
        /// Position of the window when move/resize operation began.
        /// </summary>
        Point initialMousePosition;

        /// <summary>
        /// Size of the window when move/resize operation began.
        /// </summary>
        Rectangle initialWindowRect;

        WindowJaxOperation currentOperation = WindowJaxOperation.Idle;

        void startOp(WindowJaxOperation operation)
        {
            currentOperation = operation;

            // Get the window the user is hovering his cursor over.
            IntPtr windowHandle = WindowsWindowHelper.GetWindowAt(User32.GetCursorPos());

            // There is a small chance that the windowhandle is null.
            // There is nothing we can do about that.
            if (windowHandle == null)
                return;

            WindowsWindowHelper.EnsureWindowIsInForeground(windowHandle, Handle);

            initialWindowRect = WindowsWindowHelper.GetWindowRect(windowHandle);

            initialMousePosition = Cursor.Position;

            new Thread(t =>
            {
                Point lastCursorPosition = Point.Empty;

                while (currentOperation != WindowJaxOperation.Idle)
                {
                    if (Cursor.Position == lastCursorPosition)
                    {
                        Thread.Sleep(16);
                        continue;
                    }

                    lastCursorPosition = Cursor.Position;

                    Rectangle offsettedRect = new Rectangle(initialWindowRect.Location, initialWindowRect.Size);


                    if (currentOperation == WindowJaxOperation.WindowResize)
                    {
                        offsettedRect.Width += lastCursorPosition.X - initialMousePosition.X;
                        offsettedRect.Height += lastCursorPosition.Y - initialMousePosition.Y;
                    }

                    if (currentOperation == WindowJaxOperation.WindowReposition)
                    {
                        offsettedRect.X += lastCursorPosition.X - initialMousePosition.X;
                        offsettedRect.Y += lastCursorPosition.Y - initialMousePosition.Y;
                    }

                    WindowsWindowHelper.UpdateWindowRect(windowHandle, offsettedRect);
                }
            }).Start();
        }
    }
}