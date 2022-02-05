using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace ALT.CVL
{
  public static class KeyChecker
  {
    private const string ResolveKey = "ALTSYSTEM";
    private const string AdminSNLYS = "00325-80147-40016-AAOEM";
    private const string AdminSNjdheo = "00330-80000-00000-AA198";
    private const string AdminSNKMKim = "00326-10033-84917-AA207";
    private const string testRoom1 = "00326-10012-51510-AA778";
    private const string testRoom2 = "00330-52408-41533-AAOEM";
    private const string testRoom3 = "00326-10033-11236-AA309";
    private const string testRoom4 = "00331-10000-00001-AA672";
    private static string[] altCompBoards = new string[7]
    {
      "00325-80147-40016-AAOEM",
      "00330-80000-00000-AA198",
      "00326-10033-84917-AA207",
      "00326-10012-51510-AA778",
      "00330-52408-41533-AAOEM",
      "00326-10033-11236-AA309",
      "00331-10000-00001-AA672"
    };

    public static bool CheckKey(
      string filePath,
      string deviceSerialNumber,
      bool immediatelyShutdown = true)
    {
      foreach (ManagementObject managementObject in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get())
      {
        if (((IEnumerable<string>) KeyChecker.altCompBoards).Contains<string>(managementObject.GetPropertyValue("SerialNumber").ToString()))
          return true;
      }
      return KeyDescryptor.ReadLicenseKey(filePath, "ALTSYSTEM").Equals(deviceSerialNumber);
    }
  }
}
