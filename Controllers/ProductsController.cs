using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    // localhost:5000/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        /*private static List<Product> _products;*/

        private readonly AppDataContext _appDataContext;

        public ProductsController(AppDataContext appDataContext)
        {
            _appDataContext = appDataContext;
        }

       /* public ProductsController()
        {
            _products = new List<Product>(); //önce bir liste oluşturup aşağıdakilerini bu listeye atalım
            _products.Add(new Product() { ProductId = 1, Name = "Kartole", Price = 120, IsActive = false });
            _products.Add(new Product() { ProductId = 2, Name = "Tıvhe", Price = 25, IsActive = false });
            _products.Add(new Product() { ProductId = 3, Name = "Şargıl", Price = 651, IsActive = true });
            _products.Add(new Product() { ProductId = 4, Name = "Doze", Price = 26123, IsActive = false });
            _products.Add(new Product() { ProductId = 5, Name = "Azran", Price = 454, IsActive = true });
            _products.Add(new Product() { ProductId = 6, Name = "Ribes", Price = 654, IsActive = false });
            _products.Add(new Product() { ProductId = 7, Name = "Sınun", Price = 65, IsActive = true });
            _products.Add(new Product() { ProductId = 8, Name = "Anıp", Price = 5489, IsActive = true });
        }*/

        // localhost:5000/api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products =await _appDataContext.Products.
                Select(p=> ProductToDTO(p)) //P OBJESİ. liste üzerindeki her bir elemanı al ProductToDTO ya gönder.
                //secret property sini almak istemdiğimiz için onu almadık. gerek yok.                             
            .ToListAsync();
            return Ok(products);
        }

        // localhost:5000/api/products/2
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id){
            var p =await _appDataContext.Products.FindAsync(id);
            if(p==null){
                return NotFound();
            }
            return Ok(ProductToDTO(p));        
        }

        // localhost:5000/api/products a post sorgusu attığımızda burası çalışacak
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product p){
           _appDataContext.Products.Add(p);
           await _appDataContext.SaveChangesAsync();
           return CreatedAtAction(nameof(GetProducts), new {id=p.ProductId},ProductToDTO(p));
        }

        [HttpPut("{id}")] //localhost:5000/api/products/2
        public async Task<IActionResult> UpdateProduct(int id, Product p)
        {
           if(id !=p.ProductId)
           {
            return BadRequest();
           }
           
           var product = await _appDataContext.Products.FindAsync(id);
           if(product == null)
           {
            return NotFound();
           }
           product.Name = p.Name;
           product.Price = p.Price;

           try
           {
            await _appDataContext.SaveChangesAsync();
           }
           catch(Exception e)
           {
            return NotFound();
           }
           return NoContent();
        }

        [HttpDelete("{id}")] //localhost:5000/api/products/2
        public async Task<IActionResult> DeleteProductById(int id){
            var product = await _appDataContext.Products.FindAsync(id);
            if(product==null)
            {
                return NotFound();
            }
            _appDataContext.Products.Remove(product);
            await _appDataContext.SaveChangesAsync();
            return NoContent();
        }

        private static ProductDTO ProductToDTO(Product p){
            return new ProductDTO()
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            };
        }
    }
}