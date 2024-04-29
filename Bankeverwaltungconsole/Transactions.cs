using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankeverwaltungconsole
{
    internal class Transaction
    {
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public Bankingmanger.Transaktiontype Type { get; private set; }

        public string tranfertoIBAN { get; private set; }

        public Transaction(decimal amount, Bankingmanger.Transaktiontype type)
        {
            Amount = amount;
            Date = DateTime.Now;
            Type = type;
        }

        public Transaction(decimal amount, Bankingmanger.Transaktiontype type,string IBAN)
        {
            Amount = amount;
            Date = DateTime.Now;
            Type = type;
            tranfertoIBAN = IBAN;
        }

        public override string ToString()
        {
            return $"Type: {Type}, Betrag: {Amount}€, Datum: {Date}";
        }
    }

}
