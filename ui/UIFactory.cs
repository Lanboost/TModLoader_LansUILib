using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleMod.UI;
using LansUILib.ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace LansUILib
{

    public class UIFactory
    {
        public static WrapperComponent CreatePanel(string name, bool blocking = false)
        {
            var element = blocking ? new BlockingPanel() : new UIElement();
            return new WrapperComponent(name, element);
        }

        public static WrapperComponent CreateUIPanel(string name)
        {
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

        public static WrapperComponent CreateItemSlot(LItemSlot lItemSlot)
        {
            var emptyTexture = TextureAssets.InventoryBack2;
            var element = new BetterUIImage(emptyTexture, null, true);

            lItemSlot.OnChanged += delegate ()
            {
                if (lItemSlot.Item.type > ItemID.None)
                {
                    element.SetImage(TextureAssets.Item[lItemSlot.Item.type]);
                }
                else
                {
                    element.SetImage(emptyTexture);
                }
            };

            if (lItemSlot.Item.type > ItemID.None)
            {
                element.SetImage(TextureAssets.Item[lItemSlot.Item.type]);
            }
            else
            {
                element.SetImage(emptyTexture);
            }

            var wrapper = new WrapperComponent("", element);
           

            element.OnClick += delegate (UIMouseEvent evt, UIElement listeningElement)
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
                wrapper.Invalidate();
                var curr = Main.mouseItem;
                Main.mouseItem = lItemSlot.Item;
                lItemSlot.Item = curr;
            };
            var layout = new WrapperLayout(emptyTexture);
            wrapper.SetLayout(layout);


            var container = CreateImage("Background", TextureAssets.InventoryBack2);
            container.Add(wrapper);
            return container;
        }

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




    }
}
