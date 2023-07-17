using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IShortlistingService
    {
        /// <summary>
        /// Shortlists the applications.
        /// </summary>
        void ShortlistApplications();

        /// <summary>
        /// Gets the shortlist percentage.
        /// </summary>
        /// <param name="totalExams">The total exams.</param>
        /// <returns></returns>
        double GetShortlistPercentage(int totalExams);
    }
}
