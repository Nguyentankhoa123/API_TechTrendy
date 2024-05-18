using API.Data;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext storeContext;
        private readonly IProductRepository productRepository;
        private readonly IMapper _mapper;

        public ProductsController(StoreContext storeContext, IProductRepository productRepository, IMapper mapper)
        {
            this.storeContext = storeContext;
            this.productRepository = productRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int pageNumber = 1, int pageSize = 5)
        {

            var products = await productRepository.GetAllProducts(pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductsById(int id)
        {
            var result = await productRepository.GetProductById(id);
            return Ok(result);
        }

        [HttpGet("search-products")]
        public async Task<IActionResult> SearchAsync(string nameQuery, int pageNumber = 1, int pageSize = 5)
        {
            var result = await productRepository.SearchAsync(nameQuery, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await productRepository.DeleteProduct(id);
            return Ok(result);
        }

    }
}
