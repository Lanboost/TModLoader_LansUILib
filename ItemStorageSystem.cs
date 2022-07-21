using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace LansUILib
{
    public class ItemStorageSystem
    {
        Item[] items;
        public ItemStorageSystem(int slots)
        {
            items = new Item[slots];
        }

    }
}
