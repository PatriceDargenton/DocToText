
// https://github.com/manfromarce/DocSharp
using DocSharp.Docx; // https://www.nuget.org/packages/DocSharp.Docx

// Used by DocToDocx
//using DocSharp.Binary.Doc; // https://www.nuget.org/packages/DocSharp.Binary.Doc

// https://github.com/nissl-lab/npoi
using NPOI.XWPF.UserModel; // https://www.nuget.org/packages/npoi
using NPOI.OpenXmlFormats.Wordprocessing;

using DocToText.Helpers;
using static DocToText.DocToTextEnums;

using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace DocToText;

internal static class DocumentConverter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "." + ExtensionEnum.doc.ToString(),
        "." + ExtensionEnum.docx.ToString(),
        "." + ExtensionEnum.odt.ToString()
    };

    /// <summary>
    /// Attempts to parse command-line arguments to extract the conversion mode and file path.
    /// </summary>
    /// <param name="args">The command-line arguments array.</param>
    /// <param name="mode">The conversion mode (text, markdown, ...) to be extracted from arguments.</param>
    /// <param name="filePath">The file path to be converted, extracted from arguments.</param>
    /// <param name="errorMessage">An error message if parsing fails; otherwise empty.</param>
    /// <returns>True if arguments were successfully parsed; false if parsing failed or arguments are invalid.</returns>
    /// <remarks>
    /// If only one argument is provided, it is treated as the file path with text mode as default.
    /// If two or more arguments are provided, the first must be for example "--text" or "--markdown" to specify the conversion mode.
    /// </remarks>
    public static bool TryParseArguments(string[] args, 
        out ConversionMenuEnum mode, out string filePath, out string errorMessage)
    {
        mode = ConversionMenuEnum.text;
        filePath = string.Empty;
        errorMessage = string.Empty;

        if (args.Length == 1)
        {
            filePath = args[0];
            return ValidateFilePath(filePath, out errorMessage);
        }

        if (args.Length >= 2)
        {
            var cmd = args[0];
            if (!cmd.StartsWith("--"))
            {
                errorMessage = "Invalid conversion mode. The command does not start with --.";
                return false;
            }
            var cmdOnly = cmd.Substring(2); // Remove the leading "--"
            mode = ConversionMenuFromValue(cmdOnly);

            filePath = args[1];
            return ValidateFilePath(filePath, out errorMessage);
        }

        errorMessage = "A file path argument is required.";
        return false;
    }

    /// <summary>
    /// Converts a document file to the specified format (plain text or Markdown).
    /// </summary>
    /// <param name="filePath">The path to the document file to convert (.doc, .docx, or .odt).</param>
    /// <param name="mode">The conversion mode specifying the output format (text or markdown).</param>
    /// <returns>The path to the generated output file; returns empty string if conversion fails.</returns>
    /// <remarks>
    /// Supported input formats include Microsoft Word documents (.doc, .docx) and OpenDocument Text (.odt).
    /// For .doc and .odt files, the method automatically converts them to .docx format first using either LibreOffice (if available) or DocSharp.Binary.
    /// The output file is created in the same directory as the input file with an appropriate extension (.txt or .md).
    /// </remarks>
    public static string Convert(string filePath, ConversionMenuEnum mode)
    {
        if (!ValidateFilePath(filePath, out var errorMessage))
        {
            MessageBox.Show(errorMessage, Const.AppTitle, 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return string.Empty;
        }

        bool convertedDocxFileAlreadyExists = false;
        var memFilePath = filePath;
        var restoreExistingDocxFile = false;
        var libreOfficeUsed = false;
        var tempExistingDocxFile = string.Empty;
        var docxPath = string.Empty;
        var extension = Path.GetExtension(filePath);
        var isDoc = extension.Equals("." + ExtensionEnum.doc.ToString(), StringComparison.OrdinalIgnoreCase);
        var isOdt = extension.Equals("." + ExtensionEnum.odt.ToString(), StringComparison.OrdinalIgnoreCase);
        if (isDoc || isOdt)
        {
            if (!DocToDocxMain(isOdt, ref filePath,
                out convertedDocxFileAlreadyExists, out libreOfficeUsed, out restoreExistingDocxFile,
                out tempExistingDocxFile, out docxPath)) return string.Empty;

            // If the user requested a .doc to .docx conversion, return
            //  the path to the converted .docx file
            if (mode == ConversionMenuEnum.doc_to_docx) return filePath;
        }

        string output;
        if (mode == ConversionMenuEnum.markdown)
        {
            output = new DocSharp.Docx.DocxToMarkdownConverter().ConvertToString(filePath);
        }
        else 
        {
            if (Const.UseNPOI)
            {
                using var fs = File.OpenRead(filePath);
                using var doc = new XWPFDocument(fs);
                
                // Basic approach: join all paragraphs with line breaks
                // This includes footnotes and endnotes, but does not include tables, if any
                //output = string.Join("\n", doc.Paragraphs.Select(p => p.Text));

                var includeFootnoteMarkers = 
                    (mode == ConversionMenuEnum.text_simple) ? false : true;
                if (includeFootnoteMarkers) 
                { 
                    output = ExportToPlainText(doc);

                    // Does not work: footnotes and endnotes markers are included,
                    //  but without the notes themselves
                    // (that could have been useful for disabling note numbering)
                    //output = ExportToPlainTextWithoutEndFootNotes(doc, includeFootnoteMarkers: true);
                }
                else
                    output = ExportToPlainTextWithoutEndFootNotes(doc, includeFootnoteMarkers: false);
            }
            else 
            {
                var txtConverter = new DocSharp.Docx.DocxToTxtConverter
                {
                    // Do not add empty lines between paragraphs for plain text output
                    // Useless because the right approach was not to add lines when the
                    //  document contained none, and not to delete them if the document
                    //  did contain them.
                    //EmptyLineBetweenParagraphs = false // default value: some line breaks are removed
                    //EmptyLineBetweenParagraphs = true  // useless: no one would want to add blank lines if it isn't necessary
                };
                output = txtConverter.ConvertToString(filePath);
            }
        }

        var outputExtension = 
            (mode == ConversionMenuEnum.markdown ? Const.mdExtension : Const.txtExtension);
        var outputPath = Path.ChangeExtension(memFilePath, outputExtension);

        var encoding = Encoding.UTF8;
        if (Const.Use1252Encoding && mode == ConversionMenuEnum.text)
            encoding = Encoding.GetEncoding(1252); // Plain text: Windows1252
        File.WriteAllText(outputPath, output, encoding);

        if ((isDoc || isOdt) && Const.UseLibreOfficeDocToDocxConversion && !libreOfficeUsed)
        {
            MessageBox.Show(
                "Converting .doc to .docx works better with LibreOffice,\n" +
                "for example, with languages ​​other than English.\n" +
                "Consider downloading and installing LibreOffice.",
                Const.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        if (isDoc || isOdt)
        {
            // Delete temporary .docx file if the output file was not present before
            if (restoreExistingDocxFile)
            {
                File.Delete(filePath);
                // Restore the existing .docx file
                File.Move(tempExistingDocxFile, docxPath);
            }
            else if (!convertedDocxFileAlreadyExists) File.Delete(filePath);
        }

        return outputPath;
    }

    #region NPOI: Export doc to plain text with or without endnotes/footnotes, with or without note markers

    static string ExportToPlainTextWithoutEndFootNotes(XWPFDocument doc, bool includeFootnoteMarkers)
    {
        /*
        // Exclude footnotes and endnotes from the output 
        // (but keep the numbered markers, and tables is not present)
        var paragraphsWithoutNotes = doc.Paragraphs
            .Where(p => p.Body is XWPFDocument)
            .ToList();
        output = string.Join("\n", paragraphsWithoutNotes.Select(p => p.Text));
        */

        var sb = new StringBuilder();

        foreach (var bodyElem in doc.BodyElements)
        {
            if (bodyElem is XWPFParagraph para)
            {
                var paraSb = new StringBuilder();
                foreach (var run in para.Runs)
                    AppendRunText(run, paraSb, includeFootnoteMarkers);

                sb.AppendLine(paraSb.ToString());
            }
            else if (bodyElem is XWPFTable table)
            {
                foreach (var row in table.Rows)
                {
                    var cells = row.GetTableCells().Select(c =>
                    {
                        var cellSb = new StringBuilder();
                        foreach (var p in c.Paragraphs)
                            foreach (var run in p.Runs)
                                AppendRunText(run, cellSb, includeFootnoteMarkers);
                        return cellSb.ToString();
                    });
                    sb.AppendLine(string.Join("\t", cells));
                }
            }
        }

        return sb.ToString();
    }

    static string ExportToPlainText(XWPFDocument doc)
    {
        // Build text from main document body only (paragraphs and tables).
        var sb = new StringBuilder();
        foreach (var bodyElem in doc.BodyElements)
        {
            if (bodyElem is XWPFParagraph para)
            {
                // Without footnotes and endnotes, but with markers
                //sb.AppendLine(para.ParagraphText ?? string.Empty);

                sb.AppendLine(para.Text ?? string.Empty); // Full text
            }
            else if (bodyElem is XWPFTable table)
            {
                foreach (var row in table.Rows)
                {
                    // Without footnotes and endnotes, but with markers
                    //var cells = row.GetTableCells().Select(c =>
                    //    string.Join(Environment.NewLine,
                    //        c.Paragraphs.Select(p => p.ParagraphText ?? string.Empty)));

                    // Full text
                    var cells = row.GetTableCells().Select(c =>
                        string.Join(Environment.NewLine,
                            c.Paragraphs.Select(p => p.Text ?? string.Empty)));
                    sb.AppendLine(string.Join("\t", cells));
                }
            }
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Reconstructs the text of a run manually, with or without note markers.
    /// </summary>
    static void AppendRunText(XWPFRun run, StringBuilder sb, bool includeFootnoteMarkers)
    {
        CT_R ctr = run.GetCTR();
        for (int i = 0; i < ctr.Items.Count; i++)
        {
            var item = ctr.Items[i];
            var kind = ctr.ItemsElementName[i];

            switch (kind)
            {
                case RunItemsChoiceType.t:
                    if (item is CT_Text text)
                        sb.Append(text.Value);
                    break;

                case RunItemsChoiceType.tab:
                    sb.Append('\t');
                    break;

                case RunItemsChoiceType.br:
                case RunItemsChoiceType.cr:
                    sb.Append(Environment.NewLine);
                    break;

                // Does not work: footnotes and endnotes markers are included,
                //  but without the notes themselves
                // (that could have been useful for disabling note numbering)
                case RunItemsChoiceType.footnoteReference:
                    if (includeFootnoteMarkers && item is CT_FtnEdnRef footnoteRef)
                        sb.Append("[footnoteRef:").Append(footnoteRef.id).Append(']');
                    break;

                case RunItemsChoiceType.endnoteReference:
                    if (includeFootnoteMarkers && item is CT_FtnEdnRef endnoteRef)
                        sb.Append("[endnoteRef:").Append(endnoteRef.id).Append(']');
                    break;
            }
        }
    }
    
    #endregion

    private static bool DocToDocxMain(bool isOdt, ref string filePath, 
        out bool convertedDocxFileAlreadyExists, out bool libreOfficeUsed, 
        out bool restoreExistingDocxFile, out string tempExistingDocxFile, 
        out string docxPath)
    {
        var inputFilePath = filePath;
        var outputFilePath = Path.ChangeExtension(filePath, ".docx"); // "." + ExtensionEnum.docx.ToString()
        convertedDocxFileAlreadyExists = File.Exists(outputFilePath);
        libreOfficeUsed = false;
        restoreExistingDocxFile = false;
        tempExistingDocxFile = "";
        docxPath = "";

        var existingDocxFileSuffix = "_Existing.docx";

        var success = false;

        var libreOfficeExists = File.Exists(Const.LibreOfficePath);

        if (Const.UseLibreOfficeDocToDocxConversion && libreOfficeExists)
        {
            if (convertedDocxFileAlreadyExists)
            {
                while (!FileHelper.IsFileAccessible(outputFilePath))
                {
                    if (DialogResult.Cancel == MessageBox.Show(
                        "Please close the existing .docx file before conversion:\n" + outputFilePath,
                        Const.AppTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                        return false;
                }
                // Rename the existing .docx file, because LibreOffice will overwrite it
                string dirName = Path.GetDirectoryName(filePath) ?? string.Empty;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                tempExistingDocxFile = Path.Combine(
                    dirName, fileNameWithoutExtension + existingDocxFileSuffix);
                if (!File.Exists(tempExistingDocxFile)) 
                    File.Move(outputFilePath, tempExistingDocxFile);
                docxPath = outputFilePath;
                restoreExistingDocxFile = true;
            }
            success = AppHelpers.ConvertDocOrOdtToDocxUsingLibreOffice(inputFilePath, outputFilePath);
            libreOfficeUsed = success;
        }

        if (!success)
        {
            if (isOdt) 
            {
                MessageBox.Show("Converting .odt to .docx requires LibreOffice.",
                    Const.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (!isOdt && !Const.UseDocSharpBinaryDocToDocx)
            {
                MessageBox.Show("Converting .doc to .docx requires LibreOffice.",
                    Const.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            string dirName = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            outputFilePath = Path.Combine(
                dirName, fileNameWithoutExtension + "_DocToDocx.docx");
            convertedDocxFileAlreadyExists = File.Exists(outputFilePath);

            DocSharpBinaryDocToDocx(inputFilePath, outputFilePath); // DocSharp.Binary
            if (Const.UseNPOI) NormalizeStylesForNpoi(outputFilePath);
        }

        filePath = outputFilePath;

        return true;
    }

    private static void DocSharpBinaryDocToDocx(string inputFilePath, string outputFilePath)
    {
        using var reader = new DocSharp.Binary.StructuredStorage.Reader.StructuredStorageReader(inputFilePath);
        var doc = new DocSharp.Binary.DocFileFormat.WordDocument(reader);
        using var docx = DocSharp.Binary.OpenXmlLib.WordprocessingML.WordprocessingDocument.Create(
            outputFilePath, DocSharp.Binary.OpenXmlLib.WordprocessingDocumentType.Document);
        DocSharp.Binary.WordprocessingMLMapping.Converter.Convert(doc, docx);
    }

    #region NormalizeStylesForNpoi

    private static void NormalizeStylesForNpoi(string docxPath)
    {
        using var fileStream = File.Open(docxPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        using var zip = new ZipArchive(fileStream, ZipArchiveMode.Update, leaveOpen: false);

        foreach (var entryName in new[]
        {
            "word/document.xml",
            "word/styles.xml",
            "word/stylesWithEffects.xml",
            "word/numbering.xml",
            "word/settings.xml"
        })
        {
            NormalizeWordXmlEntryForNpoi(zip, entryName);
        }
    }

    private static void NormalizeWordXmlEntryForNpoi(ZipArchive zip, string entryName)
    {
        var entry = zip.GetEntry(entryName);
        if (entry is null) return;

        string xml;
        using (var entryStream = entry.Open())
        using (var reader = new StreamReader(entryStream, Encoding.UTF8, 
            detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: false))
        {
            xml = reader.ReadToEnd();
        }

        var normalizedXml = NormalizeWordXmlForNpoi(xml);
        if (string.Equals(normalizedXml, xml, StringComparison.Ordinal)) return;

        entry.Delete();
        var newEntry = zip.CreateEntry(entryName);
        using var writeStream = newEntry.Open();
        var outputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        using var writer = new StreamWriter(writeStream, outputEncoding);
        writer.Write(normalizedXml);
    }

    private static string NormalizeWordXmlForNpoi(string xml)
    {
        try
        {
            var document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            var wordNamespace = XNamespace.Get("http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            var changed = false;

            foreach (var typeAttribute in document
                         .Descendants()
                         .Attributes(wordNamespace + "type")
                         .Where(a => string.Equals(a.Value, "list", StringComparison.OrdinalIgnoreCase)))
            {
                typeAttribute.Value = "numbering";
                changed = true;
            }

            foreach (var styleElement in document.Descendants(wordNamespace + "style"))
            {
                var typeAttribute = styleElement.Attribute(wordNamespace + "type");
                if (typeAttribute is not null) continue;

                if (styleElement.Descendants(wordNamespace + "numPr").Any())
                {
                    styleElement.SetAttributeValue(wordNamespace + "type", "numbering");
                    changed = true;
                }
            }

            foreach (var docGridTypeAttribute in document
                         .Descendants(wordNamespace + "docGrid")
                         .Attributes(wordNamespace + "type")
                         .Where(a => string.Equals(a.Value, "Default", StringComparison.Ordinal)))
            {
                docGridTypeAttribute.Value = "default";
                changed = true;
            }

            if (!changed) return xml;

            using var writer = new Utf8StringWriter();
            document.Save(writer, SaveOptions.DisableFormatting);
            return writer.ToString();
        }
        catch
        {
            var normalized = xml
                .Replace("w:type=\"list\"", "w:type=\"numbering\"", StringComparison.Ordinal)
                .Replace("w:type='list'", "w:type='numbering'", StringComparison.Ordinal)
                .Replace("<w:docGrid w:type=\"Default\"", "<w:docGrid w:type=\"default\"", StringComparison.Ordinal)
                .Replace("<w:docGrid w:type='Default'", "<w:docGrid w:type='default'", StringComparison.Ordinal);

            return normalized;
        }
    }
    
    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }

    #endregion

    private static bool ValidateFilePath(string filePath, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(filePath))
        {
            errorMessage = "File path is empty.";
            return false;
        }

        if (!File.Exists(filePath))
        {
            errorMessage = $"Input file does not exist: {filePath}";
            return false;
        }

        var extension = Path.GetExtension(filePath);
        if (!SupportedExtensions.Contains(extension))
        {
            errorMessage = "Only .doc, .docx, and .odt files are supported.";
            return false;
        }

        return true;
    }
}