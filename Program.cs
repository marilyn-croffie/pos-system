// PLEASE KEEP THE FOLLOWING HEADER INTACT IN YOUR SUBMITTED CODE other than adding your name
// after "Programmer:".

/*
 * CS 403 - Week 4 Homework
 * Programmer: Marilyn Croffie
 */

/*
 * This program is a very simple point-of-sale system - the software that might run on a 
 * cash register at a checkout counter. This is an extremely simple implementation of such
 * a system - it only accepts product codes and generates a total for the sale.
 * 
 * You've been asked to refactor this code to make it more organized and usable, and to add
 * some new functionality to the program:
 * 
 *   - Use classes and/or structs to represent data, rather than just having some arrays 
 *     and variables fluttering around.
 *   - Add the ability to print the list of all products that were sold in the sale - like
 *     a receipt.
 *   - Add the ability to ask the user (the cashier) what type of payment is being used.
 *     If a payment type other than Cash is used, accept a value for the payment number.
 *     (Don't worry about "validating" credit card numbers or anything similar.) This code
 *     includes an "enum" (we'll discuss that in class) to help get you started with this.
 *   - Imagine the store has a loyalty card system, and the only piece of information you need
 *     for that system is the customer's phone number. The customer may choose not to give their
 *     number, or they may not even be a member of the program. (Don't worry about storing a 
 *     list of valid member numbers or anything; just have a strategy for associating a 
 *     member's number with the sale.)
 * 
 * Some tips:
 * 
 *   - Use classes. Think about the data entities in this program, and then think about how you would
 *     logically group them into classes.
 *   - We will be covering a lot more about lists and similar data structures in Week 4, but for this 
 *     exercise, consider using the List<T> type (ref. Friday's lecture on generics). A List<T> has
 *     methods like Add() and Remove() along with other aggregating methods like Count(). 
 *   - Week 4's homework will be built upon this week's homework - there will be no new code given
 *     for Week 4. More information during Week 4's lectures!
 * 
 * We will get started with this homework in class on Friday. 
 */

/* 
 * For this week, please use LINQ wherever possible. 
 * You may use either query syntax or method syntax - it is your choice. 
 * How you implement these featureas is also your choice, but make sure you test your code 
 * and consider edge cases, invalid input and so on to make sure your code is robust!
 * 
 * This exercise is also to help you think about how to approach programming. 
 * You can use any strategy for implementing these features. 
 * However, do your best to explore the most efficient and "future-proof" strategy for 
 * implementing the functionality.
 * 
 * Assignment:
 *  - Provide a mechanism to print out the current transaction while it is underway - 
 *    not just afterwards as a receipt.
 *  - Following on to the previous feature, provide a function to allow a product to be removed 
 *    from the transaction while it is still underway.
 *  - Rework the application so that multiple transactions can be placed, 
 *     and transactions that have already been placed can be viewed. For example, rewrite the main method 
 *     so that the program asks the user whether they want to start a new transction or view existing transactions.
 *  - Along with the previous functionality, add a function to print out the total of all transactions 
 *    that have occurred. (Hint: add a property or method to your transaction class and then use LINQ to Sum() 
 *    it on all transaction instances.)
 *  - Devise and implement a strategy for retrieving the product database from an external source, rather than 
 *    having it stored as static values within the program.
 *      + One option that will be available to you is to use Entity Framework Core to connect to a database. 
 *        If you go this route, you do not need to worry about writing the data to a CSV or similar file 
 *        and parsing it yourself. 
 *          > If you're interested, here (https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx)
 *            is a good place to get started.
 *          > You'll also need to use NuGet (https://learn.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) 
 *            to install the EntityFrameworkCore.SqlServer package, which will give you the tools you need to access 
 *            the Entity Framework commands. You may also need additional packages - see this documentation 
 *            (https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli) 
 *            for more information.
 *      + Alternatively you can store the data in a text file (such as a CSV flat file) and write code that reads 
 *        the product database in at the start of the program. If you do this, make sure to include your data file 
 *        with your submission!
 *      + This strategy is open-ended - for example, if you are so inclined, you could also store a text file on a 
 *        web server and retrieve the file using C#'s web request functions. The core req\uirement is that the 
 *        product database not be part of the C# code - that you not use the array/collection initialization syntax 
 *        to populate the product list.
 */

