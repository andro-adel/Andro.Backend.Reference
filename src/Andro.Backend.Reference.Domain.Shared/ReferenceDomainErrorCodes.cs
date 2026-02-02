namespace Andro.Backend.Reference;

public static class ReferenceDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    // Category validation errors
    public const string InvalidCategoryName = "Reference:InvalidCategoryName";
    public const string DuplicateCategoryName = "Reference:DuplicateCategoryName";
    public const string CategoryHasProducts = "Reference:CategoryHasProducts";

    // Product validation errors
    public const string InvalidProductPrice = "Reference:InvalidProductPrice";
    public const string InvalidProductStock = "Reference:InvalidProductStock";
    public const string DuplicateProductName = "Reference:DuplicateProductName";
    public const string InsufficientStock = "Reference:InsufficientStock";
    public const string CategoryNotFound = "Reference:CategoryNotFound";
}
