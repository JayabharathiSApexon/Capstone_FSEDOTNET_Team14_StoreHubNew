using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using AutoMapper;
using StoreHub.Application.Models.Product;
using StoreHub.API.Models.Product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using StoreHub.API.Common.Services;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public ProductController(IProductService productService, IMapper mapper, IImageStorageService imageStorageService)
        {
            _productService = productService;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{productId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        {
            try
            {
                var productRequest = _mapper.Map<ProductRequestModel>(request);

                productRequest.Images = await _imageStorageService.SaveImagesAsync(request.Images);

                var product = await _productService.CreateProductAsync(productRequest);

                return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{productId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] UpdateProductRequest request)
        {
            try
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

                var productRequest = _mapper.Map<ProductRequestModel>(request);

                if (request.Images != null && request.Images.Any())
                {
                    await _imageStorageService.DeleteImagesAsync(existingProduct.Images);

                    productRequest.Images = await _imageStorageService.SaveImagesAsync(request.Images);
                }

                var product = await _productService.UpdateProductAsync(productRequest);

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{productId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            try
            {
                var existingProduct = await _productService.GetProductByIdAsync(productId);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                await _imageStorageService.DeleteImagesAsync(existingProduct.Images);

                var product = await _productService.DeleteProductAsync(productId);

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}