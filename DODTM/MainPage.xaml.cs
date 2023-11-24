using BarcodeNet.ImageSharp;
using BarcodeNet.Managment;
using CommunityToolkit.Maui.Storage;
using DODTM.Extension;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DODTM;

public partial class MainPage : ContentPage
{
    readonly string[] typeFiles = ["png", "jpeg", "bmp", "jpg"];
    readonly string[] algorithmsDefault = ["DFT", "SVD", "Perfect low and high pass filter", "Butterworth Filter", "Laplace filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter", "Barcode"];
    readonly string[] algorithmsForMac = ["DFT", "SVD", "Perfect low and high pass filter", "Butterworth Filter", "Laplace filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter"];
    string currentLanguage = "English";

    string path = "";
    string? selectedAlg = "";
    string? selectedOutput = "";
    readonly string nameOfProject = Extension.Translator.Instance["NameProject"].ToString();

    public MainPage()
    {
        InitializeComponent();
        saveList.ItemsSource = typeFiles;
        label.IsVisible = false;
        switcher.IsVisible = false;
        argEntry1.IsVisible = false;
        argEntry2.IsVisible = false;

#if MACCATALYST
        openImgBtn.IsVisible = false;
        infoButton.IsVisible = false;
        algorithmList.ItemsSource = algorithmsForMac;
#elif WINDOWS
        openImgEntry.IsVisible = false;
        algorithmList.ItemsSource = algorithmsDefault;
#endif
    }

    private async void InfoClicked(object sender, EventArgs e)
    {
        string result = await DisplayActionSheet(Extension.Translator.Instance["AppInfo"].ToString(), Extension.Translator.Instance["Thanks"].ToString(), "GitHub", Extension.Translator.Instance["Version"].ToString() + " 2.1.0", Extension.Translator.Instance["Language"].ToString() + $"  {currentLanguage}", Extension.Translator.Instance["Author"].ToString());
        if (result == null) return;
        else if (result == "GitHub") await Clipboard.SetTextAsync("https://github.com/DKAVrZoV65F/Digital-Terrain-Models");
        else if (result.Contains("English"))
        {
            Translator.Instance.CultureInfo = new CultureInfo("ru-RU");
            Translator.Instance.OnPropertyChanged();
            currentLanguage = "Русский";
        }
        else if (result.Contains("Русский"))
        {
            Translator.Instance.CultureInfo = new CultureInfo("en-US");
            Translator.Instance.OnPropertyChanged();
            currentLanguage = "English";
        }
    }

    private async void SelectedPathClicked(object sender, EventArgs e)
    {
#if MACCATALYST
        string pathLocal = openImgEntry.Text;
        if(!string.IsNullOrEmpty(pathLocal)) {
            path = pathLocal;
            imageProcess.Source = path;
        }
#elif WINDOWS
        var filePath = await FilePicker.PickAsync(default);
        if (!string.IsNullOrEmpty(filePath?.ToString()))
        {
            path = filePath.FullPath;
            imageProcess.Source = path;
        }
        return;
#endif
    }

    private void AlgorithmListChanged(object sender, SelectedItemChangedEventArgs e)
    {
        selectedAlg = e.SelectedItem.ToString();
        if (selectedAlg == null) return;

        label.IsVisible = selectedAlg.Equals("DFT");
        switcher.IsVisible = selectedAlg.Equals("DFT");
        argEntry1.IsVisible = selectedAlg.Equals("SVD") || selectedAlg.Equals("Barcode");
        argEntry2.IsVisible = selectedAlg.Equals("SVD");
    }

    private void SaveListChanged(object sender, SelectedItemChangedEventArgs e) => selectedOutput = e.SelectedItem.ToString();

