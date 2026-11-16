using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4Task1
{
    struct Product
    {
        public string Name;
        public int Price;
        public int Quantity;

        public Product(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    class Buyer
    {
        public int CardNumber;
        public int StartMoney;
        public int Money;
        public int ItemsBought;
        public int MoneySpent;

        public Buyer(int cardNumber, int startMoney)
        {
            CardNumber = cardNumber;
            StartMoney = startMoney;
            Money = startMoney;
            ItemsBought = 0;
            MoneySpent = 0;
        }
    }

    class Store
    {
        public Product[] Products;
        public int Profit;

        public Store()
        {
            Products = new Product[]
            {
                new Product("Хліб",      25, 40),
                new Product("Молоко",    40, 35),
                new Product("Яйця",      60, 30),
                new Product("Сир",      150, 25),
                new Product("Ковбаса",  220, 20),
                new Product("Шоколад",   70, 45),
                new Product("Кава",     300, 15),
                new Product("Чай",      120, 25),
                new Product("Яблука",    50, 50),
                new Product("М'ясо",    350, 20),
                new Product("Риба",     280, 20),
                new Product("Олія",      90, 30),
                new Product("Цукор",     45, 40),
                new Product("Печиво",    55, 40),
                new Product("Сік",       65, 35)
            };
            Profit = 0;
        }

        public List<int> GetAvailableProducts(int money)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < Products.Length; i++)
            {
                if (Products[i].Quantity > 0 && Products[i].Price <= money)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public void Sell(int productIndex, Buyer buyer)
        {
            int price = Products[productIndex].Price;

            Products[productIndex].Quantity--;
            buyer.Money -= price;
            buyer.MoneySpent += price;
            buyer.ItemsBought++;
            Profit += price;
        }
    }

    class Program
    {
        static Random rnd = new Random();

        static int ReadBuyersCount()
        {
            while (true)
            {
                Console.Write("Введіть кількість покупців (1-1000): ");
                if (int.TryParse(Console.ReadLine(), out int count) && count >= 1 && count <= 1000)
                {
                    return count;
                }
                Console.WriteLine("Помилка: введіть ціле число від 1 до 1000.");
            }
        }

        static List<Buyer> CreateBuyers(int count)
        {
            List<Buyer> buyers = new List<Buyer>();
            HashSet<int> usedCards = new HashSet<int>();

            while (buyers.Count < count)
            {
                int card = rnd.Next(1, 1001);
                if (usedCards.Contains(card))
                {
                    continue;
                }
                usedCards.Add(card);

                int money = rnd.Next(1000, 10001);
                buyers.Add(new Buyer(card, money));
            }
            return buyers;
        }

        static void ServeBuyer(Store store, Buyer buyer)
        {
            while (true)
            {
                List<int> available = store.GetAvailableProducts(buyer.Money);
                if (available.Count == 0)
                {
                    break;
                }

                int productIndex = available[rnd.Next(available.Count)];
                store.Sell(productIndex, buyer);
            }
        }

        static Buyer FindMaxItemsBuyer(List<Buyer> buyers)
        {
            Buyer best = buyers[0];
            foreach (Buyer b in buyers)
            {
                if (b.ItemsBought > best.ItemsBought)
                {
                    best = b;
                }
            }
            return best;
        }

        static Buyer FindMaxSpentBuyer(List<Buyer> buyers)
        {
            Buyer best = buyers[0];
            foreach (Buyer b in buyers)
            {
                if (b.MoneySpent > best.MoneySpent)
                {
                    best = b;
                }
            }
            return best;
        }

        static void PrintResults(Store store, List<Buyer> buyers)
        {
            Console.WriteLine();
            Console.WriteLine("=== Результати покупок ===");
            foreach (Buyer b in buyers)
            {
                Console.WriteLine($"Картка №{b.CardNumber}: було {b.StartMoney} грн, " +
                                  $"куплено товарів: {b.ItemsBought}, " +
                                  $"витрачено: {b.MoneySpent} грн, залишок: {b.Money} грн");
            }

            Buyer maxItems = FindMaxItemsBuyer(buyers);
            Buyer maxSpent = FindMaxSpentBuyer(buyers);

            Console.WriteLine();
            Console.WriteLine("=== Підсумки ===");
            Console.WriteLine($"Прибуток магазину: {store.Profit} грн");
            Console.WriteLine($"Найбільше товарів купив покупець з карткою №{maxItems.CardNumber} " +
                              $"({maxItems.ItemsBought} шт.)");
            Console.WriteLine($"Найбільше грошей витратив покупець з карткою №{maxSpent.CardNumber} " +
                              $"({maxSpent.MoneySpent} грн)");
        }

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            int count = ReadBuyersCount();

            Store store = new Store();
            List<Buyer> buyers = CreateBuyers(count);

            foreach (Buyer buyer in buyers)
            {
                ServeBuyer(store, buyer);
            }

            PrintResults(store, buyers);

            Console.WriteLine();
            Console.WriteLine("Натисніть Enter для виходу...");
            Console.ReadLine();
        }
    }
}
