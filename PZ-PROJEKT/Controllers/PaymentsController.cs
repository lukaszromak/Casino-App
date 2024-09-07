using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZ_PROJEKT.Data;
using PZ_PROJEKT.Models;
using PZ_PROJEKT.Models.Request;

namespace PZ_PROJEKT.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PZ_PROJEKTContext _context;

        public PaymentsController(PZ_PROJEKTContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult AddBalance()
        {
            if (User == null)
            {
                return NotFound();
            }

            List<PaymentMethod> paymentMethods = _context.PaymentMethods.ToList();

            ViewBag.PaymentMethods = paymentMethods;

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBalance(PaymentRequest paymentRequest)
        {
            Console.WriteLine(paymentRequest.Amount);
            PaymentMethod paymentMethod = _context.PaymentMethods.FirstOrDefault(paymentMethod => paymentMethod.Id == paymentRequest.PaymentMethodId);

            if (paymentMethod == null)
            {
                ModelState.AddModelError(nameof(paymentRequest.PaymentMethodId), "Wrong payment method.");

            }

            if (paymentRequest.Amount < 10 || paymentRequest.Amount > 1_000_000)
            {
                ModelState.AddModelError(nameof(paymentRequest.Amount), "Deposit amount should be between 10 and 1 000 000");
            }

            if (!ModelState.IsValid)
            {
                List<PaymentMethod> paymentMethods = _context.PaymentMethods.ToList();

                ViewBag.PaymentMethods = paymentMethods;
                return View(paymentRequest);
            }

            if (User == null)
            {
                return BadRequest();
            }

            User user = _context.User.First(user => user.SteamId.Equals(User.FindFirst("Id").Value));

            if (user == null)
            {
                return BadRequest("Cannot find user.");
            }

            user.Balance = user.Balance + paymentRequest.Amount;
            _context.Entry(user).State = EntityState.Modified;
            _context.Update(user);

            Payment payment = new Payment
            {
                Amount = paymentRequest.Amount,
                User = user,
                PaymentMethod = paymentMethod
            };

            _context.Payments.Add(payment);

            _context.SaveChanges();

            return RedirectToAction("Profile", "User", new { balanceAdded = paymentRequest.Amount });
        }
    }
}
