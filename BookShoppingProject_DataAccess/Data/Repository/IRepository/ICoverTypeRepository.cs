﻿using BookShoppingProject_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject_DataAccess.Data.Repository.IRepository
{
    public interface ICoverTypeRepository:IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
