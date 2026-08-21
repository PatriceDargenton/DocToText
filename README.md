# DocToText
MS-Word .docx &amp; .doc converter to plain text (.txt) and Markdown (.md) written in C#

---

DocToText converts Microsoft Word documents (.docx and .doc) to plain text or Markdown. It provides a simple GUI to add (or remove) context-menu commands in Windows Explorer for quick conversion.

## Table of Contents
- [Keywords](#keywords)
- [Features](#features)
- [Requirements](#requirements)
- [Dependencies](#dependencies)
- [Usage](#usage)
- [Command-line arguments](#command-line-arguments)
- [Encoding and conversion notes](#encoding-and-conversion-notes)
- [Versions](#versions)
- [Links](#links)

## Keywords
DocToText, Doc2Text, DocxToText, Docx2Text, OdtToText, DocToTxt, Doc2Txt, DocxToTxt, Docx2Txt, Odt2Text, DocToMarkdown, DocxToMarkdown, Doc2Markdown, Docx2Markdown, Odt2Markdown, DocToDocx, Doc2Docx.

## Features
- Convert .docx to plain text (.txt) or Markdown (.md)
- Convert legacy .doc files by converting them to .docx first (uses DocSharp and/or LibreOffice when available)
- Convert also .odt files by converting them to .docx first (uses LibreOffice, it must be installed)
- Simple Windows Forms GUI with options to add/remove Explorer context-menu commands
- Command-line support for automation and scripting
- Optional Windows-1252 encoding (CP-1252, Windows code page 1252, msoEncodingWestern) for plain text output (configurable in source)

## Requirements
- Windows (desktop) with .NET 10
- Optional: LibreOffice installed to improve .doc -> .docx conversion quality for non-English documents
- Required: [LibreOffice](https://www.libreoffice.org/) installed to convert .odt -> .docx

## Dependencies
- [DocSharp.Docx](https://www.nuget.org/packages/DocSharp.Docx/) ![NuGet](https://img.shields.io/nuget/v/DocSharp.Docx.svg) used to produce Markdown, and to extract plain text from .docx paragraphs, if selected in the source code
- [DocSharp.Binary.Doc](https://www.nuget.org/packages/DocSharp.Binary.Doc/) ![NuGet](https://img.shields.io/nuget/v/DocSharp.Binary.Doc.svg) used for legacy .doc (binary) format support (if LibreOffice is not installed or not used, and if the option is enabled in source code)
- [NPOI](https://www.nuget.org/packages/NPOI/) ![NuGet](https://img.shields.io/nuget/v/NPOI.svg) used to extract plain text from .docx paragraphs

These dependencies are referenced as NuGet packages in the project file.

## Usage

GUI
- Launch the application without arguments. The GUI then allows you to add or remove context-menu entries (requires Administrator privileges):
  - "Convert to plain text"
  - "Convert to Markdown"
  - "Convert to plain text without end/foot notes" (numbering foot notes and end notes sometimes makes comparison difficult)

File Explorer
- Use these new menus added (see the previous GUI paragraph), for the .doc, .docx or .odt files.

The output file is written next to the original using the same base name and extension .txt or .md.

## Command-line arguments
Run the application from a command prompt to convert a file.

Switch options:
- `--text` : convert to plain text (default if no mode is specified)
- `--markdown`         : convert to Markdown
- `<filePath>`         : path to the .doc, .docx, or .odt file to convert
- `--text_simple`      : Convert to plain text without end/foot notes
- `--doc_to_docx`      : Convert .doc to .docx using LibreOffice

Examples:
```
DocToText.exe --text "C:\Docs\report.docx"
DocToText.exe --markdown "C:\Docs\notes.doc"
DocToText.exe "C:\Docs\document.odt"
```

## Encoding and conversion notes
- By default the application uses UTF-8 encoding for plain text output. Windows-1252 (msoEncodingWestern) can be enabled by changing the configuration in the Const.cs source file (Use1252Encoding).
- Legacy .doc files are converted to .docx first. Using LibreOffice is recommended for better results with non-English documents; you can configure the LibreOffice path and enable/disable that behavior in Const.cs.
- Temporary .docx files created during conversion are cleaned up after the conversion completes. If an existing .docx with the same target name exists, the app tries to preserve it.

## Versions
See [Changelog.md](Changelog.md)

## Links
- Find Windows 11 context menus inconvenient? Look here: [SwitchExplorer](https://github.com/LesFerch/SwitchExplorer): Switch the Windows 11 default Context Menu

Text files are useful for comparing document contents, for example with:
- [TextDiffToHtml](https://github.com/PatriceDargenton/TextDiffToHtml)
- [TextDiffOptions](https://github.com/PatriceDargenton/TextDiffOptions)