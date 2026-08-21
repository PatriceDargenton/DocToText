
using Microsoft.Win32;
using System.Diagnostics;

namespace DocToText.Helpers
{
    public static class RegistryHelper
    {
        /// <summary>
        /// Checks whether a context menu command is registered in the Windows registry for a specified file type.
        /// </summary>
        /// <param name="extName">The registry path for the file extension (e.g., "SystemFileAssociations\\.docx\\shell").</param>
        /// <param name="keyName">The context menu command key name to check (e.g., "DocToText.ConvertToText").</param>
        /// <returns>True if the menu registry entry exists; false otherwise.</returns>
        public static bool MenuExists(string extName, string keyName)
        {
            using RegistryKey? key =
                Registry.ClassesRoot.OpenSubKey($@"{extName}\{keyName}");

            return key != null;
        }

        /// <summary>
        /// Creates or registers a context menu command in the Windows registry for a specified file type.
        /// </summary>
        /// <param name="extName">The registry path for the file extension (e.g., "SystemFileAssociations\\.docx\\shell").</param>
        /// <param name="keyName">The context menu command key name to create (e.g., "DocToText.ConvertToText").</param>
        /// <param name="menuText">The display text for the context menu item.</param>
        /// <param name="command">The command to execute when the menu item is selected, typically the executable path with arguments.</param>
        /// <remarks>
        /// This method creates a new registry subkey under the specified extension path with the provided key name,
        /// sets the menu display text, and creates a "command" subkey with the executable command.
        /// </remarks>
        public static void CreateMenu(
            string extName, string keyName, string menuText, string command)
        {
            using RegistryKey shell =
                Registry.ClassesRoot.CreateSubKey($@"{extName}\{keyName}");

            shell.SetValue(name: null, menuText);

            using RegistryKey cmd = shell.CreateSubKey("command");
            cmd.SetValue(name: null, command);
        }

        /// <summary>
        /// Removes (unregisters) a context menu command from the Windows registry for a specified file type.
        /// </summary>
        /// <param name="extName">The registry path for the file extension (e.g., "SystemFileAssociations\\.docx\\shell").</param>
        /// <param name="keyName">The context menu command key name to delete (e.g., "DocToText.ConvertToText").</param>
        /// <remarks>
        /// This method silently deletes the registry subkey tree without throwing an exception if the key does not exist.
        /// </remarks>
        public static void DeleteMenu(string extName, string keyName)
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(
                    $@"{extName}\{keyName}",
                    throwOnMissingSubKey: false);
            }
            catch (Exception ex)
            {
                // Key does not exist (example: a new menu item was added before registration?)
                Debug.WriteLine($"Registry key '{extName}\\{keyName}' does not exist or could not be deleted: {ex.Message}");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RegistryKeyAttribute : Attribute
    {
        public string Key { get; }

        public RegistryKeyAttribute(string key)
        {
            Key = key;
        }
    }
}