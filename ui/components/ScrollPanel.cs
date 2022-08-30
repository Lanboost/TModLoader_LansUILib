using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LansUILib.ui.components
{
    public class ScrollPanel
    {
        public Scrollbar horizontal;
        public Scrollbar vertical;
        public LComponent wrapper;
        public LComponent mask;
        public LComponent contentPanel;

        public int[] currentSize = new int[] { -1, -1, -1, -1 };

        public ScrollPanel(Scrollbar vertical, LComponent wrapper, LComponent mask, LComponent contentPanel)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
            this.wrapper = wrapper;
            this.mask = mask;
            this.contentPanel = contentPanel;

            vertical.ValueChanged += delegate ()
            {
                var pixelCount = contentPanel.GetLayout().Height -mask.GetLayout().Height;
                if (pixelCount < 0)
                {
                    contentPanel.SetMargins(0, 0, 0, -(contentPanel.GetLayout().Height));
                }
                else
                {
                    var offset = (int)(pixelCount * vertical.Value);
                    contentPanel.SetMargins(0, -offset, 0, -(contentPanel.GetLayout().Height - offset));
                }
            };

            contentPanel.GetLayout().OnUpdate += delegate ()
            {
                var l1 = mask.GetLayout();
                var l2 = contentPanel.GetLayout();
                if(l1.Width != currentSize[0] || l1.Height != currentSize[1] || l2.Width != currentSize[2] || l2.Height != currentSize[3])
                {
                    vertical.Value = 0;
                    vertical.HandleFraction = getVerticalFractionSize();
                    vertical.UpdateHandle();
                    currentSize = new int[] { l1.Width, l1.Height, l2.Width, l2.Height };
                }
            };

            mask.GetLayout().OnUpdate += delegate ()
            {
                var l1 = mask.GetLayout();
                var l2 = contentPanel.GetLayout();
                if (l1.Width != currentSize[0] || l1.Height != currentSize[1] || l2.Width != currentSize[2] || l2.Height != currentSize[3])
                {
                    vertical.Value = 0;
                    vertical.HandleFraction = getVerticalFractionSize();
                    vertical.UpdateHandle();
                    currentSize = new int[] { l1.Width, l1.Height, l2.Width, l2.Height };
                }
            };
        }

        public float getVerticalFractionSize()
        {
            return Math.Min(1, Math.Max(0.05f, ((float)mask.GetLayout().Height) / contentPanel.GetLayout().Height));
        }


    }
}
