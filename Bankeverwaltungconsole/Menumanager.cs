using System;
using System.Collections.Generic;

namespace Bankeverwaltungconsole
{
    internal class MenuManager
    {
        private Bank bank;
        private MenuHeader header;
        private Bankingmanger manger;

        public MenuManager(Bank bank, MenuHeader header)
        {
            this.bank = bank;
            this.header = header;
            this.manger = new Bankingmanger(this.bank, this.header, this);
        }

        internal void MainMenu()
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
                        manger.ListCustomers();
                        break;
                    case 1:
                        manger.CreateCustomer();
                        break;
                    case 2:
                        return;
                    default:
                        Console.WriteLine("Ungültige Auswahl, bitte versuchen Sie es erneut.");
                        break;
                }
            }
        }

        internal void AccountMenu(Customer customer)
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
                        manger.ListAccounts(customer);
                        break;
                    case 1:
                        manger.CreateAccount(customer);
                        break;
                    case 2:
                        manger.SelectAccount(customer);
                        break;
                    case 3:
                        manger.EditCustomerInfo(customer);
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

        internal void TransactionMenu(Account account)
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
                        manger.Deposit(account);
                        break;
                    case 1:
                        manger.Withdraw(account);
                        break;
                    case 2:
                        manger.PerformTransfer(account);
                        break;
                    case 3:
                        manger.ShowTransactions(account);
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

        internal int ReadMenuSelection(List<string> options,bool backoption = false,string addtext = null)
        {
            int selectedIndex = 0;
            ConsoleKeyInfo key;
            if (backoption)
            {
                options.Add("Zurück");
            }
            do
            {
                header.DisplayHeader(addtext);
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
        internal void Backtomenu()
        {
            Console.WriteLine("\nEine Taste Klicken um zurück zum Menu zu kommen...");
            Console.ReadKey();
        }
    }

    internal class MenuHeader
    {
        public Bank bank { get; set; }
        public Customer customer { get; set; }

        public Account account { get; set; }

        public MenuHeader()
        {
            this.bank = null;
            this.customer = null;
            this.account = null;
        }

        public void DisplayHeader(string additionalInfo = "")
        {
            Console.Clear();
            Console.WriteLine("Banking System Aufgabe von Florian Wielga");
            Console.WriteLine();

            if (bank != null)
            {
                Console.WriteLine($"Bank: {bank.Name}");
                Console.WriteLine($"Bankleitzahl: {bank.BankCode}");
            }

            if (customer != null)
            {
                Console.WriteLine($"Kunde: {customer.Name}");
                Console.WriteLine();
            }

            if (account != null)
            {
                Console.WriteLine($"Konto: {account.AccountNumber}");
                Console.WriteLine($"IBAN: {account.IBAN}");
                Console.WriteLine();
            }

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                Console.WriteLine(additionalInfo);
            }

            Console.WriteLine();
        }
    }
}
