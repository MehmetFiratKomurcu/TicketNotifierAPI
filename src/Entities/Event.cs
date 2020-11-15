using System;

namespace TicketNotifier.Entities
{
    public class Event
    {
        public string Place { get; set; }
        public string Stage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}