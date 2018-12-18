using System.Runtime.InteropServices;
using System.Drawing;

namespace pWindowJax.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public RECT(RECT Rectangle)
            : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            X = Left;
            Y = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Left {
            get => X;
            set => X = value;
        }
        public int Top {
            get => Y;
            set => Y = value;
        }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Height {
            get => Bottom - Y;
            set => Bottom = value - Y;
        }
        public int Width {
            get => Right - X;
            set => Right = value + X;
        }
        public Point Location {
            get => new Point(Left, Top);
            set {
                X = value.X;
                Y = value.Y;
            }
        }
        public Size Size {
            get => new Size(Width, Height);
            set {
                Right = value.Width + X;
                Bottom = value.Height + Y;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + X + "; " + "Top: " + Y + "; Right: " + Right + "; Bottom: " + Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == X && Rectangle.Top == Y && Rectangle.Right == Right && Rectangle.Bottom == Bottom;
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }
    }
}
