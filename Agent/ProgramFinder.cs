using System.Runtime.InteropServices;
using System.Text;

namespace Agent
{
    public class ProgramFinder
    {
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        private static extern long FindExecutableA(
          string lpFile, string lpDirectory, StringBuilder lpResult);

        /// <summary>
        /// Gets the path to the executable that is able to process the given file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>The absolute path to the executable or <see cref="string.Empty"/> if no executable is found or if the file is inaccessible.</returns>
        public static string FindExecutable(string filename)
        {
            StringBuilder objResultBuffer = new StringBuilder(1024);
            long result = 0;

            result = FindExecutableA(filename, string.Empty, objResultBuffer);

            if (result >= 32)
            {
                return objResultBuffer.ToString();
            }

            return string.Empty;
        }
    }
}
