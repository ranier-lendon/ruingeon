using Godot;
using System;

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
            {"Name", Name},
            {"Level", Level},
            {"Experience", Experience},
            {"BasicCoin", BasicCoin},
            {"PremiumCoin", PremiumCoin}
        };
    }

    public void RestoreState(object state)
    {
        var data = (Dictionary<string, object>)state;
        Name = (string)data["Name"];
        Level = (int)data["Level"];
        Experience = (int)data["Experience"];
        BasicCoin = (int)data["BasicCoin"];
        PremiumCoin = (int)data["PremiumCoin"];
    }
}