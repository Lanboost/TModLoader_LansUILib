using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LansUILib.ui.components;

namespace LansUILib.ui
{

    public enum AnchorPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        RightCenter,
        BottomRight,
        BottomCenter,
        BottomLeft,
        LeftCenter,
        Center,
        ExpandToParent
    }

    public class LComponent
    {
        public delegate void LMouseEvent(MouseState state);

        public event LMouseEvent MouseEnter;
        public event LMouseEvent MouseExit;
        public event LMouseEvent MouseDown;
        public event LMouseEvent MouseUp;
        public event LMouseEvent MouseMove;


        public bool MouseInteraction = false;

        public bool ContainsMouse(MouseState mouse) {
            return layout.X <= mouse.x && mouse.x < layout.X + layout.Width && layout.Y <= mouse.y && mouse.y < layout.Y + layout.Height;
        }

        public void fireMouseEnter(MouseState state) { MouseEnter?.Invoke(state); }
        public void fireMouseExit(MouseState state) { MouseExit?.Invoke(state); }
        public void fireMouseDown(MouseState state) { MouseDown?.Invoke(state); }
        public void fireMouseUp(MouseState state) { MouseUp?.Invoke(state); }
        public void fireMouseMove(MouseState state) { MouseMove?.Invoke(state); }


        public static int MinX = 0;
        public static int MinY = 1;
        public static int MaxX = 2;
        public static int MaxY = 3;
        public static int SIDE_COUNT = 4;

        protected float[] anchor = new float[] {0,0,1,1};
        protected int[] margin = new int[SIDE_COUNT];
        protected float[] pivot = new float[2] {0.5f,0.5f};

        protected List<LComponent> children = new List<LComponent>();

        protected Layout layout;
        public LImage image;
        public LImage border;
        public bool isMask;
        public string text;
        public ILColor textColor;

        protected LComponent _parent = null;
        public string name;

        public LComponent(string name)
        {
            SetLayout(Layout.None());
            this.name = name;
        }

        public LComponent Parent
        {
            get { return _parent; }
        }

        public IEnumerable<LComponent> Children
        {
            get { return children.AsEnumerable(); }
        }

        public virtual void Add(LComponent component)
        {
            if (component._parent != null)
            {
                throw new Exception("This component is already owned by someone else!");
            }
            component._parent = this;
            this.children.Add(component);
        }

        public virtual void Remove(LComponent component)
        {
            component._parent = null;
            this.children.Remove(component);
        }

        public virtual void RemoveChildren()
        {
            for(int i= children.Count- 1; i >= 0; i--)
            {
                Remove(children[i]);
            }
        }

        public virtual void SetMargin(int side, int value)
        {
            margin[side] = value;

            this.GetLayout().SetDirty();
        }

        public virtual int GetMargin(int side)
        {
            return margin[side];
        }

        public virtual float GetPivot(int side)
        {
            return pivot[side];
        }

        public virtual void SetMargins(int minX, int minY, int maxX, int maxY)
        {
            margin[MinX] = minX;
            margin[MinY] = minY;
            margin[MaxX] = maxX;
            margin[MaxY] = maxY;

            this.GetLayout().SetDirty();
        }

        public virtual void SetLayout(Layout layout)
        {
            this.layout = layout;
            layout.SetComponentOwner(this);
        }

        public virtual Layout GetLayout()
        {
            return this.layout;
        }

        public virtual void SetAnchor(int side, float value)
        {
            if(value < 0 || value > 1)
            {
                throw new Exception("Value range is 0-1");
            }
            anchor[side] = value;

            this.GetLayout().SetDirty();
        }

        public virtual void SetAnchors(float minX, float minY, float maxX, float maxY)
        {
            anchor[MinX] = minX;
            anchor[MinY] = minY;
            anchor[MaxX] = maxX;
            anchor[MaxY] = maxY;

            this.GetLayout().SetDirty();
        }

        public virtual float GetAnchor(int side)
        {
            return anchor[side];
        }

        public virtual void SetAnchor(AnchorPosition anchorPosition, bool movePivot = true)
        {
            if(anchorPosition == AnchorPosition.TopLeft)
            {
                SetAnchors(0,0,0,0);
                if(movePivot)
                {
                    pivot = new float[] { 0, 0 };
                }
            }
            else if (anchorPosition == AnchorPosition.TopCenter)
            {
                SetAnchors(0.5f, 0, 0.5f, 0);
                if (movePivot)
                {
                    pivot = new float[] { 0.5f, 0 };
                }
            }
            else if (anchorPosition == AnchorPosition.TopRight)
            {
                SetAnchors(1f, 0, 1f, 0);
                if (movePivot)
                {
                    pivot = new float[] { 1f, 0 };
                }
            }
            else if (anchorPosition == AnchorPosition.RightCenter)
            {
                SetAnchors(1f, 0.5f, 1f, 0.5f);
                if (movePivot)
                {
                    pivot = new float[] { 1f, 0.5f };
                }
            }
            else if (anchorPosition == AnchorPosition.BottomRight)
            {
                SetAnchors(1f, 1f, 1f, 1f);
                if (movePivot)
                {
                    pivot = new float[] { 1f, 1f };
                }
            }
            else if (anchorPosition == AnchorPosition.BottomCenter)
            {
                SetAnchors(0.5f, 1f, 0.5f, 1f);
                if (movePivot)
                {
                    pivot = new float[] { 0.5f, 1f };
                }
            }
            else if (anchorPosition == AnchorPosition.BottomLeft)
            {
                SetAnchors(0f, 1f, 0f, 1f);
                if (movePivot)
                {
                    pivot = new float[] { 0f, 1f };
                }
            }
            else if (anchorPosition == AnchorPosition.LeftCenter)
            {
                SetAnchors(0f, 0.5f, 0f, 0.5f);
                if (movePivot)
                {
                    pivot = new float[] { 0f, 0.5f };
                }
            }
            else if (anchorPosition == AnchorPosition.Center)
            {
                SetAnchors(0.5f, 0.5f, 0.5f, 0.5f);
                if (movePivot)
                {
                    pivot = new float[] { 0.5f, 0.5f };
                }
            }
            else if (anchorPosition == AnchorPosition.ExpandToParent)
            {
                SetAnchors(0, 0, 1, 1);
            }
        }

        public virtual void SetMargin(int minX, int minY, int maxX, int maxY)
        {
            margin[MinX] = minX;
            margin[MinY] = minY;
            margin[MaxX] = maxX;
            margin[MaxY] = maxY;

            this.GetLayout().SetDirty();
        }

        public virtual void SetSize(int x, int y, int width, int height)
        {
            if (anchor[MinX] != anchor[MaxX] || anchor[MinY] != anchor[MaxY])
            {
                throw new Exception("Size only works correctly if you have anchored to the same (MinX/MaxX and MinY/MaxY). Set that first!");
            }

            margin[MinX] = x;
            margin[MinY] = y;
            margin[MaxX] = -x-width;
            margin[MaxY] = -y-height;

            this.GetLayout().SetDirty();
        }

        public virtual int[] GetSize()
        {
            if (anchor[MinX] != anchor[MaxX] || anchor[MinY] != anchor[MaxY])
            {
                throw new Exception("Size only works correctly if you have anchored to the same (MinX/MaxX and MinY/MaxY). Set that first!");
            }

            var x = margin[MinX];
            var y = margin[MinY];
            var width = -margin[MaxX]-x;
            var height = -margin[MaxY]-y;

            return new int[] { x, y, width, height };
        }

        public virtual void Move(int deltaX, int deltaY, bool safe = true)
        {
            if (anchor[MinX] != anchor[MaxX] || anchor[MinY] != anchor[MaxY])
            {
                throw new Exception("Size only works correctly if you have anchored to the same (MinX/MaxX and MinY/MaxY). Set that first!");
            }

            if(safe)
            {
                if(this.Parent == null)
                {
                    return;
                }

                var paddingLeft = this.layout.X + this.Parent.layout.X;
                var paddingRight = this.Parent.layout.Width -this.layout.Width -paddingLeft;
                if(paddingLeft + deltaX < 0)
                {
                    deltaX = -paddingLeft;
                }
                if (paddingRight - deltaX < 0)
                {
                    deltaX = paddingRight;
                }

                var paddingTop = this.layout.Y + this.Parent.layout.Y;
                var paddingBottom = this.Parent.layout.Height - this.layout.Height - paddingTop;

                if (paddingTop + deltaY < 0)
                {
                    deltaY = -paddingTop;
                }
                if (paddingBottom - deltaY < 0)
                {
                    deltaY = paddingBottom;
                }
            }


            margin[MinX] = margin[MinX] + deltaX;
            margin[MinY] = margin[MinY] + deltaY;
            margin[MaxX] = margin[MaxX] - deltaX;
            margin[MaxY] = margin[MaxY] - deltaY;


            this.GetLayout().SetDirty();
        }

        public virtual void Resize(int deltaX, int deltaY, bool safe = true)
        {
            if (anchor[MinX] != anchor[MaxX] || anchor[MinY] != anchor[MaxY])
            {
                throw new Exception("Size only works correctly if you have anchored to the same (MinX/MaxX and MinY/MaxY). Set that first!");
            }

            if(safe)
            {
                if(this.layout.Width +deltaX < 30)
                {
                    deltaX = -(this.layout.Width - 30);
                }
                if (this.layout.Height + deltaY < 30)
                {
                    deltaY = -(this.layout.Height - 30);
                }
            }

            margin[MinX] = margin[MinX];
            margin[MinY] = margin[MinY];
            margin[MaxX] = margin[MaxX] - deltaX;
            margin[MaxY] = margin[MaxY] - deltaY;

            this.GetLayout().SetDirty();
        }


        public virtual void Invalidate()
        {
            if(this.Parent != null)
            {
                this.Parent.Invalidate();
            }
        }
    }
}
