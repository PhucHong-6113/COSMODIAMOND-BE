﻿using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Users
{
    public interface IManagerRepository
    {
        IEnumerable<Manager> GetManagers();
        Manager GetManagerById(int managerId);
    }

}
