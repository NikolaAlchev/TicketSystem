using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web.Mvc;
using TicketSystem.Models;

using System.IO.Ports;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Printer.Discovery;

namespace TicketSystem.Controllers
{
    public class TicketSystemController : Controller
    { 
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: TicketSystem
        public ActionResult Index()
        {
            Session["File"] = @"C:\Users\nikol\Desktop\Project\TicketSystem\TicketSystem\App_Data\Data.txt";
            string[] lines = System.IO.File.ReadAllLines(Session["File"].ToString());
            Session["Price"] = int.Parse(lines[0]);
            if (User.IsInRole("Administrator"))
            {
                return View("AdminView");
            }
            Ticket ticket = new Ticket();
            ticket.Price = int.Parse(lines[0].ToString());
            DateTime now = DateTime.Now;
            if(now.Day != int.Parse(lines[2]) || now.Month != int.Parse(lines[3]) || now.Year != int.Parse(lines[4]))
            {
                _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [DailyTickets]");
                lines[2] = now.Day.ToString();
                lines[3] = now.Month.ToString();
                lines[4] = now.Year.ToString();
                lines[1] = "1";
                System.IO.File.WriteAllLines(Session["File"].ToString(), lines);
            }
            ViewBag.DailyTickets = _context.DailyTickets.ToList();
            return View("CashierView", ticket);
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayUsers()
        {
            return PartialView("~/Views/PartialViews/UsersPartialView.cshtml");
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult ChangeCurrentPrice(int ?price)
        {
            if(price != null)
            {
                Session["Price"] = price;
                string[] lines = System.IO.File.ReadAllLines(Session["File"].ToString());
                lines[0] = price.ToString();
                System.IO.File.WriteAllLines(Session["File"].ToString(), lines);
            }
            ViewBag.CurrentPrice = Session["Price"];
            return PartialView("~/Views/PartialViews/ChangePricePartialView.cshtml");
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayDailyReports()
        {
            DateTime now = DateTime.Now;    
            ViewBag.CurrentDate = now.Year + "-" + now.Month.ToString().PadLeft(2, '0') + "-" + now.Day.ToString().PadLeft(2, '0');
            return PartialView("~/Views/PartialViews/DailyReportsPartialView.cshtml");
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayMonthlyReports()
        {
            ViewBag.CurrentYear = DateTime.Now.Year;
            return PartialView("~/Views/PartialViews/MonthlyReportsPartialView.cshtml");
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayYearlyReports()
        {
            return PartialView("~/Views/PartialViews/YearlyReportsPartialView.cshtml");
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayDailyReportsTable(string date)
        {
            string[] splitDate = date.Split('-');
            var day = int.Parse(splitDate[2]).ToString();
            var month = int.Parse(splitDate[1]).ToString();
            var year = splitDate[0];

            List<Ticket> tickets = _context.Tickets
                .Where(x => x.DateOfCreation.Day.ToString() == day
                && x.DateOfCreation.Month.ToString() == month
                && x.DateOfCreation.Year.ToString() == year)
                .ToList();

            return PartialView("~/Views/PartialViews/DailyReportsTablePartialView.cshtml",tickets);
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayMonthlyReportsTable(string year)
        {
            int Year = int.Parse(year);
            List<MonthlyReport> months = _context.MonthlyReports
                .Where(x => x.Year.ToString() == year).ToList();
            return PartialView("~/Views/PartialViews/MonthlyReportsTablePartialView.cshtml", months);
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DisplayYearlyReportsTable()
        {
            List<YearlyReport> years = _context.YearlyReport.ToList();
            return PartialView("~/Views/PartialViews/YearlyReportsTablePartialView.cshtml", years);
        }

        [HttpPost]
        public ActionResult CreateTicket(Ticket model)
        {
            DateTime now = DateTime.Now;
            MonthlyReport mr = _context.MonthlyReports.FirstOrDefault(x => x.Month == now.Month && x.Year == now.Year);
            MonthlyReport newMr = null;
            YearlyReport yr = _context.YearlyReport.FirstOrDefault(x => x.Year == now.Year);
            YearlyReport newYr = null;
            bool monthlyReportExists = true;
            bool yearlyReportExists = true;
            if (mr == null)
            {
                monthlyReportExists = false;
                newMr = new MonthlyReport()
                {
                    Month = now.Month,
                    Year = now.Year,
                    Sum = 0,
                    NumOfPeople = 0,
                    NumOfGroups = 0
                };
            }
            if (yr == null)
            {
                yearlyReportExists = false;
                newYr = new YearlyReport()
                {
                    Year = now.Year,
                    Sum = 0,
                    NumOfPeople = 0,
                    NumOfGroups = 0
                };
            }
            if (!model.Group)
            {
                for (int i = 0; i < model.NumOfPeople; i++)
                {
                    string[] lines = System.IO.File.ReadAllLines(Session["File"].ToString());
                    Ticket ticket = new Ticket();
                    ticket.Group = false;
                    ticket.Price = int.Parse(lines[0].ToString());
                    ticket.DateOfCreation = now;
                    ticket.NumOfPeople = 1;

                    if (now.Day == int.Parse(lines[2]) && now.Month == int.Parse(lines[3]) && now.Year == int.Parse(lines[4]))
                    {
                        ticket.Code = long.Parse(now.ToString("yyyyMMdd") + lines[1].ToString().PadLeft(4, '0'));
                        lines[1] = (int.Parse(lines[1].ToString()) + 1).ToString();
                        System.IO.File.WriteAllLines(Session["File"].ToString(), lines);
                    }
                    else
                    {
                        ticket.Code = long.Parse(now.ToString("yyyyMMdd") + 1.ToString().PadLeft(4, '0'));
                        lines[1] = "2";
                        lines[2] = now.Day.ToString();
                        lines[3] = now.Month.ToString();
                        lines[4] = now.Year.ToString();
                        System.IO.File.WriteAllLines(Session["File"].ToString(), lines);
                        _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [DailyTickets]");
                    }
                    if (monthlyReportExists)
                    {
                        mr.Sum += ticket.Price;
                        mr.NumOfPeople += ticket.NumOfPeople;
                    }
                    else
                    {
                        newMr.Sum += ticket.Price;
                        newMr.NumOfPeople += ticket.NumOfPeople;
                    }
                    if (yearlyReportExists)
                    {
                        yr.Sum += ticket.Price;
                        yr.NumOfPeople += ticket.NumOfPeople;
                    }
                    else
                    {
                        newYr.Sum += ticket.Price;
                        newYr.NumOfPeople += ticket.NumOfPeople;
                    }
                    _context.DailyTickets.Add(DailyTicket.convertToDailyTicket(ticket));
                    _context.Tickets.Add(ticket);
                    _context.SaveChanges();

                    PrintTicket(ticket);
                }
            }
            else
            {
                string[] lines = System.IO.File.ReadAllLines(Session["File"].ToString());
                model.DateOfCreation = DateTime.Now;
                model.Price = int.Parse(lines[0].ToString()) * model.NumOfPeople;

                if (model.DateOfCreation.Day == int.Parse(lines[2]) && model.DateOfCreation.Month == int.Parse(lines[3]) && model.DateOfCreation.Year == int.Parse(lines[4]))
                {
                    model.Code = long.Parse(model.DateOfCreation.ToString("yyyyMMdd") + lines[1].ToString().PadLeft(4, '0'));
                    lines[1] = (int.Parse(lines[1].ToString()) + 1).ToString();
                    System.IO.File.WriteAllLines(Session["File"].ToString(), lines);
                }
                else
                {
                    model.Code = long.Parse(model.DateOfCreation.ToString("yyyyMMdd") + 1.ToString().PadLeft(4, '0'));
                    lines[1] = "2";
                    lines[2] = model.DateOfCreation.Day.ToString();
                    lines[3] = model.DateOfCreation.Month.ToString();
                    lines[4] = model.DateOfCreation.Year.ToString();
                    System.IO.File.WriteAllLines(Session["File"].ToString(), lines);
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [DailyTickets]");
                }
                if (monthlyReportExists)
                {
                    mr.Sum += model.Price;
                    mr.NumOfPeople += model.NumOfPeople;
                    mr.NumOfGroups += 1;
                }
                else
                {
                    newMr.Sum += model.Price;
                    newMr.NumOfPeople += model.NumOfPeople;
                    newMr.NumOfGroups += 1;
                }

                if (yearlyReportExists)
                {
                    yr.Sum += model.Price;
                    yr.NumOfPeople += model.NumOfPeople;
                    yr.NumOfGroups += 1;
                }
                else
                {
                    newYr.Sum += model.Price;
                    newYr.NumOfPeople += model.NumOfPeople;
                    newYr.NumOfGroups += 1;
                }

                _context.DailyTickets.Add(DailyTicket.convertToDailyTicket(model));
                _context.Tickets.Add(model);

                PrintTicket(model);
            }

            if (!monthlyReportExists)
            {
                _context.MonthlyReports.Add(newMr);
            }
            if (!yearlyReportExists)
            {
                _context.YearlyReport.Add(newYr);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        void PrintTicket(Ticket ticket)
        {
            DiscoveredPrinter[] printers = UsbDiscoverer.GetZebraUsbPrinters().ToArray();

            if (printers.Length > 0)
            {
                try
                {
                    DiscoveredPrinter printer = printers[0];

                    Connection connection = new UsbConnection(printer.Address);

                    connection.Open();

                    ZebraPrinter zebraPrinter = ZebraPrinterFactory.GetInstance(connection);

                    string zplCommand = "^XA" +

                        "^FO170,100" +
                        "^A0R,20,25" +
                        "^FD" + ticket.Code.ToString() + "^FS" +

                        "^FO290,235" +
                        "^A0R,28,35" +
                        "^FD" + (ticket.Price / ticket.NumOfPeople).ToString() + "^FS" +

                        "^FO231,190" +
                        "^A0R,20,25" +
                        "^FD" + ticket.NumOfPeople.ToString() + "^FS" +

                        "^FO231,415" +
                        "^A0R,20,25" +
                        "^FD" + ticket.Price.ToString() + "^FS" +

                        "^FO201,235" +
                        "^A0R,20,25" +
                        "^FD" + ticket.DateOfCreation.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "^FS" +

                        "^FO345,860" +
                        "^A0N,15,17" +
                        "^FD" + ticket.Price.ToString() + "^FS" +

                        "^FO220,883" +
                        "^A0N,15,17" +
                        "^FD" + ticket.DateOfCreation.ToString("dd.MM.yyyy HH:mm") + "^FS" +

                        "^FO185,860" +
                        "^A0N,15,17" +
                        "^FD" + ticket.NumOfPeople.ToString() + "^FS" +

                        "^FO110,906" +
                        "^A0,15,17" +
                        "^FD" + ticket.Code.ToString() + "^FS" +

                        "^XZ";
                    zebraPrinter.SendCommand(zplCommand);

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    Exception innerException = ex.InnerException;
                    while (innerException != null)
                    {
                        Console.WriteLine($"Inner exception: {innerException.Message}");
                        Console.WriteLine($"Inner exception stack trace: {innerException.StackTrace}");
                        innerException = innerException.InnerException;
                    }
                }
            }
            else
            {
                Console.WriteLine("No Zebra printers found on the network.");
            }

        }


        [Authorize(Roles = "Administrator")]
        public ActionResult ManageUsers()
        {
            var users = _context.Users.ToList();
            ViewBag.AdminsId = User.Identity.GetUserId();
            return View(users);
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser(string id) {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageUsers");
        }
    }
}