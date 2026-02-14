// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShevkunenkoSite.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManifestController : ControllerBase
    {
        // GET: api/<ManifestController>
        [HttpGet]
        public ManifestModel Get(PageInfoModel pageInfo)
        {
            ManifestModel manifest = new();

            //manifest.Start_url = "https://shevkunenko.site" + pageInfo.PageFullPathWithData;

            //    await context.Response.WriteAsJsonAsync(manifest);


            return manifest;
        }

        // GET api/<ManifestController>/5
        [HttpGet("{pageid}")]
        public string Get(Guid pageid)
        {
            return "value";
        }

        // POST api/<ManifestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ManifestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ManifestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
