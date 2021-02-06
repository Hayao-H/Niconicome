using NUnit.Framework;
using Niconicome.Models.Domain.Utils;

namespace NiconicomeTest.Utils
{
    [TestFixture]
    class IOUtilsUnitTest
    {
        [TestCase("テスト", "テスト(1)")]
        [TestCase("テスト(1)", "テスト(2)")]
        [TestCase("テスト(2)", "テスト(3)")]
        [TestCase("テスト(10)", "テスト(11)")]
        [TestCase("テスト(10).exe", "テスト(11).exe")]
        public void ファイル上書きの挙動(string input, string expected)
        {
            string newPath = IOUtils.CheclFileExistsAndReturnNewFilename(input, true);
            Assert.AreEqual(expected, newPath);
        }
    }
}
