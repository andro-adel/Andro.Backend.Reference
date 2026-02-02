namespace Andro.Backend.Reference.Products;

public static class ProductConsts
{
    public const int MaxNameLength = 128;
    public const int MinNameLength = 3;
    public const int MaxDescriptionLength = 1000;
    
    public const decimal MinPrice = 0.01m;
    public const decimal MaxPrice = 1000000m;
    
    public const int MinStock = 0;
    public const int MaxStock = 100000;
}
