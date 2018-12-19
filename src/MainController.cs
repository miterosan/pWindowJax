using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using Gma.System.MouseKeyHook;
using PInvoke;
using System.Collections.Generic;

namespace pWindowJax
{
    internal class MainController : Form
    {
        private readonly IKeyboardMouseEvents keyboardMouseEvents = Hook.GlobalEvents();
        private readonly NotifyIcon notifyIcon;
        private readonly List<Keys> pressedKeys = new List<Keys>();

        private readonly List<Keys> repositionKeyCombination = new List<Keys>
        {
            Keys.LControlKey, Keys.LWin
        };

        private readonly List<Keys> resizeKeyCombinations = new List<Keys>
        {
            Keys.LMenu, Keys.LWin
        };

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

        void keyUp(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyValue;

            pressedKeys.Remove(key);

            if (currentOperation == WindowJaxOperation.WindowReposition && repositionKeyCombination.Contains(key))
                currentOperation = null;

            if (currentOperation == WindowJaxOperation.WindowResize && resizeKeyCombinations.Contains(key))
                currentOperation = null;
        }

        void keyDown(object sender, KeyEventArgs e)
        {
            Keys key = (Keys)e.KeyValue;

            if (pressedKeys.Contains(key) || currentOperation != null)
                return;

            pressedKeys.Add(key);

            if (!repositionKeyCombination.Except(pressedKeys).Any())
                startOp(WindowJaxOperation.WindowReposition);
            if (!resizeKeyCombinations.Except(pressedKeys).Any())
                startOp(WindowJaxOperation.WindowResize);
        }

        /// <summary>
        /// Position of the window when move/resize operation began.
        /// </summary>
        Point initialPosition;

        /// <summary>
        /// Size of the window when move/resize operation began.
        /// </summary>
        Rectangle initialWindowRect;

        WindowJaxOperation? currentOperation;

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

            initialPosition = Cursor.Position;

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

                    Rectangle offsettedRect = new Rectangle(initialWindowRect.Location, initialWindowRect.Size);


                    if (currentOperation == WindowJaxOperation.WindowResize)
                    {
                        offsettedRect.Width += lastCursorPosition.X - initialPosition.X;
                        offsettedRect.Height += lastCursorPosition.Y - initialPosition.Y;
                    }

                    if (currentOperation == WindowJaxOperation.WindowReposition)
                    {
                        offsettedRect.X += lastCursorPosition.X - initialPosition.X;
                        offsettedRect.Y += lastCursorPosition.Y - initialPosition.Y;
                    }

                    WindowsWindowHelper.UpdateWindowRect(windowHandle, offsettedRect);
                }
            }).Start();
        }
    }
}