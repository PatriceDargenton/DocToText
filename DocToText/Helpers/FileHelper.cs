
namespace DocToText.Helpers
{
    public static class FileHelper
    {
        const string newlineCRLF = "\r\n"; // Carriage Return + Line Feed: Environment.NewLine

        /// <summary>
        /// Checks whether a specified file exists on disk.
        /// </summary>
        /// <param name="filePath">The full path to the file to check.</param>
        /// <param name="prompt">If true and the file does not exist, displays an error message dialog (default: false).</param>
        /// <returns>True if the file exists; false otherwise.</returns>
        public static bool FileExists(string filePath, bool prompt = false)
        {
            bool exists = File.Exists(filePath);
            if (!exists && prompt)
                MessageBox.Show(
                    "Cannot find the file:" + newlineCRLF + filePath,
                    AppHelpers.AppTitle + " - File not found",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            return exists;
        }

        /// <summary>
        /// Determines whether a file is accessible and can be read or written.
        /// </summary>
        /// <param name="filePath">The full path to the file to check.</param>
        /// <param name="prompt">If true and an accessibility error occurs, displays an error dialog with details (default: false).</param>
        /// <param name="promptClose">If true and the file is not accessible for writing, displays a dialog asking the user to close the file (default: false).</param>
        /// <param name="missingOk">If true, treats a missing file as accessible (returns true); if false, requires the file to exist (default: false).</param>
        /// <param name="promptRetry">If true, includes a Retry button in the accessibility error dialog, allowing the user to retry (default: false).</param>
        /// <returns>True if the file is accessible; false if the file is not accessible or does not exist (unless missingOk is true).</returns>
        /// <remarks>
        /// This method attempts to open the file for reading to verify accessibility. If promptRetry is true and the user clicks Retry, the method retries the accessibility check.
        /// </remarks>
        public static bool IsFileAccessible(string filePath,
            bool prompt = false,
            bool promptClose = false,
            bool missingOk = false,
            bool promptRetry = false)
        {
Retry:
            if (missingOk)
            {
                if (!FileExists(filePath)) return true;
            }
            else
            {
                if (!FileExists(filePath, prompt)) return false;
            }

            DialogResult response = DialogResult.Cancel;
            try
            {
                using var fs = File.OpenRead(filePath);
                return true;
            }
            catch (Exception ex)
            {
                if (prompt)
                {
                    AppHelpers.ShowError(ex, "IsFileAccessible",
                        "Cannot access the file:" + newlineCRLF + filePath);
                }
                else if (promptClose)
                {
                    var buttons = promptRetry
                        ? MessageBoxButtons.RetryCancel
                        : MessageBoxButtons.OK;
                    string question = promptRetry ? newlineCRLF + "Do you want to retry?" : "";
                    response = MessageBox.Show(
                        "The file is not accessible for writing:" + newlineCRLF +
                        filePath + newlineCRLF +
                        "Please close it or change its protection attributes." +
                        question,
                        AppHelpers.AppTitle, buttons, MessageBoxIcon.Exclamation);
                }
            }

            if (response == DialogResult.Retry) goto Retry;
            return false;
        }
    }
}