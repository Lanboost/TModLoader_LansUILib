using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using static LansUILib.ui.LComponent;

namespace LansUILib.ui
{

    public class Layout
    {
        public delegate void UpdateEvent();

        public event UpdateEvent OnUpdate;

        public static Layout None()
        {
            return new Layout();
        }

        public virtual bool ControlsChildren()
        {
            return false;
        }

        public int Flex = 0;

        protected LComponent component;
        bool dirty = false;
        public void SetComponentOwner(LComponent component)
        {
            this.component = component;
        }

        public virtual int[] GetPrefferedSize()
        {
            return new int[] { 0, 0 };
        }

        public virtual void Refresh()
        {
            OnUpdate?.Invoke();
            //if(dirty)
            {
                foreach (var child in component.Children)
                {
                    child.GetLayout().Update();
                }
                //dirty = false;
            }
        }

        public virtual void Update()
        {
            var pLayout = component.Parent?.GetLayout();
            if (pLayout != null && !pLayout.ControlsChildren())
            {
                var x = pLayout.X + pLayout.Width * component.GetAnchor(LComponent.MinX) + component.GetMargin(LComponent.MinX);
                var y = pLayout.Y + pLayout.Height * component.GetAnchor(LComponent.MinY) + component.GetMargin(LComponent.MinY);
                var maxX = pLayout.X + pLayout.Width * component.GetAnchor(LComponent.MaxX) - component.GetMargin(LComponent.MaxX);
                var maxY = pLayout.Y + pLayout.Height * component.GetAnchor(LComponent.MaxY) - component.GetMargin(LComponent.MaxY);

                var width = Math.Max(0, maxX - x);
                var height = Math.Max(0, maxY - y);

                this._x = (int)x;
                this._y = (int)y;
                this._width = (int)width;
                this._height = (int)height;
                this.dirty = true;
            }
            this.Refresh();
        }

        public void SetDirty()
        {
            this.dirty = true;
        }

        /*
        public static void CalculateDefaultSize(LComponent component)
        {
            var x = component.Parent.X + component.Parent.Width * component.GetAnchor(LComponent.MinX) + component.GetMargin(LComponent.MinX);
            var y = component.Parent.Y + component.Parent.Height * component.GetAnchor(LComponent.MinY) + component.GetMargin(LComponent.MinY);
            var maxX = component.Parent.X + component.Parent.Width * component.GetAnchor(LComponent.MaxX) - component.GetMargin(LComponent.MaxX);
            var maxY = component.Parent.Y + component.Parent.Height * component.GetAnchor(LComponent.MaxY) - component.GetMargin(LComponent.MaxY);

            var width = Math.Max(0, maxX - x);
            var height = Math.Max(0, maxY - y);

            component.Width = (int)width;
            component.Height = (int)height;
        }

        public static void CalculateDefaultPosition(LComponent component)
        {
            var x = component.Parent.X + component.Parent.Width * component.GetAnchor(LComponent.MinX) + component.GetMargin(LComponent.MinX);
            var y = component.Parent.Y + component.Parent.Height * component.GetAnchor(LComponent.MinY) + component.GetMargin(LComponent.MinY);

            component.X = (int)x;
            component.Y = (int)y;
        }*/


        // used internaly to cache position and size
        protected int _x = 0;
        protected int _y = 0;
        protected int _width = 0;
        protected int _height = 0;

        public virtual int X
        {
            get { return _x; }
            set
            {
                if(_x != value)
                {
                    this.dirty = true;
                }
                _x = value;
            }
        }

        public virtual int Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    this.dirty = true;
                }
                _y = value;
            }
        }

        public virtual int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    this.dirty = true;
                }
                _width = value;
            }
        }

        public virtual int Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    this.dirty = true;
                }
                _height = value;
            }
        }
    }

    public class LayoutSize : Layout
    {
        int width;
        int height;
        public LayoutSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override int[] GetPrefferedSize()
        {
            return new int[] { width, height };
        }
    }

}
