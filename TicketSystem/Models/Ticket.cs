using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class Ticket
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
    }
}