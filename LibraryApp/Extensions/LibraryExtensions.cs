using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Extensions
{
    public static class LibraryExtensions
    {
        public static IEnumerable<T> Available<T>(this IEnumerable<T> items) where T : LibraryItem
            => items.Where(i => i.IsAvailable);

        public static IEnumerable<LibraryItem> Newest(this IEnumerable<LibraryItem> items, int take)
            => items.OrderByDescending(i => i.Id).Take(take);

        public static IEnumerable<LibraryItem> FindByTitleOrAuthor(this IEnumerable<LibraryItem> items, string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return items;
            var p = phrase.Trim();
            return items.Where(i =>
                i.Title.Contains(p, System.StringComparison.OrdinalIgnoreCase)
                || (i is Book b && b.Author.Contains(p, System.StringComparison.OrdinalIgnoreCase)));
        }
    }
}