using System;
using System.Collections.Generic;
using System.Linq;

namespace Bankeverwaltungconsole
{
    internal class Bankingmanger
    {
        private Bank bank;
        private MenuHeader header;
        private MenuManager menu;
        public enum AccountType
        {
            girokonto,
            sparkonto,
            tagesgeldkonto,
            kreditkonto,
        }

        public enum Transaktiontype
        {
            Deposit,
            Withdrawal,
            Transfer,
        }

        public Bankingmanger(Bank bank, MenuHeader header, MenuManager menu)
        {
            this.bank = bank;
            this.header = header;
            this.menu = menu;
        }

        internal void ListCustomers()
        {
            header.DisplayHeader();
            if (bank.Customers.Count == 0)
            {
                Console.WriteLine("Keine Kunden vorhanden.");
                menu.Backtomenu();
                return;
            }
            List<string> options = bank.Customers.Select(c => c.Name).ToList();
            options.Add("Zurück zum Hauptmenü");

            while (true)
            {
                int selectedIndex = menu.ReadMenuSelection(options);

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
                    menu.AccountMenu(selectedCustomer);
                }
            }
        }

        internal void CreateCustomer()
        {
            Console.Clear();
            header.DisplayHeader();
            int nextcustomer = bank.Customers.Count +1;
            Console.WriteLine("Geben Sie den Namen des neuen Kunden ein:");
            string name = Console.ReadLine().Trim();
            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Der Name darf nicht leer sein. Bitte erneut eingeben:");
                name = Console.ReadLine().Trim();
            }
            var customer = new Customer(name,$"{nextcustomer}");
            bank.AddCustomer(customer);
            Console.Clear();
            header.DisplayHeader();
            Datastorage.SaveData(bank);
            Console.WriteLine("\nKunde erfolgreich erstellt.");
            menu.Backtomenu();
        }

        internal void ListAccounts(Customer customer)
        {
            Console.Clear();
            var accounts = bank.GetAccountsForCustomer(customer.Id);

            if (accounts.Count == 0)
            {
                header.DisplayHeader("Keine Konten vorhanden");
                menu.Backtomenu();
                return;
            }

            header.DisplayHeader("Alle Konten");
            foreach (var account in accounts)
            {
                Console.WriteLine(account.getAccountInfos(bank));
            }
            menu.Backtomenu();
        }

        internal void CreateAccount(Customer customer)
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

            bank.CreateAccountForCustomers(customers, accountType, 0);
            Datastorage.SaveData(bank);
            Console.WriteLine("Konto erfolgreich erstellt für: " + string.Join(", ", customers.Select(c => c.Name)));
        }

        internal void SelectAccount(Customer customer)
        {
            if (bank.Accounts.Count == 0)
            {
                header.DisplayHeader("\nKeine Konten vorhanden. Bitte eins erstellen");
                menu.Backtomenu();
                return;
            }
            List<Account> accountIBANs = bank.GetAccountsForCustomer(customer.Id);
            int selectedIndex = menu.ReadMenuSelection(accountIBANs.Select(a => a.getAccountInfos(bank)).ToList(),true, "Bitte wähle ein Konto:");

            if (selectedIndex == accountIBANs.Count)
            {
                return;
            }
            Account selectedAccount = accountIBANs[selectedIndex];
            header.account = selectedAccount;
            menu.TransactionMenu(selectedAccount);
        }

        internal void EditCustomerInfo(Customer customer)
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
            menu.Backtomenu();
        }

        internal void Deposit(Account account)
        {
            Console.Clear();
            Console.WriteLine("Geben Sie den Einzahlungsbetrag ein:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Ungültige Eingabe. Der Betrag muss positiv sein.");
                return;
            }
            account.Deposit(amount);
            Datastorage.SaveData(bank);
            header.account = account;
            Console.WriteLine($"Einzahlung erfolgreich: {amount}€");
            menu.Backtomenu();
        }

        internal void Withdraw(Account account)
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
                header.account = account;
                Datastorage.SaveData(bank);
                Console.WriteLine($"Abhebung erfolgreich: {amount}€");

            }
            menu.Backtomenu();
        }

        internal void PerformTransfer(Account account)
        {
            header.DisplayHeader();
            Console.WriteLine("Möchten Sie das Geld auf ein anderes eigenes Konto überweisen? (ja/nein)");
            string response = Console.ReadLine().Trim().ToLower();

            string ibanTo = "";
            if (response == "ja" || response == "j" || response == "yes" || response == "y")
            {
                List<Account> otherAccounts = bank.GetAccountsForCustomer(header.customer.Id)
                    .Where(a => a != account)
                    .ToList();

                if (otherAccounts.Any())
                {
                    int chosenIndex = menu.ReadMenuSelection(otherAccounts.Select(a => a.getAccountInfos(bank)).ToList(),true, "Wählen Sie ein Zielkonto aus:") + 1;
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
                    menu.Backtomenu();
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

                while (ibanTo == account.IBAN)
                {
                    Console.WriteLine("Auf die eigen IBAN kann nicht überweisen werden. Bitte erneut eingeben:");
                    ibanTo = Console.ReadLine().Trim();
                }
            }
            header.DisplayHeader();
            Console.WriteLine("Geben Sie den Überweisungsbetrag ein:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Ungültige Eingabe. Der Betrag muss positiv sein.");
                menu.Backtomenu();
                return;
            }

            if (account.Transfer(ibanTo, amount,bank))
            {
                Datastorage.SaveData(bank);
                header.account = account;
                Console.WriteLine($"Überweisung von {amount}€ an {ibanTo} wurde erfolgreich durchgeführt.");
                menu.Backtomenu();
            }
            else
            {
                Console.WriteLine($"Überweisung von {amount}€ an {ibanTo} fehlgeschlagen. Konto nicht gefunden oder nicht genügend Guthaben.");
                menu.Backtomenu();
            }
        }

        internal void ShowTransactions(Account account)
        {
            header.DisplayHeader();
            Console.WriteLine($"\nAlle Transaktion vom Konto {account.AccountNumber}:");
            account.PrintTransactions();
            menu.Backtomenu();
        }
    }
}
