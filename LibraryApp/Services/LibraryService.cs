using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Services
{
    public class LibraryService
    {
        private readonly IList<LibraryItem> _items;
        private readonly IList<Reservation> _reservations;
        private readonly HashSet<string> _users;
        
        public event Action<Reservation>? OnNewReservation;
        public event Action<Reservation>? OnReservationCancelled;

        public LibraryService(IList<LibraryItem>? items = null, IList<Reservation>? reservations = null)
        {
            _items = items ?? new List<LibraryItem>();
            _reservations = reservations ?? new List<Reservation>();
            _users = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        
        public int NextItemId() => _items.Count == 0 ? 1 : _items.Max(i => i.Id) + 1;
        
        public void RegisterUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            _users.Add(email.Trim());
        }

 
        public IEnumerable<string> GetUsers() => _users.ToList();
        
        public void AddItem(LibraryItem item) => _items.Add(item);
        
        public IEnumerable<LibraryItem> ListAvailableItems()
            => _items.Where(i => i.IsAvailable).ToList();
        
        public IEnumerable<Reservation> GetUserReservations(string userEmail)
            => _reservations.Where(r => string.Equals(r.UserEmail, userEmail, StringComparison.OrdinalIgnoreCase)).ToList();
        
        public IEnumerable<LibraryItem> GetAllItems() => _items.ToList();
        
        public IEnumerable<Reservation> GetAllReservations() => _reservations.ToList();
        

        public Reservation CreateReservation(int itemId, string userEmail, DateTime from, DateTime to)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                throw new ArgumentException("User email required", nameof(userEmail));

            if (from >= to)
                throw new ArgumentException("From must be earlier than To.", nameof(from));

            var item = _items.FirstOrDefault(i => i.Id == itemId)
                       ?? throw new ArgumentException($"Item with id {itemId} not found.", nameof(itemId));
            
            bool overlaps = _reservations
                .Where(r => r.Item.Id == itemId && r.IsActive && !r.IsCancelled)
                .Any(r => DatesOverlap(r.From, r.To, from, to));

            if (overlaps)
            {
                throw new ReservationConflictException($"Reservation for item {itemId} conflicts with an existing reservation.");
            }

            var nextId = (_reservations.Count == 0) ? 1 : _reservations.Max(r => r.Id) + 1;
            var reservation = new Reservation(nextId, item, userEmail, from, to);

            _reservations.Add(reservation);
            
            item.IsAvailable = false;
            
            OnNewReservation?.Invoke(reservation);

            return reservation;
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId)
                              ?? throw new ArgumentException($"Reservation with id {reservationId} not found.", nameof(reservationId));

            if (!reservation.IsActive || reservation.IsCancelled)
                return;

            reservation.IsCancelled = true;
            reservation.IsActive = false;
            
            var item = reservation.Item;
            bool otherActive = _reservations.Any(r => r.Item.Id == item.Id && r.IsActive && !r.IsCancelled);
            if (!otherActive)
            {
                item.IsAvailable = true;
            }
            
            OnReservationCancelled?.Invoke(reservation);
        }
        
        private static bool DatesOverlap(DateTime existingFrom, DateTime existingTo, DateTime newFrom, DateTime newTo)
        {
            return existingFrom < newTo && newFrom < existingTo;
        }
        
        public void ClearAll()
        {
            _items.Clear();
            _reservations.Clear();
            _users.Clear();
        }
    }
}
