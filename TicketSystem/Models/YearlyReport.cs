using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TicketSystem.Models
{
    public class YearlyReport
    {
        [Key]
        public int Id { get; set; }
        public virtual List<MonthlyReport> MonthlyReports { get; set; }
        public int Year { get; set; }
        public int Sum { get; set; }
        public int NumOfPeople { get; set; }
        public int NumOfGroups { get; set; }

        public YearlyReport()
        {
            MonthlyReports = new List<MonthlyReport>();
        }
    }
}