using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using YourWallet.Models;

namespace YourWallet.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashBoardController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task<ActionResult> Index()
        {

            //Monthly Transactions

            DateTime StartDate = DateTime.Today.AddMonths(-1);
            DateTime EndDate = DateTime.Today;

            List<Transaction> MonthlyTransaction = await _context.Transactions.Include(x => x.Category).Where(y => y.Date >= StartDate && y.Date <= EndDate).ToListAsync();

            //Income
            int TotalMonthlyIncome=MonthlyTransaction.Where(i => i.Category.Type=="Income")
                .Sum(j=>j.Amount);
            ViewBag.TotalMonthlyIncome=TotalMonthlyIncome.ToString("C0");

            //Expense
            int TotalMonthlyExpense = MonthlyTransaction.Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalMonthlyExpense = TotalMonthlyExpense.ToString("C0");

            //Balance
            int Balance = TotalMonthlyIncome - TotalMonthlyExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture,"{0:C0}",Balance);


            return View();
        }
    }
}
