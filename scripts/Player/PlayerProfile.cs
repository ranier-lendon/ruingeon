using Godot;
using System;

public class PlayerProfile
{
    private string _name;
    private int _level;
    private int _experience;
    private int _basicCoin;
    private int _premiumCoin;

    public PlayerProfile(
        string name,
        int level,
        int experience,
        int basicCoin,
        int premiumCoin)
    {
        _name = name;
        _level = level;
        _experience = experience;
        _basicCoin = basicCoin;
        _premiumCoin = premiumCoin;
    }
}