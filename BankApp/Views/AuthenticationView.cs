namespace BankApp.Views;
﻿using BankApp.Models;

namespace BankApp.Views;

internal class AuthenticationView
{
    public Customer DisplayRegisterMenu()
    {
        Customer newCustomer = new Customer();

        Console.Write("First name: ");
        newCustomer.FirstName = Console.ReadLine();

        Console.Write("Last name: ");
        newCustomer.LastName = Console.ReadLine();

        Console.Write("Email: ");
        newCustomer.Email = Console.ReadLine();

        Console.Write("Username: ");
        newCustomer.UserName = Console.ReadLine();

        Console.Write("Password: ");
        newCustomer.Password = Console.ReadLine();

        Console.Write("Phone number: ");
        newCustomer.PhoneNumber = Console.ReadLine();

        return newCustomer;
    }
}
