using System;

namespace Domain
{
    public class Book : LibraryItem
    {
        public string Author { get; private set; }
        public string Isbn { get; private set; }

        public Book(int id, string title, string author, string isbn)
            : base(id, title)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Isbn = isbn ?? throw new ArgumentNullException(nameof(isbn));
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Book] Id:{Id} Title:\"{Title}\", Author:{Author}, ISBN:{Isbn}, Available:{IsAvailable}");
        }
    }
}