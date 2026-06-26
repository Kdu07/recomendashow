using AventuraML;
using AventuraML.Models;
using FluentAssertions;
using Xunit;

namespace InferenceService.Tests;

public class EvaluatorSplitByUserTests
{
    private static CsvRatingRow Row(int userId, string itemId) =>
        new() { UserId = userId, ItemId = itemId, Ranking = 3f };

    [Fact]
    public void SameSeed_ProducesSameSplit()
    {
        var rows = Enumerable.Range(1, 10)
            .Select(i => Row(1, $"Item{i}"))
            .ToList();

        var (train1, test1) = Evaluator.SplitByUser(rows, seed: 42);
        var (train2, test2) = Evaluator.SplitByUser(rows, seed: 42);

        train1.Select(r => r.ItemId).Should().Equal(train2.Select(r => r.ItemId));
        test1[1].Select(r => r.ItemId).Should().Equal(test2[1].Select(r => r.ItemId));
    }

    [Fact]
    public void DifferentSeeds_ProduceDifferentSplits()
    {
        var rows = Enumerable.Range(1, 20)
            .Select(i => Row(1, $"Item{i}"))
            .ToList();

        var (_, test42) = Evaluator.SplitByUser(rows, seed: 42);
        var (_, test99) = Evaluator.SplitByUser(rows, seed: 99);

        test42[1].Select(r => r.ItemId)
            .Should().NotEqual(test99[1].Select(r => r.ItemId));
    }

    [Fact]
    public void UserWithSingleInteraction_GoesEntirely_ToTrain()
    {
        var rows = new List<CsvRatingRow> { Row(userId: 1, "ItemA") };

        var (train, test) = Evaluator.SplitByUser(rows);

        train.Should().ContainSingle().Which.UserId.Should().Be(1);
        test.Should().NotContainKey(1);
    }

    [Fact]
    public void UserWithSingleInteraction_DoesNotAppear_InTestDictionary()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "ItemA"),
            Row(2, "ItemB"),
            Row(2, "ItemC"),
            Row(2, "ItemD"),
            Row(2, "ItemE"),
        };

        var (_, test) = Evaluator.SplitByUser(rows);

        test.Should().NotContainKey(1);
        test.Should().ContainKey(2);
    }

    [Fact]
    public void TotalInteractions_ArePreserved_ForSingleUser()
    {
        var rows = Enumerable.Range(1, 10)
            .Select(i => Row(1, $"Item{i}"))
            .ToList();

        var (train, test) = Evaluator.SplitByUser(rows);

        var testCount = test.GetValueOrDefault(1)?.Count ?? 0;
        (train.Count + testCount).Should().Be(rows.Count);
    }

    [Fact]
    public void TotalInteractions_ArePreserved_ForMultipleUsers()
    {
        var rows = new List<CsvRatingRow>();
        for (var u = 1; u <= 5; u++)
            for (var i = 1; i <= 8; i++)
                rows.Add(Row(u, $"Item{i}"));

        var (train, test) = Evaluator.SplitByUser(rows);

        var testCount = test.Values.Sum(v => v.Count);
        (train.Count + testCount).Should().Be(rows.Count);
    }

    [Fact]
    public void TotalInteractions_ArePreserved_WhenSomeUsersHaveSingleInteraction()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "SoloItem"),
            Row(2, "ItemA"),
            Row(2, "ItemB"),
            Row(2, "ItemC"),
        };

        var (train, test) = Evaluator.SplitByUser(rows);

        var testCount = test.Values.Sum(v => v.Count);
        (train.Count + testCount).Should().Be(rows.Count);
    }

    [Fact]
    public void UserWithManyInteractions_HasApproximately20PercentInTest()
    {
        var rows = Enumerable.Range(1, 100)
            .Select(i => Row(1, $"Item{i}"))
            .ToList();

        var (train, test) = Evaluator.SplitByUser(rows);

        test[1].Count.Should().BeCloseTo(20, delta: 3);
        train.Count.Should().BeCloseTo(80, delta: 3);
    }

    [Fact]
    public void UserWithTwoInteractions_HasExactlyOneInTest()
    {
        var rows = new List<CsvRatingRow> { Row(1, "ItemA"), Row(1, "ItemB") };

        var (train, test) = Evaluator.SplitByUser(rows);

        test[1].Should().HaveCount(1);
        train.Should().HaveCount(1);
    }

    [Fact]
    public void TrainAndTestItems_AreDisjoint_PerUser()
    {
        var rows = Enumerable.Range(1, 10)
            .Select(i => Row(1, $"Item{i}"))
            .ToList();

        var (train, test) = Evaluator.SplitByUser(rows);

        var trainItems = train.Select(r => r.ItemId).ToHashSet();
        var testItems = test[1].Select(r => r.ItemId).ToHashSet();

        trainItems.Should().NotIntersectWith(testItems);
    }

    [Fact]
    public void MultipleUsers_AreProcessedIndependently()
    {
        var rows = new List<CsvRatingRow>();
        for (var u = 1; u <= 3; u++)
            for (var i = 1; i <= 10; i++)
                rows.Add(Row(u, $"Item{u}_{i}"));

        var (train, test) = Evaluator.SplitByUser(rows);

        test.Should().HaveCount(3);
        foreach (var userId in new[] { 1, 2, 3 })
        {
            var testCount = test[userId].Count;
            test[userId].Should().AllSatisfy(r => r.UserId.Should().Be(userId));
            train.Where(r => r.UserId == userId).Should()
                .AllSatisfy(r => r.UserId.Should().Be(userId));
            (train.Count(r => r.UserId == userId) + testCount).Should().Be(10);
        }
    }

    [Fact]
    public void EmptyDataset_ReturnsBothEmptyCollections()
    {
        var (train, test) = Evaluator.SplitByUser([]);

        train.Should().BeEmpty();
        test.Should().BeEmpty();
    }
}