using System;
using System.Collections.Generic;

namespace PointOfSaleSystem
{
    // This is a construct that was not assigned as part of a presentation, so it will be used
    // here to show you how it's done: the enum.

    // Enums are value types that allow you to represent values in code using a set of keywords.
    // It can make code more readable. 

    // For example, this enum indicates the type of payment a user utilized when paying for
    // their items:
    public enum PaymentType : int
    {
        Cash = 0,
        CreditCard = 1,
        CompanyAccount = 2,
        Other = 255
    }

    // You can then use PaymentType like a type. It is stored as an "int" in memory and can be
    // used as such, but you can assign values to it by using the given identifiers. For example:
    // 
    //     PaymentType paytype = PaymentType.CreditCard;
    //
    // paytype is now actually an int with the value of 1, but using the enum lets you make the code
    // more clear as to what the meaning of the numbers are.

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public static decimal taxRate = 0.075M; // Represents the tax rate for taxable items.
        public bool Taxable { get; set; }
        public decimal ActualPrice 
        {
            get
            {
                if (Taxable)
                {
                    return Price + (Price * taxRate);
            }
                else
                {
                    return Price;
                }
            }
        }

        public Product(int id, string name, decimal price, bool taxable)
        {
            Id = id;
            Name = name;
            Price = price;
            Taxable = taxable;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Price: {Price:C}, Taxable: {Taxable}, Actual Price: {ActualPrice:C}";
        }
    }


    public class ProductList<T> where T : Product
    {
        private List<T> products = new List<T>();

        public void Add(T product)
        {
            products.Add(product);
        }

        public void Remove(T product)
        {
            products.Remove(product);
        }

        public void PrintAll()
        {
            foreach (T product in products)
            {
                Console.WriteLine(product.ToString());
            }
        }
    }


    public class LoyaltyMember
    {
        public string PhoneNumber { get; }
        private List<Sale> associatedSales;

        public LoyaltyMember(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
            associatedSales = new List<Sale>();
        }

        public void AddSale(Sale sale)
        {
            associatedSales.Add(sale);
        }

        public void RemoveSale(Sale sale)
        {
            associatedSales.Remove(sale);
        }

        public List<Sale> GetAssociatedSales()
        {
            return associatedSales;
        }
    }


    public class LoyaltyProgram
    {
        private List<LoyaltyMember> members;

        public LoyaltyProgram()
        {
            members = new List<LoyaltyMember>();
        }

        public void AddMember(LoyaltyMember member)
        {
            members.Add(member);
        }

        public void RemoveMember(string phoneNumber)
        {
            if (members != null && members.Count > 0)
            {
                LoyaltyMember memberToRemove = members.Find(m => m.PhoneNumber == phoneNumber);
                if (memberToRemove != null)
                {
                    members.Remove(memberToRemove);
                }
                else
                {
                    Console.WriteLine($"Member with phone number {phoneNumber} not found.");
                }
            }
            else
            {
                Console.WriteLine("No members found in the loyalty program.");
            }
        }

        public LoyaltyMember GetMember(string phoneNumber)
        {
            return members.Find(m => m.PhoneNumber == phoneNumber);
        }

        public bool Lookup(string phoneNumber)
        {
            foreach (LoyaltyMember member in members)
            {
                if (member.PhoneNumber == phoneNumber)
                {
                    return true;
                }
            }
            return false;
        }

    }


    public class Sale
    {
        private List<Product> cart = new List<Product>();
        private decimal saleTotal = 0M; // Holds the total of a sale.
        private PaymentType paytype;
        private string paymentNumber = "";

        public void AddProduct(Product product)
        {
            cart.Add(product);
            saleTotal += product.ActualPrice;
        }

        public void RemoveProduct(Product product)
        {
            if (cart.Contains(product))
            {
                cart.Remove(product);
                saleTotal -= product.ActualPrice;
                // Console.WriteLine($"Removed {product.Name} from sale.");
            }
            else
            {
                Console.WriteLine($"Product {product.Name} not found in sale.");
            }
        }

        public void PrintReceipt()
        {
            Console.WriteLine();
            Console.WriteLine("Sale complete.");
        
            Console.WriteLine("Receipt:");
            Console.WriteLine("===============================");
            foreach (Product product in cart)
            {
                Console.WriteLine($"{product.Name} - {product.ActualPrice:C}");
            }
            Console.WriteLine("===============================");
            Console.WriteLine($"Total: {saleTotal:C}");
            Console.WriteLine("===============================");

            cart.Clear();
        }

        public void ProcessPayment()
        {
            Console.WriteLine("What type of payment will be used?");
            Console.WriteLine("0 - Cash");
            Console.WriteLine("1 - Credit Card");
            Console.WriteLine("2 - Company Account");
            Console.WriteLine("255 - Other");

            int paymentTypeChoice = Convert.ToInt32(Console.ReadLine());

            while (!Enum.IsDefined(typeof(PaymentType), paymentTypeChoice))
            {
                Console.WriteLine("Invalid payment type choice. Please try again.");
                paymentTypeChoice = Convert.ToInt32(Console.ReadLine());
            }

            paytype = (PaymentType)paymentTypeChoice;

            if (paytype != PaymentType.Cash)
            {
                Console.WriteLine("Please enter the payment number:");
                paymentNumber = Console.ReadLine();
            }
        }

        public void Rewards(LoyaltyProgram loyaltyProgram)
        {
            Console.WriteLine("Would you like to add this purchase to your loyalty program? (y/n)");
            string response = Console.ReadLine();

            if (response.ToLower() == "y")
            {
                Console.WriteLine("Please enter your phone number:");
                string phoneNumber = Console.ReadLine();

                bool isMember = loyaltyProgram.Lookup(phoneNumber);

                if (isMember)
                {
                    LoyaltyMember existingMember = loyaltyProgram.GetMember(phoneNumber);
                    existingMember.AddSale(this);
                    Console.WriteLine("Sale added to loyalty program.");
                }
                else
                {
                    LoyaltyMember newMember = new LoyaltyMember(phoneNumber);
                    loyaltyProgram.AddMember(newMember);
                    newMember.AddSale(this);
                }
            }
        }

        public void Shopping(List<Product> products)
        {
            Console.WriteLine("Welcome to the Point of Sale System.");
            
            bool shoppingDone = false;
            while (!shoppingDone)
            {
                Console.WriteLine("Enter product code to add to cart, '-1' to remove a product from cart,  or '0' to end sale: ");
                int productId = Convert.ToInt32(Console.ReadLine());
                if (productId == 0)
                {
                    shoppingDone = true;
                    
                    continue;
                }

                if (productId == -1)
                {
                    Console.WriteLine("Enter code of product to be removed from cart");
                    int removeId = Convert.ToInt32(Console.ReadLine());

                    Product remove = cart.FirstOrDefault(p => p.Id == removeId);

                    if (remove == null)
                    {
                        Console.WriteLine($"Product with id {removeId} not found in cart. Please enter a valid product id.");
                        continue;
                    }

                    Console.WriteLine($"{remove.Name} removed from cart.");
                    RemoveProduct(remove);
                    continue;
                }

                Product product = products.FirstOrDefault(p => p.Id == productId);

                if (product == null)
                {
                    Console.WriteLine($"Product with id {productId} not found. Please enter a valid product id.");
                    continue;
                }

                Console.WriteLine($"{product.Name} added to cart.");
                AddProduct(product);
            }
        }
    }


    internal class Program
    {
        public static void Main()
        {

            // This is your product database.
            List<Product> inventory = new List<Product>()
            {
                new Product(11, "Bottled Water", 1.75M, false),
                new Product(12, "Pepsi", 2.00M, true),
                new Product(21, "Lays Potato Chips", 2.49M, true),
                new Product(22, "Nacho Cheese Doritos", 2.29M, true),
                new Product(23, "Chee-tos", 2.39M, true),
                new Product(31, "M&M's", 1.49M, true),
                new Product(32, "Reese's Peanut Butter Cups", 1.59M, true),
                new Product(41, "2% Milk Gallon", 3.19M, false),
                new Product(42, "Skim Milk Gallon", 3.19M, false)
            };

            LoyaltyProgram loyaltyProgram = new LoyaltyProgram();

            Sale sale = new Sale();
            sale.Shopping(inventory);
            sale.Rewards(loyaltyProgram);
            sale.ProcessPayment();
            sale.PrintReceipt();
            
        }
    }
}