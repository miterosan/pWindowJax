using System;
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

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }


        void keyUp(object sender, KeyEventArgs e)
        {
            ctrlPressed &= e.KeyValue != 162;
            altPressed &= e.KeyValue != 164;
            winPressed &= e.KeyValue != 91;
            shiftPressed &= e.KeyValue != 0xA0;

            if (!ctrlPressed)
                isOperating = false;
            else
                isOperationResizing = false;
        }

        bool ctrlPressed;
        bool altPressed;
        bool winPressed;
        bool shiftPressed;

        void keyDown(object sender, KeyEventArgs e)
        {
            ctrlPressed |= e.KeyValue == 162;
            altPressed |= e.KeyValue == 164;
            winPressed |= e.KeyValue == 91;
            shiftPressed |= e.KeyValue == 0xA0;

            if (ctrlPressed && winPressed)
                startOp(false);
            if ((altPressed && winPressed) || (ctrlPressed && winPressed && shiftPressed))
                startOp(true);
        }

        /// <summary>
        /// Position of the window when move/resize operation began.
        /// </summary>
        Point initialPosition;

        /// <summary>
        /// Size of the window when move/resize operation began.
        /// </summary>
        RECT windowSize;

        bool isOperating;
        bool isOperationResizing;

        void startOp(bool resize)
        {
            if (isOperating && isOperationResizing == resize)
                return; //an operation is in progress and is the same type as the one requested.

            //Make sure the window underneath the cursor is foregrounded.
            POINT p = User32.GetCursorPos();

            IntPtr hWnd = User32.WindowFromPoint(p);

            hWnd = User32.GetAncestor(hWnd, User32.GetAncestorFlags.GA_ROOT);

            IntPtr hWndSuccess = hWnd;

            if (hWndSuccess != IntPtr.Zero && hWndSuccess != User32.GetForegroundWindow())
            {
                setMainFormWindowActive();
                switchToTargetWindow(hWnd);
            }

            isOperationResizing = resize;

            IntPtr window = User32.GetForegroundWindow();
            var info = new User32.WINDOWINFO();
            User32.GetWindowInfo(window, ref info);

            initialPosition = Cursor.Position;
            windowSize = info.rcWindow;

            lock (this)
            {
                if (isOperating)
                    return; //an operation has just switch modes from move <-> resize, but is already in progress.

                isOperating = true;
            }

            new Thread(t =>
            {
                Point lastCursorPosition = Point.Empty;

                while (isOperating)
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

                    if (isOperationResizing)
                    {
                        width = width + (lastCursorPosition.X - initialPosition.X);
                        height = height + (lastCursorPosition.Y - initialPosition.Y);
                    }
                    else
                    {
                        x = x + (lastCursorPosition.X - initialPosition.X);
                        y = y + (lastCursorPosition.Y - initialPosition.Y);
                    }

                    User32.SetWindowPos(window, IntPtr.Zero, x, y, width, height, 0);
                }
            }).Start();
        }

        private void setMainFormWindowActive()
        {
            //first set the main form window as active...
            int cursorWindowThreadId = User32.GetWindowThreadProcessId(Handle, out int o);
            int foregroundWindowThreadId = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out int o2);

            if (foregroundWindowThreadId != cursorWindowThreadId)
                User32.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, true);

            User32.SetForegroundWindow(Handle);

            if (foregroundWindowThreadId != cursorWindowThreadId)
                User32.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, false);
        }

        private void switchToTargetWindow(IntPtr targetWindowHandle)
        {
            //then switch to the target window.
            int cursorWindowThreadId = User32.GetWindowThreadProcessId(targetWindowHandle, out int o1);
            int foregroundWindowThreadId = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out int o2);

            if (foregroundWindowThreadId != cursorWindowThreadId)
                User32.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, true);

            User32.SetForegroundWindow(targetWindowHandle);

            if (foregroundWindowThreadId != cursorWindowThreadId)
                User32.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, false);
        }
    }
}