using System;
using System.Linq;
using Domain;
using Services;
using Xunit;

namespace Tests
{
    public class LibraryServiceTests
    {
        [Fact]
        public void RegisterUser_AddsUser()
        {
            var lib = new LibraryService();
            lib.RegisterUser("test@ex.com");
            Assert.Contains("test@ex.com", lib.GetUsers(), StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateReservation_ForAvailableItem_Succeeds()
        {
            var lib = new LibraryService();
            var id = lib.NextItemId();
            var book = new Book(id, "T", "A", "I");
            lib.AddItem(book);
            lib.RegisterUser("u@e.com");
            var res = lib.CreateReservation(id, "u@e.com", DateTime.Now, DateTime.Now.AddDays(3));
            Assert.NotNull(res);
            Assert.False(book.IsAvailable);
            Assert.True(res.IsActive);
        }

        [Fact]
        public void CreateReservation_Conflicting_Throws()
        {
            var lib = new LibraryService();
            var id = lib.NextItemId();
            var book = new Book(id, "T", "A", "I");
            lib.AddItem(book);
            lib.RegisterUser("u@e.com");
            var from = DateTime.Now;
            var to = from.AddDays(5);
            var r1 = lib.CreateReservation(id, "u@e.com", from, to);
            Assert.Throws<Domain.ReservationConflictException>(() => lib.CreateReservation(id, "u@e.com", from.AddDays(1), to.AddDays(1)));
        }

        [Fact]
        public void CancelReservation_InvokesEventAndSetsAvailable()
        {
            var lib = new LibraryService();
            var id = lib.NextItemId();
            var book = new Book(id, "T", "A", "I");
            lib.AddItem(book);
            lib.RegisterUser("u@e.com");
            var r = lib.CreateReservation(id, "u@e.com", DateTime.Now, DateTime.Now.AddDays(2));

            Reservation? cancelled = null;
            lib.OnReservationCancelled += res => cancelled = res;

            lib.CancelReservation(r.Id);

            Assert.NotNull(cancelled);
            Assert.True(r.IsCancelled);
            Assert.False(r.IsActive);
            Assert.True(book.IsAvailable);
        }
    }
}
