namespace Basecode.Services.Interfaces;

public interface IOAuthService
{
    bool Callback(string tenant, string state, string admin_consent);
}