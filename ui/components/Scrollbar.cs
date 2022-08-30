using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LansUILib.ui.LComponent;

namespace LansUILib.ui.components
{
    

    public class Scrollbar
    {
        public delegate void LValueEvent();

        public event LValueEvent ValueChanged;


        public LComponent scrollbarComponent;
        public LComponent handle;

        protected float _value = 0;
        public float Value
        {
            get { return _value; }
            set { _value = value; ValueChanged?.Invoke(); }
        }

        public float HandleFraction = 1;

        private float mouseValue = 0;
        private int mouseDownX = 0;
        private int mouseDownY = 0;


        public Scrollbar(LComponent scrollbarComponent, LComponent handle, float value, float handleFraction)
        {
            this.scrollbarComponent = scrollbarComponent;
            this.handle = handle;
            Value = value;
            HandleFraction = handleFraction;
            var moving = false;
            handle.MouseDown += delegate (MouseState mouse)
            {
                moving = true;
                mouseValue = Value;
                mouseDownX = mouse.x;
                mouseDownY = mouse.y;
            };
            handle.MouseUp += delegate (MouseState mouse)
            {
                moving = false;
            };
            handle.MouseMove += delegate (MouseState mouse)
            {
                if(moving)
                {
                    float deltaY = (mouse.y-mouseDownY);
                    var pixels = scrollbarComponent.GetLayout().Height - handle.GetLayout().Height;
                    var valueChange = deltaY / pixels;
                    Value = Math.Clamp(mouseValue + valueChange, 0, 1);
                    UpdateHandle();
                    
                }
            };

            UpdateHandle(true);
        }

        public void UpdateHandle(bool skipRefresh = false)
        {
            var topAnchor = (1 - HandleFraction)*Value;
            var bottomAnchor = topAnchor+HandleFraction;
            if (topAnchor != handle.GetAnchor(LComponent.MinY) || bottomAnchor != handle.GetAnchor(LComponent.MaxY))
            {
                handle.SetAnchors(0, topAnchor, 1, bottomAnchor);
                if (!skipRefresh)
                {
                    handle.Invalidate();
                }
            }
        }
    }
}
