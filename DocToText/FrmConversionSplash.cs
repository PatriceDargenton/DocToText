
using System.Diagnostics; // Stopwatch

namespace DocToText;

internal sealed partial class FrmConversionSplash : Form
{
    private readonly Func<string> _conversionAction;
    private readonly Stopwatch _displayTimer = Stopwatch.StartNew();

    /// <summary>
    /// Initializes a new instance of the FrmConversionSplash class with a default empty conversion action.
    /// </summary>
    public FrmConversionSplash()
        : this(() => string.Empty)
    {
    }

    private void FrmConversionSplash_Load(object sender, EventArgs e)
    {
        this.Text = Program.GetFullAppNameAndVersion();
    }

    /// <summary>
    /// Initializes a new instance of the FrmConversionSplash class with a conversion action to execute.
    /// </summary>
    /// <param name="conversionAction">A function that performs the document conversion and returns the output file path.</param>
    public FrmConversionSplash(Func<string> conversionAction)
    {
        _conversionAction = conversionAction;
        InitializeComponent();
    }

    /// <summary>
    /// Gets the exception that occurred during document conversion, if any.
    /// </summary>
    /// <remarks>
    /// This property is null if the conversion completed successfully without errors.
    /// </remarks>
    public Exception? ConversionException { get; private set; }

    private async void FrmConversionSplash_Shown(object? sender, EventArgs e)
    {
        try
        {
            await Task.Run(_conversionAction);
        }
        catch (Exception ex)
        {
            ConversionException = ex;
        }

        var remainingDelayMs = Const.ConversionSplashMinimumDisplayDurationMs - 
            (int)_displayTimer.ElapsedMilliseconds;
        if (remainingDelayMs > 0) await Task.Delay(remainingDelayMs);

        Close();
    }
}