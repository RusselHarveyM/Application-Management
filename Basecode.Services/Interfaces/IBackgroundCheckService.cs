using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

public interface IBackgroundCheckService
{
    LogContent Create(BackgroundCheckFormViewModel form);
    BackgroundCheck GetBackgroundByCharacterRefId(int characterRefId);
}