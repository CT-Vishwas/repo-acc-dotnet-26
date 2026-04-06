using Inventory.API.Controllers;
using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Inventory.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
    }

    #region CreateProduct Tests

    public class CreateProductTests : ProductsControllerTests
    {
        [Fact]
        public async Task CreateProduct_WithValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var productRequest = new ProductRequestDTO
            {
                Name = "Test Product",
                Price = 99.99,
                Quantity = 10
            };

            var productResponse = new ProductResponseDTO
            {
                Id = 1,
                Name = "Test Product",
                Price = 99.99,
                Quantity = 10
            };

            _mockProductService
                .Setup(s => s.CreateProduct(It.IsAny<ProductRequestDTO>()))
                .ReturnsAsync(productResponse);

            // Act
            var result = await _controller.CreateProduct(productRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductsController.GetById), createdResult.ActionName);
            var responseValue = Assert.IsType<ApiResponse<ProductResponseDTO>>(createdResult.Value);
            Assert.Equal(productResponse.Id, responseValue.Data.Id);
            _mockProductService.Verify(s => s.CreateProduct(It.IsAny<ProductRequestDTO>()), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_WhenServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var productRequest = new ProductRequestDTO
            {
                Name = "Test Product",
                Price = 99.99,
                Quantity = 10
            };

            _mockProductService
                .Setup(s => s.CreateProduct(It.IsAny<ProductRequestDTO>()))
                .Returns(Task.FromResult((ProductResponseDTO)null!));

            // Act
            var result = await _controller.CreateProduct(productRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateProduct_LogsProductNameOnCreation()
        {
            // Arrange
            var productName = "Logged Product";
            var productRequest = new ProductRequestDTO
            {
                Name = productName,
                Price = 50,
                Quantity = 5
            };

            var productResponse = new ProductResponseDTO
            {
                Id = 1,
                Name = productName,
                Price = 50,
                Quantity = 5
            };

            _mockProductService
                .Setup(s => s.CreateProduct(It.IsAny<ProductRequestDTO>()))
                .ReturnsAsync(productResponse);

            // Act
            await _controller.CreateProduct(productRequest);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(productName)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateProduct_WithInvalidQuantity_ReturnsCreatedButServiceHandlesValidation(int quantity)
        {
            // Arrange
            var productRequest = new ProductRequestDTO
            {
                Name = "Test",
                Price = 99.99,
                Quantity = quantity
            };

            var productResponse = new ProductResponseDTO
            {
                Id = 1,
                Name = "Test",
                Price = 99.99,
                Quantity = quantity
            };

            _mockProductService
                .Setup(s => s.CreateProduct(It.IsAny<ProductRequestDTO>()))
                .ReturnsAsync(productResponse);

            // Act
            var result = await _controller.CreateProduct(productRequest);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            _mockProductService.Verify(s => s.CreateProduct(productRequest), Times.Once);
        }
    }

    #endregion

    #region GetProducts Tests

    public class GetProductsTests : ProductsControllerTests
    {
        [Fact]
        public async Task GetProducts_WithValidProducts_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<ProductResponseDTO>
            {
                new ProductResponseDTO { Id = 1, Name = "Product 1", Price = 10, Quantity = 5 },
                new ProductResponseDTO { Id = 2, Name = "Product 2", Price = 20, Quantity = 10 }
            };

            _mockProductService
                .Setup(s => s.GetAllProducts())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<IEnumerable<ProductResponseDTO>>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.Equal(2, returnedResponse.Data.Count());
        }

        [Fact]
        public async Task GetProducts_WithEmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyProducts = new List<ProductResponseDTO>();

            _mockProductService
                .Setup(s => s.GetAllProducts())
                .ReturnsAsync(emptyProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<IEnumerable<ProductResponseDTO>>>(okResult.Value);
            Assert.Empty(returnedResponse.Data);
        }

        [Fact]
        public async Task GetProducts_WhenServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            _mockProductService
                .Setup(s => s.GetAllProducts())
                .Returns(Task.FromResult((IEnumerable<ProductResponseDTO>)null!));

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProducts_ReturnsCorrectResponseMessage()
        {
            // Arrange
            var products = new List<ProductResponseDTO>
            {
                new ProductResponseDTO { Id = 1, Name = "Product 1", Price = 10, Quantity = 5 }
            };

            _mockProductService
                .Setup(s => s.GetAllProducts())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<IEnumerable<ProductResponseDTO>>>(okResult.Value);
            Assert.Equal("Products fetched successfully", returnedResponse.Message);
        }
    }

    #endregion

    #region GetById Tests

    public class GetByIdTests : ProductsControllerTests
    {
        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithProduct()
        {
            // Arrange
            var productId = 1;
            var product = new ProductResponseDTO
            {
                Id = productId,
                Name = "Test Product",
                Price = 99.99,
                Quantity = 10
            };

            _mockProductService
                .Setup(s => s.GetById(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<ProductResponseDTO>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.Equal(productId, returnedResponse.Data.Id);
        }

        [Fact]
        public async Task GetById_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var productId = 999;

            _mockProductService
                .Setup(s => s.GetById(productId))
                .Returns(Task.FromResult((ProductResponseDTO)null!));

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetById_CallsServiceWithCorrectId(int id)
        {
            // Arrange
            var product = new ProductResponseDTO { Id = id, Name = "Test", Price = 10, Quantity = 5 };

            _mockProductService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            await _controller.GetById(id);

            // Assert
            _mockProductService.Verify(s => s.GetById(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsCorrectResponseMessage()
        {
            // Arrange
            var product = new ProductResponseDTO { Id = 1, Name = "Test", Price = 10, Quantity = 5 };

            _mockProductService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<ProductResponseDTO>>(okResult.Value);
            Assert.Equal("Product fetched successfully", returnedResponse.Message);
        }
    }

    #endregion

    #region UpdateProduct Tests

    public class UpdateProductTests : ProductsControllerTests
    {
        [Fact]
        public async Task UpdateProduct_WithValidRequest_ReturnsOkWithUpdatedProduct()
        {
            // Arrange
            var productId = 1;
            var productRequest = new ProductRequestDTO
            {
                Name = "Updated Product",
                Price = 199.99,
                Quantity = 20
            };

            var updatedProduct = new ProductResponseDTO
            {
                Id = productId,
                Name = "Updated Product",
                Price = 199.99,
                Quantity = 20
            };

            _mockProductService
                .Setup(s => s.UpdateProduct(productId, It.IsAny<ProductRequestDTO>()))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.UpdateProduct(productId, productRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<ProductResponseDTO>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.Equal("Updated Product", returnedResponse.Data.Name);
            Assert.Equal(199.99, returnedResponse.Data.Price);
        }

        [Fact]
        public async Task UpdateProduct_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var productId = 1;
            var productRequest = new ProductRequestDTO
            {
                Name = "Updated",
                Price = 50,
                Quantity = 5
            };

            var updatedProduct = new ProductResponseDTO
            {
                Id = productId,
                Name = "Updated",
                Price = 50,
                Quantity = 5
            };

            _mockProductService
                .Setup(s => s.UpdateProduct(It.IsAny<int>(), It.IsAny<ProductRequestDTO>()))
                .ReturnsAsync(updatedProduct);

            // Act
            await _controller.UpdateProduct(productId, productRequest);

            // Assert
            _mockProductService.Verify(
                s => s.UpdateProduct(productId, productRequest),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public async Task UpdateProduct_WithDifferentIds_CallsServiceCorrectly(int productId)
        {
            // Arrange
            var productRequest = new ProductRequestDTO { Name = "Test", Price = 10, Quantity = 5 };
            var updatedProduct = new ProductResponseDTO
            {
                Id = productId,
                Name = "Test",
                Price = 10,
                Quantity = 5
            };

            _mockProductService
                .Setup(s => s.UpdateProduct(It.IsAny<int>(), It.IsAny<ProductRequestDTO>()))
                .ReturnsAsync(updatedProduct);

            // Act
            await _controller.UpdateProduct(productId, productRequest);

            // Assert
            _mockProductService.Verify(s => s.UpdateProduct(productId, It.IsAny<ProductRequestDTO>()), Times.Once);
        }
    }

    #endregion

    #region DeleteProduct Tests

    public class DeleteProductTests : ProductsControllerTests
    {
        [Fact]
        public async Task DeleteProduct_WithValidId_ReturnsOkWithSuccessMessage()
        {
            // Arrange
            var productId = 1;

            _mockProductService
                .Setup(s => s.DeleteProduct(productId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<ProductResponseDTO?>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.Equal("Product deleted successfully", returnedResponse.Message);
        }

        [Fact]
        public async Task DeleteProduct_CallsServiceWithCorrectId()
        {
            // Arrange
            var productId = 1;

            _mockProductService
                .Setup(s => s.DeleteProduct(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.DeleteProduct(productId);

            // Assert
            _mockProductService.Verify(s => s.DeleteProduct(productId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(999)]
        public async Task DeleteProduct_WithDifferentIds_CallsServiceForEach(int productId)
        {
            // Arrange
            _mockProductService
                .Setup(s => s.DeleteProduct(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.DeleteProduct(productId);

            // Assert
            _mockProductService.Verify(s => s.DeleteProduct(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNullData()
        {
            // Arrange
            _mockProductService
                .Setup(s => s.DeleteProduct(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<ApiResponse<ProductResponseDTO?>>(okResult.Value);
            Assert.Null(returnedResponse.Data);
        }
    }

    #endregion

    #region GetAll (Paged) Tests

    public class GetAllPagedTests : ProductsControllerTests
    {
        [Fact]
        public async Task GetAll_WithValidParameters_ReturnsPagedResponse()
        {
            // Arrange
            var parameters = new ProductParameters { PageNumber = 1, PageSize = 10 };
            var products = new List<ProductResponseDTO>
            {
                new ProductResponseDTO { Id = 1, Name = "Product 1", Price = 10, Quantity = 5 },
                new ProductResponseDTO { Id = 2, Name = "Product 2", Price = 20, Quantity = 10 }
            };

            var pagedResponse = new PagedResponse<IEnumerable<ProductResponseDTO>>(products, 2, 1, 10);

            _mockProductService
                .Setup(s => s.GetPagedResponseAsync(It.IsAny<ProductParameters>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.GetAll(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<PagedResponse<IEnumerable<ProductResponseDTO>>>(okResult.Value);
            Assert.True(returnedResponse.Success);
            Assert.Equal(2, returnedResponse.Data.Count());
            Assert.Equal(1, returnedResponse.CurrentPage);
        }

        [Fact]
        public async Task GetAll_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var parameters = new ProductParameters { PageNumber = 2, PageSize = 20 };
            var emptyResponse = new PagedResponse<IEnumerable<ProductResponseDTO>>(new List<ProductResponseDTO>(), 0, 2, 20);

            _mockProductService
                .Setup(s => s.GetPagedResponseAsync(It.IsAny<ProductParameters>()))
                .ReturnsAsync(emptyResponse);

            // Act
            await _controller.GetAll(parameters);

            // Assert
            _mockProductService.Verify(
                s => s.GetPagedResponseAsync(It.Is<ProductParameters>(
                    p => p.PageNumber == 2 && p.PageSize == 20)),
                Times.Once);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 20)]
        [InlineData(5, 50)]
        public async Task GetAll_WithDifferentPageParameters_ReturnsPaginatedResults(int pageNumber, int pageSize)
        {
            // Arrange
            var parameters = new ProductParameters { PageNumber = pageNumber, PageSize = pageSize };
            var response = new PagedResponse<IEnumerable<ProductResponseDTO>>(new List<ProductResponseDTO>(), 0, pageNumber, pageSize);

            _mockProductService
                .Setup(s => s.GetPagedResponseAsync(It.IsAny<ProductParameters>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAll(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<PagedResponse<IEnumerable<ProductResponseDTO>>>(okResult.Value);
            Assert.Equal(pageNumber, returnedResponse.CurrentPage);
            Assert.Equal(pageSize, returnedResponse.PageSize);
        }

        [Fact]
        public async Task GetAll_WithNoResults_ReturnsEmptyPagedResponse()
        {
            // Arrange
            var parameters = new ProductParameters { PageNumber = 1, PageSize = 10 };
            var emptyResponse = new PagedResponse<IEnumerable<ProductResponseDTO>>(new List<ProductResponseDTO>(), 0, 1, 10);

            _mockProductService
                .Setup(s => s.GetPagedResponseAsync(It.IsAny<ProductParameters>()))
                .ReturnsAsync(emptyResponse);

            // Act
            var result = await _controller.GetAll(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<PagedResponse<IEnumerable<ProductResponseDTO>>>(okResult.Value);
            Assert.Empty(returnedResponse.Data);
            Assert.Equal(0, returnedResponse.TotalCount);
        }
    }

    #endregion
}
