using DigitalLibrary.Core.Services.Intefaces;

namespace DigitalLibrary.Core.Services
{
    public class DefaultDirectoryProvider : IDirectoryProvider
    {
        public string GetDataDirectory()
        {
            var envPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(envPath, "BookApp");
            Directory.CreateDirectory(dir); 
            return dir;
        }
    }
}
