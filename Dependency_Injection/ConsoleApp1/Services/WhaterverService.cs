namespace ConsoleApp1.Services;

public class WhateverService : IWhateverService
{
    public void LogHello()
    {
        Console.WriteLine("\n --- Hello from WhateverService! --- \n");
    }
}