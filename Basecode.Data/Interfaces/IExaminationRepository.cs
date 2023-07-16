﻿using Basecode.Data.Models;

namespace Basecode.Data.Interfaces
{
    public interface IExaminationRepository
    {
        IQueryable<Examination> GetExaminationsByJobOpeningId(int jobOpeningId);
    }
}
