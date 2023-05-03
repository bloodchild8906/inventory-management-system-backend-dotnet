using Inventory.Application.Interfaces.Service;

namespace Inventory.Application.Infrastructure.Service
{
    public class PathComposer : IPathComposer
    {
        public string ComposeCompanyLogoDirectory()
        {
            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Settings"));
        }

        public string ComposeCompanyLogoPath(string fileName)
        {
            var path = ComposeCompanyLogoDirectory();
            return Path.Combine(path, fileName);
        }

        public string ComposeProfilePicturePath(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
