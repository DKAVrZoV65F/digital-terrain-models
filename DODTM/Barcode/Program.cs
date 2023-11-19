using BarcodeNet;
using BarcodeNet.ImageSharp;
using BarcodeNet.Managment;
using BarFunctions.ManagedElement;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

BarcodeNet.Internal.WindowsLibraryLoader.Instance.AdditionalPaths.Add(Path.Combine(@"\runtimes\win-x64\native"));

ISImageContainer container = new ISImageContainer(Image.Load<L8>("air1.bmp"));

int width = container.Width;
int height = container.Height;

BarCodeLineContainer<byte>? barcodeContainer = container.GetBarCode<byte>();

int index = 0;


// Отдельные баркоды
foreach (BarCodeLineRelation<byte>? item in barcodeContainer.BarCodes.Take(20)) // Take(20) ограничение до 20 элементов. Можно убрать
{
    Image<L8> img = item.Barcode.ToImageL8(false);
    img.Save($"Figures{index++}.bmp");
}

// Совместно первые две матрицы баркода
IEnumerable<BarCodeLine<byte>> barcodes = new[]
{
    barcodeContainer.BarCodes[0].Barcode,
    barcodeContainer.BarCodes[1].Barcode
};
// Можно также использовать LINQ
barcodes = barcodeContainer.BarCodes
    .Take(2)
    .Select(a => a.Barcode);

Image<L8> img2 = barcodes.ToImageL8(width, height, false);
img2.Save($"Figures_0_1.bmp");

// Отношения без родителей
Image<L8> img3 = barcodeContainer.BarCodes
    .Where(x => x.Parent == null) // Фильтр
    .Select(a => a.Barcode)
    .ToImageL8(width, height, false);
img3.Save($"Figures_no_parent.bmp");

// Отношения без детей
Image<L8> img4 = barcodeContainer.BarCodes
    .Where(x => x.Childs.Length == 0) // Фильтр
    .Select(a => a.Barcode)
    .ToImageL8(width, height, false);
img4.Save($"Figures_no_childs.bmp");

// Добавление фейкового баркода
// Возможно фиговый пример, постараюсь потом сделать, чтобы было удобнее
IEnumerable<BarPointValue<byte>> points = Enumerable.Empty<BarPointValue<byte>>();
var range = Enumerable.Range(20, 200).ToArray();
for (int i = 40; i < 80; i++)
{
    points = points.Concat(range.Select(a => new BarPointValue<byte>(i * width + a, 50)).ToArray());
}
Image<L8> img5 = barcodeContainer.BarCodes
    .Select(x => x.Barcode) // Существующие матрицы баркодов
    .Append(new BarCodeLine<byte>(points)) // Добавление новой матрицы
    .ToImageL8(width, height, false);
img5.Save("Figures_AppendBC.bmp");

Console.WriteLine(barcodeContainer.BarCodes.Length);