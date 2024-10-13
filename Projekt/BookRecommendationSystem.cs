using Microsoft.ML;
using Microsoft.ML.Data;
using Projekt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekt;
public class BookRecommendationSystem
{
    private readonly MLContext mlContext;
    private ITransformer model;

    public BookRecommendationSystem()
    {
        mlContext = new MLContext();
    }

    public void TrainModel(List<BookRating> ratings)
    {
        var data = mlContext.Data.LoadFromEnumerable(ratings);

        var dataProcessingPipeline = mlContext.Transforms.Conversion.MapValueToKey("UserIdEncoded", nameof(BookRating.UserId))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("BookIdEncoded", nameof(BookRating.BookId)));

        var options = new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "UserIdEncoded",
            MatrixRowIndexColumnName = "BookIdEncoded",
            LabelColumnName = nameof(BookRating.Rating),
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var trainer = mlContext.Recommendation().Trainers.MatrixFactorization(options);

        var trainingPipeline = dataProcessingPipeline.Append(trainer);

        model = trainingPipeline.Fit(data);
    }

    public List<BookRecommendation> GetRecommendations(string userId, int count)
    {
        var predictionEngine = mlContext.Model.CreatePredictionEngine<BookRating, BookRatingPrediction>(model);

        var allBooks = new List<string> { "Book1", "Book2", "Book3", "Book4" }; // Replace with actual book IDs
        var recommendations = new List<BookRecommendation>();

        foreach (var bookId in allBooks)
        {
            var prediction = predictionEngine.Predict(new BookRating { UserId = userId, BookId = bookId });
            recommendations.Add(new BookRecommendation { BookId = bookId, Score = prediction.Score });
        }

        return recommendations.OrderByDescending(r => r.Score).Take(count).ToList();
    }
}

public class BookRatingPrediction
{
    public float Score { get; set; }
}
