﻿using dMart.Models;

namespace DMART.Models.Interfaces
{
    public interface IUserRepository
    {
        const string conString="";
        public int RegisterUser(users user);
        public List<users> ViewUsers();
        public bool validateNewUser(users user);
        public bool validateLogin(users user);

    }
}
