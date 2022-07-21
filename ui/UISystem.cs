using LansUILib.ui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserInterface stateInterface;
        public WrapperScreen Screen;
        private bool sizeChanged = true;

        private static UISystem _instance = null;
        public static UISystem Instance
        {
            get { return _instance; }
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
                Screen = new WrapperScreen(uiState);
                Main.OnResolutionChanged += delegate (Vector2 newSize)
                {
                    sizeChanged = true;
                };
            }

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
                Screen.Update();
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("LansToggleableBuffs", DrawSomethingUI, InterfaceScaleType.UI));
            }
        }

        private bool DrawSomethingUI()
        {
            // it will only draw if the player is not on the main menu
            if (!Main.gameMenu)
            {
                stateInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }
    }

    public class UISystemUIState: UIState
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
