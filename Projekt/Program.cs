// See https://aka.ms/new-console-template for more information





using Projekt;

var ratings = new List<BookRating>
        {
            new BookRating { UserId = "user1", BookId = "book1", Rating = 5 },
            new BookRating { UserId = "user1", BookId = "book2", Rating = 3 },
            new BookRating { UserId = "user2", BookId = "book1", Rating = 4 },
            new BookRating { UserId = "user2", BookId = "book3", Rating = 5 }
        };

            var recommendationSystem = new BookRecommendationSystem();
            recommendationSystem.TrainModel(ratings);

            var recommendations = recommendationSystem.GetRecommendations("user1", 3);

            foreach (var recommendation in recommendations)
            {
                Console.WriteLine($"BookId: {recommendation.BookId}, Score: {recommendation.Score}");
            }