
using DocToText.Helpers;
using static DocToText.DocToTextEnums;

namespace DocToText;

public partial class FrmDocToText : Form
{
    private readonly bool _isAdmin;

    private const string ProgIdCreatedMarkerValue = "DocToTextCreated";

    /// <summary>
    /// Initializes a new instance of the FrmDocToText class and sets up the main application form.
    /// </summary>
    /// <remarks>
    /// This constructor checks if the application is running with administrator privileges and initializes the admin state accordingly.
    /// </remarks>
    public FrmDocToText()
    {
        InitializeComponent();
        _isAdmin = AppHelpers.IsAdministrator();
        RefreshAdminState();
    }
    
    private void FrmDocToText_Load(object sender, EventArgs e)
    {
        this.Text = Program.GetFullAppNameAndVersion();

        if (!_isAdmin)
        {
            // Disable buttons if not running as administrator
            btnAddContextMenus.Enabled = false;
            btnRemoveContextMenus.Enabled = false;
            return;
        }
        btnRunAsAdmin.Enabled = false;

        Activation();
    }

    private void Activation() 
    {
        // Enable/Disable buttons based on registry state
        bool docxMenuRegistered = AreContextMenusRegistered(ExtensionEnum.docx.ToRegistryKey());
        bool docMenuRegistered = AreContextMenusRegistered(ExtensionEnum.doc.ToRegistryKey());
        bool odtMenuRegistered = AreContextMenusRegistered(ExtensionEnum.odt.ToRegistryKey());

        bool doc_to_docxMenuExists = RegistryHelper.MenuExists(
            ExtensionEnum.doc.ToRegistryKey(),
            ConversionMenuEnum.doc_to_docx.ToRegistryKey());

        bool areMenusRegistered = docxMenuRegistered && docMenuRegistered && odtMenuRegistered && 
            doc_to_docxMenuExists;
        btnAddContextMenus.Enabled = !areMenusRegistered;
        btnRemoveContextMenus.Enabled = areMenusRegistered;
    }

    private void BtnRunAsAdmin_Click(object sender, EventArgs e)
    {
        if (AppHelpers.RestartAsAdministrator(out string errorMessage, showError: false))
            Close();
        else
            SetStatus($"Failed to restart as administrator: {errorMessage}");
    }

    private void RefreshAdminState()
    {
        lblAdminState.Text = _isAdmin ?
            "Admin state: Running as administrator" : 
            "Admin state: Running as standard user";
        btnRunAsAdmin.Enabled = !_isAdmin;
    }
    
    private void SetStatus(string message)
    {
        lblStatus.Text = message;
    }

    #region "Context menus"

    private void BtnAddContextMenus_Click(object sender, EventArgs e)
    {
        try
        {
            AddContextMenus();
            SetStatus("Context menus were added successfully.");
            Activation();
        }
        catch (Exception ex)
        {
            SetStatus($"Failed to add context menus: {ex.Message}");
        }
    }

    private void BtnRemoveContextMenus_Click(object sender, EventArgs e)
    {
        try
        {
            RemoveContextMenus();
            SetStatus("Context menus were removed successfully.");
            Activation();
        }
        catch (Exception ex)
        {
            SetStatus($"Failed to remove context menus: {ex.Message}");
        }
    }

    private static bool AreContextMenusRegistered(string extName)
    {
        var textMenuExists = RegistryHelper.MenuExists(extName, 
            ConversionMenuEnum.text.ToRegistryKey());
        var markdownMenuExists = RegistryHelper.MenuExists(extName, 
            ConversionMenuEnum.markdown.ToRegistryKey());
        var text_simpleMenuExists = RegistryHelper.MenuExists(extName, 
            ConversionMenuEnum.text_simple.ToRegistryKey());
        return textMenuExists && markdownMenuExists && text_simpleMenuExists;
    }

    private static void AddContextMenus()
    {
        RegisterContextMenus(ExtensionEnum.docx.ToRegistryKey());
        RegisterContextMenus(ExtensionEnum.doc.ToRegistryKey());
        RegisterContextMenus(ExtensionEnum.odt.ToRegistryKey());

        if (Const.UseNPOI)
            RegisterDocContextMenus(ExtensionEnum.doc.ToRegistryKey());
    }

    private static void RemoveContextMenus()
    {
        UnregisterContextMenus(ExtensionEnum.docx.ToRegistryKey());
        UnregisterContextMenus(ExtensionEnum.doc.ToRegistryKey());
        UnregisterContextMenus(ExtensionEnum.odt.ToRegistryKey());

        if (Const.UseNPOI)
            UnregisterDocContextMenus(ExtensionEnum.doc.ToRegistryKey());
    }

    private static void RegisterContextMenus(string extName)
    {
        var exe = Application.ExecutablePath;

        RegistryHelper.CreateMenu(
            extName,
            ConversionMenuEnum.text.ToRegistryKey(),
            ConversionMenuEnum.text.ToDescription(),
            $"{exe} --{ConversionMenuEnum.text} \"%1\""); // --text

        RegistryHelper.CreateMenu(
            extName,
            ConversionMenuEnum.markdown.ToRegistryKey(),
            ConversionMenuEnum.markdown.ToDescription(),
            $"{exe} --{ConversionMenuEnum.markdown} \"%1\"");

        RegistryHelper.CreateMenu(
            extName,
            ConversionMenuEnum.text_simple.ToRegistryKey(),
            ConversionMenuEnum.text_simple.ToDescription(),
            $"{exe} --{ConversionMenuEnum.text_simple} \"%1\"");
    }

    private static void RegisterDocContextMenus(string extName)
    {
        var exe = Application.ExecutablePath;

        RegistryHelper.CreateMenu(
            extName,
            ConversionMenuEnum.doc_to_docx.ToRegistryKey(),
            ConversionMenuEnum.doc_to_docx.ToDescription(),
            $"{exe} --{ConversionMenuEnum.doc_to_docx} \"%1\"");
    }

    private static void UnregisterContextMenus(string extName)
    {
        RegistryHelper.DeleteMenu(extName, ConversionMenuEnum.text.ToRegistryKey());
        RegistryHelper.DeleteMenu(extName, ConversionMenuEnum.markdown.ToRegistryKey());
        RegistryHelper.DeleteMenu(extName, ConversionMenuEnum.text_simple.ToRegistryKey());
    }
    
    private static void UnregisterDocContextMenus(string extName)
    {
        RegistryHelper.DeleteMenu(extName, ConversionMenuEnum.doc_to_docx.ToRegistryKey());
    }

    #endregion
}