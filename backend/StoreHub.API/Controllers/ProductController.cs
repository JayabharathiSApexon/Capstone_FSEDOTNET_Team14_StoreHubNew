using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using StoreHub.Application.Models.Product;
using StoreHub.API.Models.Product;
using Microsoft.AspNetCore.Authorization;
using StoreHub.API.Services.Interfaces;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageStorageService _productImageStorageService;
        private readonly IProductRequestMapper _productRequestMapper;

        public ProductController(
            IProductService productService,
            IProductRequestMapper productRequestMapper,
            IProductImageStorageService productImageStorageService)
        {
            _productService = productService;
            _productRequestMapper = productRequestMapper;
            _productImageStorageService = productImageStorageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok(await _productService.GetAllProductsAsync());
        }

        [HttpGet("{productId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            var images = await _productImageStorageService.SaveNewImagesAsync(request.Images);
            var productRequest = _productRequestMapper.ToCreateModel(request, images);

            var product = await _productService.CreateProductAsync(productRequest);

            return CreatedAtAction(
                nameof(GetProductById),
                new { productId = product.Id },
                product);
        }

        [HttpPut("{productId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] UpdateProductRequest request)
        {
            if (productId != request.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _productService.GetProductByIdAsync(request.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            var images = new List<ProductImageRequestModel>();

            if (request.Images != null && request.Images.Any())
            {
                images = await _productImageStorageService.ReplaceImagesAsync(
                    existingProduct.Images.Select(image => image.ImageUrl),
                    request.Images);
            }

            var productRequest = _productRequestMapper.ToUpdateModel(request, images);

            var product = await _productService.UpdateProductAsync(productRequest);

            return Ok(product);
        }

        [HttpDelete("{productId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var product = await _productService.DeleteProductAsync(productId);

            return Ok(product);
        }
    }
}