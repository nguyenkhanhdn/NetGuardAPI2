using Microsoft.ML;
using System;
using System.IO;

namespace NetGuardAPI.Controllers
{
    public class SentimentEngine
    {
        //Module huấn luyện mô hình
        public void TrainingModel()
        {
            var mlContext = new MLContext();
            //Creating the ML Pipeline
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(SentimentData.Label),
                    featureColumnName: "Features"));

            //Training the Model
            var data_file = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "data", "HuanLuyenKHKT.csv"));
            //var data_file = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "data", "final_adult_content_vi.txt"));
            var trainDataView = mlContext.Data.LoadFromTextFile<SentimentData>(data_file, separatorChar: ';', hasHeader: true);
            //var trainDataView = mlContext.Data.LoadFromEnumerable(trainingData); Load data from text
            var model = pipeline.Fit(trainDataView);
            //Save the model

            // Save the trained model to a file
            var modelPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "models", "model.zip"));
            mlContext.Model.Save(model, trainDataView.Schema, modelPath);
        }
        //Chẩn đoán
        public SentimentPrediction Predict(string inputText)
        {
            var mlContext = new MLContext();

            //Load the trained model
            var modelPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "models", "model.zip"));
            var model = mlContext.Model.Load(modelPath, out var modelInputSchema);

            var inputData = new SentimentData { Text = inputText };

            // Create prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

            var result = predictionEngine.Predict(inputData);

            return result;

        }
    }

}
