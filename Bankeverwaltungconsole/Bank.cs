using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bankeverwaltungconsole.Bankingmanger;

namespace Bankeverwaltungconsole
{
    internal class Bank
    {
        public string Name { get; set; }
        public string BankCode { get; set; }  // Bankleitzahl
        private int nextAccountNumber = 1;
        public List<Customer> Customers { get; set; }

        public Bank(string name, string bankCode)
        {
            Name = name;
            BankCode = bankCode;
            Customers = new List<Customer>();
        }

        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer);
        }

        public Customer FindCustomer(string name)
        {
            return Customers.Find(c => c.Name == name);
        }

        // Generiert eine neue, einzigartige Kontonummer für die Bank
        private string GenerateAccountNumber()
        {
            return BankCode + nextAccountNumber++.ToString("D10"); // Erzeugt eine Kontonummer mit führenden Nullen
        }

        // Erstellt ein neues Konto und fügt es einem Kunden hinzu
        public void CreateAccountForCustomers(List<Customer> customerNames, AccountType accountType, decimal initialBalance)
        {
            var account = new Account(GenerateAccountNumber(), accountType, initialBalance,this);
            foreach (var customer in customerNames)
            {
                if (customer != null)
                {
                    customer.AddAccount(account);
                    account.AddOwner(customer);
                }
            }
        }

        // Methode zum Suchen eines Kontos anhand der IBAN
        public Account FindAccountByIBAN(string iban)
        {
            foreach (var customer in Customers)
            {
                foreach (var account in customer.Accounts)
                {
                    if (account.IBAN == iban)
                    {
                        return account;
                    }
                }
            }
            return null;
        }
    }

}
