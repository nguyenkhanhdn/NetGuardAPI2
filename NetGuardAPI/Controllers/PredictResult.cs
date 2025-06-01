using Microsoft.ML.Data;

namespace NetGuardAPI.Controllers
{
    public class PredictResult
    {

        [ColumnName("PredictedLabel")]
        public  string Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}