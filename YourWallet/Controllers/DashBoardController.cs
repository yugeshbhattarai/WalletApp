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

            DateTime StartDate = DateTime.Today.AddDays(-6);
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

            //Chart
            ViewBag.DoughnutChartData = MonthlyTransaction
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                }).OrderByDescending(l=>l.amount).ToList();

            List<ChartData> IncomeData = MonthlyTransaction
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j=>j.Date)
                .Select(k=>new ChartData()
                {
                day=k.First().Date.ToString("dd-MMM"),
                income=k.Sum(l=>l.Amount)
                }).ToList();

            List<ChartData> ExpenseData = MonthlyTransaction
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new ChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                }).ToList();

            string[] sevenDays = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();


            ViewBag.ChartData = from day in sevenDays
                                join income in IncomeData on day equals income.day
                                into dayIncomeJoined
                                from income in dayIncomeJoined.DefaultIfEmpty()
                                join expense in ExpenseData on day equals expense.day
                                into dayExpenseJoined
                                from expense in dayExpenseJoined.DefaultIfEmpty()
                                select new
                                {
                                    day=day,
                                    income = income == null ? 0 : income.income,
                                    expense = expense == null ? 0 : expense.expense,
                                };



            ViewBag.RecentTransactions = await _context.Transactions
               .Include(i => i.Category)
               .OrderByDescending(j => j.Date)
               .Take(5)
               .ToListAsync();

            return View();
        }
    }

    public class ChartData
    {
        public string day;
        public int income;
        public int expense;
    }
}
