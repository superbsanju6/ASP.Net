using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DocumentFormat.OpenXml.Office2010.Excel;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes
{
    internal class Rasterizer
    {
        private GhostscriptVersionInfo _lastInstalledVersion = null;
        private GhostscriptRasterizer _rasterizer = null;
        private List<string> imageNames;

        internal List<string> CreateImagesFromPDF(string fileName)
        {
            const int desired_x_dpi = 300;
            const int desired_y_dpi = 300;

            //string inputPdfPath = @"C:\Users\mkrue\Desktop\PDFImages\2nhruzqyeft.pdf";
            //string outputPath = @"C:\Users\mkrue\Desktop\PDFImages\";

            string outputPath = AppSettings.UploadFolderPhysicalPath;

            FileInfo fi = new FileInfo(fileName);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);


            _lastInstalledVersion =
                GhostscriptVersionInfo.GetLastInstalledVersion(
                        GhostscriptLicense.GPL | GhostscriptLicense.AFPL,
                        GhostscriptLicense.GPL);

            _rasterizer = new GhostscriptRasterizer();

            _rasterizer.Open(fi.FullName, _lastInstalledVersion, false);

            imageNames = new List<string>();

            //Create a jpeg for each page in the pdf.  File name will be PDF file name + page number
            for (int pageNumber = 1; pageNumber <= _rasterizer.PageCount; pageNumber++)
            {
                string pageFilePath = Path.Combine(outputPath, fileNameWithoutExt + "-" + pageNumber + ".jpeg");

                Image img = _rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);
                img.Save(pageFilePath, ImageFormat.Jpeg);
                imageNames.Add(fileNameWithoutExt + "-" + pageNumber + ".jpeg");
            }

            return imageNames;
        }
    }
  
}