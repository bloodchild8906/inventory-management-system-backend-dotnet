namespace Inventory.Application.Interfaces.Service
{
    public interface IPathComposer
    {
        string ComposeProfilePicturePath(string fileName);
        string ComposeCompanyLogoPath(string fileName);
        string ComposeCompanyLogoDirectory();
    }
}
