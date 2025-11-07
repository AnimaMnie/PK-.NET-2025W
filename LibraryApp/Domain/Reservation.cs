using System;

namespace Domain
{
    public class Reservation
    {
        public int Id { get; }
        public LibraryItem Item { get; }
        public string UserEmail { get; }
        public DateTime From { get; }
        public DateTime To { get; }
        
        public bool IsActive { get; set; } = true;
        public bool IsCancelled { get; set; } = false;

        public Reservation(int id, LibraryItem item, string userEmail, DateTime from, DateTime to)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(userEmail)) throw new ArgumentException("User email required", nameof(userEmail));
            if (from >= to) throw new ArgumentException("From must be earlier than To.", nameof(from));

            Id = id;
            Item = item;
            UserEmail = userEmail;
            From = from;
            To = to;
        }
    }
}