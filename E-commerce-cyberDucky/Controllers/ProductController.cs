using Application.IService;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/products")]
    [ApiController]
    public class ProductController :BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        /// <summary>
        ///  get all product for customer,admin
        /// </summary>

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
           [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize, search, sort);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }/// <summary>
        /// get all product for custome, admin
        /// </summary>

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// get all product type for customer, admin
        /// </summary>

        [AllowAnonymous]
        [HttpGet("type")]
        public async Task<IActionResult> GetAllProductType([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
          [FromQuery] string search = "", [FromQuery] string sort = "", [FromQuery]string idtype = "")
        {
            var result = await _productService.GetAllProductType(page, pageSize, search, sort, idtype);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }/// <summary>
        /// create product for admin
        /// </summary>
        //[Authorize(Roles = "Admin")]      
        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(CreateProductDTO product)
        {
            var result = await _productService.CreateProductAsync(product);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        /// <summary>
        /// update product for admin
        /// </summary>      
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]      
        public async Task<IActionResult> UpdateProductAsync(int id, CreateProductDTO product)
        {
            var result = await _productService.UpdateProductAsync(product);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        /// <summary>
        /// delete product for admin
        /// </summary>    
        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

    }
}
