using backend.Data;
using backend.Migrations;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly DataContext _context;

        public SearchController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MobilePhones>>> SearchMobilePhones(string searchString = "")
        {
            // Исключения, которые приводят сразу к выводу ошибки 
            if (searchString == "")
            {
                return Ok("Empty");
            }

            // Преобразования строки к тому или иному виду 
            searchString = searchString.Trim();
            searchString = searchString.ToLower();
            searchString = Regex.Replace(searchString, @"\s+", " ");

            //Исключение повторяющихся слов
            HashSet<string> str = new HashSet<string>();
            string[] data = searchString.Split(' ');
            for (int i = 0; i < data.Length; i++)
            {
                str.Add(data[i]);
            }

            searchString = string.Join(" ", str);

            string hashtags = "";
            string hashtags2 = "";
            string productName = "";

            int spaceCounter = 0;

            for (int i = 0; i < searchString.Length; i++)
            {
                if (searchString[i].Equals(' '))
                {
                    spaceCounter++;
                }
            }

            int hashtagsCounter = 0;

            while (spaceCounter >= 0)
            {
                string temp = searchString.Split()[spaceCounter];
                if (temp.Contains('#') && _context.MobilePhones.Any(p => p.Hashtags.ToLower().Contains(temp)))
                {
                    hashtagsCounter++;
                    if (hashtagsCounter <= 1)
                    {
                        hashtags = temp;
                    }
                    else if (hashtagsCounter > 1)
                    {
                        hashtags2 = temp;
                    }
                }
                else if (_context.MobilePhones.Any(p => p.ProductName.ToLower().Contains(temp)))
                {
                    productName = temp;
                }

                spaceCounter--;
            }

            var products = _context.MobilePhones;

            if (hashtags != "" && hashtags2 != "" && productName != "")
            {
                var threeOptions = products.Where(u => (u.Hashtags.ToLower().Contains(hashtags) && u.Hashtags.ToLower().Contains(hashtags2) && u.ProductName.ToLower().Contains(productName)));

                return Ok(threeOptions);
            }
            else if (hashtags != "" && hashtags2 != "" && productName == "")
            {
                var twoOptions = products.Where(u => u.Hashtags.ToLower().Contains(hashtags) && u.Hashtags.ToLower().Contains(hashtags2));

                return Ok(twoOptions);
            }
            else if (hashtags != "" || productName != "")
            {
                var twoOptions = products.Where(u => u.Hashtags.ToLower().Contains(hashtags) && u.ProductName.ToLower().Contains(productName));

                return Ok(twoOptions);
            }

            var oneOption = products.Where(u => u.Hashtags.ToLower().Contains(searchString) || u.ProductName.ToLower().Contains(searchString));

            if (!oneOption.Any())
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();

            return Ok(oneOption);
        }
    }


}