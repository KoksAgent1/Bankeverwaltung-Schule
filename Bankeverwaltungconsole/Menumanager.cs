using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankeverwaltungconsole
{
    internal class Menumanager
    {
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

        public void DisplayHeader(string additionalInfo = "" )
        {
            Console.Clear();
            Console.WriteLine("Banking System Aufgabe von Florian Wielga");

            if (bank != null)
            {
                Console.WriteLine($"Bank: {bank.Name}");
            }

            if (customer != null)
            {
                Console.WriteLine($"Kunde: {customer.Name}");
            }

            if (account != null)
            {
                Console.WriteLine($"Konto: {account.AccountNumber}");
            }

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                Console.WriteLine(additionalInfo);
            }

            Console.WriteLine();
        }
    }
}
