using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Application.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_cyberDucky.Controllers
{
    public class ProductImagesController : BaseController
    {
        private readonly Cloudinary _cloudinary;
        private readonly AppDbContext _context;

        public ProductImagesController(IOptions<Cloud> config, AppDbContext context)
        {
            var cloudinaryAccount = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(cloudinaryAccount);
            _context = context;
        }
        /// <summary>
        /// upload image product when create for admin
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{productId}/images")]
        public async Task<IActionResult> UploadProductImages(int productId, [FromForm] List<IFormFile> files)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound("Product not found");

            var uploadedImageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream),
                            Transformation = new Transformation().Crop("fill").Gravity("face")
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                        if (uploadResult.Url != null)
                        {
                            uploadedImageUrls.Add(uploadResult.Url.ToString());

                            var productImage = new ProductImages
                            {
                                ProductId = productId,
                                ImageUrl = uploadResult.Url.ToString()
                            };

                            _context.ProductImage.Add(productImage);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { imageUrls = uploadedImageUrls });
        }
        /// <summary>
        /// update images product for admin
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="imageId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateProductImage(int productId, int imageId, IFormFile file)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound("Product not found");

            var productImage =
                await _context.ProductImage.FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.Id == imageId);
            if (productImage == null)
                return NotFound("Product image not found");

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Crop("fill").Gravity("face")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }

            if (uploadResult.Url == null)
                return BadRequest("Could not upload image");

            // Update the image URL in the database
            productImage.ImageUrl = uploadResult.Url.ToString();
            await _context.SaveChangesAsync();

            return Ok(new { imageUrl = productImage.ImageUrl });
        }
    }
}
