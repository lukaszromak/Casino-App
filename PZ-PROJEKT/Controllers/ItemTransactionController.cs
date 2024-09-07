using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZ_PROJEKT.Data;
using PZ_PROJEKT.Models;

namespace PZ_PROJEKT.Controllers
{
    public class ItemTransactionController : Controller
    {
        private readonly PZ_PROJEKTContext _context;

        public ItemTransactionController(PZ_PROJEKTContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            ItemTransaction itemTransaction = await _context.ItemTransactions.Include(it => it.Items).ThenInclude(i => i.Item).FirstOrDefaultAsync(x => x.Id == id);

            if(itemTransaction == null)
            {
                return NotFound();
            }

            return View(itemTransaction);
        }
    }
}
