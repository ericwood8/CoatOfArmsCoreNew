using System.Drawing.Imaging;

namespace CoatOfArmsCore
{
    /// <summary> Image action routines, but no dialogs except errors. </summary>
    public static class ImageActionToolbox
    {
        /// <summary> Gets an image from a file.  </summary> 
        /// <param name="fileName">File name </param> 
        /// <returns>Returns the image OR null if cancelled or aborted </returns>
        internal static Image GetImage(string fileName)
        {
            return Image.FromFile(fileName);
        }

        /// <summary> Saves an image to a file.  </summary>
        /// <param name="imageToSave">Image to Save </param>
        /// <param name="newFileName">New file name </param> 
        /// <returns>Returns the image OR null if cancelled or aborted </returns>
        internal static void SaveImage(Image imageToSave, string newFileName)
        {
            ImageFormat format = FileExtensionToolbox.GetImageFormat(newFileName); 

            try
            {
                if (format.Equals(ImageFormat.Jpeg)) // we need to know if JPG file, because it has special save rules.
                {
                    imageToSave.Save(newFileName, BuildJpgImageCodecInfo(), BuildSpecial75JpgEncoderParameters());
                }
                else
                {
                    imageToSave.Save(newFileName, format);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to save image to {ImageFormatName(format)} format. Error: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Image file saved to {newFileName}", "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary> Determines a copyright string to put on the bottom of images.  </summary> 
        /// <param name="companyName">Your name </param> 
        /// <returns>Returns the copyright string </returns>
        internal static string WatermarkCopyright(string companyName = "Your Name")
        {
            return $"{companyName} {char.ConvertFromUtf32(169)} {DateTime.Now.Year}, All Rights Reserved";
        }

        internal static bool SetWatermarkFont(ref Font currentWatermarkFont, ref Color currentWatermarkColor)
        {
            DialogResult dr = ImageDialogToolbox.ShowFontDialog(ref currentWatermarkFont, ref currentWatermarkColor);
            return dr != DialogResult.Cancel;
        }

        /// <summary> Draws Watermark on screen at X and Y position </summary>
        /// <param name="image">Image to watermark </param>
        /// <param name="atTopFlag">Put watermark on top?</param>
        /// <param name="watermarkText"> text of the watermark </param>
        /// <param name="watermarkFont"> font of the watermark </param>
        /// <param name="watermarkColor"> color of the watermark </param>
        /// <param name="containerTop"> the container's top Y coordinate </param>
        /// <param name="opacityChoice"> enumeration of opacity values </param>
        internal static void DrawWatermark(Image image, bool atTopFlag, string watermarkText, Font watermarkFont,
            Color watermarkColor, int containerTop, string opacityChoice)
        {
            Point startPointOfWatermark = CoordinatesToolbox.CalcStartTextPosition(image, atTopFlag, watermarkText, watermarkFont, containerTop);

            int opacity = CalcOpacity(opacityChoice);
            SolidBrush myBrush = BuildWatermarkBrush(opacity, watermarkColor);

            // Get a graphics context
            Graphics g = Graphics.FromImage(image);
            // draw the water mark text on the screen at the X and Y position
            g.DrawString(watermarkText, watermarkFont, myBrush, startPointOfWatermark);
        }

        /// <summary> Takes existing image and overlays image over top in the center </summary>
        /// <param name="currentImage">Existing Image </param>
        /// <param name="dir">Default directory to look for overlay image </param>
        /// <returns>True if overlay happened, False if error.</returns>
        internal static bool OverlayChosenImage(ref Image currentImage, ref string dir)
        {
            // ASSUMES the current image is the background and the image you are opening is the overlay image. 
            string overlayFile = "";
            Image? imageOverlay = ImageDialogToolbox.ShowOpenImageDialog(ref dir, ref overlayFile);

            if (imageOverlay == null)
            {
                return false; // if the user did not select a file, return  
            }

            currentImage = Overlay(currentImage, imageOverlay);

            return true;
        }

        internal static bool UnderlayImage(ref Image currentImage, ref string dir)
        {
            throw new NotImplementedException();
        }

        /// <summary> Switches all pixels of one chosen color to another.
        ///    Warning - Transparent color areas will look like background color, but have a different ARGB from their solid cousins. </summary>
        /// <param name="currentImage">Image</param> 
        internal static Image ChooseToSwitchColor(Image image)
        {
            Color oldColor = HeraldryColors.SolidBlack(); // default to solid black

            DialogResult drOld = ImageDialogToolbox.ShowColorDialogWithTitle("Pick Old Color To Switch", ref oldColor);
            if (drOld == DialogResult.Cancel)
                return image;

            Color newColor = HeraldryColors.SolidWhite(); // default to solid white

            DialogResult drNew = ImageDialogToolbox.ShowColorDialogWithTitle("Pick New Color", ref newColor);
            if (drNew == DialogResult.Cancel)
                return image;

            return HeraldryColors.SwitchColor(image, oldColor, newColor);
        }

        internal static Image BuildCoatOfArms()
        {
            const string programPath = @"C:\EricStuff\Computer\MyGame\CoatOfArmsCoreNew\";

            // randomly pick shape from directory
            Image shapeImage = PickImageFromPngs(programPath + "ShieldShapeFiles");

            // randomly pick pattern file from directory (include solid)
            string ordinaryFile = FileToolbox.PickFromPngs(programPath + "OrdinaryFiles");
            bool isSolid = FileToolbox.IsSolid(ordinaryFile);
            Image pattern = GetImage(ordinaryFile);
            // the pattern needs to have 2 different colors to be visible
            Color nonWhite = HeraldryColors.PickNonWhite();
            Color different = HeraldryColors.PickDifferent(nonWhite);
            
            Image coloredPattern = HeraldryColors.ColorThePattern(isSolid, nonWhite, different, pattern);

            // merge shield and colored pattern
            Image tempComposite = BackgroundShieldShape.AddShieldToImage(coloredPattern, shapeImage);

            // randomly pick symbol from directory 
            Image symbolImage = PickImageFromPngs(programPath + "ChargesFiles");

            if (symbolImage == null)
            {
                return tempComposite; // if blank or no symbol, just use what exists so far
            }

            // ASSUME transparent background on symbols (a.k.a. charges)
            Color symbolColor = HeraldryColors.SymbolColor(nonWhite, different);
            Image coloredSymbol = HeraldryColors.BlackToColor(symbolImage, symbolColor); // exclude other colors so symbol will be visible

            // merge symbol with composite
            return Overlay(tempComposite, coloredSymbol);
        }

        #region Privates
        public static Image PickImageFromPngs(string directory)
        {
            string fileName = FileToolbox.PickFromPngs(directory);
            return GetImage(fileName);
        }

        /// <summary> Creates composite picture overlaying smaller on bigger. </summary> 
        /// <returns> Returns the composite image. </returns>
        private static Image Overlay(Image largerBackground, Image smallerOverlay)
        {
            Point pointSoCentered = new Point(CoordinatesToolbox.CalcCenter(largerBackground.Width, smallerOverlay.Width), CoordinatesToolbox.CalcCenter(largerBackground.Height, smallerOverlay.Height));

            Image newCompositeImage = new Bitmap(largerBackground.Width, largerBackground.Height);

            using (Graphics gr = Graphics.FromImage(newCompositeImage))
            {
                gr.DrawImage(largerBackground, CoordinatesToolbox.TopLeftCorner()); // draw larger background
                gr.DrawImage(smallerOverlay, pointSoCentered); // overlay smaller 
            }

            return newCompositeImage;
        }

        /// <summary> Given format, calc pretty name </summary>
        private static string ImageFormatName(ImageFormat format)
        {
            if (format.Equals(ImageFormat.Bmp))
                return "BMP";
            else if (format.Equals(ImageFormat.Gif))
                return "GIF";
            else if (format.Equals(ImageFormat.Icon))
                return "Icon";
            else if (format.Equals(ImageFormat.Wmf))
                return "WMF";
            else if (format.Equals(ImageFormat.Png))
                return "PNG";
            else if (format.Equals(ImageFormat.Exif))
                return "EXIF";
            else if (format.Equals(ImageFormat.Emf))
                return "EMF";
            else if (format.Equals(ImageFormat.Jpeg))
                return "Jpeg";
            else if (format.Equals(ImageFormat.Tiff))
                return "Tiff";
            else
                throw new NotImplementedException();
        }

        /// <summary> Get an ImageCodecInfo object that represents the JPEG codec. </summary> 
        private static ImageCodecInfo BuildJpgImageCodecInfo()
        {
            return GetEncoderInfo("image/jpeg");
        }

        /// <summary> Save the bitmap as a JPEG file with quality level 75. </summary> 
        private static EncoderParameters BuildSpecial75JpgEncoderParameters()
        {
            return new EncoderParameters(1) { Param = { [0] = new EncoderParameter(Encoder.Quality, 75L) } };
        }

        /// <summary> Return the available image encoders </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().First(x => x.MimeType.Equals(mimeType));
        }

        /// <summary> Returns the opacity of the watermark </summary>
        private static int CalcOpacity(string sOpacity)
        {
            return sOpacity switch
            {
                "100%" => 255, // 1 * 255 = fully opaque (solid)
                "75%" => 191, // .75 * 255 
                "50%" => 127, // .5 * 255 
                "25%" => 64, // .25 * 255
                "10%" => 25, // .10 * 255 
                _ => 127
            };
        }

        /// <summary> Create a solid brush to write the watermark text on the image </summary> 
        private static SolidBrush BuildWatermarkBrush(int opacity, Color watermarkColor)
        {
            return new SolidBrush(Color.FromArgb(opacity, watermarkColor));
        }
        #endregion
    }
}
