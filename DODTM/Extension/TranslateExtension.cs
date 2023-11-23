namespace DODTM.Extension;

public class TranslateExtension : IMarkupExtension
{
    public required string Key { get; set; }
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = Translator.Instance,
        };
        return binding;
    }
}
