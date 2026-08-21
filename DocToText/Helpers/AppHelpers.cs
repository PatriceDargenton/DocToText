
using System.Diagnostics; // Process
using System.Security.Principal; // WindowsIdentity

namespace DocToText.Helpers
{
    public static class AppHelpers
    {
        /// <summary>
        /// Gets the application title set during initialization.
        /// </summary>
        public static string? AppTitle { get; private set; }

        /// <summary>
        /// Sets the application title.
        /// </summary>
        /// <param name="title">The title to set for the application.</param>
        public static void SetAppTitle(string title) => AppTitle = title;
        
        /// <summary>
        /// Gets the application version set during initialization.
        /// </summary>
        public static string? AppVersion { get; private set; }
        
        /// <summary>
        /// Sets the application version.
        /// </summary>
        /// <param name="version">The version string to set for the application.</param>
        public static void SetAppVersion(string version) => AppVersion = version;

        /// <summary>
        /// Displays a formatted error message dialog and optionally copies the error details to the clipboard.
        /// </summary>
        /// <param name="ex">The exception containing error information.</param>
        /// <param name="finalErrorMessage">Output parameter that receives the formatted error message.</param>
        /// <param name="functionTitle">Optional title of the function where the error occurred.</param>
        /// <param name="info">Optional additional information about the error context.</param>
        /// <param name="errorDetail">Optional detailed error information.</param>
        /// <param name="copyToClipboard">Indicates whether to copy the error message to the clipboard (default: true).</param>
        public static void ShowError(Exception ex,
            ref string finalErrorMessage,
            string functionTitle = "",
            string info = "",
            string errorDetail = "",
            bool copyToClipboard = true)
        {
            if (!Cursors.Default.Equals(Cursor.Current))
                Cursor.Current = Cursors.Default;

            string msg = "";
            const string newlineCRLF = "\r\n"; // Carriage Return + Line Feed: Environment.NewLine
            if (!string.IsNullOrEmpty(functionTitle))
                msg = "Function: " + functionTitle;
            if (!string.IsNullOrEmpty(info))
                msg += newlineCRLF + info;
            if (!string.IsNullOrEmpty(errorDetail))
                msg += newlineCRLF + errorDetail;
            if (!string.IsNullOrEmpty(ex.Message))
            {
                msg += newlineCRLF + ex.Message.Trim();
                if (ex.InnerException != null)
                    msg += newlineCRLF + ex.InnerException.Message;
            }

            if (copyToClipboard)
                CopyToClipboard(msg);

            finalErrorMessage = msg;
            MessageBox.Show(msg, AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Displays a formatted error message dialog and optionally copies the error details to the clipboard.
        /// </summary>
        /// <param name="ex">The exception containing error information.</param>
        /// <param name="functionTitle">Optional title of the function where the error occurred.</param>
        /// <param name="info">Optional additional information about the error context.</param>
        /// <param name="errorDetail">Optional detailed error information.</param>
        /// <param name="copyToClipboard">Indicates whether to copy the error message to the clipboard (default: true).</param>
        public static void ShowError(Exception ex,
            string functionTitle = "",
            string info = "",
            string errorDetail = "",
            bool copyToClipboard = true)
        {
            string unused = "";
            ShowError(ex, ref unused, functionTitle, info, errorDetail, copyToClipboard);
        }

        /// <summary>
        /// Sets the application cursor to indicate a wait state or resets it to the default cursor.
        /// </summary>
        /// <param name="deactivate">If true, resets the cursor to the default; if false, sets the wait/hourglass cursor (default: false).</param>
        public static void SetWaitCursor(bool deactivate = false)
        {
            Cursor.Current = deactivate ? Cursors.Default : Cursors.WaitCursor;
        }

        /// <summary>
        /// Copies the specified text to the system clipboard.
        /// </summary>
        /// <param name="info">The text content to copy to the clipboard.</param>
        /// <remarks>
        /// If the clipboard operation fails, an error message is displayed to the user.
        /// </remarks>
        public static void CopyToClipboard(string info)
        {
            try
            {
                var dataObj = new DataObject();
                dataObj.SetData(DataFormats.Text, info);
                Clipboard.SetDataObject(dataObj);
            }
            catch (Exception ex)
            {
                ShowError(ex, "CopyToClipboard", copyToClipboard: false);
            }
        }

        /// <summary>
        /// Determines whether the current process is running with administrator privileges.
        /// </summary>
        /// <returns>True if running with administrator privileges; false otherwise.</returns>
        public static bool IsAdministrator()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Restarts the current application with administrator privileges using UAC elevation.
        /// </summary>
        /// <param name="errorMessage">Output parameter containing the error message if restart fails; otherwise empty.</param>
        /// <param name="showError">Indicates whether to display an error dialog if restart fails (default: true).</param>
        /// <returns>True if the restart was initiated successfully; false if it failed.</returns>
        /// <remarks>
        /// This method uses the Windows "runas" verb to elevate the application. The current process continues running after this call.
        /// </remarks>
        public static bool RestartAsAdministrator(out string errorMessage, 
            bool showError = true)
        {
            errorMessage = string.Empty;    
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                if (showError) ShowError(ex, "RestartAsAdministrator", copyToClipboard: false);
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converts a .doc or .odt document file to .docx format using LibreOffice in headless mode.
        /// </summary>
        /// <param name="inputFilePath">The full path to the input .doc or .odt file.</param>
        /// <param name="outputFilePath">The full path where the converted .docx file should be saved.</param>
        /// <returns>True if conversion succeeded; false if conversion failed or an error occurred.</returns>
        /// <remarks>
        /// This method requires LibreOffice to be installed and accessible at the path specified in Const.LibreOfficePath.
        /// The method runs LibreOffice in headless mode with no UI. If the conversion fails, an error message is displayed.
        /// </remarks>
        public static bool ConvertDocOrOdtToDocxUsingLibreOffice(string inputFilePath, string outputFilePath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Const.LibreOfficePath,
                    Arguments = $"--headless --convert-to docx \"{inputFilePath}\" --outdir \"{Path.GetDirectoryName(outputFilePath)}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(startInfo);
                process?.WaitForExit();

                if (process?.ExitCode != 0)
                {
                    MessageBox.Show(
                        $"Error during file conversion: {process?.StandardError.ReadToEnd()}", 
                        AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex, "ConvertDocToDocxUsingLibreOffice", copyToClipboard: false);
                return false;
            }
        }

        public static string? RemoveAfterParenthesis(string? input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            int index = input.IndexOf('(');
            if (index >= 0) return input.Substring(0, index).Trim();

            return input;
        }
    }
}