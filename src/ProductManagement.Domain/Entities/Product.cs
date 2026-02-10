using ProductManagement.Domain.Enums;
using ProductManagement.Domain.Exceptions;
using ProductManagement.Domain.ValueObjects;

namespace ProductManagement.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public ProductStatus Status { get; private set; }
    public string? Sku { get; private set; }

    private Product() { }

    public Product(string name, string description, Money price, int stockQuantity, string? sku = null)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateStockQuantity(stockQuantity);

        Name = name;
        Description = description;
        Price = price ?? throw new DomainException("Price cannot be null.");
        StockQuantity = stockQuantity;
        Sku = sku;
        Status = stockQuantity > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
    }

    public void Update(string name, string description, Money price, int stockQuantity, string? sku)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateStockQuantity(stockQuantity);

        Name = name;
        Description = description;
        Price = price ?? throw new DomainException("Price cannot be null.");
        StockQuantity = stockQuantity;
        Sku = sku;
        UpdatedAt = DateTime.UtcNow;

        UpdateStatus();
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to add must be greater than zero.");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdateStatus();
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to remove must be greater than zero.");

        if (quantity > StockQuantity)
            throw new DomainException("Insufficient stock quantity.");

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
        UpdateStatus();
    }

    public void Activate()
    {
        if (StockQuantity <= 0)
            throw new DomainException("Cannot activate a product with zero stock.");

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateStatus()
    {
        if (Status == ProductStatus.Discontinued)
            return;

        Status = StockQuantity > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.");

        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters.");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description cannot be empty.");

        if (description.Length > 2000)
            throw new DomainException("Product description cannot exceed 2000 characters.");
    }

    private static void ValidateStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");
    }
}
