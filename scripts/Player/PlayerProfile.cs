using Godot;
using System;
using System.Collections.Generic;

public class PlayerProfile : ISaveable
{
    public string ID => "player_profile";
    
    public string Name { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }
    public int BasicCoin { get; private set; }
    public int PremiumCoin { get; private set; }

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
        var data = (Dictionary<string, object>)state;
        Name = (string)data["name"];
        Level = (int)data["level"];
        Experience = (int)data["experience"];
        BasicCoin = (int)data["basic_coin"];
        PremiumCoin = (int)data["premium_coin"];
    }
}