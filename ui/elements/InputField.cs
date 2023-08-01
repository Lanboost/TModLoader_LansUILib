using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.UI;

namespace LansUILib.ui.elements
{

    public enum EInputFieldType
    {
        Text,
        Integer,
        Float
    }

    // Class mostly taken from Terraria.ModLoader.UI.UIFocusInputTextField which isnt public...
    public class InputField : UIElement
    {
        internal bool Focused = false;
        internal string CurrentString = "";

        private readonly string _hintText;
        private int _textBlinkerCount;
        private int _textBlinkerState;
        public bool UnfocusOnTab { get; internal set; } = false;

        public EInputFieldType inputType;

        public delegate void EventHandler(object sender, EventArgs e);

        public event EventHandler OnTextChange;
        public event EventHandler OnUnfocus;
        public event EventHandler OnTab;

        public InputField(string hintText, EInputFieldType inputType = EInputFieldType.Text)
        {
            _hintText = hintText;
            this.inputType = inputType;
        }

        public void SetText(string text)
        {
            if (text == null)
                text = "";
            var validText = ValidateText(text);
            if (CurrentString != validText)
            {
                CurrentString = validText;
                OnTextChange?.Invoke(this, new EventArgs());
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            Main.clrInput();
            Focused = true;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (!ContainsPoint(MousePosition) && Main.mouseLeft) // TODO: && focused maybe?
            {
                Focused = false;
                OnUnfocus?.Invoke(this, new EventArgs());
            }
            base.Update(gameTime);
        }
        private static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected string ValidateText(string text)
        {
            if(EInputFieldType.Integer == this.inputType)
            {
                // Strip everything that is not 0-9
                return Regex.Replace(text, "[^$0-9]", "");
            }
            else if (EInputFieldType.Float == this.inputType)
            {
                // Strip everything that is not 0-9
                var stripped = Regex.Replace(text, "[^$0-9.]", "");

                var output = stripped.Split('.');
                return output[0] + '.' + string.Join(" ", output.Skip(1));
            }
            return text;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //Rectangle hitbox = GetInnerDimensions().ToRectangle();
            //Main.spriteBatch.Draw(TextureAssets.MagicPixel, hitbox, Color.Red * 0.6f);

            if (Focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(CurrentString);
                if (!newString.Equals(CurrentString))
                {
                    newString = ValidateText(newString);
                    if (!newString.Equals(CurrentString))
                    {
                        CurrentString = newString;
                        OnTextChange?.Invoke(this, new EventArgs());
                    }
                }
                else
                {
                    CurrentString = newString;
                }
                if (JustPressed(Keys.Tab))
                {
                    if (UnfocusOnTab)
                    {
                        Focused = false;
                        OnUnfocus?.Invoke(this, new EventArgs());
                    }
                    OnTab?.Invoke(this, new EventArgs());
                }
                if (++_textBlinkerCount >= 20)
                {
                    _textBlinkerState = (_textBlinkerState + 1) % 2;
                    _textBlinkerCount = 0;
                }
            }
            string displayString = CurrentString;
            if (_textBlinkerState == 1 && Focused)
            {
                displayString += "|";
            }
            CalculatedStyle space = GetDimensions();
            if (CurrentString.Length == 0 && !Focused)
            {
                Utils.DrawBorderString(spriteBatch, _hintText, new Vector2(space.X, space.Y), Color.Gray);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), Color.White);
            }
        }
    }
}