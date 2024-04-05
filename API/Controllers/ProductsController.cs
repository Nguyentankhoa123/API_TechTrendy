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
    }
}
