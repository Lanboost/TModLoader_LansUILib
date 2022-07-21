using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.IO;

namespace LansUILib.ui
{

    public enum LItemSlotType
    {
        PetAndLight,
        Pet,
        Light,
        Item
    }
    public class LItemSlot
    {
        public delegate void EmptyEvent();

        public static LItemSlot[] Create(int count, LItemSlotType type = LItemSlotType.PetAndLight)
        {
            var arr = new LItemSlot[count];

            for(int i = 0; i < count; i++)
            {
                arr[i] = new LItemSlot(type);
            }
            return arr;
        }

        public static void Load(TagCompound tag, string name, ref LItemSlot[] items)
        {
            var arr = tag.GetList<TagCompound>(name).Select(ItemIO.Load).ToArray();
            for(int i=0; i<arr.Length; i++)
            {
                items[i].Item = arr[i];
            }
        }
        public static void Save(TagCompound tag, string name, LItemSlot[] items)
        {
            var itemArr = items.Select(value => value.Item).ToArray();

            tag[name] = itemArr.Select(ItemIO.Save).ToList();
        }


        public LItemSlotType type;
        public event EmptyEvent OnChanged;
        public LItemSlot(LItemSlotType type)
        {
            this.type = type;
        }

        public Item _item = new Item();

        public Item Item
        {
            get { return _item; }
            set { 
                _item = value; 
                OnChanged.Invoke();
            }
        }

        public void Update()
        {
            if(this.type == LItemSlotType.PetAndLight || this.type == LItemSlotType.Pet)
            {
                Utils.Swap(ref Main.LocalPlayer.miscEquips[0], ref _item);
                Main.LocalPlayer.UpdatePet(Main.myPlayer);
                Utils.Swap(ref Main.LocalPlayer.miscEquips[0], ref _item);
            }
            if (this.type == LItemSlotType.PetAndLight || this.type == LItemSlotType.Light)
            {
                Utils.Swap(ref Main.LocalPlayer.miscEquips[1], ref _item);
                Main.LocalPlayer.UpdatePetLight(Main.myPlayer);
                Utils.Swap(ref Main.LocalPlayer.miscEquips[1], ref _item);
            }
        }
    }
}
