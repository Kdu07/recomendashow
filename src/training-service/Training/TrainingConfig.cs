namespace TrainingService.Training;

public sealed class TrainingConfig
{
    public int NFactors { get; init; } = 10;
    public double LrAll { get; init; } = 0.001;
    public double RegAll { get; init; } = 0.20;
    public int NEpochs { get; init; } = 50;
    public double RatingMin { get; init; } = 1.0;
    public double RatingMax { get; init; } = 5.0;
    public int RandomSeed { get; init; } = 42;
    public double TestSplitRatio { get; init; } = 0.20;

    public static readonly TrainingConfig Default = new();
}
