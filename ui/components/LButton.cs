using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LansUILib.ui.LComponent;

namespace LansUILib.ui.components
{
    public class LButton
    {
        public event LMouseEvent OnClicked;

        public LComponent Panel;
        public LComponent Text;

        public LButton(LComponent panel, LComponent text, ILColor normalColor, ILColor hoverColor, ILColor interactColor)
        {
            Panel = panel;

            var mouseDownX = 0;
            var mouseDownY = 0;

            Panel.image.Color = normalColor;

            Panel.MouseEnter += delegate (MouseState state)
            {
                Panel.image.Color = hoverColor;
            };

            Panel.MouseExit += delegate (MouseState state)
            {
                Panel.image.Color = normalColor;
            };

            Panel.MouseDown += delegate (MouseState state)
            {
                mouseDownX = state.x;
                mouseDownY = state.y;
                Panel.image.Color = interactColor;
            };

            Panel.MouseUp += delegate (MouseState state)
            {
                if(Math.Abs(mouseDownX-state.x) < 5 && Math.Abs(mouseDownY - state.y) < 5)
                {
                    OnClicked?.Invoke(state);
                }

                if (!state.AnyButtonDown())
                {
                    Panel.image.Color = hoverColor;
                }
            };
            Text = text;
        }
    }
}
