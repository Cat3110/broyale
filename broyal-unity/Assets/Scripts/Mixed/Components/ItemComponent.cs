using System;
using System.Collections.Generic;
using RemoteConfig;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ItemComponent : IComponentData
{
    public int Id;
}


public static class PlayerDataExt
{
    public struct CrystalPlace
    {
        private const int red = 0;
        private const int blue = 1;
        private const int green = 2;
        private const int reserved = 3;

        private int _value;

        public CrystalPlace(int value) => _value = value;
        public static CrystalPlace Red => new CrystalPlace( red );
        public static CrystalPlace Blue => new CrystalPlace( blue );
        public static CrystalPlace Green => new CrystalPlace( green );
        
        public static implicit operator int(CrystalPlace place) => place._value;
        public static explicit operator CrystalPlace(int value) => new CrystalPlace(value);
    }
    
    public static uint GetItemCount(this PlayerData data, CrystalPlace place)
    {
        var flags = BitConverter.GetBytes(data.inventory);
        return flags[place];
    }
    
    public static List<(CrystalPlace,uint)> GetItems(this PlayerData data)
    {
        var flags = BitConverter.GetBytes(data.inventory);
        var items = new List<(CrystalPlace, uint)>();

        for (int i = 0; i < flags.Length; i++)
        {
            if (flags[i] > 0)
            {
                items.Add(( new CrystalPlace(i), flags[i] ));
            }
        }

        return items;
    }

    public static void AddItem(ref this PlayerData data, ItemInfo itemInfo)
    {
        var flags = BitConverter.GetBytes(data.inventory);

        if (itemInfo.Power > 0) // Red
        {
            data.power += itemInfo.Power;
            
            flags[CrystalPlace.Red] += 1;
        }
        else if (itemInfo.Magic > 0) // Blue
        {
            data.magic += itemInfo.Magic;
            
            flags[CrystalPlace.Blue] += 1;
        }
        else if (itemInfo.Health > 0) // Green
        {
            data.maxHealth += itemInfo.Health;
            data.health += itemInfo.Health;
            
            flags[CrystalPlace.Green] += 1;
        }
        
        data.inventory = BitConverter.ToUInt32(flags,0);
    }
}