using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class DailyTicket
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Code")]
        public long Code { get; set; }
        [Display(Name = "Price")]
        public int Price { get; set; }
        [Display(Name = "Date")]
        public DateTime DateOfCreation { get; set; }
        [Display(Name = "Group")]
        public bool Group { get; set; }
        [Display(Name = "People")]
        public int NumOfPeople { get; set; }

        public static DailyTicket convertToDailyTicket (Ticket ticket)
        {
            DailyTicket dailyTicket = new DailyTicket ();
            dailyTicket.Code = ticket.Code;
            dailyTicket.Price = ticket.Price;
            dailyTicket.Group = ticket.Group;
            dailyTicket.NumOfPeople = ticket.NumOfPeople;
            dailyTicket.DateOfCreation = ticket.DateOfCreation;
            return dailyTicket;
        }
    }
}