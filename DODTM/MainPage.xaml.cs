using BarcodeNet.ImageSharp;
using BarcodeNet.Managment;
using CommunityToolkit.Maui.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics.CodeAnalysis;

namespace DODTM
{
    public partial class MainPage : ContentPage
    {
        readonly string[] typeFiles = ["png", "jpeg", "bmp", "jpg"];
        string path = "";

        public MainPage()
        {
            InitializeComponent();
            algorithmPicker.SelectedIndex = 0;
            saveAsPicker.SelectedIndex = 0;
        }

        private async void SelectedPathClicked(object sender, EventArgs e)
        {
            var filePath = await FilePicker.PickAsync(default);
            if (string.IsNullOrEmpty(filePath?.ToString()))
            {
                await DisplayAlert("Image processing", "The path to the file is not specified!", "ОK");
                return;
            }
            path = filePath.FullPath;
            imageProcess.Source = path;
        }

        private void Switcher_Toggled(object sender, ToggledEventArgs e) => label.Text = (e.Value) ? $"Mask on" : "Mask off";

        private void PickerSelectedIndexChanged(object sender, EventArgs e)
        {
            label.IsVisible = algorithmPicker.SelectedIndex == 0;
            switcher.IsVisible = algorithmPicker.SelectedIndex == 0;
            argEntry1.IsVisible = algorithmPicker.SelectedIndex == 1 || algorithmPicker.SelectedIndex == 7;
            argEntry2.IsVisible = algorithmPicker.SelectedIndex == 1;
        }

        [RequiresAssemblyFiles("Calls MauiCurs.MainPage.executeImage(String, String)")]
        private async void ProcessClicked(object sender, EventArgs e)
        {
            string arg1 = (label.Text.Contains("True") && algorithmPicker.SelectedIndex == 0) ? "True" : (label.Text.Contains("False") && algorithmPicker.SelectedIndex == 0) ? "" : argEntry1.Text;
            string arg2 = argEntry2.Text;

            if (string.IsNullOrEmpty(path))
            {
                await DisplayAlert("Image processing", "The path to the file is not specified!", "ОK");
                return;
            }

            if (algorithmPicker.SelectedIndex == 1 && (string.IsNullOrEmpty(argEntry1.Text) || string.IsNullOrEmpty(argEntry2.Text))) return;

            await DisplayAlert("Image processing", "Select folder to save files", "ОK");
            var pathFolder = await FolderPicker.PickAsync(default);

            if (string.IsNullOrEmpty(pathFolder.Folder?.Path))
            {
                await DisplayAlert("Image processing", "The path to the file is not specified!", "ОK");
                return;
            }
            int algorithmIndex = algorithmPicker.SelectedIndex;
            string ext = Path.GetExtension(path).Replace(".", "");
            foreach (var type in typeFiles)
            {
                if (ext.Equals(type))
                {
                    Directory.CreateDirectory(pathFolder.Folder.Path + $"\\{algorithmPicker.SelectedItem}");

                    if (algorithmIndex == 7)
                    {
                        int arg = Int32.Parse(arg1);
                        ExecuteBarcode(path, arg, pathFolder.Folder.Path + $"\\{algorithmPicker.SelectedItem}", outputFile: saveAsPicker.SelectedItem.ToString());
                    }
                    else ExecuteImage(algorithmIndex, path, pathFolder.Folder.Path + $"\\{algorithmPicker.SelectedItem}", arg1, arg2);
                    await DisplayAlert("Image processing", $"Completed successfully and save in {pathFolder.Folder.Path + $"\\{algorithmPicker.SelectedItem}"}", "ОK");
                    return;
                }
            }
            await DisplayAlert("Image processing", "The file does not contain the required extension", "ОK");
        }

        [RequiresAssemblyFiles("Calls System.Reflection.Assembly.Location")]
        private async void ExecuteImage(int algorithmIndex, string pathImage, string folderPath, string arg1, string arg2)
        {

            string pythonPath = "C:\\Users\\" + Environment.UserName + "\\PROG\\";
            pythonPath += algorithmIndex switch
            {
                1 => "SVD.exe",
                2 => "PLaHPF.exe",
                3 => "ButterworthF.exe",
                4 => "SDIFULaplaceF.exe",
                5 => "FDFGaussianF.exe",
                6 => "FDFLaplaceF.exe",
                _ => "DFT.exe",
            };
            System.Diagnostics.ProcessStartInfo procStartInfo = new(pythonPath, $"\"{pathImage}\" \"{folderPath}\" \"{saveAsPicker.SelectedItem}\" \"{arg1}\" \"{arg2}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            System.Diagnostics.Process proc = new()
            {
                StartInfo = procStartInfo
            };
            proc.Start();
            _ = proc.StandardOutput.ReadToEnd();

            await DisplayAlert("Image processing", "Process complete successfully", "ОK");
        }

        private static void ExecuteBarcode(string pathToImg, int length, string pathToSave, string outputFile = "png")
        {
            ISImageContainer container = new(SixLabors.ImageSharp.Image.Load<L8>(pathToImg));
            int width = container.Width;
            int height = container.Height;
            BarCodeLineContainer<byte> barcodeContainer = container.GetBarCode<byte>();

            Image<L8> img = barcodeContainer.BarCodes
                .Where(x => x.Barcode.Length == length)
                .Select(a => a.Barcode)
                .ToImageL8(width, height, false);
            img.Save($"{pathToSave}\\Equals_{length}.{outputFile}");

            Image<L8> img1 = barcodeContainer.BarCodes
                .Where(x => x.Barcode.Length != length)
                .Select(a => a.Barcode)
                .ToImageL8(width, height, false);
            img1.Save($"{pathToSave}\\Another_{length}.{outputFile}");
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
