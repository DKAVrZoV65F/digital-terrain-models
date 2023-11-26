using BarcodeNet.ImageSharp;
using BarcodeNet.Managment;
using CommunityToolkit.Maui.Storage;
using DODTM.Extension;
using DODTM.Resources.Strings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;

namespace DODTM;

public partial class MainPage : ContentPage
{
    public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

    readonly string[] typeFiles = ["png", "jpeg", "bmp", "jpg"];
    readonly string[] algorithmsDefault = ["DFT", "SVD", "Perfect low and high pass filter", "Butterworth Filter", "Laplace filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter", "Barcode"];
    readonly string[] algorithmsForMac = ["DFT", "SVD", "Perfect low and high pass filter", "Butterworth Filter", "Laplace filter", "Gaussian filter", "Frequency Domain Filter Laplace Filter"];
    string currentLanguage = "English";
    readonly string RUSSIAN = "Русский";
    readonly string ENGLISH = "English";
    string path = "";
    string? selectedAlg = "";
    string? selectedOutput = "";
    private readonly string nameOfProject = AppInfo.Current.Name;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
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
        string result = await DisplayActionSheet(LocalizationResourceManager["AppInfo"].ToString(), LocalizationResourceManager["Thanks"].ToString(), "GitHub", LocalizationResourceManager["Version"].ToString() + $" {AppInfo.Current.VersionString}", LocalizationResourceManager["Language"].ToString() + $"  {currentLanguage}", LocalizationResourceManager["Author"].ToString());
        
