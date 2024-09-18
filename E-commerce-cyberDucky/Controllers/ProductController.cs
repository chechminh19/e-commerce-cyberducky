using Application.IService;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("Allow")]
    [Route("api/products")]
    [ApiController]
    [Authorize(Roles = "Staff,Admin,Customer")]
    public class ProductController :BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
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
        }
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
        }
        [AllowAnonymous]
        //[Authorize(Roles = "Staff,Admin")]      
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
    }
}
