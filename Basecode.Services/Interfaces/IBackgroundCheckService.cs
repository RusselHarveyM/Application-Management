using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IBackgroundCheckService
    {
        LogContent Create(BackgroundCheckFormViewModel form);
        BackgroundCheck GetBackgroundByCharacterRefId(int characterRefId);
    }
}
