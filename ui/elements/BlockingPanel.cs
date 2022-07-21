using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LansUILib.ui
{
    public class BlockingPanel : UIElement
    {
        bool blockingOnHover = true;
        public BlockingPanel()
        {
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if(blockingOnHover)
            {
                if(this.IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
            base.DrawSelf(spriteBatch);
        }
    }
}