        if (result == null) return;
        else if (result == "GitHub") await Clipboard.SetTextAsync("https://github.com/DKAVrZoV65F/Digital-Terrain-Models");
        else if (result.Contains(ENGLISH) || result.Contains(RUSSIAN))
        {
            var switchToCulture = AppResources.Culture.TwoLetterISOLanguageName.
                Equals("en", StringComparison.InvariantCultureIgnoreCase) ?
                new CultureInfo("ru-RU") : new CultureInfo("en-US");

            LocalizationResourceManager.Instance.SetCulture(switchToCulture);

            currentLanguage = (currentLanguage.Equals(ENGLISH)) ? RUSSIAN : ENGLISH;
        }
    }

    private async void SelectedPathClicked(object sender, EventArgs e)
    {
#if MACCATALYST
        string pathLocal = openImgEntry.Text;
        
        if (!string.IsNullOrEmpty(pathLocal))
        {
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
            await DisplayAlert(nameOfProject, LocalizationResourceManager["ErrorWithAlgorithm"].ToString(), "ОK");
            return;
        }

        bool mask = switcher.IsChecked;
        string arg1 = selectedAlg.Equals("DFT") ? switcher.IsChecked.ToString() : arg1 = argEntry1.Text;
        string arg2 = argEntry2.Text;

        if (string.IsNullOrEmpty(path))
        {
            await DisplayAlert(nameOfProject, LocalizationResourceManager["ErrorWithPathToFile"].ToString(), "ОK");
            return;
        }

        if (selectedAlg.Equals("SVD") && (string.IsNullOrEmpty(argEntry1.Text) || string.IsNullOrEmpty(argEntry2.Text))) return;

        string pathFolder = "";
        string ext = Path.GetExtension(path).Replace(".", "");
        foreach (var type in typeFiles)
        {
            if (ext.Equals(type))
            {
                int width = (!string.IsNullOrEmpty(widthImgEntry.Text) && Int32.Parse(widthImgEntry.Text) > 800) ? Int32.Parse(widthImgEntry.Text) : 800;
                int height = (!string.IsNullOrEmpty(heightImgEntry.Text) && Int32.Parse(heightImgEntry.Text) > 800) ? Int32.Parse(heightImgEntry.Text) : 800;

                string outputFileImage = saveList.SelectedItem as string ?? "png";
#if MACCATALYST
                pathFolder = "/Users/" + Environment.UserName;
                ExecuteImage(algorithmSelect: selectedAlg, pathImage: path, folderPath: pathFolder, outputFile: outputFileImage, arg1: arg1, arg2: arg2, widthImg: width, heightImg: height);
#elif WINDOWS
                var pathFolderLoc = await FolderPicker.PickAsync(default);

                if (string.IsNullOrEmpty(pathFolderLoc.Folder?.Path))
                {
                    await DisplayAlert(nameOfProject, LocalizationResourceManager["ErrorWithPathFolder"].ToString(), "ОK");
                    return;
                }
                pathFolder = pathFolderLoc.Folder.Path + "\\" + selectedAlg;
                Directory.CreateDirectory(pathFolder);

                if (selectedAlg.Equals("Barcode")) MainPage.ExecuteBarcode(pathToImg: path, rangeOrLength: arg1, pathToSave: pathFolder, outputFile: outputFileImage, widthImg: width, heightImg: height);
                else ExecuteImage(algorithmSelect: selectedAlg, pathImage: path, folderPath: pathFolder, outputFile: outputFileImage, arg1: arg1, arg2: arg2, widthImg: width, heightImg: height);
                await DisplayAlert(nameOfProject, (LocalizationResourceManager["SuccessWork"].ToString() + $" {pathFolder}"), "ОK");
                return;
#endif
            }
        }
        await DisplayAlert(nameOfProject, LocalizationResourceManager["ErrorWithImageRequires"].ToString(), "ОK");
    }

    [RequiresAssemblyFiles("Calls System.Reflection.Assembly.Location")]
    private async void ExecuteImage(string algorithmSelect, string pathImage, string folderPath, string outputFile, string arg1 = "0", string arg2 = "0", int widthImg = 800, int heightImg = 800)
    {
        int dpiImg = (!string.IsNullOrEmpty(dpiImgEntry.Text)) ? Int32.Parse(widthImgEntry.Text) : 100;
        int startRange = 0;
        int endRange = 1;

        string[] words = arg1.Split('-');
        bool rangeSelected = false;

        if (words.Length > 1)
        {
            rangeSelected = Int32.TryParse(words[1].Trim(), out endRange);
            rangeSelected = rangeSelected && Int32.TryParse(words[0].Trim(), out startRange);
        }

        endRange = rangeSelected == false && words.Length > 1 ? Int32.Parse(words[0].Trim()) : Int32.Parse(arg1);

#if MACCATALYST
        string command = $"./DODTM_Algorithms \"{algorithmSelect}\" \"{pathImage}\" \"{folderPath}\" \"{outputFile}\" \"{widthImg}\" \"{heightImg}\" \"{dpiImg}\" \"{arg2}\" \"{startRange}\" \"{endRange}\"";
        await Clipboard.SetTextAsync(command);
        await DisplayAlert(nameOfProject, "The command was copied to the clipboard.\n Paste this command to the command bar where located DODTM_Algorithm:\n" + command, "ОK");
#elif WINDOWS
        System.Drawing.Image img = new Bitmap(pathImage);

        var bmp = new Bitmap(img.Width, img.Height,
                          System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
        using (var gr = Graphics.FromImage(bmp))
            gr.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height));
        bmp.Save(folderPath + "\\ConvertedImageTo32Bit.png");

        string pythonPath = "C:\\Users\\" + Environment.UserName + "\\DODTM_Algorithms.exe";

        System.Diagnostics.ProcessStartInfo procStartInfo = new(pythonPath, $"\"{algorithmSelect}\" \"{pathImage}\" \"{folderPath}\" \"{outputFile}\" \"{widthImg}\" \"{heightImg}\" \"{dpiImg}\" \"{arg2}\" \"{startRange}\" \"{endRange}\"")
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
#endif
    }

    private static void ExecuteBarcode(string pathToImg, string rangeOrLength, string pathToSave, string outputFile, int widthImg, int heightImg)
    {
        ISImageContainer container = new(SixLabors.ImageSharp.Image.Load<L8>(pathToImg));
        int width = container.Width;
        int height = container.Height;

        if (widthImg > width) width = widthImg;
        if (heightImg > height) height = heightImg;
        BarCodeLineContainer<byte> barcodeContainer = container.GetBarCode<byte>();
        string[] words = rangeOrLength.Split('-');
        bool rangeSelected = false;

        if (words.Length > 1)
        {
            rangeSelected = Int32.TryParse(words[0].Trim(), out _);
            rangeSelected = Int32.TryParse(words[1].Trim(), out _);
        }

        if (rangeSelected)
        {
            int length_1 = Int32.Parse(words[0].Trim());
            int length_2 = Int32.Parse(words[1].Trim());

            for (int range = length_1; range <= length_2; range++)
            {
                Image<L8> img = barcodeContainer.BarCodes
                .Where(x => x.Barcode.Length == range)
                .Select(a => a.Barcode)
                .ToImageL8(width, height, false);
                img.Save($"{pathToSave}\\Equals_{range}.{outputFile}");
                img.Dispose();

                Image<L8> img1 = barcodeContainer.BarCodes
                    .Where(x => x.Barcode.Length != range)
                    .Select(a => a.Barcode)
                    .ToImageL8(width, height, false);
                img1.Save($"{pathToSave}\\Another_{range}.{outputFile}");
                img1.Dispose();
            }
        }
        else
        {
            int length = Int32.Parse(words[0].Trim());

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
