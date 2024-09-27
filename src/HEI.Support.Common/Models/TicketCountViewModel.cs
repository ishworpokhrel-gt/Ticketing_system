using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEI.Support.Common.Models
{
    public class TicketCountViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int CompletedTickets { get; set; }
        public int ClosedTickets { get; set; }
    }
}
