using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Humanizer.On;

namespace LansUILib.ui
{
    public class PanelSettings
    {
        public float[] anchor = new float[] { 0, 0, 1, 1 };
        public int[] margin = new int[LComponent.SIDE_COUNT];
        public float[] pivot = new float[2] { 0.5f, 0.5f };

        public virtual void SetMargin(int side, int value)
        {
            margin[side] = value;
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
            margin[LComponent.MinX] = minX;
            margin[LComponent.MinY] = minY;
            margin[LComponent.MaxX] = maxX;
            margin[LComponent.MaxY] = maxY;
        }

        public virtual void SetAnchor(int side, float value)
        {
            if (value < 0 || value > 1)
            {
                throw new Exception("Value range is 0-1");
            }
            anchor[side] = value;
        }

        public virtual void SetAnchors(float minX, float minY, float maxX, float maxY)
        {
            anchor[LComponent.MinX] = minX;
            anchor[LComponent.MinY] = minY;
            anchor[LComponent.MaxX] = maxX;
            anchor[LComponent.MaxY] = maxY;
        }

        public virtual float GetAnchor(int side)
        {
            return anchor[side];
        }

        public virtual void SetAnchor(AnchorPosition anchorPosition, bool movePivot = true)
        {
            if (anchorPosition == AnchorPosition.TopLeft)
            {
                SetAnchors(0, 0, 0, 0);
                if (movePivot)
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
            margin[LComponent.MinX] = minX;
            margin[LComponent.MinY] = minY;
            margin[LComponent.MaxX] = maxX;
            margin[LComponent.MaxY] = maxY;
        }

        public virtual void SetSize(int x, int y, int width, int height)
        {
            if (anchor[LComponent.MinX] != anchor[LComponent.MaxX] || anchor[LComponent.MinY] != anchor[LComponent.MaxY])
            {
                throw new Exception("Size only works correctly if you have anchored to the same (MinX/MaxX and MinY/MaxY). Set that first!");
            }

            margin[LComponent.MinX] = x;
            margin[LComponent.MinY] = y;
            margin[LComponent.MaxX] = -x - width;
            margin[LComponent.MaxY] = -y - height;

        }

        public virtual int[] GetSize()
        {
            if (anchor[LComponent.MinX] != anchor[LComponent.MaxX] || anchor[LComponent.MinY] != anchor[LComponent.MaxY])
            {
                throw new Exception("Size only works correctly if you have anchored to the same (MinX/MaxX and MinY/MaxY). Set that first!");
            }

            var x = margin[LComponent.MinX];
            var y = margin[LComponent.MinY];
            var width = -margin[LComponent.MaxX] - x;
            var height = -margin[LComponent.MaxY] - y;

            return new int[] { x, y, width, height };
        }
    }
}
