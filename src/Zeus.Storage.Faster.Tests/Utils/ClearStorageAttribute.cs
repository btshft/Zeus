using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace Zeus.Storage.Faster.Tests.Utils
{
    public class ClearStorageAttribute : BeforeAfterTestAttribute
    {
        private readonly string _path;

        public ClearStorageAttribute(string path)
        {
            _path = path;
        }

        /// <inheritdoc />
        public override void After(MethodInfo methodUnderTest)
        {
            if (Directory.Exists(_path))
                Directory.Delete(_path, recursive: true);
        }
    }
}