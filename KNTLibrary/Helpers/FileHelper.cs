using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;
using TaleWorlds.Core;

namespace KNTLibrary.Helpers
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
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Could not save file '{fileName}'!", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));
            }
        }

        public static T Load<T>(string directoryPath, string fileName)
        {
            try
            {
                var filePath = Path.Combine(directoryPath, $"{fileName}.xml");
                if (!File.Exists(filePath))
                {
                    FileHelper.Save<T>(default, directoryPath, fileName);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(fileStream);
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Could not load file '{fileName}'! Using default model.", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));

                return (T)Activator.CreateInstance(typeof(T));
            }
        }

        public static T Load<T>(T data, string directoryPath, string fileName)
        {
            try
            {
                var filePath = Path.Combine(directoryPath, $"{fileName}.xml");
                if (!File.Exists(filePath))
                {
                    FileHelper.Save<T>(data, directoryPath, fileName);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(fileStream);
                }
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Could not load file '{fileName}'! Using default model.", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage(exception.ToString(), ColorHelper.Red));

                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }
}