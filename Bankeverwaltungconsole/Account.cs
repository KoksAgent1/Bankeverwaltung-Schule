using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bankeverwaltungconsole.Bankingmanger;

namespace Bankeverwaltungconsole
{
    internal class Account
    {
        public string AccountNumber { get; private set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; private set; }
        public string IBAN { get; private set; }
        public List<Transaction> Transactions { get; private set; }
        public List<Customer> Owners { get; private set; }  // Änderung hier
        private Bank Bank { get; set; }

        public Account(string accountNumber, AccountType accountType, decimal initialBalance, Bank bank)
        {
            AccountNumber = accountNumber;
            AccountType = accountType;
            Balance = initialBalance;
            Bank = bank;
            Transactions = new List<Transaction>();
            Owners = new List<Customer>();  // Liste von Kunden
            IBAN = $"DE{new Random().Next(10, 99)}{bank.BankCode}{accountNumber}";  // Einfache Simulation einer IBAN
        }

        public void AddOwner(Customer customer)
        {
            Owners.Add(customer);
        }

        public string GetOwnerNames()
        {
            return string.Join(", ", Owners.Select(o => o.Name));
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Deposit));
            Console.WriteLine($"Eingezahlt: {amount}€, Neuer Kontostand: {Balance}€");
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Withdrawal));
                Console.WriteLine($"Abgehoben: {amount}€, Verbleibender Kontostand: {Balance}€");
                return true;
            }
            return false;
        }

        public bool Transfer(string ibanTo, decimal amount)
        {
            Account targetAccount = Bank.FindAccountByIBAN(ibanTo);
            if (targetAccount != null && this.Withdraw(amount))
            {
                targetAccount.Deposit(amount);
                Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Transfer,ibanTo));
                return true;
                Console.WriteLine($"Transfer of {amount}€ to IBAN {ibanTo} successful.");
            }
            else
            {
                return false;
                Console.WriteLine("Transfer failed: Account not found or insufficient funds.");
            }
        }

        public void PrintTransactions()
        {
            foreach (var transaction in Transactions)
            {
                Console.WriteLine(transaction);
            }
        }
    }

}
