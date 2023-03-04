using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataBookApi.Interfaces;
using DataBookApi.Models;

namespace DataBookApi.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DataBookApiController : ControllerBase
    {
        private readonly IDataBookData dataBookData;
        public DataBookApiController(IDataBookData dBData)
        {
            dataBookData = dBData;
        }

        // GET api/MyApi
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<IDataBook> Get()
        {
            return dataBookData.GetAllDatabooks();
        }

        // GET api/MyApi/1
        [HttpGet("{id}")]
        public IDataBook GetDataBookById(int id)
        {
            return dataBookData.ReadDataBook(id);
        }

        // POST api/MyApi
        [HttpPost]
        public void Post([FromBody] DataBook dataBook)
        {
            dataBookData.CreateDataBook(dataBook);
        }

        // PUT api/MyApi/3
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public void Put(int id, [FromBody] DataBook dataBook)
        {
            dataBookData.UpdateDataBookById(id, dataBook);
        }

        // DELETE api/MyApi/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            dataBookData.DeleteDataBookById(id);
        }

    }
}
