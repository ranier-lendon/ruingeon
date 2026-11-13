public class ItemStack
{
    public string Id { get; set; }
    public int Amount { get; set; }

    public ItemStack(string id, int amount)
    {
        Id = id;
        Amount = amount;
    }
}