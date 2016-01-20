using System.Runtime.InteropServices;
using System.Text;

namespace Agent
{
    public class ProgramFinder
    {
        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        public static extern long FindExecutableA(
          string lpFile, string lpDirectory, StringBuilder lpResult);

        public static string FindExecutable(
          string pv_strFilename)
        {
            StringBuilder objResultBuffer =
              new StringBuilder(1024);
            long lngResult = 0;

            lngResult =
              FindExecutableA(pv_strFilename,
                string.Empty, objResultBuffer);

            if (lngResult >= 32)
            {
                return objResultBuffer.ToString();
            }

            return string.Format(
              "Error: ({0})", lngResult);
        }
    }
}
