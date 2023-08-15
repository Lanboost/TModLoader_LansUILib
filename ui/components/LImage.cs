using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LansUILib.ui.components
{
    public interface ILColor
    {
        public T GetValue<T>();
    }

    public interface ILSprite
    {
        public T GetValue<T>();
        public bool Animated();
        public CornerBox GetCurrentAnimationOffset();
    }

    public enum ImageFillMode
    {
        Normal,
        Stretch,
        CornerBox
    }

    public class CornerBox
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public CornerBox(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    public class LImage
    {
        public ILColor Color;
        public ILSprite Sprite;
        public ImageFillMode FillMode;
        public CornerBox CornerBox;
        public Nullable<Vector2> MaxSize;

        public LImage(ILColor color, ILSprite sprite, CornerBox cornerBox)
        {
            Color = color;
            Sprite = sprite;
            CornerBox = cornerBox;
            FillMode = ImageFillMode.CornerBox;
        }

        public LImage(ILColor color, ILSprite sprite, ImageFillMode fillMode = ImageFillMode.Stretch, Nullable<Vector2> maxSize = null)
        {
            Color = color;
            Sprite = sprite;
            FillMode = fillMode;
            MaxSize = maxSize;
        }
    }
}
