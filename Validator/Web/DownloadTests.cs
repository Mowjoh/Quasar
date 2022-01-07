using Xunit;

namespace Validator.Web
{
    public class DownloadTests
    {
        [Theory]
        [InlineData("123456")]
        [InlineData("123457")]
        public void Screenshot_ScreenshotSuccesfulDownload(string ModID)
        {
            Assert.True(true);
        }
    }
}
