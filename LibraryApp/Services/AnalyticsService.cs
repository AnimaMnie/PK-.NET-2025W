using System;
using System.Linq;
using Domain;

namespace Services
{
    public class AnalyticsService
    {
        private readonly LibraryService _library;

        public AnalyticsService(LibraryService library)
        {
            _library = library ?? throw new ArgumentNullException(nameof(library));
        }

        public double AverageLoanLengthDays()
        {
            var finished = _library.GetAllReservations().Where(r => !r.IsActive && !r.IsCancelled).ToList();
            if (!finished.Any()) return 0.0;
            return finished.Average(r => (r.To - r.From).TotalDays);
        }

        public int TotalLoans()
        {
            return _library.GetAllReservations().Count(r => !r.IsCancelled);
        }

        public string MostPopularItemTitle()
        {
            var groups = _library.GetAllReservations()
                .Where(r => !string.IsNullOrWhiteSpace(r.Item.Title))
                .GroupBy(r => r.Item.Title)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (!groups.Any()) return "Brak danych";
            var topCount = groups.First().Count;
            var top = groups.Where(g => g.Count == topCount).OrderBy(g => g.Title).First();
            return top.Title;
        }

        public double FulfillmentRate()
        {
            var all = _library.GetAllReservations().ToList();
            if (!all.Any()) return 0.0;
            var fulfilled = all.Count(r => !r.IsCancelled);
            return (double)fulfilled / all.Count;
        }

        public double LogPopularityScore(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("title required", nameof(title));
            var count = _library.GetAllReservations().Count(r => string.Equals(r.Item.Title, title, StringComparison.OrdinalIgnoreCase));
            if (count <= 0) return 0.0;
            return Math.Log(count);
        }
    }
}
