namespace DDS.Net.Connector.Helpers
{
    public static class ExtFilePath
    {
        /// <summary>
        /// Returns trimmed parts of provided string split by given character.
        /// </summary>
        /// <param name="str">The string to be split.</param>
        /// <param name="splitter">The letter on which the string is to be split.</param>
        /// <returns>List of parts - non-null</returns>
        internal static List<string> TrimmedParts(this string str, char splitter)
        {
            if (string.IsNullOrEmpty(str) == false)
            {
                string[] parts = str.Split(splitter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    return new List<string>(parts);
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Creates folder hierarchy for the file name provided.
        /// </summary>
        /// <param name="filename">Path of the file.</param>
        public static void CreateFoldersForRelativeFilename(this string filename)
        {
            if (string.IsNullOrEmpty(filename)) return;

            string[] folders = filename.Split(
                new char[] { '\\', '/' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (folders.Length > 1)
            {
                string foldername = "";

                for (int i = 0; i < folders.Length - 1; i++)
                {
                    if (foldername == "")
                    {
                        foldername = folders[i];
                    }
                    else
                    {
                        foldername = $"{foldername}{Path.DirectorySeparatorChar}{folders[i]}";
                    }
                }

                Directory.CreateDirectory(foldername);
            }
        }
    }
}
