using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class MonthlyReport
    {
        [Key]
        public int Id { get; set; }
        public virtual List<Ticket> Tickets { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Sum { get; set; }
        public int NumOfPeople { get; set; }
        public int NumOfGroups { get; set; }

        public MonthlyReport() {
            Tickets = new List<Ticket>();
        }
    }
}