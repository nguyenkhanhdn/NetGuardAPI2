using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace NetGuardAPI.Controllers
{
    [System.Web.Http.Route("api/[controller]")]
    [ApiController]
    public class ImageSEController : ControllerBase
    {
        [HttpPost("Predict")]
        [Route("api/Predict")] // API endpoint: /api/Predict
        public async Task<ActionResult<SentimentPrediction>> Predict(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "File không hợp lệ hoặc trống." });
            }

            var uploadPath = NetGuardUtil.GetUploadDir();

            //Tạo ra file ngẫu nhiên và đặt tên cho file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageBytes = System.IO.File.ReadAllBytes(filePath);
            NetGaurd4KidMLModel.ModelInput modelInput = new NetGaurd4KidMLModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

            //var sortedScoresWithLabel = ImgMLModel.Predict(sampleData);
            var sortedScoresWithLabel = NetGaurd4KidMLModel.PredictAllLabels(modelInput);


            var maxScoreLabel = sortedScoresWithLabel.OrderByDescending(x => x.Value).FirstOrDefault();
            var predict = new PredictResult
            {
                Prediction = maxScoreLabel.Key.ToString(),
                Probability = maxScoreLabel.Value
            };
            return Ok(predict);
        }
    }
}
