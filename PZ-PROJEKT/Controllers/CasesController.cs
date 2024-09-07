using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;
using PZ_PROJEKT.Models;
using PZ_PROJEKT.Models.Request;
using Newtonsoft.Json;
using PZ_PROJEKT.Data;
using Microsoft.AspNetCore.Authorization;

namespace PZ_PROJEKT.Controllers
{
    public class CasesController : Controller
    {
        private readonly PZ_PROJEKTContext _context;
        private readonly int TOTAL_CASE_WEIGHT = 1_000_000;

        public CasesController(PZ_PROJEKTContext context)
        {
            _context = context;
        }

        // GET: Cases
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var cases = from c in _context.Case
                        select c;

            if(!String.IsNullOrEmpty(searchString))
            {
                cases = cases.Where(c => c.Name.ToLower().Contains(searchString.ToLower()));
            }

            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("price_desc") || sortOrder.Equals("price_asc") ? "name_desc" : sortOrder.Equals("name_desc") ? "name_asc" : "";
            ViewBag.PriceSortParam = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("name_desc") || sortOrder.Equals("name_asc") ? "price_desc" : sortOrder.Equals("price_desc") ? "price_asc" : "";
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = currentFilter;

            switch(sortOrder)
            {
                case "name_desc":
                    cases = cases.OrderByDescending(c => c.Name);
                    break;
                case "name_asc":
                    cases = cases.OrderBy(c => c.Name);
                    break;
                case "price_desc":
                    cases = cases.OrderByDescending(c => c.Price);
                    break;
                case "price_asc":
                    cases = cases.OrderBy(c => c.Price);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var guwno = await cases.ToPagedListAsync(pageNumber, pageSize);

            return View(guwno);
        }

        // GET: Cases/Details/5
        public IActionResult Details(int id)
        {
            Case caseDetails = _context.Case
                .Include(c => c.CaseItems)
                .ThenInclude(c => c.Skin)
                .Where(c => c.Id == id)
                .Select(c => new Case
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImagePath = c.ImagePath,
                    Price = c.Price,
                    CaseItems = c.CaseItems,
                    TotalWeight = c.TotalWeight,
                    TimesOpened = c.TimesOpened,
                    Creator = c.Creator
                })
                .FirstOrDefault();

            if (caseDetails == null)
            {
                return NotFound();
            }

            return View(caseDetails);
        }

        // GET: Cases/Create
        public IActionResult Create()
        {
            List<Item> items = _context.Item.ToList();
            ViewBag.AvailableItems = items;

            return View();
        }

        // POST: Cases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCaseRequest request)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(request.Items?.Count);
                if(!IsPercentageSumValid(request))
                {
                    return StatusCode(400, "Invalid percentage sum");
                }

                List<int> idList = request.Items.Select(item => item.itemId).ToList();
                List<Item> items = _context.Item.Where(item => idList.Contains(item.Id)).ToList();

                if(items.Count != request.Items.Count)
                {
                    return StatusCode(400, "Cannot found items given in request");
                }

                int sum = 0;

                List<CaseItem> caseItems = new List<CaseItem>();

                foreach(Item item in items)
                {
                    var tmp = request.Items.Where(i => i.itemId == item.Id).FirstOrDefault();

                    if(tmp == null)
                    {
                        return StatusCode(400);
                    }

                    if (tmp.percentageValue == 0) continue;

                    sum += tmp.percentageValue * (int)Math.Ceiling(item.Price);
                    caseItems.Add(new CaseItem
                    {
                        Skin = item,
                        Weight = tmp.percentageValue * 10_000
                    });
                }

                float price = sum / 100 + (sum / 100) * 0.1f;

                User user = await _context.User.FirstAsync(user => user.SteamId.Equals(User.FindFirst("Id").Value));

                Case @case = new Case
                {
                    Name = request.Name,
                    CaseItems = caseItems,
                    ImagePath = "",
                    Price = price,
                    TotalWeight = TOTAL_CASE_WEIGHT,
                    TimesOpened = 0,
                    Creator = user
                };

                _context.Case.Add(@case);
                _context.SaveChanges();

