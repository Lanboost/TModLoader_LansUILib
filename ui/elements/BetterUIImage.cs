using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LansUILib.ui
{
    public class BetterUIImage:UIElement
    {
        protected Asset<Texture2D> _texture;
        public float ImageScale = 1f;
        public float Rotation;
        public bool ScaleToFit;
        public bool AllowResizingDimensions = true;
        public Color Color = Color.White;
        public Vector2 NormalizedOrigin = Vector2.Zero;
        public bool RemoveFloatingPointsFromDrawPosition;

        bool blockingOnHover = false;
        DrawAnimation drawAnimation;

        public BetterUIImage(Asset<Texture2D> texture, DrawAnimation drawAnimation = null, bool blockingOnHover = false)
        {
            SetImage(texture);
            this.blockingOnHover = blockingOnHover;
            this.drawAnimation = drawAnimation;
        }

        public void SetImage(Asset<Texture2D> texture)
        {
            _texture = texture;
            if (AllowResizingDimensions)
            {
                Width.Set(_texture.Width(), 0f);
                Height.Set(_texture.Height(), 0f);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (blockingOnHover)
            {
                if (this.IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }

            CalculatedStyle dimensions = GetDimensions();
            Texture2D texture2D = null;
            Rectangle sourceRect = new Rectangle();
            if (_texture != null) {
                texture2D = _texture.Value;

                if (drawAnimation != null)
                {
                    sourceRect = drawAnimation.GetFrame(texture2D);
                }
                else
                {
                    sourceRect = new Rectangle(0,0, texture2D.Width, texture2D.Height);
                }
            }

            if (ScaleToFit)
            {
                spriteBatch.Draw(texture2D, dimensions.ToRectangle(), sourceRect, Color);
                return;
            }

            Vector2 vector = sourceRect.Size();
            Vector2 vector2 = dimensions.Position() + vector * (1f - ImageScale) / 2f + vector * NormalizedOrigin;
            if (RemoveFloatingPointsFromDrawPosition)
                vector2 = vector2.Floor();

            spriteBatch.Draw(texture2D, vector2, sourceRect, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
        }
    }
}
