﻿using ViAPI.StaticMethods;

namespace ViAPI.Entities;

public class RegistredUser : User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Hash { get; set; }

    public RegistredUser(Guid guid, string firstname, string username, string email, string hash) : base(guid, firstname)
    {
        InputChecker.CheckStringException(username, hash);
        if (email.IsEmail() is false) throw new ArgumentException("The Email string has an invalid format or NULL.");
        Username = username.ToLower();
        Email = email.ToLower();
        Hash = hash;
    }

    public override string ToString() => $"{base.ToString()}, Username: {Username}, Email: {Email}, Hash: {Hash}";
}