using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController( ILogger<CatalogController> logger, IProductRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        //this definition indicates that the get method return signature of  something like this
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products =  await _repository.GetProducts();
            return  Ok(products);
        }

        [HttpGet("id:length(24)",Name ="GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            var product = await _repository.GetProductById(id);
            if (product == null)
            {
                _logger.LogError($"Product with id:{id} not found...");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet ]
        [Route("[action]/{category}", Name= "GetProductByCategory")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products =await _repository.GetProductsByCategory(category);
            return Ok(products);
        }


        [HttpGet]
        [Route("[action]/{productName}", Name = "GetProductByName")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string productName)
        {
            var products = await _repository.GetProductsByName(productName);
            if(products == null)
            {
                _logger.LogError($"Products with name:{productName} not found...");
                return NotFound();
            }
            return Ok(products);
        }


        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct",new { id=product.Id}, product);
            //this routing send to us line36 to retrieve the product
        }


        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {   // USE IACTIONRESULT
            // if you dont want to return any SPECIFIC type of the response
            return Ok(await _repository.UpdateProduct(product));
        }


        [HttpDelete("id:length(24)", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteProduct(string id)
        {
            var product = await _repository.DeleteProduct(id);
      
            return Ok(product);
        }


    }
}