    [RequiresAssemblyFiles("Calls MauiCurs.MainPage.executeImage(String, String)")]
    private async void ProcessClicked(object sender, EventArgs e)
    {
        if (selectedAlg == null || selectedAlg == "")
        {
            await DisplayAlert(nameOfProject, Extension.Translator.Instance["ErrorWithAlgorithm"].ToString(), "ОK");
            return;
        }

        bool mask = switcher.IsChecked;
        string arg1 = selectedAlg.Equals("DFT") ? switcher.IsChecked.ToString() : arg1 = argEntry1.Text;
        string arg2 = argEntry2.Text;

        if (string.IsNullOrEmpty(path))
        {
            await DisplayAlert(nameOfProject, Extension.Translator.Instance["ErrorWithPathToFile"].ToString(), "ОK");
            return;
        }

        if (selectedAlg.Equals("SVD") && (string.IsNullOrEmpty(argEntry1.Text) || string.IsNullOrEmpty(argEntry2.Text))) return;

        string pathFolder = "";

#if MACCATALYST
        pathFolder = "~";
#elif WINDOWS
        var pathFolderLoc = await FolderPicker.PickAsync(default);

        if (string.IsNullOrEmpty(pathFolderLoc.Folder?.Path))
        {
            await DisplayAlert(nameOfProject, Extension.Translator.Instance["ErrorWithPathFolder"].ToString(), "ОK");
            return;
        }
        pathFolder = pathFolderLoc.Folder.Path + "\\" + selectedAlg;
#endif

        string ext = Path.GetExtension(path).Replace(".", "");
        foreach (var type in typeFiles)
        {
            if (ext.Equals(type))
            {
                int width = (!string.IsNullOrEmpty(widthImgEntry.Text) && Int32.Parse(widthImgEntry.Text) > 800) ? Int32.Parse(widthImgEntry.Text) : 800;
                int height = (!string.IsNullOrEmpty(heightImgEntry.Text) && Int32.Parse(heightImgEntry.Text) > 800) ? Int32.Parse(heightImgEntry.Text) : 800;

                string outputFileImage = (string)saveList.SelectedItem ?? "png";
#if MACCATALYST
try
{
    Directory.CreateDirectory("~" + $"\\{selectedAlg}");
}
catch (Exception except)
{
    await DisplayAlert(nameOfProject, except.Message, "ОK");
}
                //
#elif WINDOWS
                Directory.CreateDirectory(pathFolder);
#endif
                if (selectedAlg.Equals("Barcode"))
                {
                    int arg = Int32.Parse(arg1);
                    MainPage.ExecuteBarcode(pathToImg: path, length: arg, pathToSave: pathFolder, outputFile: outputFileImage, widthImg: width, heightImg: height);
                }
                else ExecuteImage(algorithmSelect: selectedAlg, pathImage: path, folderPath: pathFolder, outputFile: outputFileImage, arg1: arg1, arg2: arg2, widthImg: width, heightImg: height);
                await DisplayAlert(nameOfProject, (Extension.Translator.Instance["SuccessWork"].ToString() + $" {pathFolder}"), "ОK");
                return;
            }
        }
        await DisplayAlert(nameOfProject, Extension.Translator.Instance["ErrorWithImageRequires"].ToString(), "ОK");
    }

    [RequiresAssemblyFiles("Calls System.Reflection.Assembly.Location")]
    private void ExecuteImage(string algorithmSelect, string pathImage, string folderPath, string outputFile, string arg1 = "0", string arg2 = "0", int widthImg = 800, int heightImg = 800)
    {
        string pythonPath = "C:\\Users\\" + Environment.UserName + "\\DODTM_Algorithms.exe";

        int dpiImg = (!string.IsNullOrEmpty(dpiImgEntry.Text)) ? Int32.Parse(widthImgEntry.Text) : 100;
        System.Diagnostics.ProcessStartInfo procStartInfo = new(pythonPath, $"\"{algorithmSelect}\" \"{pathImage}\" \"{folderPath}\" \"{outputFile}\" \"{widthImg}\" \"{heightImg}\" \"{dpiImg}\" \"{arg1}\" \"{arg2}\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        System.Diagnostics.Process proc = new()
        {
            StartInfo = procStartInfo
        };
        _ = proc.Start();
        _ = proc.StandardOutput.ReadToEnd();
    }

    private static void ExecuteBarcode(string pathToImg, int length, string pathToSave, string outputFile, int widthImg, int heightImg)
    {
        ISImageContainer container = new(SixLabors.ImageSharp.Image.Load<L8>(pathToImg));
        int width = container.Width;
        int height = container.Height;

        if (widthImg > width) width = widthImg;
        if (heightImg > height) height = heightImg;
        BarCodeLineContainer<byte> barcodeContainer = container.GetBarCode<byte>();

        Image<L8> img = barcodeContainer.BarCodes
            .Where(x => x.Barcode.Length == length)
            .Select(a => a.Barcode)
            .ToImageL8(width, height, false);
        img.Save($"{pathToSave}\\Equals_{length}.{outputFile}");
        img.Dispose();

        Image<L8> img1 = barcodeContainer.BarCodes
            .Where(x => x.Barcode.Length != length)
            .Select(a => a.Barcode)
            .ToImageL8(width, height, false);
        img1.Save($"{pathToSave}\\Another_{length}.{outputFile}");
        img1.Dispose();
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
