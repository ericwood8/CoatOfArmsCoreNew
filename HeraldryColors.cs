using System.Collections.Generic;
using System.Drawing.Imaging;

namespace CoatOfArmsCore
{
    /// <summary> Understands the valid and limited tinctures (a.k.a. colors) in Heraldry </summary>
    public static class HeraldryColors
    {
        public static Image BlackToColor(Image image, Color color)
        {
            return SwitchColor(image, SolidBlack(), color);
        }

        public static Color PickNonWhite()
        {
            return PickHeraldryTincture([SolidWhite()]);
        }

        public static Color PickDifferent(Color nonWhite)
        {
            return PickHeraldryTincture([nonWhite]);
        }

        public static Color SymbolColor(Color nonWhite, Color different)
        {
            return PickHeraldryTincture([nonWhite, different]);
        }

        public static Color SolidBlack()
        {
            return Color.FromArgb(255, 0, 0, 0); // black = Sable in medieval heraldry
        }

        public static Color SolidWhite()
        {
            return Color.FromArgb(255, 255, 255, 255);
        }

        /// <summary> Switches all pixels of one chosen color to another.  </summary>
        /// <param name="image">Image</param>
        /// <param name="oldColor">Old Color in ARGB format </param>
        /// <param name="newColor">New Color in ARGB format </param>
        public static Image SwitchColor(Image image, Color oldColor, Color newColor)
        {
            if (oldColor.Name.Equals(newColor.Name))
            {
                // do nothing, because no real color change
                return image;
            }

            ColorMap[] remapTable = [new ColorMap { OldColor = oldColor, NewColor = newColor }];

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            using (Graphics gr = Graphics.FromImage(image))
            {
                gr.DrawImage(
                    image,
                    new Rectangle(0, 0, image.Width, image.Height), // destination rectangle
                    0, 0, // upper-left corner of source rectangle
                    image.Width, // width of source rectangle
                    image.Height, // height of source rectangle 
                    GraphicsUnit.Pixel,
                    imageAttributes);
            }

            return image; // return adjusted image
        }

        public static Image ColorThePattern(bool isSolidBackground, Color nonWhite, Color different, Image image)
        {
            // switch black part of pattern to non-white
            // prevent solid white because of next step is going to switch white to something else
            Image coloredPattern = BlackToColor(image, nonWhite);

            if (!isSolidBackground)
            {
                // switch white part of pattern white to random color
                return WhiteToColor(coloredPattern, different);
            }

            return coloredPattern;
        }

        #region Privates
        private static Image WhiteToColor(Image image, Color color)
        {
            return SwitchColor(image, SolidWhite(), color);
        }

        private static Color Murrey()
        {
            return Color.FromArgb(255, 197, 75, 140); // mulberry
        }

        private static Color Sanguine()
        {
            return Color.FromArgb(255, 178, 34, 34); // blood red/brick red
        }

        private static Color Or()
        {
            return Color.FromArgb(255, 255, 215, 0); // gold
        }

        private static Color Gules()
        {
            return Color.FromArgb(255, 255, 0, 0); // red
        }

        private static Color Azure()
        {
            return Color.FromArgb(255, 0, 0, 255); // blue
        }

        private static Color Vert()
        {
            return Color.FromArgb(255, 0, 128, 0); // green
        }

        private static Color Purpure()
        {
            return Color.FromArgb(255, 128, 0, 128); // purple
        }

        private static Color Tenne()
        {
            return Color.FromArgb(255, 205, 87, 0); // tawny orange
        }


        /// <summary> Picks a random Tincture from Heraldry.
        ///           Continue picking until find a color not duplicated, so adjacent colors are different. </summary>
        /// <param name="excludeColors">List of colors used so far. Empty list is fine for no exclusions. </param>
        /// <returns> Return random color from the limited list of tinctures from heraldry. </returns>
        public static Color PickHeraldryTincture(List<Color> excludeColors)
        {
            Random rand = new Random();

            // Continue picking until find a color not duplicated so adjacent colors are different
            do
            {
                int r = rand.Next(1, 100);
                if (r < 10)
                {
                    if (!excludeColors.Contains(Azure()))
                    {
                        return Azure();
                    }
                }
                else if (r < 20)
                {
                    if (!excludeColors.Contains(Gules()))
                    {
                        return Gules();
                    }
                }
                else if (r < 30)
                {
                    if (!excludeColors.Contains(Murrey()))
                    {
                        return Murrey();
                    }
                }
                else if (r < 40)
                {
                    if (!excludeColors.Contains(Sanguine()))
                    {
                        return Sanguine();
                    }
                }
                else if (r < 50)
                {
                    if (!excludeColors.Contains(Or()))
                    {
                        return Or();
                    }
                }
                else if (r < 60)
                {
                    if (!excludeColors.Contains(Vert()))
                    {
                        return Vert();
                    }
                }
                else if (r < 70)
                {
                    if (!excludeColors.Contains(Purpure()))
                    {
                        return Purpure();
                    }
                }
                else if (r < 80)
                {
                    if (!excludeColors.Contains(Tenne()))
                    {
                        return Tenne();
                    }
                }
                else if (r < 90)
                {
                    if (!excludeColors.Contains(SolidWhite()))
                    {
                        return SolidWhite();
                    }
                }
                else
                {
                    if (!excludeColors.Contains(SolidBlack()))
                    {
                        return SolidBlack();
                    }
                }
            } while (true);
        }
        #endregion
    }
}
