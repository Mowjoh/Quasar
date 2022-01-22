using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Workshop.Web
{
    public static class Downloader
    {
        public static async Task<int> DownloadScreenshot(string ScreenshotURL, string ModGuid, string QuasarDataPath)
        {
            string downloadextension = ScreenshotURL.Split('.')[ScreenshotURL.Split('.').Length - 1];

            string imageSource = QuasarDataPath + @"\Library\Screenshots\" + ModGuid + "." + downloadextension;
            string wimageSource = QuasarDataPath + @"\Library\Screenshots\" + ModGuid + ".webp";

            WebClient client = new WebClient();

            using (WebClient cli = new WebClient())
            {
                int tries = 0;
                while(tries < 3)
                try
                {
                    await Task.Run(() => cli.DownloadFile(new Uri(ScreenshotURL), imageSource));
                    tries = 3;
                }
                catch (Exception e)
                {
                    tries++;
                }
                
            }


            if (downloadextension != "webp")
            {
                try
                {
                    using (FileStream fsSource = new FileStream(imageSource, FileMode.Open, FileAccess.Read))
                    {
                        byte[] ImageData = new byte[fsSource.Length];
                        fsSource.Read(ImageData, 0, ImageData.Length);

                        using (var webPFileStream = new FileStream(wimageSource, FileMode.Create))
                        {
                            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                            {

                                imageFactory.Load(ImageData)
                                            .Format(new WebPFormat())
                                            .Quality(100)
                                            .Save(webPFileStream);
                            }
                        }
                    }



                    if (File.Exists(wimageSource))
                        File.Delete(imageSource);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            return 0;
        }
    }
}
