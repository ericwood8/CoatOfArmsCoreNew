using System.Drawing.Imaging;

namespace CoatOfArmsCore
{
    public static class FileExtensionToolbox
    {
        /// <summary> IsJpeg file? </summary>
        public static bool IsJpg(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return GetExtension(fileName).Equals("jpg", StringComparison.CurrentCultureIgnoreCase) ||
                   GetExtension(fileName).Equals("jpeg", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary> Returns a special pipe-delimited string of formats and extensions. 
        ///    Has current one at top, because assumes if you used PNG that you will continue to use PNG. </summary>
        public static string GetImageFilterWithCurrentAtTop(string fileName, out string extension)
        {
            extension = GetExtension(fileName, false);
            return GetImageSaveFilterWithCurrentAtTop(extension);
        }

        /// <summary> Given file extension calc format of image </summary>
        public static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"Unable to determine file extension for fileName: {fileName}");
            }

            return extension.ToLower() switch
            {
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".ico" => ImageFormat.Icon,
                ".wmf" => ImageFormat.Wmf,
                ".png" => ImageFormat.Png,
                ".exif" => ImageFormat.Exif,
                ".emf" => ImageFormat.Emf,
                ".jpg" => ImageFormat.Jpeg,
                ".jpeg" => ImageFormat.Jpeg,
                ".tif" => ImageFormat.Tiff,
                ".tiff" => ImageFormat.Tiff,
                _ => throw new NotImplementedException()
            };
        }

        #region Privates
        /// <summary> Gets the extension (to know how to limit the save option to the current image file type).
        ///           Extension will be all upper or lower depending on parameter flag.</summary> 
        private static string GetExtension(string currentFile, bool toUpper = false)
        {
            if (currentFile == null)
            {
                return "png"; // default to png since it allows opacity
            }
            string ext = Path.GetExtension(currentFile);
            return PullOffLeadingPeriod(toUpper ? ext.ToUpper() : ext.ToLower());
        }

        /// <summary> Want current extension to be on top. Want list of other image files, except for the current extension. </summary> 
        private static string GetImageSaveFilterWithCurrentAtTop(string strExt)
        {
            string filter = $"{strExt} Image File|*.{strExt}"; // current extension at top
            if (strExt.NotEqual("BMP"))
            {
                filter += "|Bitmap Files|*.bmp";
            }

            if (strExt.NotEqual("EMF")) // don't use ELSE
            {
                filter += "|Enhanced Windows MetaFile|*.emf";
            }

            if (strExt.NotEqual("EXIF"))
            {
                filter += "|Exchangeable Image File|*.exif";
            }

            if (strExt.NotEqual("GIF"))
            {
                filter += "|Gif Files|*.gif";
            }

            if (strExt.NotEqual("JPG"))
            {
                filter += "|JPEG Files|*.jpg";
            }

            if (strExt.NotEqual("PNG"))
            {
                filter += "|PNG Files|*.png";
            }

            if (strExt.NotEqual("TIF"))
            {
                filter += "|TIFF Files|*.tif";
            }

            if (strExt.NotEqual("WMF"))
            {
                filter += "|Windows MetaFile|*.wmf";
            }

            return filter;
        }

        /// <summary> REMOVE pulls off leading period </summary> 
        private static string PullOffLeadingPeriod(string s)
        {
            return s.Remove(0, 1);
        }
        #endregion
    }
}
