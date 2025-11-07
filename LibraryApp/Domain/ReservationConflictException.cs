using System;

namespace Domain
{
    public class ReservationConflictException : Exception
    {
        public ReservationConflictException() { }
        public ReservationConflictException(string message) : base(message) { }
        public ReservationConflictException(string message, Exception inner) : base(message, inner) { }
    }
}