using System;
using TicketNotifier.Checkers.Interfaces;

namespace TicketNotifier.Checkers.Implementations
{
    public class CheckEvent: ICheckEvent
    {
        public void SayHello()
        {
            Console.WriteLine("mfk");
        }
    }
}