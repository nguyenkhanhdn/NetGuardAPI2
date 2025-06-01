using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace NetGuardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SEController : ControllerBase
    {
        //api/SE/Predict?text=a
        //[EnableCors("Policy1")]
        [HttpGet("Predict")]
        public async Task<ActionResult<SentimentPrediction>> Get(string text)
        {
            SentimentPrediction prediction = new SentimentPrediction();
            SentimentEngine sentimentEngine = new SentimentEngine();
            try
            {
                prediction = await Task.Run(() => sentimentEngine.Predict(text));
            }
            catch (FileNotFoundException fnf)
            {
                sentimentEngine.TrainingModel();
                prediction = sentimentEngine.Predict(text);
                Console.Write(fnf.Message);
            }
            return Ok(prediction);
        }
        // GET api/<SEController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SEController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            
        }

        // PUT api/<SEController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SEController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
