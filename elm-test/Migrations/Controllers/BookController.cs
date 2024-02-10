using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Migrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IDapperContextDb _db;
        public BookController(IDapperContextDb db)
        {
            _db = db;
        }
        // GET: api/<BookController>

        //[HttpGet]
        //public async Task<IEnumerable<ReturnResponse>> GetBooksOnScroll(int sc )
        //{
        //    return await _db.GetBooksOnScroll(sc);
        //}
        // GET api/<BookController>/5
        [HttpGet]
        public async Task<IEnumerable<ReturnResponse>> Get(string key)
        {
           return await _db.SearchBooks(key);
            
        }

        // POST api/<BookController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
