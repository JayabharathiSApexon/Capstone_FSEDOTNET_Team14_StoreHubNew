using Microsoft.AspNetCore.Mvc;
using StoreHub.Application.Interfaces.Services;
using AutoMapper;
using StoreHub.Application.Models.Product;
using StoreHub.API.Models.Product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace StoreHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper, IWebHostEnvironment environment)
        {
            _productService = productService;
            _mapper = mapper;
            _environment = environment;
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
            var uploadsFolder = Path.Combine(
                            _environment.WebRootPath,
                            "uploads",
                            "products");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var productRequest = _mapper.Map<ProductRequestModel>(request);

            productRequest.Images = new List<ProductImageRequestModel>();

            int displayOrder = 1;

            foreach (var image in request.Images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);

                await image.CopyToAsync(stream);

                productRequest.Images.Add(new ProductImageRequestModel
                {
                    ImageUrl = $"/uploads/products/{fileName}",
                    IsPrimary = displayOrder == 1,
                    DisplayOrder = displayOrder++
                });
            }

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

            var productRequest = _mapper.Map<ProductRequestModel>(request);

            var existingProduct = await _productService.GetProductByIdAsync(request.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            productRequest.Images = new List<ProductImageRequestModel>();

            // Save uploaded images
            if (request.Images != null && request.Images.Any())
            {
                foreach (var image in existingProduct.Images)
                {
                    var filePath = Path.Combine(
                        _environment.WebRootPath,
                        image.ImageUrl.TrimStart('/')
                            .Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var uploadPath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products");

                Directory.CreateDirectory(uploadPath);

                int displayOrder = 1;

                foreach (var image in request.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    productRequest.Images.Add(new ProductImageRequestModel
                    {
                        ImageUrl = $"/uploads/products/{fileName}",
                        IsPrimary = displayOrder == 1,
                        DisplayOrder = displayOrder++
                    });
                }
            }

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