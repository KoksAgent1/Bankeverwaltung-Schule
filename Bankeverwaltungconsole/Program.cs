using Bankeverwaltungconsole;
using System;
using System.Collections.Generic;

class Program
{
    private static Bank bank;
    private static MenuHeader header;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("\nWillkommen beim Bankprogramm!\n");
        header = new MenuHeader();

        bank = Datastorage.LoadData() ?? CreateBank();
        header.bank = bank;

        MenuManager menuManager = new MenuManager(bank, header);
        menuManager.MainMenu();

    }
    
    static Bank CreateBank()
    {
        Console.WriteLine("\nEs wurde keine Bank gefunden! Bitte eine anlegen");
        Console.Write("Name der Bank: ");
        string bankName = Console.ReadLine();
        Console.Write("Bankleitzahl: ");
        string bankCode = Console.ReadLine();
        Datastorage.SaveData(bank);
        return new Bank(bankName, bankCode);
    }




}
