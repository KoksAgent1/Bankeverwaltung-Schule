using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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

        public Account(string accountNumber, AccountType accountType, decimal initialBalance, Bank bank)
        {
            AccountNumber = accountNumber;
            AccountType = accountType;
            Balance = initialBalance;
            Transactions = new List<Transaction>();
            IBAN = $"DE{new Random().Next(10, 99)}{bank.BankCode}{accountNumber}";
        }

        [JsonConstructor]
        public Account(string accountNumber, AccountType accountType, decimal balance, List<Transaction> transactions, string IBAN)
        {
            AccountNumber = accountNumber;
            AccountType = accountType;
            Balance = balance;
            Transactions = transactions;
            this.IBAN = IBAN;
        }
        
        public void Deposit(decimal amount,bool isTransfer = false, string Iban = null)
        {
            Balance += amount;
            if (!isTransfer)
            {
                Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Deposit)); 
                Console.WriteLine($"Eingezahlt: {amount}€, Neuer Kontostand: {Balance}€");
            }
            else
            {
                Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Transfer,Iban,true));
            }
            
        }

        public bool Withdraw(decimal amount,bool isTransfer = false)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                if (!isTransfer)
                {
                    Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Withdrawal)); 
                    Console.WriteLine($"Abgehoben: {amount}€, Verbleibender Kontostand: {Balance}€");
                }
                
                return true;
            }
            return false;
        }

        public bool Transfer(string ibanTo, decimal amount,Bank bank)
        {
            Account targetAccount = bank.FindAccountByIBAN(ibanTo);
            if (targetAccount != null && this.Withdraw(amount,true))
            {
                
                targetAccount.Deposit(amount,true,IBAN);
                Transactions.Add(new Transaction(amount, Bankingmanger.Transaktiontype.Transfer, ibanTo,false));
                return true;
            }
            else
            {
                return false;
            }
        }

        public string getAccountInfos(Bank bank)
        {
            bool IsShared = bank.GetCustomersForAccount(AccountNumber).Count > 1;
            string shardstatus = IsShared ? "Geteiltes Konto" : "Eigenes Konto";
            return $"{shardstatus}, Kontonummer: {AccountNumber}, Type: {AccountType}";
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
