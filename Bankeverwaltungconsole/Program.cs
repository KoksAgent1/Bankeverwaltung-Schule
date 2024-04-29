using Bankeverwaltungconsole;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bankeverwaltungconsole.Bankingmanger;

class Program
{
    private static Bank bank;
    private static MenuHeader header;

    static void Main(string[] args)
    {
        Console.WriteLine("\nWillkommen beim Bankprogramm!\n");
        header = new MenuHeader();
        
        bank = CreateBank();
        header.bank = bank;
        MainMenu();
        
    }

    static Bank CreateBank()
    {
        Console.WriteLine("\nEs wurde keine Bank gefunden! Bitte eine anlegen");
        Console.Write("Name der Bank: ");
        string bankName = Console.ReadLine();
        Console.Write("Bankleitzahl: ");
        string bankCode = Console.ReadLine();
        
        return new Bank(bankName, bankCode);
    }

    static void MainMenu()
    {
        List<string> options = new List<string> {
            "Kunden auflisten",
            "Neuen Kunden erstellen",
            "Beenden"
        };

        while (true)
        {
            int selectedIndex = ReadMenuSelection(options);
            
            switch (selectedIndex)
            {
                case 0:
                    ListCustomers();
                    break;
                case 1:
                    CreateCustomer();
                    break;
                case 2:
                    return;
                default:
                    Console.WriteLine("Ungültige Auswahl, bitte versuchen Sie es erneut.");
                    break;
            }
        }
    }

    static void ListCustomers()
    {
        header.DisplayHeader();
        if (bank.Customers.Count == 0)
        {
            Console.WriteLine("Keine Kunden vorhanden.");
            Backtomenu();
            return;
        }
        List<string> options = bank.Customers.Select(c => c.Name).ToList();
        options.Add("Zurück zum Hauptmenü");

        while (true)
        {
            int selectedIndex = ReadMenuSelection(options);

            if (selectedIndex == options.Count - 1)
            {
                // Wenn die Option "Zurück zum Hauptmenü" ausgewählt wird
                header.customer = null;
                return;
            }
            else
            {
                // Kundenspezifisches Menü öffnen
                Customer selectedCustomer = bank.Customers[selectedIndex];
                header.customer = selectedCustomer;
                AccountMenu(selectedCustomer);
            }
        }
    }

    static void CreateCustomer()
    {
        Console.Clear();
        header.DisplayHeader();
        Console.WriteLine("Geben Sie den Namen des neuen Kunden ein:");
        string name = Console.ReadLine().Trim();
        while (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Der Name darf nicht leer sein. Bitte erneut eingeben:");
            name = Console.ReadLine().Trim();
        }
        var customer = new Customer(name);
        bank.AddCustomer(customer);
        Console.Clear();
        header.DisplayHeader();
        
        Console.WriteLine("\nKunde erfolgreich erstellt.");
        Backtomenu();
    }

    static void AccountMenu(Customer customer)
    {
        List<string> options = new List<string> {
            "Konten auflisten",
            "Konto erstellen",
            "Konto auswählen",
            "Kundeninfo bearbeiten",
            "Zurück zum Kundenmenü"
        };

        while (true)
        {
            Console.WriteLine($"\nKontomenü für {customer.Name}:");
            int selectedIndex = ReadMenuSelection(options);

            switch (selectedIndex)
            {
                case 0:
                    ListAccounts(customer);
                    break;
                case 1:
                    CreateAccount(customer);
                    break;
                case 2:
                    SelectAccount(customer);
                    break;
                case 3:
                    EditCustomerInfo(customer);
                    break;
                case 4:
                    header.customer = null;
                    return;
                default:
                    Console.WriteLine("Ungültige Auswahl, bitte versuchen Sie es erneut.");
                    break;
            }
        }
    }

    static void ListAccounts(Customer customer)
    {
        Console.Clear();
        header.DisplayHeader("Alle Konten");
        foreach (var account in customer.Accounts)
        {
            Console.WriteLine($"Kontonummer: {account.AccountNumber}, Type: {account.AccountType}, Kontostand: {account.Balance}");
        }
        Backtomenu();
    }

