using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace LansUILib.ui
{
    public class WrapperLayout : Layout
    {
        Asset<Texture2D> asset;
        DrawAnimation drawAnimation;
        Rectangle? rectangle;
        public WrapperLayout(Asset<Texture2D> asset, DrawAnimation drawAnimation = null, Rectangle? rectangle = null)
        {
            this.asset = asset;
            this.drawAnimation = drawAnimation;
            this.rectangle = rectangle;
        }

        public override int[] GetPrefferedSize()
        {
            if (this.asset.IsLoaded)
            {
                var t = this.asset.Value;

                if (drawAnimation != null)
                {
                    return new int[] { t.Width, t.Height / drawAnimation.FrameCount };
                }
                else if(rectangle != null)
                {
                    return new int[] { rectangle.Value.Width, rectangle.Value.Height  };
                }
                else
                {
                    return new int[] { t.Width, t.Height };
                }
            }
            else
            {
                return new int[] { 32, 32 };
            }
        }
    }

    public class WrapperLayoutText : Layout
    {
        
        public override int[] GetPrefferedSize()
        {
            DynamicSpriteFont value = FontAssets.MouseText.Value;
            Vector2 vector = value.MeasureString(component.text);

            return new int[] { (int)vector.X, (int)vector.Y };
        }
    }
}
