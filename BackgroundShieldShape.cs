using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CoatOfArmsCore
{
    internal class BackgroundShieldShape
    {
        public static Image AddShieldToImage(Image pattern, Image shieldShapeImage)
        {
            // resize Pattern because many shield shapes do not conform to uniform white space border
            Bitmap patternBitmap = ResizeImage(pattern, shieldShapeImage.Width, shieldShapeImage.Height);

            // overlay shield shape
            return FrameImage(shieldShapeImage, patternBitmap, true, new Point(0, 0));
        }

        #region Privates
        /// <summary> High quality resize of the image to the specified width and height.
        /// https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }

        /// <summary> Creates composite picture of frame with interior picture.  
        ///    Assumes:
        ///      1) frame is larger than picture inside
        ///      2) the frame might chop off some of the interior picture.
        ///      3) frames will have opacity in the center.
        ///      4) the interior picture is to be centered.
        /// </summary>
        /// <returns> Returns the composite image. </returns>
        private static Image FrameImage(Image frame, Image picture, bool shouldCenter, Point startPicture)
        {
            startPicture = (shouldCenter) ? CoordinatesToolbox.CalcCenterPoint(frame, picture) : startPicture;
            Image newCompositeImage = new Bitmap(frame.Width, frame.Height);

            using (Graphics gr = Graphics.FromImage(newCompositeImage))
            {
                gr.DrawImage(picture, startPicture);  // start with picture inside
                gr.DrawImage(frame, CoordinatesToolbox.TopLeftCorner()); // draw frame over (assuming that it has opacity) 
            }

            return newCompositeImage;
        }
        #endregion
    }
}
