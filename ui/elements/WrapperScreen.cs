using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LansUILib.ui.components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace LansUILib.ui
{



    public struct MouseState
    {
        public int x;
        public int y;
        public int deltaX;
        public int deltaY;
        public bool[] mouseTriggerDown;
        public bool[] mouseTriggerUp;
        public bool[] mouseIsDown;
        public int scroll;


        public bool AnyButtonDown()
        {
            return mouseIsDown.Contains(true);
        }
    }

    public abstract class Screen
    {
        public BaseComponent baseComponent;

        protected LComponent currentMouseTarget = null;
        protected bool[] mouseState = new bool[] { false, false, false };

        public bool shouldInvalidate = false;

        public Screen()
        {
            baseComponent = new BaseComponent(this);
        }


        public void Add(LComponent component)
        {
            baseComponent.Add(component);
        }

        public void Remove(LComponent component)
        {
            baseComponent.Remove(component);
        }

        public void ScreenSizeChanged(int width, int heigth)
        {
            baseComponent.SetScreenSize(width, heigth);
        }

        public LComponent GetMouseTarget(MouseState mouse, LComponent component)
        {
            foreach (var child in component.Children)
            {
                var res = GetMouseTarget(mouse, child);
                if(res != null)
                {
                    return res;
                }
            }

            if(component.ContainsMouse(mouse) && component.MouseInteraction)
            {
                return component;
            }
            return null;
        }

        public void Update()
        {
            while(shouldInvalidate)
            {
                shouldInvalidate = false;
                baseComponent.GetLayout().Update();


            }

            //var s = this.PrintJson();
            //var t = 0;

            var mouse = getMouseState();

            var currentTarget = GetMouseTarget(mouse, baseComponent);

            if (currentMouseTarget != null && (mouse.deltaX != 0 || mouse.deltaY != 0))
            {
                currentMouseTarget.fireMouseMove(mouse);
            }

            if (currentTarget != currentMouseTarget)
            {
                // If we have a mouse down we should not change target
                if (currentMouseTarget != null)
                {
                    var anyButtonIsHeldDown = mouse.mouseIsDown.Contains(true);
                    if(anyButtonIsHeldDown)
                    {
                        if(mouse.mouseTriggerDown.Contains(true))
                        {
                            currentMouseTarget.fireMouseDown(mouse);
                        }
                        if (mouse.mouseTriggerUp.Contains(true))
                        {
                            currentMouseTarget.fireMouseUp(mouse);
                        }

                        mouseState = mouse.mouseIsDown;

                        return;
                    }
                    else
                    {
                        // Release previous target
                        if (mouseState.Contains(true))
                        {
                            currentMouseTarget.fireMouseUp(mouse);
                        }

                        currentMouseTarget.fireMouseExit(mouse);

                        mouseState = mouse.mouseIsDown;
                    }
                }

                currentMouseTarget = currentTarget;
                if (currentMouseTarget != null)
                {
                    currentMouseTarget.fireMouseEnter(mouse);
                    if (mouse.mouseTriggerDown.Contains(true))
                    {
                        currentMouseTarget.fireMouseDown(mouse);
                    }
                    mouseState = mouse.mouseIsDown;
                }
            }
            else
            {
                if (currentMouseTarget != null)
                {
                    if (mouse.mouseTriggerDown.Contains(true))
                    {
                        currentMouseTarget.fireMouseDown(mouse);
                    }
                    if (mouse.mouseTriggerUp.Contains(true))
                    {
                        currentMouseTarget.fireMouseUp(mouse);
                    }

                    mouseState = mouse.mouseIsDown;
                }
            }
        }

        public void Draw()
        {
            Draw(baseComponent);
        }

        public abstract void Draw(LComponent component);

        public abstract MouseState getMouseState(bool skipUpdate = false);


        class IdLComponent
        {
            public int id;
            public LComponent component;

            public IdLComponent(int id, LComponent component)
            {
                this.id = id;
                this.component = component;
            }
        }

        public string PrintJson()
        {
            var id = -1;
            var json = new StringBuilder();
            Queue<IdLComponent> todo = new Queue<IdLComponent>();
            todo.Enqueue(new IdLComponent(-1, baseComponent));
            while (todo.Count != 0)
            {
                var elem = todo.Dequeue();
                var myId = id++;
                var layout = elem.component.GetLayout();
                json.Append($"{myId},{elem.id},{elem.component.name},{layout.X},{layout.Y},{layout.Width},{layout.Height}\n");
                foreach(var c in elem.component.Children)
                {
                    todo.Enqueue(new IdLComponent(myId, c));
                }
            }
            return json.ToString();

        }
    }

    public class BaseComponent : LComponent
    {
        Screen screen;
        public BaseComponent(Screen screen) :base("Screen")
        {
            this.screen = screen;
        }

        public void SetScreenSize(int width, int height)
        {
            this.GetLayout().Width = width;
            this.GetLayout().Height = height;
            this.Invalidate();
        }

        public override void Invalidate()
        {
            screen.shouldInvalidate = true;
        }
    }

    public class WrapperLColor: ILColor
    {
        public Color value;

        public WrapperLColor(Color value)
        {
            this.value = value;
        }

        public T GetValue<T>()
        {
            return (T)(object)value;
        }
    }

    public class WrapperLSprite : ILSprite
    {
        public Asset<Texture2D> value;
        public DrawAnimation animation;
        public Rectangle? rectangle;
        protected CornerBox animationBox = new CornerBox(0,0,0,0);

        public WrapperLSprite(Asset<Texture2D> value, DrawAnimation animation = null, Rectangle? rectangle = null)
        {
            this.value = value;
            this.animation = animation;
            this.rectangle = rectangle;
        }

        public bool Animated()
        {
            return animation != null || rectangle != null;
        }

        public CornerBox GetCurrentAnimationOffset()
        {
            if (value.Value != null) {
                if (animation != null)
                {
                    var vertical = animation as DrawAnimationVertical;
                    if (vertical != null)
                    {
                        var frameSize = value.Value.Height / vertical.FrameCount;

                        var frameId = vertical.Frame;
                        if (vertical.PingPong && vertical.Frame >= vertical.FrameCount)
                            frameId = vertical.FrameCount * 2 - 2 - vertical.Frame;

                        animationBox.Top = frameId * frameSize;
                        animationBox.Bottom = value.Value.Height - animationBox.Top - frameSize;
                    }
                    else
                    {

                    }
                }
                else if(rectangle != null)
                {
                    animationBox.Top = rectangle.Value.Top;
                    animationBox.Left = rectangle.Value.Left;
                    animationBox.Right = value.Value.Width - animationBox.Left - rectangle.Value.Width;
                    animationBox.Bottom = value.Value.Height - animationBox.Top - rectangle.Value.Height;

                }
            }
            return animationBox;
        }

        public T GetValue<T>()
        {
            return (T)(object)value;
        }
    }

    public class WrapperScreen : Screen
    {
        protected int mouseX = 0;
        protected int mouseY = 0;
        protected bool[] mouseState = new bool[] {false, false, false};


        public override void Draw(LComponent component)
        {
            Draw(component, Main.spriteBatch);
        }


        private Rectangle AbsoluteCoordinatesToRectangle(int x1, int y1, int x2, int y2)
        {
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        private Rectangle[] CreateBorderSlices(int[] x, int[] y)
        {
            var xindices = new int[] { 0, 1, 1, 2, 2, 3, 2, 3, 2, 3, 1, 2, 0, 1, 0, 1 };
            var yindices = new int[] { 0, 1, 0, 1, 0, 1, 1, 2, 2, 3, 2, 3, 2, 3, 1, 2 };

            var result = new Rectangle[xindices.Length/2];
            for(int i=0; i<xindices.Length/2; i++)
            {
                result[i] = AbsoluteCoordinatesToRectangle(x[xindices[i * 2]], y[yindices[i * 2]], x[xindices[i * 2 + 1]], y[yindices[i * 2+1]]);
            }
            return result;
        }

        protected void DrawImageNormal(LComponent component, LImage image, SpriteBatch spriteBatch)
        {
            var color = image.Color.GetValue<Color>();
            var textureAsset = image.Sprite.GetValue<Asset<Texture2D>>();

            if (textureAsset.Value != null)
            {
                var textureWidth = textureAsset.Value.Width;
                var textureHeight = textureAsset.Value.Height;
                CornerBox offsets = new CornerBox(0,0,0,0);
                if (image.Sprite.Animated())
                {
                    offsets = image.Sprite.GetCurrentAnimationOffset();
                    textureHeight = textureHeight - offsets.Top - offsets.Bottom;
                }
                var offsetX = component.GetLayout().X + (component.GetLayout().Width - textureWidth) / 2;
                var offsetY = component.GetLayout().Y + (component.GetLayout().Height - textureHeight) / 2;
                
                spriteBatch.Draw(textureAsset.Value,
                    new Rectangle(offsetX, offsetY, textureWidth, textureHeight),
                    new Rectangle(offsets.Left, offsets.Top, textureWidth, textureHeight)
                , color);
                
                
            }
        }

        protected void DrawImageStretch(LComponent component, LImage image, SpriteBatch spriteBatch)
        {
            var color = image.Color.GetValue<Color>();
            var textureAsset = image.Sprite.GetValue<Asset<Texture2D>>();

            if (textureAsset.Value != null)
            {
                var textureWidth = textureAsset.Value.Width;
                var textureHeight = textureAsset.Value.Height;
                CornerBox offsets = new CornerBox(0, 0, 0, 0);
                if (image.Sprite.Animated())
                {
                    offsets = image.Sprite.GetCurrentAnimationOffset();
                    textureHeight = textureHeight - offsets.Top - offsets.Bottom;
                    textureWidth = textureWidth - offsets.Left - offsets.Right;
                }
                var offsetX = component.GetLayout().X + (component.GetLayout().Width - textureWidth) / 2;
                var offsetY = component.GetLayout().Y + (component.GetLayout().Height - textureHeight) / 2;

                spriteBatch.Draw(textureAsset.Value,
                    new Rectangle(component.GetLayout().X, component.GetLayout().Y, component.GetLayout().Width, component.GetLayout().Height),
                    new Rectangle(offsets.Left, offsets.Top, textureWidth, textureHeight)
                , color);

            }
        }

        protected void DrawText(LComponent component, SpriteBatch spriteBatch)
        {
            DynamicSpriteFont value = FontAssets.MouseText.Value;
            Vector2 vector = value.MeasureString(component.text);

            Utils.DrawBorderString(spriteBatch, component.text, new Vector2(component.GetLayout().X +(component.GetLayout().Width - vector.X)/2, component.GetLayout().Y + (component.GetLayout().Height - vector.Y) / 2), component.textColor.GetValue<Color>(),1,0,-0.1f);
        }

        protected void DrawImageCornerBox(LComponent component, LImage image, SpriteBatch spriteBatch)
        {


            var left = component.GetLayout().X;
            var leftCorner = left + image.CornerBox.Left;
            var right = left + component.GetLayout().Width;
            var rightCorner = right - image.CornerBox.Right;

            var top = component.GetLayout().Y;
            var topCorner = top + image.CornerBox.Top;
            var bottom = top + component.GetLayout().Height;
            var bottomCorner = bottom - image.CornerBox.Bottom;


            var color = image.Color.GetValue<Color>();
            var textureAsset = image.Sprite.GetValue<Asset<Texture2D>>();

            if(textureAsset.Value != null)
            {
                var texture = textureAsset.Value;

                var spriteLeft = 0;
                var spriteLeftCorner = image.CornerBox.Left;
                var spriteRightCorner = texture.Width-image.CornerBox.Right;
                var spriteRight = texture.Width;
                var spriteTop = 0;
                var spriteTopCorner = image.CornerBox.Top;
                var spriteBottomCorner = texture.Height - image.CornerBox.Bottom;
                var spriteBottom = texture.Height;

                var slices = CreateBorderSlices(new int[] { left, leftCorner, rightCorner, right }, new int[] { top, topCorner, bottomCorner, bottom });
                var spriteSlices = CreateBorderSlices(new int[] { spriteLeft, spriteLeftCorner, spriteRightCorner, spriteRight }, new int[] { spriteTop, spriteTopCorner, spriteBottomCorner, spriteBottom });

                for(int i=0; i<8; i++)
                {
                    spriteBatch.Draw(texture, slices[i], spriteSlices[i], color);
                }
                spriteBatch.Draw(texture, AbsoluteCoordinatesToRectangle(leftCorner, topCorner, rightCorner, bottomCorner), AbsoluteCoordinatesToRectangle(spriteLeftCorner, spriteTopCorner, spriteRightCorner, spriteBottomCorner), color);

            }
        }

        public void Draw(LComponent component, SpriteBatch spriteBatch)
        {
            if (component.image != null)
            {
                if (component.image.FillMode == ImageFillMode.CornerBox)
                {
                    DrawImageCornerBox(component, component.image, spriteBatch);
                }
                else if (component.image.FillMode == ImageFillMode.Normal)
                {
                    DrawImageNormal(component, component.image, spriteBatch);
                }
                else if (component.image.FillMode == ImageFillMode.Stretch)
                {
                    DrawImageStretch(component, component.image, spriteBatch);
                }
            }
            if (component.border != null)
            {
                if (component.border.FillMode == ImageFillMode.CornerBox)
                {
                    DrawImageCornerBox(component, component.border, spriteBatch);
                }
                else if (component.border.FillMode == ImageFillMode.Normal)
                {
                    DrawImageNormal(component, component.border, spriteBatch);
                }
                else if (component.border.FillMode == ImageFillMode.Stretch)
                {
                    DrawImageStretch(component, component.border, spriteBatch);
                }
            }

            if(component.isMask)
            {
                RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
                var enabled = rasterizerState.ScissorTestEnable;

                

                RasterizerState r = new RasterizerState();

                r.ScissorTestEnable = true;


                int left = Math.Max(0,component.GetLayout().X);
                int top = Math.Max(0, component.GetLayout().Y);
                int width = Math.Min(component.GetLayout().Width, spriteBatch.GraphicsDevice.Viewport.Width - left);
                int height = Math.Min(component.GetLayout().Height, spriteBatch.GraphicsDevice.Viewport.Height - top);
                var start = Vector2.Transform(new Vector2(left, top), Main.UIScaleMatrix);
                var end = Vector2.Transform(new Vector2(width, height), Main.UIScaleMatrix);
                var rect = new Rectangle((int)start.X, (int)start.Y, (int)end.X, (int)end.Y);


                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, r, null, Main.UIScaleMatrix);

                
                var combinedLeft = Math.Max(scissorRectangle.X, rect.X);
                var combinedTop = Math.Max(scissorRectangle.Y, rect.Y);
                var combinedRight = Math.Min(scissorRectangle.X+ scissorRectangle.Width, rect.X+rect.Width);
                var combinedBottom = Math.Min(scissorRectangle.Y+ scissorRectangle.Height, rect.Y+rect.Height);

                spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                    combinedLeft,
                    combinedTop,
                    combinedRight- combinedLeft,
                    combinedBottom- combinedTop
                );


                foreach (var childComp in component.Children)
                {
                    Draw(childComp, spriteBatch);
                }

                spriteBatch.End();

                
                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, rasterizerState, null, Main.UIScaleMatrix);
                spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                rasterizerState.ScissorTestEnable = enabled;
                
            }
            else
            {
                if (component.text != null)
                {
                    DrawText(component, spriteBatch);
                }
                foreach (var childComp in component.Children)
                {
                    Draw(childComp, spriteBatch);
                }
            }
        }

        public override MouseState getMouseState(bool skipUpdate = false)
        {
            int mouseX = 0;
            int mouseY = 0;
            bool[] buttonDown = null;

            if(!Main.hasFocus)
            {
                buttonDown = new bool[] { false, false, false };
                mouseX = -100;
                mouseY = -100;
            }
            else
            {
                buttonDown = new bool[] { Main.mouseLeft, Main.mouseRight, Main.mouseMiddle };
                mouseX = Main.mouseX;
                mouseY = Main.mouseY;
            }

            var state = new MouseState();
            state.x = mouseX;
            state.y = mouseY;
            state.deltaX = mouseX-this.mouseX;
            state.deltaY = mouseY-this.mouseY;
            state.scroll = PlayerInput.ScrollWheelDeltaForUI;
            state.mouseIsDown = buttonDown;
            state.mouseTriggerDown = new bool[] { !mouseState[0] && buttonDown[0], !mouseState[1] && buttonDown[1], !mouseState[2] && buttonDown[2] };
            state.mouseTriggerUp = new bool[] { mouseState[0] && !buttonDown[0], mouseState[1] && !buttonDown[1], mouseState[2] && !buttonDown[2] };

            if (!skipUpdate)
            {
                this.mouseX = mouseX;
                this.mouseY = mouseY;
                this.mouseState = buttonDown;
            }

            foreach(var child in baseComponent.Children)
            {
                if(child.ContainsMouse(state))
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }

            return state;
        }
    }
}
