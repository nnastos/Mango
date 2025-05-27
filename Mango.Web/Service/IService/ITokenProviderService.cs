namespace Mango.Web.Service.IService
{
    public interface ITokenProviderService
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
    }
}
