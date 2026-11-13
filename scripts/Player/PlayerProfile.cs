using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public class PlayerProfile : ISaveable
{
    public string ID => "player_profile";
    
    public string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int BasicCoin { get; set; }
    public int PremiumCoin { get; set; }
    public Dictionary<string, int> Inventory { get; set; } = new Dictionary<string, int>();
    public string[] Equipments { get; set; } = new string[4];

    public PlayerProfile(
        string name,
        int level,
        int experience,
        int basicCoin,
        int premiumCoin)
    {
        Name = name;
        Level = level;
        Experience = experience;
        BasicCoin = basicCoin;
        PremiumCoin = premiumCoin;
    }

    public object CaptureState()
    {
        return new Dictionary<string, object>
        {
            {"name", Name},
            {"level", Level},
            {"experience", Experience},
            {"basic_coin", BasicCoin},
            {"premium_coin", PremiumCoin}
        };
    }

    public void RestoreState(object state)
    {
        var element = (JsonElement)state;
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(element.GetRawText());
        Name = data["name"].GetString();
        Level = data["level"].GetInt32();
        Experience = data["experience"].GetInt32();
        BasicCoin = data["basic_coin"].GetInt32();
        PremiumCoin = data["premium_coin"].GetInt32();
    }

    public void AddExp(int amount)
    {
        Experience += amount;
    }

    public void AddLoot(DungeonRewards loots)
    {
        BasicCoin += loots.Gold;
        PremiumCoin += loots.PremiumGold;
        Experience += loots.BonusExp;

        if (loots.Items == null)
            return;

        foreach (var item in loots.Items)
        {
            if (Inventory.ContainsKey(item.Id))
                Inventory[item.Id] += item.Amount;
            else
                Inventory.Add(item.Id, item.Amount);
        }
    }
}