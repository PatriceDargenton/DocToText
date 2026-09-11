
namespace DocToText
{
    /// <summary>
    /// Application-wide constants for the DocToText application.
    /// </summary>
    internal static class Const
    {
        #region Application Information

        /// <summary>
        /// The main application title.
        /// </summary>
        public const string AppTitle = "DocToText";

        /// <summary>
        /// The current version date of the application in dd/MM/yyyy format.
        /// </summary>
        public const string DateVersion = "11/09/2026";

        /// <summary>
        /// The subtitle describing the application's primary functionality.
        /// </summary>
        public const string AppTitleDescription = ": MS-Word conversion to plain text";

        #endregion

        #region UI Configuration

        /// <summary>
        /// Minimum duration in milliseconds for displaying the conversion splash screen.
        /// </summary>
        public const int ConversionSplashMinimumDisplayDurationMs = 2000; // 2 seconds

        #endregion

        #region File Extensions

        /// <summary>
        /// File extension for Markdown documents.
        /// </summary>
        public const string mdExtension = ".md";

        /// <summary>
        /// File extension for plain text documents.
        /// </summary>
        public const string txtExtension = ".txt";

        #endregion

        #region Feature Configuration

        /// <summary>
        /// Determines whether to use Windows-1252 encoding (msoEncodingWestern) 
        ///  for text files instead of UTF-8.
        /// Note: Using Windows-1252 will result in the loss of Unicode characters, if any.
        /// </summary>
        public const bool Use1252Encoding = false;

        /// <summary>
        /// Determines whether to use the NPOI library for document processing instead of DocSharp.Docx.
        /// Both libraries are compatible, but NPOI preserves all line breaks whereas DocSharp.Docx removes
        ///  some of them (the new EmptyLineBetweenParagraphs option does not yet work) and
        ///  and converts tables into text, unlike NPOI.
        /// </summary>
        public const bool UseNPOI = true;

        /// <summary>
        /// Determines whether to use LibreOffice for converting .doc and .odt files to .docx format.
        /// </summary>
        public const bool UseLibreOfficeDocToDocxConversion = true;

        /// <summary>
        /// Determines whether to use DocSharp.Binary for converting .doc files to .docx format.
        /// Sometimes it works, but it's not reliable.
        /// </summary>
        public const bool UseDocSharpBinaryDocToDocx = false;

        /// <summary>
        /// Determines whether to use a simplified text conversion method that ignores footnotes and endnotes.
        /// No more useful, because footnotes and endnotes are now exported whitout numbering.
        /// </summary>
        public const bool UseTextSimple = false;


        /// <summary>
        /// Numbered footnotes and endnotes can make it difficult to compare versions of a document.
        /// </summary>
        public const bool UseNumberedNotes = false;

        /// <summary>
        /// Remove the first space at the beginning of each footnote and endnote (version without numbered notes).
        /// </summary>
        public const bool TrimStartNotes = true;

        /// <summary>
        /// The file path to the LibreOffice executable (soffice.exe) used for document conversions.
        /// </summary>
        public const string LibreOfficePath = 
            @"C:\Program Files\LibreOffice\program\soffice.exe";

        #endregion
    }
}