public class EvaluatorBuildCategoryAffinityTests
{
    private static CsvRatingRow Row(int userId, string itemId, string category, float ranking) =>
        new() { UserId = userId, ItemId = itemId, TipoEvento = category, Ranking = ranking };

    [Fact]
    public void UserWithSingleCategory_HasAffinity1()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "ItemA", "Teatro", 5f),
            Row(1, "ItemB", "Teatro", 3f),
        };

        var affinity = Evaluator.BuildCategoryAffinity(rows);

        affinity["1"]["Teatro"].Should().Be(1.0f);
    }

    [Fact]
    public void AffinitySum_IsExactly1_ForAnyUser()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "ItemA", "Teatro",  5f),
            Row(1, "ItemB", "Musical", 3f),
            Row(1, "ItemC", "Danca",   2f),
        };

        var affinity = Evaluator.BuildCategoryAffinity(rows);

        affinity["1"].Values.Sum().Should().BeApproximately(1.0f, precision: 1e-6f);
    }

    [Fact]
    public void TwoCategories_WithEqualRankingSum_HaveAffinity05Each()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "ItemA", "Teatro",  3f),
            Row(1, "ItemB", "Musical", 3f),
        };

        var affinity = Evaluator.BuildCategoryAffinity(rows);

        affinity["1"]["Teatro"].Should().BeApproximately(0.5f, precision: 1e-6f);
        affinity["1"]["Musical"].Should().BeApproximately(0.5f, precision: 1e-6f);
    }

    [Fact]
    public void Affinity_IsWeightedByRankingSum_NotByCount()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "ItemA", "Teatro",  4f),
            Row(1, "ItemB", "Musical", 1f),
            Row(1, "ItemC", "Musical", 1f),
        };

        var affinity = Evaluator.BuildCategoryAffinity(rows);

        affinity["1"]["Teatro"].Should().BeApproximately(4f / 6f, precision: 1e-6f);
        affinity["1"]["Musical"].Should().BeApproximately(2f / 6f, precision: 1e-6f);
    }

    [Fact]
    public void UserKey_IsStringRepresentationOfUserId()
    {
        var rows = new List<CsvRatingRow> { Row(42, "ItemA", "Teatro", 3f) };

        var affinity = Evaluator.BuildCategoryAffinity(rows);

        affinity.Should().ContainKey("42");
        affinity.Should().NotContainKey("0");
    }

    [Fact]
    public void MultipleUsers_HaveIndependentAffinities()
    {
        var rows = new List<CsvRatingRow>
        {
            Row(1, "ItemA", "Teatro",  5f),
            Row(2, "ItemB", "Musical", 5f),
        };

        var affinity = Evaluator.BuildCategoryAffinity(rows);

        affinity["1"].Should().ContainKey("Teatro").And.NotContainKey("Musical");
        affinity["2"].Should().ContainKey("Musical").And.NotContainKey("Teatro");
    }

    [Fact]
    public void EmptyDataset_ReturnsEmptyDictionary()
    {
        var affinity = Evaluator.BuildCategoryAffinity([]);

        affinity.Should().BeEmpty();
    }
}

public class EvaluatorEvaluateWithPredictorTests
{
    private static CsvRatingRow Row(int userId, string itemId, string category, float ranking) =>
        new() { UserId = userId, ItemId = itemId, TipoEvento = category, Ranking = ranking };

    // User 1 consumed ItemA/ItemB in train → ItemC and ItemD are candidates for user 1.
    // Test set for user 1 contains ItemC.
    private static List<CsvRatingRow> TrainRows() =>
    [
        Row(1, "ItemA", "Teatro",  3f),
        Row(1, "ItemB", "Musical", 3f),
        Row(2, "ItemC", "Opera",   3f),
        Row(2, "ItemD", "Teatro",  3f),
    ];

    private static Dictionary<int, List<CsvRatingRow>> TestByUser() =>
        new() { [1] = [Row(1, "ItemC", "Opera", 3f)] };

