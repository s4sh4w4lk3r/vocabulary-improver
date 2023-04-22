using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console;

partial class Improver
{
    VIDictionary vIDictionary;
    public Improver(VIDictionary vIDictionary)
    {
        this.vIDictionary = vIDictionary;
    }
    static string PasswordInput()
    {
        string password = string.Empty;
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            password += key.KeyChar;
        } while (key.Key != ConsoleKey.Enter);
        System.Console.WriteLine();
        return password;
    }
    public static void ShutDown()
    {
        Thread.Sleep(4000);
        Environment.Exit(0);
    }
    public void Start()
    {
        if (vIDictionary.DictList.Count == 0)
        {
            System.Console.WriteLine("Bad path or empty file.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine($"Welcome! dictionary size is {vIDictionary.DictList.Count} words.");
        foreach (var item in vIDictionary.DictList)
        {
            System.Console.WriteLine();
            Console.Write($"{item.Key} ---> ");
            Console.ReadLine();
            Console.WriteLine($"Correct answer ---> {item.Value}\nDid you answer correctly? y/n");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Y:
                    vIDictionary.IncreaseRating(item.Key);
                    break;

                case ConsoleKey.N:
                    vIDictionary.ReduceRating(item.Key);
                    break;

                default:
                    Console.WriteLine("Skipped");
                    break;
            }
        }
        Console.WriteLine("End of the dictionary.");
        Console.ReadKey();
    }
    public static void StartDatabase()
    {
        Console.Write("Entering database parameters\nHostname: ");
        string hostname = Console.ReadLine()!;
        Console.Write("Port (skip for default): ");
        string port = Console.ReadLine()!;
        Console.Write("Username: ");
        string username = Console.ReadLine()!;
        Console.Write("Password: ");
        string password = PasswordInput();
        Console.Write("Database name: ");
        string db_name = Console.ReadLine()!;
        Console.Write("Table name: ");
        string tableName = Console.ReadLine()!;
        System.Console.WriteLine();
        if (port == string.Empty) port = "3306";
        if (hostname == string.Empty || username == string.Empty || password == string.Empty || db_name == string.Empty || tableName == string.Empty)
        {
            System.Console.Write("Some parameter was not entered. The application will be closed.");
            ShutDown();
        }

        DBProcessing database = null!;
        try
        {
            database = new DBProcessing(hostname, port, username, password, db_name, tableName);
        }
        catch
        {
            Console.Write("Bad login. The application will be closed.");
            ShutDown();
        }
        System.Console.WriteLine("1 - Start, 2 - Create table");
        if (Console.ReadKey(true).Key == ConsoleKey.D1)
        {
        VIDictionary viDict = new VIDictionary(database, database.GetDict());
        Improver improver = new Improver(viDict);
        improver.Start();
        ShutDown();
        }
        if (Console.ReadKey(true).Key == ConsoleKey.D2)
        {
            database.CreateDict();
            System.Console.WriteLine("OK");
            StartDatabase();
        }
    }
    public static void StartLocally()
    {
        System.Console.WriteLine("1 - create empty dictionary, 2 - open an existing dictionary");
        switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    FileProcessing.ConsoleCreateFile();
                    System.Console.WriteLine();
                    StartLocally();
                    break;
                case ConsoleKey.D2:
                    FileProcessing.ConsoleOpenFile();
                    StartLocally();
                    break;
            }
    }
}