using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using PInvoke;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace pWindowJax
{
    internal class MainController : Form
    {
        public string Version { get; set; }
        private readonly NotifyIcon notifyIcon;
        private readonly GlobalKeyCombinationWatcher<WindowJaxOperation> watcher = new GlobalKeyCombinationWatcher<WindowJaxOperation>();

        /// <summary>
        /// Position of the window when move/resize operation began.
        /// </summary>
        private Point initialMousePosition;

        /// <summary>
        /// Size of the window when move/resize operation began.
        /// </summary>
        private Rectangle initialWindowRect;

        WindowJaxOperation currentOperation = WindowJaxOperation.Idle;

        public MainController()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("pWindowJax.icon.ico")),
                Text = "pWindowJax",
                Visible = true,

                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Info", delegate { new InfoBox { Version = Version }.Show(); }),
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

        private void actionChanged()
        {
            if (watcher.CurrentAction == currentOperation)
                return;

            updateIcon();

            // end the current operation if the combination is released
            if (watcher.CurrentAction == WindowJaxOperation.Idle || currentOperation != WindowJaxOperation.Idle)
            {
                currentOperation = WindowJaxOperation.Idle;
                return;
            }

            new Thread(() => 
            {

                startOp(watcher.CurrentAction);

            }).Start();
        }

        private void updateIcon()
        {
            switch (watcher.CurrentAction)
            {
                case WindowJaxOperation.Idle:
                    notifyIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("pWindowJax.icon.ico"));
                    break;
                case WindowJaxOperation.WindowReposition:
                    notifyIcon.Icon = toIcon(Cursors.WaitCursor);
                    break;
                case WindowJaxOperation.WindowResize:
                    notifyIcon.Icon = toIcon(Cursors.SizeAll);
                    break;
            }
        }

        private Icon toIcon(Cursor cursor)
        {
            var bitmap = new Bitmap(32, 32);
            using (var graphics = Graphics.FromImage(bitmap))
                cursor.Draw(graphics, new Rectangle(0, 0, 32, 32));

            return Icon.FromHandle(bitmap.GetHicon());
        }

        private void startOp(WindowJaxOperation operation)
        {
            currentOperation = operation;

            // Get the window the user is hovering his cursor over.
            IntPtr windowHandle = WindowsWindowHelper.GetWindowAt(User32.GetCursorPos());

            // There is a small chance that the windowhandle is null.
            // There is nothing we can do about that.
            if (windowHandle == null)
                return;

            Invoke((Action) (() => {
                WindowsWindowHelper.EnsureWindowIsInForeground(windowHandle, Handle);
            }));

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