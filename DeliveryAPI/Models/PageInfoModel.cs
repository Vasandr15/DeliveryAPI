namespace DeliveryAPI.Models;

public class PageInfoModel
{
    public PageInfoModel(int size, int count, int current)
    {
        Size = size;
        Count = count;
        Current = current;
    }

    public int Size { get; set; }
    
    public int Count { get; set; }
    
    public int Current { get; set; }
}