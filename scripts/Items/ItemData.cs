using System.Collections.Generic;

namespace ItemData 
{
    public class ItemInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
    
    public static class ItemDatabase
    {
        public static Dictionary<string, ItemInfo> ItemList = new Dictionary<string, ItemInfo>
        {
            {
                "Wood", new ItemInfo
                {
                    Name = "Wood",
                    Path = "res://assets/Items/wood.jpg",
                }
            },
            {
                "Iced Wood", new ItemInfo
                {
                    Name = "Iced Wood",
                    Path = "res://assets/Items/iced-wood.jpg",
                }
            },
            {
                "Burned Wood", new ItemInfo
                {
                    Name = "Burned Wood",
                    Path = "res://assets/Items/burned-wood.jpg",
                }
            },
            {
                "Steel", new ItemInfo
                {
                    Name = "Steel",
                    Path = "res://assets/Items/steel.jpg",
                }
            },
            {
                "Iced Steel", new ItemInfo
                {
                    Name = "Iced Steel",
                    Path = "res://assets/Items/iced-steel.jpg",
                }
            },
            {
                "Burned Steel", new ItemInfo
                {
                    Name = "Burned Steel",
                    Path = "res://assets/Items/burned-steel.jpg",
                }
            },
            {
                "Reinforced Steel", new ItemInfo
                {
                    Name = "Reinforced Steel",
                    Path = "res://assets/Items/reinforced-steel.jpg",
                }
            },
            {
                "Iced Reinforced Steel", new ItemInfo
                {
                    Name = "Iced Reinforced Steel",
                    Path = "res://assets/Items/iced-reinforced-steel.jpg",
                }
            },
            {
                "Burned Reinforced Steel", new ItemInfo
                {
                    Name = "Burned Reinforced Steel",
                    Path = "res://assets/Items/burned-reinforced-steel.jpg",
                }
            },
            {
                "String", new ItemInfo
                {
                    Name = "String",
                    Path = "res://assets/Items/string.jpg",
                }
            },
            {
                "Iced String", new ItemInfo
                {
                    Name = "Iced String",
                    Path = "res://assets/Items/iced-string.jpg",
                }
            },
            {
                "Burned String", new ItemInfo
                {
                    Name = "Burned String",
                    Path = "res://assets/Items/burned-string.jpg",
                }
            }
        };
    }
}