using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    public interface IBackgroundCheckRepository
    {
        /// <summary>
        /// Creates the specified form.
        /// </summary>
        /// <param name="form">The form.</param>
        void Create(BackgroundCheck form);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IQueryable<BackgroundCheck> GetAll();
        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        BackgroundCheck GetById(int id);
    }
}
