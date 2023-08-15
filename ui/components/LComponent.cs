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

        protected PanelSettings panelSettings;

        protected List<LComponent> children = new List<LComponent>();

        protected Layout layout;
        public LImage image;
        public LImage border;
        public bool isMask;
        public string text;
        public ILColor textColor;

        protected LComponent _parent = null;
        public string name;

        public LComponent(string name, PanelSettings settings = null)
        {
            SetLayout(Layout.None());
            this.name = name;
            this.panelSettings = settings;
            if(this.panelSettings == null)
            {
                this.panelSettings = new PanelSettings();
            }
            
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
            panelSettings.SetMargin(side, value);

            this.GetLayout().SetDirty();
        }

        public virtual int GetMargin(int side)
        {
            return panelSettings.GetMargin(side);
        }

        public virtual float GetPivot(int side)
        {
            return panelSettings.GetPivot(side);
        }

        public virtual void SetMargins(int minX, int minY, int maxX, int maxY)
        {
            panelSettings.SetMargins(minX, minY, maxX, maxY);

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
            panelSettings.SetAnchor(side, value);

            this.GetLayout().SetDirty();
        }

        public virtual void SetAnchors(float minX, float minY, float maxX, float maxY)
        {
            panelSettings.SetAnchors(minX, minY, maxX, maxY);

            this.GetLayout().SetDirty();
        }

        public virtual float GetAnchor(int side)
        {
            return panelSettings.GetAnchor(side);
        }

        public virtual void SetAnchor(AnchorPosition anchorPosition, bool movePivot = true)
        {
            panelSettings.SetAnchor(anchorPosition, movePivot);
        }

        public virtual void SetMargin(int minX, int minY, int maxX, int maxY)
        {
            panelSettings.SetMargin(minX, minY, maxX, maxY);

            this.GetLayout().SetDirty();
        }

        public virtual void SetSize(int x, int y, int width, int height)
        {
            panelSettings.SetSize(x, y, width, height);
            this.GetLayout().SetDirty();
        }

        public virtual int[] GetSize()
        {
            return panelSettings.GetSize();
        }

        public virtual void Move(int deltaX, int deltaY, bool safe = true)
        {
            if (panelSettings.anchor[MinX] != panelSettings.anchor[MaxX] || panelSettings.anchor[MinY] != panelSettings.anchor[MaxY])
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


            panelSettings.margin[MinX] = panelSettings.margin[MinX] + deltaX;
            panelSettings.margin[MinY] = panelSettings.margin[MinY] + deltaY;
            panelSettings.margin[MaxX] = panelSettings.margin[MaxX] - deltaX;
            panelSettings.margin[MaxY] = panelSettings.margin[MaxY] - deltaY;


            this.GetLayout().SetDirty();
        }

        public virtual void Resize(int deltaX, int deltaY, bool safe = true)
        {
            if (panelSettings.anchor[MinX] != panelSettings.anchor[MaxX] || panelSettings.anchor[MinY] != panelSettings.anchor[MaxY])
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

            panelSettings.margin[MinX] = panelSettings.margin[MinX];
            panelSettings.margin[MinY] = panelSettings.margin[MinY];
            panelSettings.margin[MaxX] = panelSettings.margin[MaxX] - deltaX;
            panelSettings.margin[MaxY] = panelSettings.margin[MaxY] - deltaY;

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
