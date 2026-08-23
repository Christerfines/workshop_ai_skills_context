using System.Text.Json;

namespace NordicBike.Portal;

public static class CatalogSource
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static List<Product> Load(string contentRoot)
    {
        var catalogPath = Path.Combine(contentRoot, "catalog", "products.json");
        if (!File.Exists(catalogPath)) throw new FileNotFoundException("The local product catalog is missing.", catalogPath);

        var products = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(catalogPath), SerializerOptions) ?? [];
        Validate(products, contentRoot);
        return products.Where(product => product.IsActive).ToList();
    }

    private static void Validate(IReadOnlyCollection<Product> products, string contentRoot)
    {
        var activeProducts = products.Where(product => product.IsActive).ToList();
        ValidateCatalogSize(activeProducts);
        ValidateUniqueValues(activeProducts.Select(product => product.Id), "Product IDs must be unique.");
        ValidateUniqueValues(activeProducts.Select(product => product.Slug), "Product slugs must be unique.");

        var productIds = activeProducts.Select(product => product.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var product in activeProducts) ValidateProduct(product, productIds, contentRoot);
    }

    private static void ValidateCatalogSize(IReadOnlyCollection<Product> products)
    {
        if (products.Count is < 50 or > 100) throw new InvalidOperationException("The active product catalog must contain between 50 and 100 products.");
    }

    private static void ValidateUniqueValues(IEnumerable<string> values, string message)
    {
        if (values.Distinct(StringComparer.OrdinalIgnoreCase).Count() != values.Count()) throw new InvalidOperationException(message);
    }

    private static void ValidateProduct(Product product, ISet<string> productIds, string contentRoot)
    {
        if (string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.Category) || string.IsNullOrWhiteSpace(product.Type)) throw new InvalidOperationException($"Product {product.Id} is missing catalog identity.");
        if (string.IsNullOrWhiteSpace(product.Slug) || string.IsNullOrWhiteSpace(product.Image) || string.IsNullOrWhiteSpace(product.ImageAlt)) throw new InvalidOperationException($"Product {product.Id} is missing catalog presentation data.");
        if (!IsLocalPath(product.Image)) throw new InvalidOperationException($"Product {product.Id} must use a repository-relative image path.");
        if (!product.Image.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException($"Product {product.Id} must use a local JPEG catalog image.");
        if (product.ImageWidth <= 0 || product.ImageHeight <= 0) throw new InvalidOperationException($"Product {product.Id} must have valid image dimensions.");
        if (product.CompatibleBikes.Any(bikeId => !productIds.Contains(bikeId))) throw new InvalidOperationException($"Product {product.Id} references an unknown compatible product.");
        if (product.Gallery.Any(image => !IsLocalPath(image))) throw new InvalidOperationException($"Product {product.Id} has an invalid gallery path.");

        EnsureLocalAsset(contentRoot, product.Image, product.Id);
        foreach (var image in product.Gallery) EnsureLocalAsset(contentRoot, image, product.Id);
    }

    private static bool IsLocalPath(string path) => path.StartsWith("/images/products/", StringComparison.Ordinal) && !path.Contains("..", StringComparison.Ordinal);

    private static void EnsureLocalAsset(string contentRoot, string url, string productId)
    {
        var relativePath = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var filePath = Path.Combine(contentRoot, "wwwroot", relativePath);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"The local image for product {productId} is missing.", filePath);
    }
}
