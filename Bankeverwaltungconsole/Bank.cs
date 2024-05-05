using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static Bankeverwaltungconsole.Bankingmanger;

namespace Bankeverwaltungconsole
{
    [Serializable]
    internal class Bank
    {
        public string Name { get; set; }
        public string BankCode { get; set; }  // Bankleitzahl
        public int nextAccountNumber { get; set; }
        public List<Customer> Customers { get; set; }

        public List<Account> Accounts { get; set; }
        public List<AccountCustomerMapping> AccountCustomerMappings { get; set; }

        public Bank(string name, string bankCode)
        {
            Name = name;
            BankCode = bankCode;
            Customers = new List<Customer>();
            Accounts = new List<Account>();
            AccountCustomerMappings = new List<AccountCustomerMapping>();
            nextAccountNumber = 1;
        }

        [JsonConstructor]
        public Bank(string name, string bankCode,int nextAccountNumber, List<AccountCustomerMapping> accountCustomerMappings,List<Customer> customers, List<Account> accounts)
        {
            Name = name;
            BankCode = bankCode; 
            this.Customers = customers ?? new List<Customer>();
            this.AccountCustomerMappings = accountCustomerMappings ?? new List<AccountCustomerMapping>();
            this.nextAccountNumber = nextAccountNumber;
            this.Accounts = accounts ?? new List<Account>();
        }

        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer);
        }

        public void AddCustomerToAccount(string accountId, string customerId)
        {
            var mapping = AccountCustomerMappings.FirstOrDefault(m => m.AccountId == accountId);
            if (mapping == null)
            {
                mapping = new AccountCustomerMapping { AccountId = accountId };
                AccountCustomerMappings.Add(mapping);
            }
            if (!mapping.CustomerIds.Contains(customerId))
            {
                mapping.CustomerIds.Add(customerId);
            }
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
            var account = new Account(GenerateAccountNumber(), accountType, initialBalance, this);
            Accounts.Add(account);
            foreach (var customer in customerNames)
            {
                if (customer != null)
                {
                    
                    AddCustomerToAccount(account.AccountNumber,customer.Id);
                }
            }
        }

        // Methode zum Suchen eines Kontos anhand der IBAN
        public Account FindAccountByIBAN(string iban)
        {
            foreach (var customer in Customers)
            {
                foreach (var account in Accounts)
                {
                    if (account.IBAN == iban)
                    {
                        return account;
                    }
                }
            }
            return null;
        }

        public List<Customer> GetCustomersForAccount(string accountId)
        {
            var mapping = AccountCustomerMappings.FirstOrDefault(m => m.AccountId == accountId);
            if (mapping != null)
            {
                return Customers.Where(c => mapping.CustomerIds.Contains(c.Id)).ToList();
            }
            return new List<Customer>();
        }

        public List<Account> GetAccountsForCustomer(string customerId)
        {
            var accountIds = AccountCustomerMappings
                .Where(m => m.CustomerIds.Contains(customerId))
                .Select(m => m.AccountId)
                .Distinct()
                .ToList();

            if (!accountIds.Any())
            {
                return new List<Account>();
            }

            return Accounts.Where(a => accountIds.Contains(a.AccountNumber)).ToList();
        }


    }

}
