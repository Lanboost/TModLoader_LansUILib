﻿using LansUILib.ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace LansUILib
{
    public class UISystem: ModSystem
    {
        public int realMouseX;
        public int realMouseY;

        public UserInterface stateInterface;
        public WrapperScreen Screen;
        private bool sizeChanged = true;

        public Asset<Texture2D> cursor = null;
        public Vector2 cursorOffset = new Vector2(0,0);

        private static bool ilhookscreated = false;
        private static UISystem _instance = null;
        public static UISystem Instance
        {
            get { return _instance; }
        }

        public void SetCursor(Asset<Texture2D> cursor, Vector2 cursorOffset) {
            this.cursor = cursor;
            this.cursorOffset = cursorOffset;
        }

        public void ClearCursor()
        {
            this.cursor = null;
        }

        private static bool HasParameter(params string[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (Program.LaunchParameters.ContainsKey(keys[i]))
                    return true;
            }

            return false;
        }

        public override void Load()
        {
            _instance = this;
            if (!Main.dedServ)
            {
                var uiState = new UISystemUIState();
                uiState.Initialize();
                stateInterface = new UserInterface();
                stateInterface.SetState(uiState);
                Screen = new WrapperScreen();


                // Test UI
                if (HasParameter("-uidebug"))
                {
                    PanelSettings panelSettings = new PanelSettings();
                    panelSettings.SetAnchor(AnchorPosition.TopLeft);
                    panelSettings.SetSize(250, 250, 400, 400);
                    var panel = UIFactory.CreatePanel("Unlimited Pets Main", panelSettings, true, false);

                    //panel.SetAnchor(LansUILib.ui.AnchorPosition.Center);
                    //panel.SetSize(-200, -150, 400, 300);

                    var inner = new LComponent("Inner");
                    inner.isMask = true;
                    panel.Add(inner);

                    inner.SetMargins(15, 15, 15, 15);
                    inner.SetLayout(new LayoutFlow(new bool[] { false, false }, new bool[] { true, true }, LayoutFlowType.Vertical, 0, 0, 0, 0, 10));


                    inner.Add(UIFactory.CreateText("Summon panel (Show with pet ui)", true));
                    inner.Add(UIFactory.CreateText("For summon order check description", true));

                    inner.Add(UIFactory.CreateText($"Current Minion Slots: 0", true));
                    inner.Add(UIFactory.CreateText($"Max Minion Slots: 0", true));

                    var scrollpanel = UIFactory.CreateScrollPanel();
                    scrollpanel.wrapper.GetLayout().Flex = 1;

                    //var recipePanel = LansUILib.UIFactory.CreatePanel("Recipe Panel", false, false);
                    var scrollContentPanel = scrollpanel.contentPanel;
                    scrollContentPanel.SetLayout(new LayoutFlow(new bool[] { true, true }, new bool[] { false, false }, LayoutFlowType.Vertical, 0, 0, 0, 0, 10));
                    inner.Add(scrollpanel.wrapper);


                    for(var i=0; i<19; i++)
                    {
                        var recipePanelCurr = UIFactory.CreatePanel("Recipe Panel Current", false, false);
                        recipePanelCurr.MouseInteraction = false;
                        recipePanelCurr.SetLayout(new LayoutFlow(new bool[] { true, true }, new bool[] { false, true }, LayoutFlowType.Horizontal, 3, 3, 3, 3, 5));

                        
                        recipePanelCurr.Add(UIFactory.CreateText($"slot{i}", true));
                        var minusButton = UIFactory.CreateButton("-1");
                        minusButton.Panel.SetLayout(new LayoutSize(30, 30));
                        minusButton.OnClicked += delegate (MouseState state)
                        {
                        };
                        recipePanelCurr.Add(minusButton.Panel);
                        var plusButton = UIFactory.CreateButton("+1");
                        plusButton.Panel.SetLayout(new LayoutSize(30, 30));
                        plusButton.OnClicked += delegate (MouseState state)
                        {
                        };
                        recipePanelCurr.Add(plusButton.Panel);
                        var fillButton = UIFactory.CreateButton("Fill");
                        fillButton.Panel.SetLayout(new LayoutSize(60, 30));
                        fillButton.OnClicked += delegate (MouseState state)
                        {
                        };
                        recipePanelCurr.Add(fillButton.Panel);

                        scrollContentPanel.Add(recipePanelCurr);
                    }

                    inner.Add(UIFactory.CreateText($"Test 222", true));

                    /*
                    var panel = UIFactory.CreatePanel("Main panel");
                    panel.SetAnchor(AnchorPosition.TopLeft);
                    panel.SetSize(100, 100, 200, 200);

                    panel.MouseEnter += delegate (MouseState state)
                    {
                        panel.image.Color = new WrapperLColor(new Color(63, 82, 151) * 0.2f);
                    };

                    panel.MouseExit += delegate (MouseState state)
                    {
                        panel.image.Color = new WrapperLColor(new Color(63, 82, 151) * 0.7f);
                    };
                    var scrollpanel = UIFactory.CreateScrollPanel();
                    scrollpanel.wrapper.SetAnchors(0, 0, 0.5f, 1);
                    scrollpanel.wrapper.SetMargins(10, 10, 10, 10);
                    panel.Add(scrollpanel.wrapper);

                    var button = UIFactory.CreateButton("Button1");
                    button.Panel.SetAnchors(0.5f, 0, 1f, 1);
                    button.Panel.SetMargins(10, 10, 10, 10);

                    panel.Add(button.Panel);
                    */
                    Screen.Add(panel);
                }


                Main.OnResolutionChanged += delegate (Vector2 newSize)
                {
                    sizeChanged = true;
                };


                if (!ilhookscreated)
                {
                    ilhookscreated = true;
                    Terraria.IL_Main.DrawCursor += AddCursorHandlerDrawCursor;
                    Terraria.IL_Main.DrawThickCursor += AddCursorHandlerDrawThickCursor;
                    //IL.Terraria.Main.DrawInventory += StartOfInterfaces;
                    Terraria.UI.IL_GameInterfaceLayer.Draw += StartOfInterfaces;
                }
            }

        }

        public static void MoveMouseIfHover(GameInterfaceLayer gameInterfaceLayer)
        {
            var name = gameInterfaceLayer.Name;
            if(name == "LansUiLib" || name == "Vanilla: Cursor" || name == "Vanilla: Mouse Item / NPC Head")
            {
                return;
            }
            if (UISystem.Instance.ControlsMouse())
            {
                Main.mouseX = -1;
                Main.mouseY = -1;
            }
        }
        public static void InjectBeforeDraw(ILContext il, System.Action<GameInterfaceLayer> action)
        {
            var c = new ILCursor(il);
            var curr = c.Next;
            while(curr != null)
            {
                if (curr.OpCode == OpCodes.Callvirt)
                {
                    var op = (MethodReference)curr.Operand;
                    if(op.FullName.Contains("GameInterfaceLayer") && op.FullName.Contains("DrawSelf")) {
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Call, action.GetMethodInfo());
                        return;
                    }
                }
                c.Goto(curr, MoveType.After);
                curr = c.Next;
            }

        }

        public void StartOfInterfaces(ILContext iLContext)
        {
            InjectBeforeDraw(iLContext, MoveMouseIfHover); 
        }

        public void AddCursorHandlerDrawCursor(ILContext iLContext)
        {
            InjectUtils.InjectSkipOnBoolean(iLContext, SkipRenderCursorAndDraw);
        }

        public void AddCursorHandlerDrawThickCursor(ILContext iLContext)
        {
            InjectUtils.InjectSkipOnBooleanWithReturnValue(iLContext, SkipRenderCursor, DefaultVector2);
        }

        public static bool SkipRenderCursor()
        {
            return Instance.cursor != null;
        }

        public static object DefaultVector2()
        {
            return new Vector2(0, 0);
        }

        public static bool SkipRenderCursorAndDraw()
        {
            if(Instance.cursor != null)
            {
                if (Instance.cursor.Value != null)
                {
                    Main.spriteBatch.Draw(Instance.cursor.Value, new Vector2(Main.mouseX, Main.mouseY) + Instance.cursorOffset, Color.White);
                }
                return true;
            }
            return false;
        }

        

        public override void Unload()
        {
            base.Unload();
            _instance = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu)
            {
                if(sizeChanged)
                {
                    var dim = UserInterface.ActiveInstance.GetDimensions();
                    Screen.ScreenSizeChanged((int)dim.Width, (int)dim.Height);
                    sizeChanged = false;
                }
                stateInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("LansUiLib", DrawSomethingUI, InterfaceScaleType.UI));
            }
        }

        private bool DrawSomethingUI()
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu)
            {
                Screen.Update();
                Screen.Draw();
                //stateInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        public bool ControlsMouse()
        {
            var mouse = Screen.getMouseState(true);

            var currentTarget = Screen.GetMouseTarget(mouse, Screen.baseComponent);

            if (currentTarget != null)
            {
                return true;
            }
            return false;
        }
    }


    public class UISystemUIState: UIState
    {

        public UISystemUIState()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        
    }
}
