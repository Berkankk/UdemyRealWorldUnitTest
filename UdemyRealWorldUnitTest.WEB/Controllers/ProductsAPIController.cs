using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.WEB.Models;
using UdemyRealWorldUnitTest.WEB.Repository;

namespace UdemyRealWorldUnitTest.WEB.Controllers
{

    //api/productsapi/ istek at 
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IRepository<Product> _productsRepository;

        public ProductsAPIController(IRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }

        [HttpGet("{a}/{b}")]
        public IActionResult Add(int a, int b)
        {
            return Ok(new Helpers.Helper().add(a, b));
        }

        // GET: api/ProductsAPI
        [HttpGet]
        public async Task<IActionResult> GetProducts()//Her metot bir endpointtir
        {
            var product = await _productsRepository.GetAll();
            return Ok(product);//200 dönecek başarılı olacak 201 yeni ürün girişi olduğu söyler
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {


            var product = await _productsRepository.GetById(id);

            if (product == null)
            {
                return NotFound(); //product null ise notfounddön 
            }

            return Ok(product); //Burasıda başarılı olursa productı dön
        }


        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)//Güncellemenin update metotu yoktur 
        {
            if (id != product.Id) //id ler eşit değilse 
            {
                return BadRequest();
            }


            _productsRepository.Update(product);

            return NoContent();//204 durum kodu döner
        }


        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _productsRepository.Create(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);//201 durum kodu dönmüş
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _productsRepository.GetById(id);
            if (product == null)
            {
                return NotFound(); //id null ise bulunamadı döner 
            }

            _productsRepository.Delete(product); //product içinde yakaldığımız id başarılı ise sil dedik

            return NoContent();//Action da dönebilirdik ama böylesi daha iyi 
        }

        private bool ProductExists(int id)//Data var mı yok mu onu görüyoruz 
        {
            Product product = _productsRepository.GetById(id).Result; //Await kullanmak yerine result verdik 
            ;
            if (product == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
