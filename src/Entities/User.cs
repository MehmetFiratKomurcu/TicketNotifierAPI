using System;
using System.Collections.Generic;

namespace TicketNotifier.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Event> Events { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
    }
}