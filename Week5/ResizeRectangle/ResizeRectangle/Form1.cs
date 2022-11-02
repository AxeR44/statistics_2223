using System.Reflection.Metadata.Ecma335;

namespace ResizeRectangle
{
    public partial class Form1 : Form
    {
        private Bitmap b;
        private Graphics g;
        private Rectangle r;
        private bool isTransforming = false, isMouseDown = false;
        private List<Rectangle> ResizeHandlers;
        private int oldMouseX, oldMouseY;
        private int oldX, oldY, oldW, oldH;
        private enum EventType
        {
            EVENT_NULL,
            EVENT_DRAG,
            EVENT_RESIZE_TOPLEFT,
            EVENT_RESIZE_BOTLEFT,
            EVENT_RESIZE_TOPRIGHT,
            EVENT_RESIZE_BOTRIGHT,
            EVENT_RESIZE_TOP,
            EVENT_RESIZE_BOT,
            EVENT_RESIZE_LEFT,
            EVENT_RESIZE_RIGHT
        }

        private EventType eventType = EventType.EVENT_NULL;

        public Form1()
        {
            InitializeComponent();
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = Graphics.FromImage(b);
            g.Clear(Color.White);
            r = new Rectangle(50, 50, 200, 200);
            this.ResizeHandlers = new List<Rectangle>();
            g.DrawRectangle(Pens.Red, r);
            pictureBox1.Image = b;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.ControlKey)
            {
                this.isTransforming = true;
                DrawResizeHandlers();
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.ControlKey)
            {
                this.isTransforming = false;
                redraw();
            }
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                int deltaX = e.X - this.oldMouseX, deltaY = e.Y - this.oldMouseY;
                if (isTransforming)
                {
                    switch (this.eventType)
                    {
                        case EventType.EVENT_RESIZE_TOPLEFT:
                            this.r.X = oldX + deltaX;
                            this.r.Y = oldY + deltaY;
                            this.r.Width = oldW - deltaX;
                            this.r.Height = oldH - deltaY;
                            break;
                        case EventType.EVENT_RESIZE_BOTLEFT:
                            this.r.X = oldX + deltaX;
                            this.r.Width = oldW - deltaX;
                            this.r.Height = oldH + deltaY;
                            break;
                        case EventType.EVENT_RESIZE_TOPRIGHT:
                            this.r.Y = oldY + deltaY;
                            this.r.Width = oldW + deltaX;
                            this.r.Height = oldH - deltaY;
                            break;
                        case EventType.EVENT_RESIZE_BOTRIGHT:
                            this.r.Width = oldW + deltaX;
                            this.r.Height = oldH + deltaY;
                            break;
                        case EventType.EVENT_RESIZE_TOP:
                            this.r.Y = oldY + deltaY;
                            this.r.Height = oldH - deltaY;
                            break;
                        case EventType.EVENT_RESIZE_BOT:
                            this.r.Height = oldH + deltaY;
                            break;
                        case EventType.EVENT_RESIZE_LEFT:
                            this.r.X = oldX + deltaX;
                            this.r.Width = oldW - deltaX;
                            break;
                        case EventType.EVENT_RESIZE_RIGHT:
                            this.r.Width = oldW + deltaX;
                            break;
                    }
                    redraw();
                    DrawResizeHandlers();
                }
                else
                {
                    this.r.X = oldX + deltaX;
                    this.r.Y = oldY + deltaY;
                    redraw();
                }
            }
            else
            {
                EventType tmpEvent = GetEvent(e.X, e.Y);
                switch (tmpEvent)
                {
                    case EventType.EVENT_DRAG:
                        Cursor.Current = Cursors.SizeAll;
                        break;
                    case EventType.EVENT_RESIZE_TOPLEFT:
                    case EventType.EVENT_RESIZE_BOTRIGHT:
                        Cursor.Current = Cursors.SizeNWSE;
                        break;
                    case EventType.EVENT_RESIZE_TOPRIGHT:
                    case EventType.EVENT_RESIZE_BOTLEFT:
                        Cursor.Current = Cursors.SizeNESW;
                        break;
                    case EventType.EVENT_RESIZE_TOP:
                    case EventType.EVENT_RESIZE_BOT:
                        Cursor.Current = Cursors.SizeNS;
                        break;
                    case EventType.EVENT_RESIZE_RIGHT:
                    case EventType.EVENT_RESIZE_LEFT:
                        Cursor.Current = Cursors.SizeWE;
                        break;
                    case EventType.EVENT_NULL:
                        Cursor.Current = Cursors.Default;
                        break;
                }
            }
        }

        private void onMouseDown(object sender, MouseEventArgs e)
        {
            this.oldMouseX = e.X;
            this.oldMouseY = e.Y;
            this.oldX = r.X;
            this.oldY = r.Y;
            this.oldH = r.Height;
            this.oldW = r.Width;
            this.eventType = GetEvent(this.oldMouseX, this.oldMouseY);
            if(this.eventType != EventType.EVENT_NULL)
            {
                this.isMouseDown = true;
            }
            else
            {
                this.isMouseDown=false;
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            this.isMouseDown = false;
        }

        private void DrawResizeHandlers()
        {
            this.ResizeHandlers.Clear();
            this.ResizeHandlers.Add(new Rectangle(r.Left - 5, r.Top - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Left - 5, r.Bottom - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Right - 5, r.Top - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Right - 5, r.Bottom - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Left + (r.Width / 2) - 5, r.Top - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Left + (r.Width / 2) - 5, r.Bottom - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Left - 5, r.Top + (r.Height / 2) - 5, 10, 10));
            this.ResizeHandlers.Add(new Rectangle(r.Right - 5, r.Top + (r.Height / 2) - 5, 10, 10));
            g.DrawRectangles(Pens.Red, this.ResizeHandlers.ToArray());
            pictureBox1.Image = this.b;
        }


        private EventType GetEvent(int MouseX, int MouseY)
        {
            if (this.isTransforming)
            {
                foreach (Rectangle handler in this.ResizeHandlers)
                {
                    if (handler.Contains(MouseX, MouseY))
                    {
                        return (EventType)(this.ResizeHandlers.IndexOf(handler) + 2);
                    }
                }
            }
            else {
                if (r.Contains(MouseX, MouseY))
                {
                    return EventType.EVENT_DRAG;
                }
            }
            return EventType.EVENT_NULL;
        }

        private void redraw()
        {
            this.g.Clear(Color.White);
            g.DrawRectangle(Pens.Red, this.r);
            pictureBox1.Image = this.b;
        }

    }
}