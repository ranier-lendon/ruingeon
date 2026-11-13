using Godot;
using System;
using System.Collections.Generic;

public class DungeonRunState
{
    public string CurrentDungeon { get; set; }
    public int CurrentFloor { get; set; }
    public List<ItemStack> AcquiredItems { get; set; } = new List<ItemStack>();
    public int AcquiredGold { get; set; }
    public int AcquiredPremiumGold { get; set; }

    public void AddItem(string itemName, int amount)
    {
        ItemStack stack = new ItemStack(itemName, amount);
        AcquiredItems.Add(stack);
    }

    public void AddGold(int amount)
    {
        AcquiredGold += amount;
    }

    public void AddPremiumGold(int amount)
    {
        AcquiredPremiumGold += amount;
    }

    public DungeonRewards GetRewards()
    {
        return new DungeonRewards 
        {
            Items = AcquiredItems,
            Gold = AcquiredGold,
            PremiumGold = AcquiredPremiumGold
        };
    }
}

public class DungeonRewards
{
    public List<ItemStack> Items { get; set; } = new();
    public int Gold { get; set; }
    public int PremiumGold { get; set; }
}