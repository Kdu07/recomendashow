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
