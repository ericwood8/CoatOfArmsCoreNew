namespace CoatOfArmsCore
{
    public static class FileToolbox
    {
        public static string PickFromPngs(string directory)
        {
            string sFile = PickPng(directory);
            if (string.IsNullOrWhiteSpace(sFile))
            {
                throw new Exception($"No PNG images found in directory {directory}");
            }
            return sFile;
        }

        public static bool IsSolid(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            return IsFileName(filePath, "Solid.png");
        }

        public static bool HasNoSymbol(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            return IsFileName(filePath, "blank.png");
        }

        #region private
        private static bool IsFileName(string sFile, string criteriaFileName)
        {
            if (string.IsNullOrEmpty(sFile) || string.IsNullOrEmpty(criteriaFileName))
                return false;
            return new FileInfo(sFile).Name.Equals(criteriaFileName);
        }

        private static string PickPng(string directory)
        {
            Random rand = new Random();
            string[] files = Directory.GetFiles(directory, "*.png");
            return files[rand.Next(files.Length)];
        }
        #endregion
    }
}
