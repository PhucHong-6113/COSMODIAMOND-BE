﻿namespace Services.Users
{
    public interface IAuthenticateService
    {
        string Authenticate(string email, string password);
        void Logout(string token);
    }
}