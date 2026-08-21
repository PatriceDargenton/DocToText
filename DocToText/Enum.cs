
using DocToText.Helpers;
using System.ComponentModel;

namespace DocToText
{
    public class DocToTextEnums
    {
        [Description("Command menu")]
        [DefaultValue(ConversionMenuEnum.text)]
        public enum ConversionMenuEnum
        {
            [Description("Convert to plain text")]
            [RegistryKey("DocToText.ConvertToText")]
            text,

            [Description("Convert to Markdown")]
            [RegistryKey("DocToText.ConvertToMarkdown")]
            markdown,

            [Description("Convert to plain text without end/foot notes")]
            [RegistryKey("DocToText.ConvertToTextSimple")]
            text_simple,

            [Description("Convert to docx")]
            [RegistryKey("DocToText.ConvertDocToDocx")]
            doc_to_docx,
        }

        public enum ExtensionEnum
        {
            /// <summary>
            /// File extension for Microsoft Word 97-2003 documents.
            /// </summary>
            [RegistryKey("SystemFileAssociations\\.doc\\shell")]
            doc,

            /// <summary>
            /// File extension for Microsoft Word 2007+ documents (Office Open XML).
            /// </summary>
            [RegistryKey("SystemFileAssociations\\.docx\\shell")]
            docx,

            /// <summary>
            /// File extension for OpenDocument Text documents.
            /// </summary>
            [RegistryKey("SystemFileAssociations\\.odt\\shell")]
            odt,
        }

        public static ConversionMenuEnum ConversionMenuFromValue(string value)
        {
            ConversionMenuEnum x;
            try
            {
                x = EnumExtensions.GetValueFromValue<ConversionMenuEnum>(value);
            }
            catch (Exception /* ex */)
            {
                x = ConversionMenuDefault();
            }
            return x;
        }

        public static ConversionMenuEnum ConversionMenuDefault()
        {
            var x = DocToText.EnumHelper.GetDefaultValue<ConversionMenuEnum>();
            return x;
        }
    }
}
