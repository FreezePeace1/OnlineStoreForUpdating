using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MobilePhonesController : ControllerBase
    {
        private readonly DataContext _context;
        public MobilePhonesController(DataContext context) {
            _context = context;
        }

        /*[HttpGet]
        public async Task<ActionResult<List<MobilePhones>>> GetMobilePhonesLimited()
        {

            //Возвращаем всё
            //return Ok(await _context.MobilePhones.ToArrayAsync());
            var limitedRequest = _context.MobilePhones.OrderByDescending(t => t.Article).Take(1000).ToArrayAsync();

            return Ok(await limitedRequest);
        } */

        [HttpGet]
        public IActionResult GetImage(string imageName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", imageName);
            var image = System.IO.File.OpenRead(path);
            return File(image, "image/png");
        }


        [HttpPost]
        public async Task<ActionResult<List<MobilePhones>>> PostMobilePhones(MobilePhones mobilePhones)
        {
            _context.Add(mobilePhones);
            await _context.SaveChangesAsync();
            return Ok(await _context.MobilePhones.ToArrayAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<MobilePhones>>> UpdateMobilePhones(MobilePhones mobilePhones)
        {
            var dbProduct = await _context.MobilePhones.FindAsync(mobilePhones.Id);
            if(dbProduct == null) {
                return BadRequest("MobilePhones not found");
            }
            dbProduct.Price = mobilePhones.Price;
            dbProduct.ProductName = mobilePhones.ProductName;
            dbProduct.Quantity = mobilePhones.Quantity;
            dbProduct.Description = mobilePhones.Description;
            dbProduct.Manufacturer = mobilePhones.Manufacturer;
            dbProduct.Colour = mobilePhones.Colour;
            dbProduct.Hashtags = mobilePhones.Hashtags;
            dbProduct.images = mobilePhones.images;

            await _context.SaveChangesAsync();
            return Ok(await _context.MobilePhones.ToArrayAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<MobilePhones>>> DeleteMobilePhones(int id)
        {
            var dbProduct = await _context.MobilePhones.FindAsync(id);
            if (dbProduct == null)
            {
                return BadRequest("MobilePhones not found");
            }
            _context.MobilePhones.Remove(dbProduct);
            await _context.SaveChangesAsync();

            return Ok(await _context.MobilePhones.ToListAsync());
        }
    }
}