using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Revolutions.Library.Helpers
{
    public static class FileHelper
    {
        public static void Save<T>(T data, string directoryPath, string fileName)
        {
            try
            {
                var securityRules = new DirectorySecurity();
                securityRules.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), FileSystemRights.FullControl, AccessControlType.Allow));

                Directory.CreateDirectory(directoryPath, securityRules);
                var filePath = Path.Combine(directoryPath, $"{fileName}.xml");

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(fileStream, data);
                }
            }
            catch (Exception)
            {
            }
        }

        public static T Load<T>(string directoryPath, string fileName)
        {
            try
            {
                var filePath = Path.Combine(directoryPath, $"{fileName}.xml");
                if (!File.Exists(filePath))
                {
                    Save<T>(default, directoryPath, fileName);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(fileStream);
                }
            }
            catch (Exception)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }
}