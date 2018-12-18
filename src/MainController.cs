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
        Rectangle initialWindowRect;

        WindowJaxOperation? currentOperation;

        void startOp(WindowJaxOperation operation)
        {
            currentOperation = operation;

            // Get the window the user is hovering his cursor over.
            IntPtr windowHandle = WindowsWindowHelper.GetWindowAt(User32.GetCursorPos());

            // There is a small chance that the windowhandle can be null.
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