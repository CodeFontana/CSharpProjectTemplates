using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersionNeutral]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [HttpPost]
    public void Post([FromBody] string value)
    {

    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {

    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {

    }
}
