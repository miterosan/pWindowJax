using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using pWindowJax.Native;
using Gma.System.MouseKeyHook;

namespace pWindowJax
{
    internal class MainController : Form
    {
        private readonly IKeyboardMouseEvents keyboardMouseEvents = Hook.GlobalEvents();

        public MainController()
        {
            keyboardMouseEvents.KeyDown += keyDown;
            keyboardMouseEvents.KeyUp += keyUp;

            contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Quit", delegate { Application.Exit(); });

            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("pWindowJax.icon.ico")),
                Text = "pWindowJax",
                Visible = true,

                ContextMenu = contextMenu
            };

            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Visible = false;
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams pm = base.CreateParams;
                pm.ExStyle |= 0x80;
                return pm;
            }
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
        private readonly NotifyIcon notifyIcon;
        private ContextMenu contextMenu;

        void startOp(bool resize)
        {
            if (isOperating && isOperationResizing == resize)
                return; //an operation is in progress and is the same type as the one requested.

            //Make sure the window underneath the cursor is foregrounded.
            if (User32DllImports.GetCursorPos(out POINT p))
            {
                IntPtr hWnd = User32DllImports.WindowFromPoint(p);
                IntPtr hWndSuccess = IntPtr.Zero;

                while (hWnd != IntPtr.Zero)
                {
                    hWndSuccess = hWnd;
                    hWnd = User32DllImports.GetParent(hWnd);
                }

                if (hWndSuccess != IntPtr.Zero && hWndSuccess != User32DllImports.GetForegroundWindow())
                {
                    {
                        //first set the main form window as active...
                        uint cursorWindowThreadId = User32DllImports.GetWindowThreadProcessId(Handle, IntPtr.Zero);
                        uint foregroundWindowThreadId = User32DllImports.GetWindowThreadProcessId(User32DllImports.GetForegroundWindow(), IntPtr.Zero);

                        if (foregroundWindowThreadId != cursorWindowThreadId)
                            User32DllImports.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, true);

                        User32DllImports.SetForegroundWindow(Handle);

                        if (foregroundWindowThreadId != cursorWindowThreadId)
                            User32DllImports.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, false);
                    }

                    {
                        //then switch to the target window.
                        uint cursorWindowThreadId = User32DllImports.GetWindowThreadProcessId(hWndSuccess, IntPtr.Zero);
                        uint foregroundWindowThreadId = User32DllImports.GetWindowThreadProcessId(User32DllImports.GetForegroundWindow(), IntPtr.Zero);

                        if (foregroundWindowThreadId != cursorWindowThreadId)
                            User32DllImports.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, true);

                        User32DllImports.SetForegroundWindow(hWndSuccess);

                        if (foregroundWindowThreadId != cursorWindowThreadId)
                            User32DllImports.AttachThreadInput(foregroundWindowThreadId, cursorWindowThreadId, false);
                    }
                }
            }

            isOperationResizing = resize;

            IntPtr window = User32DllImports.GetForegroundWindow();
            WINDOWINFO info = new WINDOWINFO();
            User32DllImports.GetWindowInfo(window, ref info);

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

                    if (isOperationResizing)
                    {
                        User32DllImports.SetWindowPos(window, IntPtr.Zero, windowSize.X, windowSize.Y, windowSize.Width + (lastCursorPosition.X - initialPosition.X), windowSize.Height + (lastCursorPosition.Y - initialPosition.Y), 0);
                    }
                    else
                    {
                        User32DllImports.SetWindowPos(window, IntPtr.Zero, windowSize.X + (lastCursorPosition.X - initialPosition.X), windowSize.Y + (lastCursorPosition.Y - initialPosition.Y), windowSize.Width, windowSize.Height, 0);
                    }
                }
            }).Start();
        }
    }
}