    private static readonly IReadOnlyDictionary<string, string> ItemCats =
        new Dictionary<string, string>
        {
            ["ItemA"] = "Teatro", ["ItemB"] = "Musical",
            ["ItemC"] = "Opera",  ["ItemD"] = "Teatro",
        };

    // User 1 strongly prefers Teatro (0.9) over Opera (0.1).
    private static readonly IReadOnlyDictionary<string, Dictionary<string, float>> CategoryAffinity =
        new Dictionary<string, Dictionary<string, float>>
        {
            ["1"] = new() { ["Teatro"] = 0.9f, ["Opera"] = 0.1f },
        };

    [Fact]
    public void PerfectPredictor_YieldsSvdHitRate1()
    {
        float? predict(int _, string item) => item == "ItemC" ? 5f : 1f;

        var report = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 1);

        report.SvdRanking.HitRateAtK.Should().Be(1.0);
    }

    [Fact]
    public void PredictorThatNeverHits_YieldsSvdHitRate0()
    {
        float? predict(int _, string item) => item == "ItemD" ? 5f : 1f;

        var report = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 1);

        report.SvdRanking.HitRateAtK.Should().Be(0.0);
    }

    [Fact]
    public void Alpha1_HybridFollowsSvd_Alpha0_HybridFollowsAffinity()
    {
        // SVD scores ItemC highest; affinity pushes ItemD (Teatro=0.9) above ItemC (Opera=0.1).
        // alpha=1.0 → hybrid = SVD order → ItemC top-1 → hit
        // alpha=0.0 → hybrid = affinity order → ItemD top-1 → miss
        float? predict(int _, string item) => item == "ItemC" ? 5f : 1f;

        var reportAlpha1 = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 1, alpha: 1.0f);

        var reportAlpha0 = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 1, alpha: 0.0f);

        reportAlpha1.HybridRanking.HitRateAtK.Should().Be(1.0);
        reportAlpha0.HybridRanking.HitRateAtK.Should().Be(0.0);
    }

    [Fact]
    public void Alpha_DoesNotAffect_SvdRanking()
    {
        float? predict(int _, string item) => item == "ItemC" ? 5f : 1f;

        var reportAlpha1 = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 1, alpha: 1.0f);

        var reportAlpha0 = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 1, alpha: 0.0f);

        reportAlpha1.SvdRanking.HitRateAtK.Should().Be(reportAlpha0.SvdRanking.HitRateAtK);
    }

    [Fact]
    public void PerfectRatingPredictor_YieldsRmseNearZero()
    {
        float? predict(int _, string __) => 3f;

        var report = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity);

        report.Rating.Rmse.Should().BeApproximately(0.0, precision: 1e-6);
        report.Rating.PairCount.Should().Be(1);
    }

    [Fact]
    public void PerfectPredictor_YieldsNonNegativeRankingMetrics()
    {
        float? predict(int _, string item) => item == "ItemC" ? 5f : 1f;

        var report = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), TestByUser(), ItemCats, CategoryAffinity, topK: 2);

        report.SvdRanking.PrecisionAtK.Should().BeGreaterThanOrEqualTo(0.0);
        report.SvdRanking.RecallAtK.Should().BeGreaterThanOrEqualTo(0.0);
        report.SvdRanking.CategoryPrecisionAtK.Should().BeGreaterThanOrEqualTo(0.0);
        report.HybridRanking.PrecisionAtK.Should().BeGreaterThanOrEqualTo(0.0);
        report.HybridRanking.RecallAtK.Should().BeGreaterThanOrEqualTo(0.0);
        report.HybridRanking.CategoryPrecisionAtK.Should().BeGreaterThanOrEqualTo(0.0);
    }

    [Fact]
    public void MaxRankingUsers_LimitsUsersEvaluatedInRanking()
    {
        float? predict(int _, string item) => item == "ItemC" ? 5f : 1f;
        var twoUserTest = new Dictionary<int, List<CsvRatingRow>>
        {
            [1] = [Row(1, "ItemC", "Opera", 3f)],
            [2] = [Row(2, "ItemA", "Teatro", 3f)],
        };

        var report = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), twoUserTest, ItemCats, CategoryAffinity,
            topK: 1, maxRankingUsers: 1);

        report.UsersEvaluated.Should().Be(1);
    }

    [Fact]
    public void EmptyTestSet_YieldsUsersEvaluated0_AndZeroMetrics()
    {
        float? predict(int _, string __) => 3f;

        var report = Evaluator.EvaluateWithPredictor(
            predict, TrainRows(), new Dictionary<int, List<CsvRatingRow>>(),
            ItemCats, CategoryAffinity);

        report.UsersEvaluated.Should().Be(0);
        report.Rating.PairCount.Should().Be(0);
        report.SvdRanking.HitRateAtK.Should().Be(0.0);
        report.HybridRanking.HitRateAtK.Should().Be(0.0);
    }
}
