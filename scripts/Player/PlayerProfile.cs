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
}