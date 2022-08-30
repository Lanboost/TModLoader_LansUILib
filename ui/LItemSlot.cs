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

        public static void Load(TagCompound tag, string name, ref LItemSlot[] items, LItemSlotType lItemSlotType = LItemSlotType.Item)
        {
            var arr = tag.GetList<TagCompound>(name).Select(ItemIO.Load).ToArray();
            if(items.Length != arr.Length)
            {
                items = new LItemSlot[arr.Length];
            }
            for(int i=0; i<arr.Length; i++)
            {
                if(items[i] == null)
                {
                    items[i] = new LItemSlot(lItemSlotType);
                }
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
                if (!value.IsAir)
                {
                    Main.instance.LoadItem(value.type);
                }
                OnChanged?.Invoke();
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

    public class LItemSlotSerializer : TagSerializer<LItemSlot, TagCompound>
    {
        public override TagCompound Serialize(LItemSlot value) {
            return ItemIO.Save(value.Item);
    }

        public override LItemSlot Deserialize(TagCompound tag)
        {
            var slot = new LItemSlot(LItemSlotType.Item);
            slot.Item = ItemIO.Load(tag);
            return slot;
        }
    }
}
