﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LansUILib.ui
{
    // This UIHoverImageButton class inherits from UIImageButton. 
    // Inheriting is a great tool for UI design. 
    // By inheriting, we get the Image drawing, MouseOver sound, and fading for free from UIImageButton
    // We've added some code to allow the Button to show a text tooltip while hovered. 
    public class UIHoverImageToggleButton : UIImageButton
    {
        internal string HoverTextChecked;
        internal string HoverTextUnchecked;

        bool _IsChecked = false;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                if (_IsChecked)
                {
                    base.SetImage(texture_checked);
                }
                else
                {
                    base.SetImage(texture_unchecked);
                }

            }
        }

        public delegate void CheckEvent(bool val);

        public event CheckEvent OnChecked;

        Asset<Texture2D> texture_checked;
        Asset<Texture2D> texture_unchecked;
        public UIHoverImageToggleButton(Asset<Texture2D> texture_checked, Asset<Texture2D> texture_unchecked, string hoverTextchecked, string hoverTextunchecked) : base(texture_unchecked)
        {
            HoverTextChecked = hoverTextchecked;
            HoverTextUnchecked = hoverTextunchecked;
            this.OnLeftClick += new MouseEvent(PlayButtonClicked);
            this.texture_checked = texture_checked;
            this.texture_unchecked = texture_unchecked;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            /*
            if (IsMouseHovering)
            {

                TooltipPanel.Instance.Show(this);
                TooltipPanel.Instance.X = Main.mouseX;
                TooltipPanel.Instance.Y = Main.mouseY;
                



            }
            else
            {
                TooltipPanel.Instance.Hide(this);
            }*/
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                
            }
        }

        private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            this.IsChecked = !IsChecked;

            OnChecked?.Invoke(this.IsChecked);
        }
    }
}