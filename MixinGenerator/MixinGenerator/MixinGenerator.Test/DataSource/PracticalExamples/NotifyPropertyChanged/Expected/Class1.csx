using System.ComponentModel;

partial class Class1
{
    private NotifyPropertyChanged _npc;
}

[Mixin]
public struct NotifyPropertyChanged : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [Accessibility(Accessibility.Protected)]
    public void OnPropertyChanged([This] object @this, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(@this, args);

    [Accessibility(Accessibility.Protected)]
    public void OnPropertyChanged([This] object @this, string propertyName) => OnPropertyChanged(@this, new PropertyChangedEventArgs(propertyName));

    [Accessibility(Accessibility.Protected)]
    public void SetProperty<T>(ref T storage, T value, [This] object @this, PropertyChangedEventArgs args)
    {
        if (!EqualityComparer<T>.Default.Equals(storage, value))
        {
            storage = value;
            OnPropertyChanged(@this, args);
        }
    }

    [Accessibility(Accessibility.Protected)]
    public void SetProperty<T>(ref T storage, T value, [This] object @this, string propertyName) => SetProperty(ref storage, value, @this, new PropertyChangedEventArgs(propertyName));
}

namespace System.ComponentModel
{
    public interface INotifyPropertyChanged { event PropertyChangedEventHandler PropertyChanged; }
    public delegate void PropertyChangedEventHandler();
    public class PropertyChangedEventArgs { public PropertyChangedEventArgs(string propertyName) { } }
}
