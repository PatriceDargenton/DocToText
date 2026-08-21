
using DocToText.Helpers;
using System.Reflection;
using System.Text;

namespace DocToText
{
    internal static class Program
    {

#if DEBUG
        /// <summary>
        /// Indicates whether the application is running in Debug configuration.
        /// </summary>
        public const bool IsDebug = true;
        /// <summary>
        /// Indicates whether the application is running in Release configuration.
        /// </summary>
        public const bool IsRelease = false;
#else
        /// <summary>
        /// Indicates whether the application is running in Debug configuration.
        /// </summary>
        public const bool IsDebug = false;
        /// <summary>
        /// Indicates whether the application is running in Release configuration.
        /// </summary>
        public const bool IsRelease = true;
#endif

        /// <summary>
        /// Gets the application name from the assembly.
        /// </summary>
        public static string AppTitle => GetAppName(); 
        /// <summary>
        /// Gets the application version from the assembly.
        /// </summary>
        public static string AppVersion => GetAppVersion();

        private static string GetAppName()
        {
            var asm = Assembly.GetExecutingAssembly();
            var asmName = asm?.GetName();
            string name = asmName?.Name ?? Const.AppTitle;
            return name;
        }

        private static string GetAppVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            var asmName = asm?.GetName();
            Version? v = asmName?.Version;
            var versionTxt = v?.Major + "." + v?.Minor + v?.Build;
            return versionTxt;
        }

        /// <summary>
        /// Gets the complete application name and version string including build configuration.
        /// </summary>
        /// <returns>A formatted string containing the application name, version, date, and description, with " - Debug" appended if running in debug mode.</returns>
        public static string GetFullAppNameAndVersion()
        {
            var fullName = $"{Const.AppTitle} - {Program.AppVersion} ({Const.DateVersion}) {Const.AppTitleDescription}";
            if (Program.IsDebug) fullName += " - Debug";
            return fullName;
        }

        /// <summary>
        /// The main entry point for the application.
        /// Initializes the application and either runs the GUI or processes command-line arguments for batch conversion.
        /// </summary>
        /// <param name="args">Optional command-line arguments for batch document conversion.</param>
        [STAThread]
        static void Main(string[] args)
        {
            AppHelpers.SetAppTitle(AppTitle);
            AppHelpers.SetAppVersion(AppVersion);

            // Support for legacy code pages (like Windows-1252) is not
            //  available by default in .NET 5 and later.
            if (Const.Use1252Encoding) 
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // To customize application configuration such as set high DPI settings or
            //  default font, see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (args.Length > 0)
            {
                RunArgumentMode(args);
                return;
            }

            Application.Run(new FrmDocToText());
        }

        private static void RunArgumentMode(string[] args)
        {
            try
            {
                if (!DocumentConverter.TryParseArguments(args, 
                    out var mode, out var filePath, out var errorMessage))
                {
                    MessageBox.Show(errorMessage, Const.AppTitle, 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var splashForm = new FrmConversionSplash(
                    () => DocumentConverter.Convert(filePath, mode));
                splashForm.ShowDialog();

                if (splashForm.ConversionException is not null)
                {
                    throw splashForm.ConversionException;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Conversion failed: {ex.Message}", Const.AppTitle, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}