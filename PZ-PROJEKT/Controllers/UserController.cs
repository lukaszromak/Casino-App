using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZ_PROJEKT.Data;
using PZ_PROJEKT.Models;

namespace PZ_PROJEKT.Controllers
{
	public class UserController : Controller
	{
        private readonly PZ_PROJEKTContext _context;

        public UserController(PZ_PROJEKTContext context)
        {
            _context = context;
        }

        [Authorize]
		public IActionResult Profile(float? balanceAdded)
		{
            if(User == null)
            {
                return NotFound();
            }

            User user = _context.User.First(user => user.SteamId.Equals(User.FindFirst("Id").Value));

            if (user == null)
            {
                return NotFound();
            }

            if (balanceAdded != null && balanceAdded > 0)
            {
                ViewBag.BalanceAdded = balanceAdded;
            }

            return View(user);
		}

        [Authorize]
        public IActionResult Inventory()
        {
            if(User == null)
            {
                return NotFound();
            }

            User user = _context.User.First(user => user.SteamId.Equals(User.FindFirst("Id").Value));

            if (user == null)
            {
                return NotFound();
            }

            List<Inventory> inventory = _context.Inventory.Include(i => i.Item).Where(i => i.User == user).ToList();

            return View(inventory);
        }

        [Authorize]
        public IActionResult Transactions(bool? openedCases, bool? payments, bool? soldCases)
        {
            if(User == null)
            {
                return NotFound();
            }

            bool includeOpenedCases = false;
            bool includePayments = false;
            bool includeSoldCases = false;
            ViewBag.OpenedCasesChecked = "";
            ViewBag.PaymentsChecked = "";
            ViewBag.SoldCasesChecked = "";

            if (openedCases == null && payments == null && soldCases == null)
            {
                includeOpenedCases = true;
                includePayments = true;
                includeSoldCases = true;
                ViewBag.OpenedCasesChecked = "checked";
                ViewBag.PaymentsChecked = "checked";
                ViewBag.SoldCasesChecked = "checked";
            }

            if (openedCases != null && openedCases == true)
            {
                includeOpenedCases = true;
                ViewBag.OpenedCasesChecked = "checked";
            }

            if (payments != null && payments == true)
            {
                includePayments = true;
                ViewBag.PaymentsChecked = "checked";
            }

            if (soldCases != null && soldCases == true)
            {
                includeSoldCases = true;
                ViewBag.SoldCasesChecked = "checked";
            }

            User user = _context.User.First(user => user.SteamId.Equals(User.FindFirst("Id").Value));

            if (user == null)
            {
                return NotFound();
            }

            List<Payment> paymentList = new List<Payment>();

            if(includePayments)
            {
                paymentList = _context.Payments.Where(payment => payment.User.Equals(user)).ToList();
            }

            List<CaseHistory> caseHistory = new List<CaseHistory>();

            if(includeOpenedCases)
            {
                caseHistory = _context.CaseHistory.Include(caseh => caseh.Case).Where(caseh => caseh.User.Equals(user)).ToList();
            }

            List<ItemTransaction> itemTransactions = new List<ItemTransaction>();

            if(includeSoldCases)
            {
                itemTransactions = _context.ItemTransactions.Where(item => item.User.Equals(user)).ToList();
            }

            Console.WriteLine(paymentList.Count);
            Console.WriteLine(caseHistory);

            List<Transaction> transactions = paymentList
                .Select(p => new Transaction { Type = "PAYMENT" ,PaymentMethod = p.PaymentMethod, Date = p.PaymentTime, Amount = p.Amount})
                .Concat(caseHistory.Select(ch => new Transaction { Type = "CASE", Case = ch.Case, Date = ch.OpenedTime, Amount = ch.Price }))
                .Concat(itemTransactions.Select(it => new Transaction { Type = "ITEMS SOLD", ItemTransaction = it, Date = it.TransactionTime, Amount = it.Total}))
                .OrderByDescending(t => t.Date)
                .ToList();

            return View(transactions);
        }

        public record struct Transaction(string Type, ItemTransaction? ItemTransaction, Case? Case, DateTime? Date, PaymentMethod? PaymentMethod, float Amount); 
	}
}
