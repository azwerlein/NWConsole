using NLog;
using NWConsole.models;

public class Program
{
    public static Logger Logger = LogManager.LoadConfiguration(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
    static void Main(string[] args)
    {
        Logger.Info("Program started");
        Console.WriteLine("Welcome!");

        NWContext db = new NWContext();
        bool loop = true;
        while (loop)
        {
            Console.WriteLine("Please enter your selection.");
            Console.WriteLine("1) Add new products");
            Console.WriteLine("2) Edit a product");
            Console.WriteLine("3) Display all products");
            Console.WriteLine("4) Find one product");
            Console.WriteLine("Enter q to quit");
            string response = Console.ReadLine().ToLower();
            Logger.Info($"User selection: {response}");
            switch (response)
            {
                case "1":
                    AddProducts(db);
                    break;
                case "2":
                    EditProduct(db);
                    break;
                case "3":
                    DisplayProducts(db);
                    break;
                case "4":
                    FindProduct(db);
                    break;
                case "q":
                    loop = false;
                    break;
                default:
                    Console.WriteLine("Invalid response!");
                    break;
            }
            Console.WriteLine("\n");
        }

        Console.WriteLine("Have a good one!");
    }

    private static void AddProducts(NWContext db)
    {
        while (true)
        {
            string name = "";
            while (name == "")
            {
                Console.Write("Product name: ");
                name = Console.ReadLine();
                if (name == "")
                {
                    Console.WriteLine("Invalid response! The name can't be blank.");
                    Logger.Info($"Invalid response received for product name - {name}");
                }
            }

            Console.Write("Supplier ID: ");
            string supplier = Console.ReadLine();
            int supplierID;
            int.TryParse(supplier, out supplierID);

            Console.Write("Category ID: ");
            string category = Console.ReadLine();
            int categoryID;
            int.TryParse(category, out categoryID);

            Console.Write("Quantity per unit: ");
            string quantityPerUnit = Console.ReadLine();

            Console.Write("Unit price: ");
            string price = Console.ReadLine();
            decimal unitPrice;
            decimal.TryParse(price, out unitPrice);

            Console.Write("Units in stock: ");
            string stock = Console.ReadLine();
            short unitsInStock;
            short.TryParse(stock, out unitsInStock);

            Console.Write("Units on order: ");
            string onOrder = Console.ReadLine();
            short unitsOnOrder;
            short.TryParse(onOrder, out unitsOnOrder);

            Console.Write("Reorder level: ");
            string reorder = Console.ReadLine();
            short reorderLevel;
            short.TryParse(reorder, out reorderLevel);

            Console.Write("Discontinued: ");
            string discontinuedInput = Console.ReadLine();
            bool discontinued = false;
            bool.TryParse(discontinuedInput, out discontinued);

            Product product = new Product();
            product.ProductName = name;
            product.SupplierId = supplierID;
            product.CategoryId = categoryID;
            product.QuantityPerUnit = quantityPerUnit;
            product.UnitPrice = unitPrice;
            product.UnitsInStock = unitsInStock;
            product.UnitsOnOrder = unitsOnOrder;
            product.ReorderLevel = reorderLevel;
            product.Discontinued = discontinued;
            db.Products.Add(product);

            Console.WriteLine("Add another product? (Y/N)");
            if (Console.ReadLine().ToLower() != "y")
            {
                db.SaveChanges();
                return;
            }
        }

    }

    private static void EditProduct(NWContext db)
    {
        Product product = FindProduct(db);
        if (product == null)
        {
            return;
        }

        Console.WriteLine("Enter '\\' for any field you don't want to change.");

        string name = "";
        while (name == "")
        {
            Console.Write("Product name: ");
            name = Console.ReadLine();
            if (name == "")
            {
                Console.WriteLine("Invalid response! The name can't be blank.");
                Logger.Info($"Invalid response received for product name - {name}");
            }
            else if (name != "\\")
            {
                product.ProductName = name;
            }
        }

        Console.Write("Supplier ID: ");
        string supplier = Console.ReadLine();
        if (supplier != "\\")
        {
            int supplierID;
            int.TryParse(supplier, out supplierID);
            product.SupplierId = supplierID;
        }

        Console.Write("Category ID: ");
        string category = Console.ReadLine();
        if (category != "\\")
        {
            int categoryID;
            int.TryParse(category, out categoryID);
            product.CategoryId = categoryID;
        }

        Console.Write("Quantity per unit: ");
        string quantityPerUnit = Console.ReadLine();
        if (quantityPerUnit != "\\")
        {
            product.QuantityPerUnit = quantityPerUnit;
        }

        Console.Write("Unit price: ");
        string price = Console.ReadLine();
        if (price != "\\")
        {
            decimal unitPrice;
            decimal.TryParse(price, out unitPrice);
            product.UnitPrice = unitPrice;
        }

        Console.Write("Units in stock: ");
        string stock = Console.ReadLine();
        if (stock != "\\")
        {
            short unitsInStock;
            short.TryParse(stock, out unitsInStock);
            product.UnitsInStock = unitsInStock;
        }

        Console.Write("Units on order: ");
        string onOrder = Console.ReadLine();
        if (onOrder != "\\")
        {
            short unitsOnOrder;
            short.TryParse(onOrder, out unitsOnOrder);
            product.UnitsOnOrder = unitsOnOrder;
        }

        Console.Write("Reorder level: ");
        string reorder = Console.ReadLine();
        if (reorder != "\\")
        {
            short reorderLevel;
            short.TryParse(reorder, out reorderLevel);
            product.ReorderLevel = reorderLevel;
        }

        Console.Write("Discontinued: ");
        string discontinuedInput = Console.ReadLine();
        if (discontinuedInput != "\\")
        {
            bool discontinued = false;
            bool.TryParse(discontinuedInput, out discontinued);
            product.Discontinued = discontinued;
        }
        
        Console.WriteLine("\nNew product info:");
        DisplayProductInfo(product);
        db.SaveChanges();
    }

    /**
     * Display all products to the console. The user gets to decide if they want to see all products, discontinued products, or active products.
     */
    private static void DisplayProducts(NWContext db)
    {
        bool active = true;
        bool discontinued = true;
        Console.WriteLine("Which products would you like to see?");
        Console.WriteLine("1) All products");
        Console.WriteLine("2) Active products");
        Console.WriteLine("3) Discontinued products");

        IEnumerable<Product> query;

        string response = Console.ReadLine().ToLower();
        switch (response)
        {
            case "1":
                query = db.Products.OrderBy(p => p.ProductName);
                break;
            case "2":
                discontinued = false;
                query = db.Products.Where(p => !p.Discontinued).OrderBy(p => p.ProductName);
                break;
            case "3":
                active = false;
                query = db.Products.Where(p => p.Discontinued).OrderBy(p => p.ProductName);
                break;
            default:
                Console.WriteLine("Invalid response!");
                Logger.Error($"Invalid response {response} on display products prompt");
                return;
        }

        Console.WriteLine($"{query.Count()} products returned: \n");

        if (active)
        {
            Console.WriteLine("Active products: ");
            var activeProducts = discontinued ? query.Where(p => !p.Discontinued) : query; // This is a slight optimization if the query is already filtered.
            foreach (var item in activeProducts)
            {
                Console.WriteLine(item.ProductName);
            }
        }

        if (discontinued)
        {
            Console.WriteLine("\nDiscontinued products: ");
            var discontinuedProducts = active ? query.Where(p => p.Discontinued) : query;
            foreach (var item in discontinuedProducts)
            {
                Console.WriteLine(item.ProductName);
            }
        }
    }

    private static Product FindProduct(NWContext db)
    {
        Product product = SearchByIdOrName(db);
        if (product == null)
        {
            return null;
        }

        DisplayProductInfo(product);
        return product;
    }

    private static void DisplayProductInfo(Product product)
    {
    
        Console.WriteLine($"Product ID: {product.ProductId}");
        Console.WriteLine($"Product Name: {product.ProductName}");
        Console.WriteLine($"Supplier ID: {product.SupplierId}");
        Console.WriteLine($"Category ID: {product.CategoryId}");
        Console.WriteLine($"Quantity per unit: {product.QuantityPerUnit}");
        Console.WriteLine($"Unit price: {product.UnitPrice}");
        Console.WriteLine($"Units in stock: {product.UnitsInStock}");
        Console.WriteLine($"Units on order: {product.UnitsOnOrder}");
        Console.WriteLine($"Reorder level: {product.ReorderLevel}");
        Console.WriteLine($"Discontinued: {product.Discontinued}");
    }

    private static Product SearchByIdOrName(NWContext db)
    {
        var query = db.Products;
        Console.WriteLine("Would you like to find a product by its ID or name?");
        Console.WriteLine("1) ID");
        Console.WriteLine("2) Name");
        string response = Console.ReadLine().ToLower();
        Product? product;
        switch (response)
        {
            case "1":
                Console.Write("ID: ");
                string id = Console.ReadLine().ToLower();
                int number;
                try
                {
                    number = int.Parse(id);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    return null;
                }
                product = query.Where(p => p.ProductId == number).FirstOrDefault();
                break;
            case "2":
                Console.Write("Name: ");
                string name = Console.ReadLine().ToLower();
                product = query.Where(p => p.ProductName.ToLower() == name).FirstOrDefault();
                break;
            default:
                Console.WriteLine("Invalid response!");
                Logger.Error($"Invalid response {response} on find product prompt");
                return null;
        }
        if (product == null)
        {
            Logger.Info("There is no product that matches the given description.");
        }
        return product;
    }
}