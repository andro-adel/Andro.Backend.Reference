using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Andro.Backend.Reference.Categories;
using Andro.Backend.Reference.Products;

namespace Andro.Backend.Reference.Data;

public class ReferenceDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IRepository<Product, Guid> _productRepository;

    public ReferenceDataSeedContributor(
        IRepository<Category, Guid> categoryRepository,
        IRepository<Product, Guid> productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await SeedCategoriesAsync();
        await SeedProductsAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        if (await _categoryRepository.GetCountAsync() > 0)
        {
            return;
        }

        // Electronics Category
        var electronicsId = new Guid("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f");
        await _categoryRepository.InsertAsync(
            new Category(
                electronicsId,
                "Electronics",
                "Electronic devices and accessories"
            ),
            autoSave: true
        );

        // Clothing Category
        var clothingId = new Guid("4b182d26-7c6d-5d6f-0d2f-2d2a2d2a2d2a");
        await _categoryRepository.InsertAsync(
            new Category(
                clothingId,
                "Clothing",
                "Fashion and apparel"
            ),
            autoSave: true
        );

        // Books Category
        var booksId = new Guid("5c293e37-8d7e-6e7a-1e3a-3e3b3e3b3e3b");
        await _categoryRepository.InsertAsync(
            new Category(
                booksId,
                "Books",
                "Books and publications"
            ),
            autoSave: true
        );

        // Home & Garden Category
        var homeGardenId = new Guid("6d3a4f48-9e8f-7f8b-2f4b-4f4c4f4c4f4c");
        await _categoryRepository.InsertAsync(
            new Category(
                homeGardenId,
                "Home & Garden",
                "Home improvement and garden supplies"
            ),
            autoSave: true
        );

        // Sports Category
        var sportsId = new Guid("7e4b5a59-0f9a-8a9c-3a5c-5a5d5a5d5a5d");
        await _categoryRepository.InsertAsync(
            new Category(
                sportsId,
                "Sports",
                "Sports equipment and fitness gear"
            ),
            autoSave: true
        );
    }

    private async Task SeedProductsAsync()
    {
        if (await _productRepository.GetCountAsync() > 0)
        {
            return;
        }

        var electronicsId = new Guid("3a071c15-6b5c-4c5e-9c1e-1c1f1c1f1c1f");
        var clothingId = new Guid("4b182d26-7c6d-5d6f-0d2f-2d2a2d2a2d2a");
        var booksId = new Guid("5c293e37-8d7e-6e7a-1e3a-3e3b3e3b3e3b");
        var homeGardenId = new Guid("6d3a4f48-9e8f-7f8b-2f4b-4f4c4f4c4f4c");
        var sportsId = new Guid("7e4b5a59-0f9a-8a9c-3a5c-5a5d5a5d5a5d");

        // Electronics Products
        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Laptop Pro 15",
                1299.99m,
                50,
                electronicsId,
                "High-performance laptop with 15-inch display, 16GB RAM, 512GB SSD"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Wireless Mouse",
                29.99m,
                200,
                electronicsId,
                "Ergonomic wireless mouse with precision tracking"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "USB-C Hub",
                49.99m,
                150,
                electronicsId,
                "7-in-1 USB-C hub with HDMI, USB 3.0, and SD card reader"
            ),
            autoSave: true
        );

        // Clothing Products
        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Cotton T-Shirt",
                19.99m,
                300,
                clothingId,
                "100% cotton comfortable t-shirt available in multiple colors"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Denim Jeans",
                59.99m,
                120,
                clothingId,
                "Classic fit denim jeans with stretch fabric"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Winter Jacket",
                89.99m,
                80,
                clothingId,
                "Warm winter jacket with waterproof outer layer"
            ),
            autoSave: true
        );

        // Books Products
        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Clean Code",
                45.99m,
                100,
                booksId,
                "A Handbook of Agile Software Craftsmanship by Robert C. Martin"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Design Patterns",
                54.99m,
                75,
                booksId,
                "Elements of Reusable Object-Oriented Software"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "The Pragmatic Programmer",
                42.99m,
                90,
                booksId,
                "Your Journey to Mastery - 20th Anniversary Edition"
            ),
            autoSave: true
        );

        // Home & Garden Products
        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Garden Tool Set",
                79.99m,
                60,
                homeGardenId,
                "Complete 10-piece garden tool set with carrying case"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "LED Desk Lamp",
                34.99m,
                150,
                homeGardenId,
                "Adjustable LED desk lamp with USB charging port"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Plant Pot Set",
                24.99m,
                200,
                homeGardenId,
                "Set of 5 ceramic plant pots with drainage holes"
            ),
            autoSave: true
        );

        // Sports Products
        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Yoga Mat",
                29.99m,
                180,
                sportsId,
                "Non-slip yoga mat with carrying strap, 6mm thick"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Resistance Bands Set",
                24.99m,
                220,
                sportsId,
                "Set of 5 resistance bands with different tension levels"
            ),
            autoSave: true
        );

        await _productRepository.InsertAsync(
            new Product(
                Guid.NewGuid(),
                "Running Shoes",
                79.99m,
                100,
                sportsId,
                "Lightweight running shoes with excellent cushioning"
            ),
            autoSave: true
        );
    }
}
