using Newtonsoft.Json;
using System;

namespace Bankeverwaltungconsole
{
    internal class Transaction
    {
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public Bankingmanger.Transaktiontype Type { get; private set; }

        public bool Receiver { get; private set; }

        public string tranfertoIBAN { get; private set; }

        public Transaction(decimal amount, Bankingmanger.Transaktiontype type)
        {
            Amount = amount;
            Date = DateTime.Now;
            Type = type;
            Receiver = false;

        }

        public Transaction(decimal amount, Bankingmanger.Transaktiontype type, string IBAN,bool receiver)
        {
            Amount = amount;
            Date = DateTime.Now;
            Type = type;
            tranfertoIBAN = IBAN;
            Receiver = receiver; 
        }

        [JsonConstructor]
        public Transaction(decimal amount, Bankingmanger.Transaktiontype type, string tranfertoIBAN, DateTime date, bool receiver)
        {
            Amount = amount;
            Date = date;
            Type = type;
            this.tranfertoIBAN = tranfertoIBAN;
            Receiver = receiver;
        }

        public override string ToString()
        {
            if (Receiver && !string.IsNullOrEmpty(tranfertoIBAN))
            {
                return $"Type: {Type}, Betrag: {Amount}€, Datum: {Date} Erhalten von: {tranfertoIBAN}";
            }else if (!Receiver && !string.IsNullOrEmpty(tranfertoIBAN))
            {
                return $"Type: {Type}, Betrag: {Amount}€, Datum: {Date} Gesendet an: {tranfertoIBAN}";
            }
            else
            {
                return $"Type: {Type}, Betrag: {Amount}€, Datum: {Date}";
            }
            
        }
    }

}
