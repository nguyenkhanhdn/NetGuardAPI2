using Microsoft.AspNetCore.Mvc;
using NetGuardAPI.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace NetGuardAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TextPredict()
        {
            return View();
        }
        public IActionResult TrainModel()
        {
            SentimentEngine se  = new SentimentEngine();
            se.TrainingModel();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ImagePredict()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VideoPredict()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VideoPredict(string img_data)
        {
            var fileName = "img" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "") + ".png";
            string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            using (FileStream fs = new FileStream(SavePath, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                byte[] data = Convert.FromBase64String(img_data);
                bw.Write(data);
            }

            var imageBytes = System.IO.File.ReadAllBytes(SavePath);

            NetGaurd4KidMLModel.ModelInput modelInput = new NetGaurd4KidMLModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

            var sortedScoresWithLabel = NetGaurd4KidMLModel.PredictAllLabels(modelInput);
            var maxScoreLabel = sortedScoresWithLabel.OrderByDescending(x => x.Value).FirstOrDefault();

            var result = new PredictResult
            {
                Prediction = maxScoreLabel.Key.ToString(),
                Probability = maxScoreLabel.Value
            };

            // Cách 1: trả về plain text để phù hợp với JavaScript đoạn này: response.text()
            return Content($"{result.Prediction} ({result.Probability})");

            // Cách 2: nếu muốn trả về dạng JSON (cần sửa JavaScript đoạn xử lý lại thành JSON.parse)
            // return Json(result);
        }


        [HttpGet]
        public IActionResult ImgPredict()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<SentimentPrediction>> ImgPredict(IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                var fileext = Path.GetExtension(fileUpload.FileName);
                var myUniqueFileName = Guid.NewGuid() + fileext;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", myUniqueFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                var imageBytes = System.IO.File.ReadAllBytes(filePath);
                NetGaurd4KidMLModel.ModelInput modelInput = new NetGaurd4KidMLModel.ModelInput()
                {
                    ImageSource = imageBytes,
                };

                //var sortedScoresWithLabel = ImgMLModel.Predict(sampleData);
                var sortedScoresWithLabel = NetGaurd4KidMLModel.PredictAllLabels(modelInput);

               
                var maxScoreLabel = sortedScoresWithLabel.OrderByDescending(x => x.Value).FirstOrDefault();
                var result = new PredictResult
                {
                    Prediction = maxScoreLabel.Key.ToString(),
                    Probability = maxScoreLabel.Value
                };

                ViewBag.Prediction = maxScoreLabel.Key.ToString();
                ViewBag.Probability = maxScoreLabel.Value;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