    static void CreateAccount(Customer customer)
    {
        Console.Clear();
        header.DisplayHeader();
        Console.WriteLine("Wenn weitere Kunden zu diesem Konto hinzufügen werden sollen, gebe die Namen getrennt durch Kommas ein. Drücke Enter, um fortzufahren ohne zusätzliche Kunden hinzuzufügen:");
        string input = Console.ReadLine();
        List<string> customerNames = input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(name => name.Trim())
                                          .ToList();

        // Überprüfen, ob zusätzliche Kunden existieren und sie zur Liste hinzufügen
        List<Customer> customers = new List<Customer> { customer }; // Starten mit dem übergebenen Kunden
        foreach (var name in customerNames)
        {
            var additionalCustomer = bank.FindCustomer(name);
            if (additionalCustomer == null)
            {
                Console.WriteLine($"Kunde {name} nicht gefunden.");
                continue;
            }
            customers.Add(additionalCustomer);
        }

        // Kontotyp auswählen
        Console.WriteLine("Wähle den Kontotyp (1: Girokonto, 2: Sparkonto):");
        if (!int.TryParse(Console.ReadLine(), out int accountTypeChoice) || accountTypeChoice < 1 || accountTypeChoice > 2)
        {
            Console.WriteLine("Ungültige Auswahl. Bitte wählen  1 für Girokonto oder 2 für Sparkonto.");
            return;
        }
        AccountType accountType = (AccountType)(accountTypeChoice - 1);

        bank.CreateAccountForCustomers(customers, accountType,0);
        
        Console.WriteLine("Konto erfolgreich erstellt für: " + string.Join(", ", customers.Select(c => c.Name)));
    }

    static void SelectAccount(Customer customer)
    {
        if (customer.Accounts.Count == 0)
        {
            header.DisplayHeader("\nKeine Konten vorhanden. Bitte eins erstellen");
            Backtomenu();
            return;
        }
        List<string> accountIBANs = customer.Accounts.Select(a => a.IBAN).ToList();
        Console.WriteLine("\nBitte wähle ein Konto:");
        int selectedIndex = ReadMenuSelection(accountIBANs);

        Account selectedAccount = customer.Accounts[selectedIndex];
        header.account = selectedAccount;
        TransactionMenu(selectedAccount);
    }

    static void EditCustomerInfo(Customer customer)
    {
        Console.Clear();
        header.DisplayHeader();
        Console.WriteLine("Geben Sie den Namen des neuen Kunden ein:");
        string newName = Console.ReadLine().Trim();
        while (string.IsNullOrEmpty(newName))
        {
            Console.WriteLine("Der Name darf nicht leer sein. Bitte erneut eingeben:");
            newName = Console.ReadLine().Trim();
        }
        customer.Name = newName;
        header.customer = customer;
        
        Console.WriteLine("Kontoinformation erfolgreich bearbeitet");
        Backtomenu();
    }

    static void TransactionMenu(Account account)
    {
        List<string> options = new List<string> {
            "Einzahlen",
            "Abheben",
            "Überweisen",
            "Transaktionen anzeigen",
            "Zurück zum Kontomenü"
        };

        while (true)
        {
            
            Console.WriteLine($"\nTransaktionsmenü für Konto: {account.AccountNumber}");
            int selectedIndex = ReadMenuSelection(options);

            switch (selectedIndex)
            {
                case 0:
                    Deposit(account);
                    break;
                case 1:
                    Withdraw(account);
                    break;
                case 2:
                    PerformTransfer(account);
                    break;
                case 3:
                    ShowTransactions(account);
                    break;
                case 4:
                    header.account = null;
                    return;
                default:
                    Console.WriteLine("Ungültige Auswahl, bitte versuchen Sie es erneut.");
                    break;
            }
        }
    }

    static void Deposit(Account account)
    {
        Console.Clear();
        Console.WriteLine("Geben Sie den Einzahlungsbetrag ein:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Ungültige Eingabe. Der Betrag muss positiv sein.");
            return;
        }
        account.Deposit(amount);
        