                return Ok($"Cases/Details/{@case.Id}");
            }
            return Ok("xddd");
        }

        // GET: Cases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @case = await _context.Case.FindAsync(id);
            if (@case == null)
            {
                return NotFound();
            }
            return View(@case);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImgPath,Price,TotalWeight,TimesOpened,Creator")] Case @case)
        {
            if (id != @case.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@case);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaseExists(@case.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@case);
        }

        // GET: Cases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @case = await _context.Case
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@case == null)
            {
                return NotFound();
            }

            return View(@case);
        }

        // POST: Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @case = await _context.Case.FindAsync(id);
            if (@case != null)
            {
                _context.Case.Remove(@case);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Open")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> OpenCase([FromRoute] int id, [FromQuery] int itemsToRoll)
        {
            if(itemsToRoll != 1)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "No multiple rolls.");
            }

            var @case = await _context.Case.Include(c => c.CaseItems).ThenInclude(c => c.Skin).Where(c => c.Id == id).SingleOrDefaultAsync();

            if (@case == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Case not found.");
            }

            if (User == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "User not authenticated.");
            }

            User user = await _context.User.FirstAsync(user => user.SteamId.Equals(User.FindFirst("Id").Value));

            if (user == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "User not authenticated.");
            }

            if (user.Balance < @case.Price)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Insufficient balance to open case.");
            }

            if (@case.CaseItems.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Chest has no items xd.");
            }

            List<CaseItem> items = GetRandomItems(@case, itemsToRoll);

            Inventory inventory = await _context.Inventory.FirstOrDefaultAsync(i => i.User == user && i.Item == items[0].Skin);

            if(inventory != null)
            {
                inventory.Count++;
                _context.Update(inventory);
            } else
            {
                Inventory inventory1 = new Inventory
                {
                    User = user,
                    Item = items[0].Skin,
                    Count = 1
                };
                _context.Add(inventory1);
            }

            user.Balance -= @case.Price * items.Count;

            CaseHistory caseHistory = new CaseHistory
            {
                User = user,
                Case = @case,
                Price = -@case.Price
            };

            _context.CaseHistory.Add(caseHistory);
            _context.Update(user);
            _context.SaveChanges();

            return Json(JsonConvert.SerializeObject(items));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SellCase([FromBody] List<SellCase> request)
        {
            if (request == null || request.Count == 0) return BadRequest();

            if (User == null) return Unauthorized();

            User user = await _context.User.FirstAsync(user => user.SteamId.Equals(User.FindFirst("Id").Value));

            if (user == null) return Unauthorized();

            List<int> ids = new List<int>();

            foreach (var item in request)
            {
                if(item.Count > 0)
                {
                    ids.Add(item.Id);
                }
            }

            List<Inventory> inventory = _context.Inventory.Include(i => i.Item).Where(i => ids.Contains(i.Id)).ToList();
            ItemTransaction transaction = new ItemTransaction()
            {
                Items = new List<TransactionItem>(),
                TransactionType = TRANSACTION_TYPES.SOLD
            };

            float total = 0;

            foreach (var item in inventory)
            {
                var sc = request.FirstOrDefault(i => i.Id == item.Id);
                if (sc == null) continue;

                if (sc.Count > 0)
                {
                    var inventoryItem = inventory.FirstOrDefault(i => i.Id == item.Id);
                    inventoryItem.Count -= sc.Count;
                    user.Balance += item.Item.Price * sc.Count;
                    total += item.Item.Price * sc.Count;
                    AddItemToTransaction(transaction, item.Item, sc.Count);
                }
            }

            if(transaction.Items.Count > 0)
            {
                transaction.User = user;
                transaction.Total = total;
                _context.Add(transaction);
            }

            _context.SaveChanges();

            return Ok("Cases sold");
        }

        private bool CaseExists(int id)
        {
            return _context.Case.Any(e => e.Id == id);
        }

        private bool IsPercentageSumValid(CreateCaseRequest request)
        {
            int sum = 0;

            foreach(var item in request.Items)
            {
                sum += item.percentageValue;
                Console.WriteLine($"SUMA: {sum}");

                if(sum > 100)
                {
                    return false;
                }
            }
            Console.WriteLine($"SUMA NA KONIEC: {sum}");

            return sum == 100;
        }

        private void AddItemToTransaction(ItemTransaction transaction, Item item, int count)
        {
            int idx = transaction.Items.FindIndex(i => i.Item.Id == item.Id);

            if(idx == -1)
            {
                transaction.Items.Add(new TransactionItem
                {
                    Item = item,
                    Count = count
                });
                return;
            }
        }

        private List<CaseItem> GetRandomItems(Case @case, int itemsToRoll)
        {
            List<CaseItem> result = new List<CaseItem>();

            if (@case.CaseItems.Count < 0) return result;

            for (int i = 0; i < itemsToRoll; i++)
            {
                int temp = 0;
                Random random = new Random();
                int randomNumber = random.Next(0, @case.TotalWeight);
                int itemIdx = -1;

                Console.WriteLine(randomNumber);

                for (int j = 0; j < @case.CaseItems.Count; j++)
                {
                    temp += @case.CaseItems[j].Weight;
                    Console.WriteLine(temp);

                    if (randomNumber < temp)
                    {
                        itemIdx = j;
                        break;
                    }
                }

                if (itemIdx == -1)
                {
                    itemIdx = @case.CaseItems.Count - 1;
                }

                result.Add(@case.CaseItems[itemIdx]);
            }

            return result;
        }
    }
}
