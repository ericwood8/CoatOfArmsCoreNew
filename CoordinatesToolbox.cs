namespace CoatOfArmsCore
{
    /// <summary> Used for calculating coordinates on images </summary>
    public static class CoordinatesToolbox
    {
        public static Point CalcCenterPoint(Image frame, Image picture)
        {
            return new Point(CalcCenter(frame.Width, picture.Width), CalcCenter(frame.Height, picture.Height));
        }

        /// <summary> if 110 tall and put 50 tall inside, we want to start the inside at position 30 so have 30 at top and 30 on bottom </summary>
        /// <returns> Position to start drawing </returns>
        public static int CalcCenter(int max, int min)
        {
            if (min > max)
            {
                throw new ArgumentException("CalcCenter: Min is greater than Max.");
            }

            return (max.Equals(min)) ? 0 : (max - min) / 2;
        }

        /// <summary> Calculate X and Y coordinates for watermark whether on top or bottom </summary>
        public static Point CalcStartTextPosition(Image image, bool atTopFlag, string watermarkText,
            Font watermarkFont, int containerTop)
        {
            // Get a graphics context
            Graphics g = Graphics.FromImage(image);

            // Calculate the size of the text
            SizeF size = g.MeasureString(watermarkText, watermarkFont);

            // Set the drawing position based on the users selection of placing the text at the bottom or top of the image
            int x = (int)(image.Width - size.Width) / 2;
            int y = atTopFlag ? (int)(containerTop + size.Height) / 2 : (int)(image.Height - size.Height);

            return new Point(x, y);
        }

        public static Point TopLeftCorner()
        {
            return new Point(0, 0);
        }
    }
}
