using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
