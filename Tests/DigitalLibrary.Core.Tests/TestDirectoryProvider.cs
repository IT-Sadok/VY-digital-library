using DigitalLibrary.Core.Services.Intefaces;

namespace DigitalLibrary.Core.Tests
{
    public class TestDirectoryProvider : IDirectoryProvider
    {
        private readonly string _testDirectory;

        public TestDirectoryProvider()
        {
            _testDirectory = Path.Combine(Environment.CurrentDirectory, "TestData");
            Directory.CreateDirectory(_testDirectory);
        }

        public string GetDataDirectory()
        {
            return _testDirectory;
        }
    }


}
