using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Utils
{
    public static class GeneralClassification
    {
        /// <summary>
        /// Classifies competitors from given file
        /// </summary>
        /// <param name="path">
        ///     Path to file to read
        /// </param>
        /// Exceptions:
        /// <exception cref="System.ArgumentException">
        ///     path is a zero-length string, contains only white space, or contains one or more
        ///     invalid characters as defined by System.IO.Path.InvalidPathChars.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     path is null.
        /// </exception>
        /// <exception cref="System.IO.PathTooLongException">
        ///     The specified path, file name, or both exceed the system-defined maximum length.
        ///     For example, on Windows-based platforms, paths must be less than 248 characters,
        ///     and file names must be less than 260 characters.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///     The specified path is invalid (for example, it is on an unmapped drive).
        /// </exception>
        /// <exception cref="System.IO.IOException">
        ///     An I/O error occurred while opening the file.
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">
        ///     path specified a file that is read-only.-or- This operation is not supported
        ///     on the current platform.-or- path specified a directory.-or- The caller does
        ///     not have the required permission.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///     The file specified in path was not found.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     path is in an invalid format.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        ///     The caller does not have the required permission.
        /// </exception>
        public static void Classify(string path)
        {
            string[] allLines = File.ReadAllLines(path);

            foreach (var line in allLines)
            {
                ProcessLine(line);
            }
        }

        private static void ProcessLine(string line)
        {

        }
    }
}
