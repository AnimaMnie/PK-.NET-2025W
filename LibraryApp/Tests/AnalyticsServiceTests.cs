using System;
using Domain;
using Services;
using Xunit;

namespace Tests
{
    public class AnalyticsServiceTests
    {
        [Fact]
        public void AverageLoanLengthDays_Empty_ReturnsZero()
        {
            var lib = new LibraryService();
            var analytics = new AnalyticsService(lib);
            Assert.Equal(0.0, analytics.AverageLoanLengthDays());
        }

        [Fact]
        public void MostPopularItemTitle_WorksWithData()
        {
            var lib = new LibraryService();
            var analytics = new AnalyticsService(lib);
            var id1 = lib.NextItemId();
            lib.AddItem(new Book(id1, "AAA", "X", "I1"));
            var id2 = lib.NextItemId();
            lib.AddItem(new Book(id2, "BBB", "Y", "I2"));
            lib.RegisterUser("a@b.com");

            lib.CreateReservation(id1, "a@b.com", DateTime.Now, DateTime.Now.AddDays(1));
            lib.CreateReservation(id1, "a@b.com", DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));
            lib.CreateReservation(id2, "a@b.com", DateTime.Now.AddDays(4), DateTime.Now.AddDays(5));

            var top = analytics.MostPopularItemTitle();
            Assert.Equal("AAA", top);
        }
    }
}