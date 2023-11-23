using DODTM.Extension;
using System.Globalization;

namespace DODTM;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        CultureInfo ci = CultureInfo.InstalledUICulture;
        Translator.Instance.CultureInfo = new CultureInfo(ci.Name);
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var windows = base.CreateWindow(activationState);

        const int minWidth = 700;
        const int minHeight = 550;

        windows.MinimumWidth = minWidth;
        windows.MinimumHeight = minHeight;

        return windows;
    }
}
