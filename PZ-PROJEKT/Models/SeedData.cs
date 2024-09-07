using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PZ_PROJEKT.Data;
using System;
using System.Linq;

namespace PZ_PROJEKT.Models;

public static class SeedData
{
	private static List<string> caseNames = new List<string>
	{
		"Lion",
		"Tiger",
		"Elephant",
		"Giraffe",
		"Zebra",
		"Panda",
		"Kangaroo",
		"Leopard",
		"Penguin",
		"Dolphin",
		"Koala",
		"Eagle",
		"Wolf",
		"Shark",
		"Whale",
		"Fox",
		"Bear",
		"Rabbit",
		"Hippo",
		"Cheetah"
	};
    private static List<String> skinNames = new List<String> {
			"AWP Desert Hydra",
			"Desert Eagle Fennec Fox",
			"MP5-SD Oxide Oasis",
			"Glock-18 Pink DDPAT",
			"XM1014 Elegant Vines",
			"AUG Sand Storm",
			"USP-S Purple DDPAT",
			"M249 Humidor",
			"SG553 Desert Blossom",
			"MP9 Music Box",
			"Famas CaliCamo",
			"Dual Berettas Drift Wood",
			"P90 Verdant Growth",
			"CZ75-Auto Midnight Palm",
			"P250 Drought",
			"MAG-7 Navy Sheen",
			"PP-Bizon Anolis",
			"SSG 08 Prey",
			"MAC-10 Sienna Danmask",
			"AK-47 Gold Arabesque"
		};
	private static List<float> skinPrices = new List<float> {
		1249.00f,
		129.71f,
		4.79f,
		1.25f,
		0.34f,
		1.89f,
		1.74f,
		0.03f,
		0.11f,
		0.05f,
		0.02f,
		0.01f,
		0.02f,
		0.02f,
		0.01f,
		0.01f,
		0.01f,
		0.01f,
		0.01f,
		1465.29f
	};
	private static bool coolLook = true;
	public static void Initialize(IServiceProvider serviceProvider)
	{
		using (var context = new PZ_PROJEKTContext(
			serviceProvider.GetRequiredService<
				DbContextOptions<PZ_PROJEKTContext>>())
			)
		{
			context.Item.ExecuteDelete();
			context.Case.ExecuteDelete();
			context.PaymentMethods.ExecuteDelete();
			context.ItemTransactions.ExecuteDelete();
			context.User.ExecuteDelete();
			// Look for any movies.
			if (context.Case.Any())
			{
				return;   // DB has been seeded
			}

			User sys = context.User.FirstOrDefault(user => user.SteamId == "SYSTEM");

			if(sys == null)
			{
				sys = new User();
				sys.SteamId = "SYSTEM";
				sys.Balance = 0;
				context.User.Add(sys);
			}

			User adminUser = new User();
			adminUser.SteamId = "76561198193578373";
			adminUser.Balance = 5000;
			context.User.Add(adminUser);

			User testUser = new User();
            testUser.SteamId = "69";
            testUser.Balance = 1_000_000;
            context.User.Add(testUser);

            context.SaveChanges();

            sys = context.User.FirstOrDefault(user => user.SteamId == "SYSTEM");
			testUser = context.User.FirstOrDefault(user => user.SteamId == "69");

            var mockCaseItems = new List<CaseItem>();
			var mockCaseItems2 = new List<CaseItem>();
			Item item = null;

			string img = "";
			string extension = coolLook ? "webp" : "png";

            for (int i = 0; i < 20; i++)
			{
				img = $"{i + 1}." + extension;
				string name = coolLook ? skinNames[i] : $"{i + 1}";
				float price = coolLook ? skinPrices[i] : i;

				item = new Item
				{
					Name = name,
					ImagePath = img,
					Price = price
				};

				mockCaseItems.Add(new CaseItem
				{
					Skin = item,
					Weight = 50_000
				});

				if (i < 2)
				{
					mockCaseItems2.Add(new CaseItem
                    {
                        Skin = item,
                        Weight = 50_000
                    });
                }
			}

			var mockCases = new List<Case>();

			for (int i = 0; i < 20; i++)
			{
				mockCases.Add(new Case
				{
                    Name = caseNames[i],
                    ImagePath = "case1.webp",
                    Price = 100.0f * (20 - i),
                    TotalWeight = 1_000_000,
                    TimesOpened = 0,
                    Creator = sys,
                });
			}

			mockCases[0].CaseItems = mockCaseItems;
			mockCases[1].CaseItems = mockCaseItems2;

			/*PAYMENT METHODS*/

			List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

			paymentMethods.Add(new PaymentMethod
			{
				Name = "Blik",
			});
			paymentMethods.Add(new PaymentMethod
			{
				Name = "Przelewy24"
			});
			paymentMethods.Add(new PaymentMethod
			{
				Name = "Paysafecard"
			});
            paymentMethods.Add(new PaymentMethod
			{
				Name = "SMS"
			});

			context.PaymentMethods.AddRange(paymentMethods);

            context.Case.AddRange(
				mockCases.ToArray()
			);

			context.SaveChanges();

			/** TEST INVENTORY **/

			List<Inventory> inventories = new List<Inventory>();
			List<Item> items = context.Item.ToList();

			foreach (Item i in items)
			{
				inventories.Add(new Inventory
				{
					User = adminUser,
					Item = i,
					Count = 2
				});
			}

			context.Inventory.AddRange(inventories);
			context.SaveChanges();
		}
	}
}