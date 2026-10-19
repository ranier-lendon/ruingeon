using Godot;
using System;

public class PlayerProfile
{
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
}