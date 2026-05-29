using TrainingService.Data;
using TrainingService.Export;
using TrainingService.Training;

var rawPath     = args.ElementAtOrDefault(0) ?? "data/raw/database_sympla.csv";
var datasetPath = args.ElementAtOrDefault(1) ?? "data/dataset/dataset.csv";
var weightsPath = args.ElementAtOrDefault(2) ?? "data/models/svd_weights.json";

Console.WriteLine("=== Aventura Training Service ===");
Console.WriteLine($"Input:   {rawPath}");
Console.WriteLine($"Dataset: {datasetPath}");
Console.WriteLine($"Output:  {weightsPath}");
Console.WriteLine();

var records = DataProcessor.Process(rawPath, datasetPath);
var weights = SvdTrainer.Train(records, TrainingConfig.Default);
WeightsExporter.Export(weights, weightsPath);

Console.WriteLine("\nPipeline concluído.");
