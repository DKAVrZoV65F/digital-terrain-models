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
        readonly string[] algorithms = ["DFT", "SVD", "PLaHPF", "ButterworthF", "SDIFULaplaceF", "FDFGaussianF", "FDFLaplaceF", "Barcode"];
        string path = "";
        string? selectedAlg = "";
        string? selectedOutput = "";

        public MainPage()
        {
            InitializeComponent();
            algorithmList.ItemsSource = algorithms;
            saveList.ItemsSource = typeFiles;

            label.IsVisible = false;
            switcher.IsVisible = false;
            argEntry1.IsVisible = false;
            argEntry2.IsVisible = false;
        }

        private async void SelectedPathClicked(object sender, EventArgs e)
        {
            var filePath = await FilePicker.PickAsync(default);
            if (!string.IsNullOrEmpty(filePath?.ToString()))
            {
                path = filePath.FullPath;
                imageProcess.Source = path;
            }
            return;
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
            if (selectedAlg == null) return;

            bool mask = switcher.IsChecked;
            string arg1 = (mask && selectedAlg.Equals("DFT")) ? "True" : (!mask && selectedAlg.Equals("DFT")) ? "" : argEntry1.Text;
            string arg2 = argEntry2.Text;

            if (string.IsNullOrEmpty(path))
            {
                await DisplayAlert("Image processing", "The path to the file is not specified!", "ОK");
                return;
            }

            if (selectedAlg.Equals("SVD") && (string.IsNullOrEmpty(argEntry1.Text) || string.IsNullOrEmpty(argEntry2.Text))) return;
            var pathFolder = await FolderPicker.PickAsync(default);

            if (string.IsNullOrEmpty(pathFolder.Folder?.Path))
            {
                await DisplayAlert("Image processing", "The path to the file is not specified!", "ОK");
                return;
            }
            string ext = Path.GetExtension(path).Replace(".", "");
            foreach (var type in typeFiles)
            {
                if (ext.Equals(type))
                {
                    Directory.CreateDirectory(pathFolder.Folder.Path + $"\\{selectedAlg}");

                    if (selectedAlg.Equals("DFT"))
                    {
                        int arg = Int32.Parse(arg1);
                        ExecuteBarcode(path, arg, pathFolder.Folder.Path + $"\\{selectedAlg}", outputFile: (string)saveList.SelectedItem);
                    }
                    else ExecuteImage(selectedAlg, path, pathFolder.Folder.Path + $"\\{selectedAlg}", arg1, arg2);
                    await DisplayAlert("Image processing", $"Completed successfully and save in {pathFolder.Folder.Path + $"\\{selectedAlg}"}", "ОK");
                    return;
                }
            }
            await DisplayAlert("Image processing", "The file does not contain the required extension", "ОK");
        }

        [RequiresAssemblyFiles("Calls System.Reflection.Assembly.Location")]
        private void ExecuteImage(string algorithmSelect, string pathImage, string folderPath, string arg1, string arg2)
        {
            string pythonPath = "C:\\Users\\" + Environment.UserName + "\\PROG\\";
            pythonPath += algorithmSelect switch
            {
                "SVD" => "SVD.exe",
                "PLaHPF" => "PLaHPF.exe",
                "ButterworthF" => "ButterworthF.exe",
                "SDIFULaplaceF" => "SDIFULaplaceF.exe",
                "FDFGaussianF" => "FDFGaussianF.exe",
                "FDFLaplaceF" => "FDFLaplaceF.exe",
                _ => "DFT.exe",
            };
            System.Diagnostics.ProcessStartInfo procStartInfo = new(pythonPath, $"\"{pathImage}\" \"{folderPath}\" \"{saveList.SelectedItem}\" \"{arg1}\" \"{arg2}\"")
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
