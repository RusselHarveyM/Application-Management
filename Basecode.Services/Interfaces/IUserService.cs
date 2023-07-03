﻿using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IUserService
    {
        List<User> RetrieveAll();
        void Add(User user);
        User GetById(int id);
        void Update(User user);
        void Delete(int id);
    }
}
