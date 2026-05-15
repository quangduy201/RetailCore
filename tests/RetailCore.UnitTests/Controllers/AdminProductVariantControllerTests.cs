using Microsoft.AspNetCore.Mvc;
using RetailCore.Shared.DTOs.Product;
using RetailCore.Shared.Enums;
using RetailCore.Shared.Requests.Product;

namespace RetailCore.UnitTests.Controllers;

public class AdminProductVariantControllerTests
{
    private readonly Mock<IProductVariantService> _variantServiceMock;
    private readonly AdminProductVariantsController _controller;

    public AdminProductVariantControllerTests()
    {
        _variantServiceMock = new Mock<IProductVariantService>();
        _controller = new AdminProductVariantsController(_variantServiceMock.Object);
    }

    [Fact]
    public async Task GetByProductId_ShouldReturnOkWithVariants()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var expectedVariants = new List<ProductVariantDto>
        {
            CreateVariantDto("SKU-1"),
            CreateVariantDto("SKU-2")
        };

        _variantServiceMock
            .Setup(service => service.GetByProductIdAsync(productId))
            .ReturnsAsync(expectedVariants);

        // Act
        var result = await _controller.GetByProductId(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<ProductVariantDto>>(okResult.Value);

        Assert.Equal(2, value.Count);
        Assert.Equal("SKU-1", value[0].Sku);
        Assert.Equal("SKU-2", value[1].Sku);

        _variantServiceMock.Verify(
            service => service.GetByProductIdAsync(productId),
            Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithVariant()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        var expectedVariant = CreateVariantDto("SKU-1");
        expectedVariant.Id = variantId;

        _variantServiceMock
            .Setup(service => service.GetByIdAsync(variantId))
            .ReturnsAsync(expectedVariant);

        // Act
        var result = await _controller.GetById(variantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ProductVariantDto>(okResult.Value);

        Assert.Equal(variantId, value.Id);
        Assert.Equal(expectedVariant.Sku, value.Sku);
        Assert.Equal(expectedVariant.Name, value.Name);
        Assert.Equal(expectedVariant.Price, value.Price);
        Assert.Equal(expectedVariant.Stock, value.Stock);

        _variantServiceMock.Verify(
            service => service.GetByIdAsync(variantId),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenRequestIsValid_ShouldReturnCreatedAtActionWithCreatedId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = CreateVariantRequest();
        var createdId = Guid.NewGuid();

        _variantServiceMock
            .Setup(service => service.CreateAsync(productId, request))
            .ReturnsAsync(createdId);

        // Act
        var result = await _controller.Create(productId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(AdminProductVariantsController.GetById), createdResult.ActionName);
        Assert.Equal(createdId, createdResult.Value);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(productId, createdResult.RouteValues["productId"]);
        Assert.Equal(createdId, createdResult.RouteValues["id"]);

        _variantServiceMock.Verify(
            service => service.CreateAsync(productId, request),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenSkuIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = CreateVariantRequest();
        request.Sku = "";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(productId, request));

        // Assert
        Assert.Equal("SKU is required.", exception.Message);

        _variantServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<Guid>(), It.IsAny<CreateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenPriceIsNegative_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = CreateVariantRequest();
        request.Price = -1;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(productId, request));

        // Assert
        Assert.Equal("Price cannot be negative.", exception.Message);

        _variantServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<Guid>(), It.IsAny<CreateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenStockIsNegative_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = CreateVariantRequest();
        request.Stock = -1;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(productId, request));

        // Assert
        Assert.Equal("Stock cannot be negative.", exception.Message);

        _variantServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<Guid>(), It.IsAny<CreateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenAttributeValueIdsIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = CreateVariantRequest();
        request.AttributeValueIds = [];

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Create(productId, request));

        // Assert
        Assert.Equal("Variant must contain attribute values.", exception.Message);

        _variantServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<Guid>(), It.IsAny<CreateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenRequestIsValid_ShouldReturnNoContent()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var request = UpdateVariantRequest();

        _variantServiceMock
            .Setup(service => service.UpdateAsync(variantId, request))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(variantId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _variantServiceMock.Verify(
            service => service.UpdateAsync(variantId, request),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenSkuIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var request = UpdateVariantRequest();
        request.Sku = " ";

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(variantId, request));

        // Assert
        Assert.Equal("SKU is required.", exception.Message);

        _variantServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenPriceIsNegative_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var request = UpdateVariantRequest();
        request.Price = -1;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(variantId, request));

        // Assert
        Assert.Equal("Price cannot be negative.", exception.Message);

        _variantServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenStockIsNegative_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var request = UpdateVariantRequest();
        request.Stock = -1;

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(variantId, request));

        // Assert
        Assert.Equal("Stock cannot be negative.", exception.Message);

        _variantServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenAttributeValueIdsIsEmpty_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var variantId = Guid.NewGuid();
        var request = UpdateVariantRequest();
        request.AttributeValueIds = [];

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Update(variantId, request));

        // Assert
        Assert.Equal("Variant must contain attribute values.", exception.Message);

        _variantServiceMock.Verify(
            service => service.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProductVariantRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        var variantId = Guid.NewGuid();

        _variantServiceMock
            .Setup(service => service.DeleteDraftAsync(variantId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(variantId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _variantServiceMock.Verify(
            service => service.DeleteDraftAsync(variantId),
            Times.Once);
    }

    private static ProductVariantDto CreateVariantDto(string sku)
    {
        return new ProductVariantDto
        {
            Id = Guid.NewGuid(),
            Sku = sku,
            Name = "Variant Name",
            Description = "Variant description",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = ProductVariantStatus.Active,
            Images =
            [
                new ProductVariantImageDto
                {
                    Id = Guid.NewGuid(),
                    Url = "image.jpg",
                    SortOrder = 1,
                    IsPrimary = true
                }
            ],
            Attributes =
            [
                new ProductVariantAttributeDto
                {
                    AttributeId = Guid.NewGuid(),
                    AttributeName = "Color",
                    AttributeValueId = Guid.NewGuid(),
                    AttributeValue = "Black"
                }
            ]
        };
    }

    private static CreateProductVariantRequest CreateVariantRequest()
    {
        return new CreateProductVariantRequest
        {
            Sku = "SKU-1",
            Name = "Variant Name",
            Description = "Variant description",
            Price = 999,
            CompareAtPrice = 1099,
            Stock = 10,
            Status = ProductVariantStatus.Active,
            Images =
            [
                new ProductVariantImageRequest
                {
                    Url = "image.jpg",
                    SortOrder = 1,
                    IsPrimary = true
                }
            ],
            AttributeValueIds =
            [
                Guid.NewGuid()
            ]
        };
    }

    private static UpdateProductVariantRequest UpdateVariantRequest()
    {
        return new UpdateProductVariantRequest
        {
            Sku = "SKU-1",
            Name = "Updated Variant",
            Description = "Updated description",
            Price = 1199,
            CompareAtPrice = 1299,
            Stock = 20,
            Status = ProductVariantStatus.Inactive,
            Images =
            [
                new ProductVariantImageRequest
                {
                    Url = "updated-image.jpg",
                    SortOrder = 1,
                    IsPrimary = true
                }
            ],
            AttributeValueIds =
            [
                Guid.NewGuid()
            ]
        };
    }
}
