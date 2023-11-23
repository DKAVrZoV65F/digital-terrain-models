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

/*
                   ``
  `.              `ys
  +h+             +yh-
  yyh:           .hyys
 .hyyh.          oyyyh`
 /yyyyy`        .hyydy/
 syyhhy+        oyyhsys
 hyyyoyh.      .hyyy:hh`
.hyyyy:ho      +yyys-yh-
:hyyyh-oh.    `hyyyo-oy/
/yyyyh-:h+    -hyyh/-oy+
+yyyyh:-yy    +yyyh--oyo
+yyyyh/-sh.   syyyh--oyo
+yyyyh/-oy/  `hyyyy--syo
+yyyyh/-+y+  `hyyys--yy+
:yyyyh/-+ys  .hyyyo-:hy:
.hyyyh+-+ys  .hyyyo-oyh`
`yyyyyo-oyy  .hyyy+-yyy
 +yyyys-syy  `hyyh/oyy/
 .hyyyh-hyy  `hyyh/hyh
  oyyyhshys   yyyhyyy+
  oyyyhshys   yyyhyyy+
   /hyyyyyo`.-oyyyyh/
   `syyyyyyyhyyyyyyho.
    .hyyyyhNdyyyyyyymh/`
*/