        Console.WriteLine($"Einzahlung erfolgreich: {amount}€");
        Backtomenu();
    }

    static void Withdraw(Account account)
    {
        Console.Clear();
        Console.WriteLine("Geben Sie den Abhebungsbetrag ein:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Ungültige Eingabe. Der Betrag muss positiv sein.");
            return;
        }
        if (!account.Withdraw(amount))
        {
            Console.WriteLine("Abhebung fehlgeschlagen: Nicht genügend Guthaben.");
        }
        else
        {
            Console.WriteLine($"Abhebung erfolgreich: {amount}€");
            
        }
        Backtomenu();
    }

    static void PerformTransfer1(Account account)
    {
        Console.Clear();
        Console.WriteLine("Geben Sie die IBAN des Zielkontos ein:");
        string ibanTo = Console.ReadLine().Trim();
        while (string.IsNullOrEmpty(ibanTo))
        {
            Console.WriteLine("Die IBAN darf nicht leer sein. Bitte erneut eingeben:");
            ibanTo = Console.ReadLine().Trim();
        }

        Console.WriteLine("Geben Sie den Überweisungsbetrag ein:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Ungültige Eingabe. Der Betrag muss positiv sein.");
            return;
        }

        if (account.Transfer(ibanTo, amount))
        {
            Console.WriteLine($"Überweisung von {amount}€ an {ibanTo} wurde erfolgreich durchgeführt.");
        }
        else
        {
            Console.WriteLine($"Überweisung von {amount} an {ibanTo} fehlgeschlagen. Konto nicht gefunden oder nicht aussreichendes Guthaben");
        }
       
        Backtomenu();
    }

    static void PerformTransfer(Account account)
    {
        Console.WriteLine("Möchten Sie das Geld auf ein anderes eigenes Konto überweisen? (ja/nein)");
        string response = Console.ReadLine().Trim().ToLower();

        string ibanTo = "";
        if (response.ToLower() != "nein" || response.ToLower() != "n" || response.ToLower() != "no")
        {
            List<Account> otherAccounts = account.Owners.SelectMany(c => c.Accounts)
                                                         .Where(a => a != account) // Alle anderen Konten des Kunden, nicht das aktuelle
                                                         .Distinct() // Entfernt Duplikate, falls Kunden mehrere Konten teilen
                                                         .ToList();

            if (otherAccounts.Any())
            {
                Console.WriteLine("Wählen Sie ein Zielkonto aus:");
                for (int i = 0; i < otherAccounts.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. Konto: {otherAccounts[i].IBAN} - {otherAccounts[i].AccountType}");
                }
                Console.WriteLine($"{otherAccounts.Count + 1}. Abbrechen");

                int chosenIndex = ReadMenuSelection(otherAccounts.Select(a => $"Konto: {a.IBAN} - {a.AccountType}").ToList()) + 1;
                if (chosenIndex == otherAccounts.Count + 1)
                {
                    return; // Benutzer wählt Abbrechen
                }
                else
                {
                    ibanTo = otherAccounts[chosenIndex - 1].IBAN;
                }
            }
            else
            {
                Console.WriteLine("Keine weiteren Konten vorhanden.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Geben Sie die IBAN des Zielkontos ein:");
            ibanTo = Console.ReadLine().Trim();
            while (string.IsNullOrEmpty(ibanTo))
            {
                Console.WriteLine("Die IBAN darf nicht leer sein. Bitte erneut eingeben:");
                ibanTo = Console.ReadLine().Trim();
            }
        }

        Console.WriteLine("Geben Sie den Überweisungsbetrag ein:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Ungültige Eingabe. Der Betrag muss positiv sein.");
            return;
        }

        if (!account.Withdraw(amount))
        {
            Console.WriteLine("Überweisung fehlgeschlagen: Nicht genügend Guthaben.");
            return;
        }

        // Die Bank-Klasse oder die entsprechende Methode zur Überweisung muss hier die Überweisung verarbeiten.
        Account targetAccount = bank.FindAccountByIBAN(ibanTo);
        if (targetAccount != null)
        {
            targetAccount.Deposit(amount);
            
            Console.WriteLine($"Überweisung von {amount}€ an {ibanTo} wurde erfolgreich durchgeführt.");
        }
        else
        {
            Console.WriteLine("Konto nicht gefunden.");
            account.Deposit(amount); // Rückgängig machen der Abhebung, da das Zielkonto nicht gefunden wurde
        }
    }


    static void ShowTransactions(Account account)
    {
        Console.WriteLine($"\nAlle Transaktion vom Konto {account.AccountNumber}:");
        account.PrintTransactions();
        Backtomenu();
    }

    static int ReadMenuSelection(List<string> options)
    {
        int selectedIndex = 0;
        ConsoleKeyInfo key;

        do
        {
            header.DisplayHeader();
            Console.WriteLine("Navigieren Sie mit den Pfeiltasten und wählen Sie mit Enter.\n");
            for (int i = 0; i < options.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(options[i]);
                Console.ResetColor();
            }

            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + options.Count) % options.Count;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % options.Count;
            }

        } while (key.Key != ConsoleKey.Enter);

        return selectedIndex;
    }
    static void Backtomenu()
    {
        Console.WriteLine("\nEine Taste Klicken um zurück zum Menu zu kommen...");
        Console.ReadKey();
    }




}
