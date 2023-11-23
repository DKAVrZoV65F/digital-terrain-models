using DODTM.Resources.Strings;
using System.Globalization;

namespace DODTM.Extension;

public class Translator
{
    public string this[string key] => AppResources.ResourceManager.GetString(key, CultureInfo);
    public CultureInfo? CultureInfo { get; set; }
    public static Translator Instance { get; set; } = new Translator();
}
