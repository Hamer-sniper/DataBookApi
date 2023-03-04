using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataBookApi.Interfaces;
using DataBookApi.Models;

namespace DataBookApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DataBookApiController : ControllerBase
    {
        private readonly IDataBookData dataBookData;
        public DataBookApiController(IDataBookData dBData)
        {
            dataBookData = dBData;
        }

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<IDataBook> GetDataBooks()
        {
            return dataBookData.GetAllDatabooks();
        }

        [HttpGet("{id}")]
        public IDataBook GetDataBookById(int id)
        {
            return dataBookData.ReadDataBook(id);
        }

        [HttpPost]
        public void Post([FromBody] DataBook dataBook)
        {
            dataBookData.CreateDataBook(dataBook);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public void Put(int id, [FromBody] DataBook dataBook)
        {
            dataBookData.UpdateDataBookById(id, dataBook);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public void Delete(int id)
        {
            dataBookData.DeleteDataBookById(id);
        }

    }
}
