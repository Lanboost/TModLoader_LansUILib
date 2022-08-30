using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExampleMod.UI;
using LansUILib.ui;
using LansUILib.ui.components;
using LansUILib.ui.elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.Minimap;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace LansUILib
{

    public class UIFactory
    {

        private static void SetCursorOnHover(LComponent component, string cursor, Vector2 offset)
        {
            component.MouseEnter += delegate (MouseState state)
            {
                UISystem.Instance.SetCursor(ModContent.Request<Texture2D>(cursor, AssetRequestMode.ImmediateLoad), offset);
            };

            component.MouseExit += delegate (MouseState state)
            {
                UISystem.Instance.ClearCursor();
            };
        }
        public static LComponent CreatePanel(string name, bool draggable = true, bool resizeable = true)
        {
            var panel = new LComponent(name);
            panel.MouseInteraction = true;
            var backgroundTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/PanelBackground", AssetRequestMode.ImmediateLoad);
            panel.image = new LImage(new WrapperLColor(new Color(63, 82, 151) * 0.7f), new WrapperLSprite(backgroundTexture), new CornerBox(12, 12, 12, 12));
            var borderTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/PanelBorder", AssetRequestMode.ImmediateLoad);
            panel.border = new LImage(new WrapperLColor(Color.Black), new WrapperLSprite(borderTexture), new CornerBox(12, 12, 12, 12));

            
            
            if (draggable)
            {
                var dragging = false;
                panel.MouseDown += delegate (MouseState state){ dragging = true; };

                panel.MouseUp += delegate (MouseState state) { dragging = false; };

                panel.MouseMove += delegate (MouseState state)
                {
                    if (dragging)
                    {
                        panel.Move(state.deltaX, state.deltaY);
                        panel.Invalidate();
                    }
                };

                SetCursorOnHover(panel, "LansUILib/move", new Vector2(-12, -12));
            }
            if (resizeable)
            {
                {
                    var resize = new LComponent(name + "resize1");
                    resize.MouseInteraction = true;
                    resize.SetAnchors(0, 0, 1, 0);
                    resize.SetMargins(10, 0, 10, -10);
                    resize.border = new LImage(new WrapperLColor(Color.Black), new WrapperLSprite(borderTexture), new CornerBox(12, 12, 12, 12));

                    var dragging = false;
                    resize.MouseDown += delegate (MouseState state) { dragging = true; };

                    resize.MouseUp += delegate (MouseState state) { dragging = false; };

                    resize.MouseMove += delegate (MouseState state)
                    {
                        if (dragging)
                        {
                            panel.Move(0, state.deltaY);
                            panel.Resize(0, -state.deltaY);
                            panel.Invalidate();
                        }
                    };

                    SetCursorOnHover(resize, "LansUILib/resizevertical", new Vector2(-12, -12));
                    panel.Add(resize);
                }
                {
                    var resize = new LComponent(name + "resize2");
                    resize.MouseInteraction = true;
                    resize.SetAnchors(0, 1, 1, 1);
                    resize.SetMargins(10, -10, 10, 0);
                    resize.border = new LImage(new WrapperLColor(Color.Black), new WrapperLSprite(borderTexture), new CornerBox(12, 12, 12, 12));
                    var dragging = false;
                    resize.MouseDown += delegate (MouseState state) { dragging = true; };
                    resize.MouseUp += delegate (MouseState state) { dragging = false; };

                    resize.MouseMove += delegate (MouseState state)
                    {
                        if (dragging)
                        {
                            panel.Resize(0, state.deltaY);
                            panel.Invalidate();
                        }
                    };
                    SetCursorOnHover(resize, "LansUILib/resizevertical", new Vector2(-12, -12));
                    panel.Add(resize);
                }
                {
                    var resize = new LComponent(name + "resize3");
                    resize.MouseInteraction = true;
                    resize.SetAnchors(0, 0, 0, 1);
                    resize.SetMargins(0, 10, -10, 10);
                    resize.border = new LImage(new WrapperLColor(Color.Black), new WrapperLSprite(borderTexture), new CornerBox(12, 12, 12, 12));
                    var dragging = false;
                    resize.MouseDown += delegate (MouseState state) { dragging = true; };
                    resize.MouseUp += delegate (MouseState state) { dragging = false; };

                    resize.MouseMove += delegate (MouseState state)
                    {
                        if (dragging)
                        {
                            panel.Move(state.deltaX, 0);
                            panel.Resize(-state.deltaX, 0);
                            panel.Invalidate();
                        }
                    };
                    SetCursorOnHover(resize, "LansUILib/resizehorizontal", new Vector2(-12, -12));
                    panel.Add(resize);
                }
                {
                    var resize = new LComponent(name + "resize4");
                    resize.MouseInteraction = true;
                    resize.SetAnchors(1, 0, 1, 1);
                    resize.SetMargins(-10, 10, 0, 10);
                    resize.border = new LImage(new WrapperLColor(Color.Black), new WrapperLSprite(borderTexture), new CornerBox(12, 12, 12, 12));
                    var dragging = false;
                    resize.MouseDown += delegate (MouseState state) { dragging = true; };
                    resize.MouseUp += delegate (MouseState state) { dragging = false; };

                    resize.MouseMove += delegate (MouseState state)
                    {
                        if (dragging)
                        {
                            panel.Resize(state.deltaX, 0);
                            panel.Invalidate();
                        }
                    };
                    SetCursorOnHover(resize, "LansUILib/resizehorizontal", new Vector2(-12, -12));
                    panel.Add(resize);
                }
            }

            return panel;
        }

        public static ScrollPanel CreateScrollPanel()
        {
            var panel = new LComponent("ScrollPanel");
            var backgroundTexture = ModContent.Request<Texture2D>("LansUILib/move", AssetRequestMode.ImmediateLoad);

            var maskPanel = new LComponent("ScrollPanelMask");
            maskPanel.isMask = true;
            //maskPanel.image = new LImage(new WrapperLColor(Color.White), new WrapperLSprite(backgroundTexture));
            maskPanel.SetMargins(0, 0, 20, 0);
            panel.Add(maskPanel);

            var contentPanel = new LComponent("ScrollPanelContent");
            //contentPanel.image = new LImage(new WrapperLColor(Color.White), new WrapperLSprite(backgroundTexture));
            contentPanel.SetAnchors(0, 0, 1, 0);
            contentPanel.SetMargins(0, 0, 0, -500);
            maskPanel.Add(contentPanel);

            var scrollbar = UIFactory.CreateScrollbar();
            scrollbar.scrollbarComponent.SetAnchors(1, 0, 1, 1);
            scrollbar.scrollbarComponent.SetMargins(-20, 0, 0, 0);
            
            panel.Add(scrollbar.scrollbarComponent);
            var scrollpanelComponent = new ScrollPanel(scrollbar, panel, maskPanel, contentPanel);

            return scrollpanelComponent;
        }

        public static Scrollbar CreateScrollbar()
        {
            var backgroundTexture = ModContent.Request<Texture2D>("LansUILib/move", AssetRequestMode.ImmediateLoad);

            var panel = new LComponent("Scrollbar");
            panel.image = new LImage(new WrapperLColor(Color.White),
                new WrapperLSprite(
                    Main.Assets.Request<Texture2D>("Images/UI/Scrollbar", AssetRequestMode.ImmediateLoad)
                ), new CornerBox(6, 6, 6, 6)
            );
            var handleTexture = new WrapperLSprite(
                    Main.Assets.Request<Texture2D>("Images/UI/ScrollbarInner", AssetRequestMode.ImmediateLoad)
                );
            
            panel.MouseInteraction = true;
            var handle = new LComponent("ScrollbarHandle");
            handle.MouseInteraction = true;


            var defaultImage = new LImage(new WrapperLColor(Color.White * 0.8f), handleTexture, new CornerBox(6, 6, 6, 6));
            var hoverImage = new LImage(new WrapperLColor(Color.White), handleTexture, new CornerBox(6, 6, 6, 6));

            handle.image = defaultImage;
            panel.Add(handle);
            var bar = new Scrollbar(panel, handle, 0.7f, 0.3f);
            handle.MouseEnter += delegate (MouseState e)
            {
                handle.image = hoverImage;
            };

            handle.MouseExit += delegate (MouseState e)
            {
                handle.image = defaultImage;
            };
            return bar;
        }

        public static LButton CreateButton(string buttonText)
        {
            var panel = CreatePanel("Button", false, false);
            panel.MouseInteraction = true;
            var text = CreateText(buttonText);
            panel.Add(text);


            return new LButton(panel,text, new WrapperLColor(Color.White), new WrapperLColor(Color.White * 0.8f), new WrapperLColor(Color.White * 0.6f));
        }

        public static LComponent CreateText(string value, bool useLayout = false)
        {
            var text = new LComponent("Text");
            text.text = value;
            text.textColor = new WrapperLColor(Color.White);
            if (useLayout)
            {
                text.SetLayout(new WrapperLayoutText());
            }

            return text;
        }

        public static LComponent CreateImage(string texture, bool useLayout = false)
        {
            var component = new LComponent("Image");
            var backgroundTexture = ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad);
            component.image = new LImage(new WrapperLColor(Color.White), new WrapperLSprite(backgroundTexture));
            if (useLayout)
            {
                component.SetLayout(new WrapperLayout(backgroundTexture, null));
            }

            return component;
        }

        public static LComponent CreateImage(Asset<Texture2D> texture, DrawAnimation animation, bool useLayout = false)
        {
            var component = new LComponent("Image");
            component.image = new LImage(new WrapperLColor(Color.White), new WrapperLSprite(texture, animation));
            if (useLayout)
            {
                component.SetLayout(new WrapperLayout(texture, animation));
            }

            return component;
        }

        public static LComponent CreateImage(Asset<Texture2D> texture, Rectangle rectangle, bool useLayout = false)
        {
            var component = new LComponent("Image");
            component.image = new LImage(new WrapperLColor(Color.White), new WrapperLSprite(texture, null, rectangle));
            if (useLayout)
            {
                component.SetLayout(new WrapperLayout(texture, null, rectangle));
            }

            return component;
        }

        /*
        public static WrapperComponent CreateUIPanel(string name, bool blocking = true, bool draggable = true, bool resizeable = true)
        {
            //var panel = new Panel();
            var element = new UIPanel();

            return new WrapperComponent(name, element);
        }

        public static WrapperComponent CreateScrollPanel(string name, LComponent contentPanel)
        {
            var element = new UIScrollPanel(contentPanel);
            return new WrapperComponent(name, element);
        }

        public static WrapperComponent CreateImage(string name, Asset<Texture2D> texture, DrawAnimation drawAnimation = null)
        {
            var element = new BetterUIImage(texture, drawAnimation);
            var loaded = texture.IsLoaded;
            WrapperComponent wrapper = null;
            Func<bool> del = null;
            if(!loaded)
            {
                del = delegate ()
                {
                    if(texture.IsLoaded)
                    {
                        wrapper.updateDelegate = null;
                        return true;
                    }
                    return false;
                };
            }
            wrapper = new WrapperComponent(name, element, del);
            var layout = new WrapperLayout(texture, drawAnimation);
            wrapper.SetLayout(layout);
            return wrapper;
        }
        */
        public static LComponent CreateItemSlot(LItemSlot lItemSlot)
        {
            
            var panel = CreatePanel("ItemSlot", false, false);
            panel.MouseInteraction = true;
            panel.SetLayout(new LayoutSize(32, 32));
            var itemSprite = new LComponent("ItemSlotSprite");

            var updateItemSprite = delegate ()
            {
                if (lItemSlot.Item.type > ItemID.None)
                {
                    itemSprite.image = new LImage(new WrapperLColor(Color.White), 
                        new WrapperLSprite(TextureAssets.Item[lItemSlot.Item.type], Main.itemAnimations[lItemSlot.Item.type]), ImageFillMode.Normal);
                }
                else
                {
                    itemSprite.image = null;
                }
            };

            lItemSlot.OnChanged += delegate ()
            {
                updateItemSprite();
            };

            updateItemSprite();


            panel.MouseUp += delegate (MouseState state)
            {
                if (!Main.mouseItem.IsAir)
                {
                    var error = false;
                    if (lItemSlot.type == LItemSlotType.PetAndLight && !Main.vanityPet[Main.mouseItem.buffType] && !Main.lightPet[Main.mouseItem.buffType])
                    {
                        error = true;
                    }
                    if (lItemSlot.type == LItemSlotType.Pet && !Main.vanityPet[Main.mouseItem.buffType])
                    {
                        error = true;
                    }
                    if (lItemSlot.type == LItemSlotType.Light && !Main.lightPet[Main.mouseItem.buffType])
                    {
                        error = true;
                    }

                    if (error)
                    {
                        Main.NewText("Cannot swap item, as the slot does not accept the given item.", new Color(255, 0, 0));
                        return;
                    }
                }
                
                var curr = Main.mouseItem;
                Main.mouseItem = lItemSlot.Item;
                lItemSlot.Item = curr;
                panel.Invalidate();
            };

            panel.Add(itemSprite);
            return panel;
        }
        /*
        public static WrapperComponent CreateButton(Asset<Texture2D> texture)
        {
            var element = new UIImageButton(texture);
            var loaded = texture.IsLoaded;
            WrapperComponent wrapper = null;
            Func<bool> del = null;
            if (!loaded)
            {
                del = delegate ()
                {
                    if (texture.IsLoaded)
                    {
                        wrapper.updateDelegate = null;
                        return true;
                    }
                    return false;
                };
            }
            wrapper = new WrapperComponent("", element, del);
            return wrapper;
        }

        public static WrapperComponent CreateText(string name, string text)
        {
            var modlabel = new UIText(text);
            var wrapper = new WrapperComponent(name,modlabel);
            wrapper.SetLayout(new WrapperLayoutText());
            return wrapper;
        }

        public static WrapperComponent CreateInputField(string name, string text, EInputFieldType inputType = EInputFieldType.Text)
        {
            var element = new InputField(text, inputType);
            var wrapper = new WrapperComponent(name, element);
            var panel = CreateUIPanel("Panel");
            panel.SetLayout(new LayoutFlow(new bool[] { true, true }, new bool[] { false, false }, LayoutFlowType.Vertical, 0, 0, 24, 24, 0));
            panel.Add(wrapper);
            return panel;
        }

        public static WrapperComponent CreateImageButtonLabel(string name, Asset<Texture2D> texture, string hoverText,
            UIElement.MouseEvent mouseClick, UIElement.MouseEvent mouseOver)
        {
            var element = new UIImageButtonLabel(texture, hoverText);
            element.OnClick += mouseClick;
            element.OnMouseOver += mouseOver;
            var wrapper = new WrapperComponent(name, element);
            wrapper.SetLayout(new WrapperLayout(texture));
            return wrapper;
        }

        public static WrapperComponent CreateHoverImageToggleButton(string name, Asset<Texture2D> texture_checked, Asset<Texture2D> texture_unchecked, string hoverTextchecked, string hoverTextunchecked,
            UIHoverImageToggleButton.CheckEvent onChecked, UIElement.MouseEvent mouseOver)
        {
            var element = new UIHoverImageToggleButton(texture_checked, texture_unchecked, hoverTextchecked, hoverTextunchecked);
            element.OnChecked += onChecked;
            element.OnMouseOver += mouseOver;
            var wrapper = new WrapperComponent(name, element);
            wrapper.SetLayout(new WrapperLayout(texture_checked));
            return wrapper;
        }



        public static WrapperComponent CreateMinimapButton(Asset<Texture2D> texture, string hoverText)
        {
            var instance = Terraria.Main.MinimapFrameManagerInstance;
            Type type = instance.GetType();
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            // get the field info
            FieldInfo finfo = type.GetField("ActiveSelection", bindingFlags);

            // get the value
            MinimapFrame value = (MinimapFrame) finfo.GetValue(instance);

            //value.FramePosition
            return null;
        }*/




    }
}
