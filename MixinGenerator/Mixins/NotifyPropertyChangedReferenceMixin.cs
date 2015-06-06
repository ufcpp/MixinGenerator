using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mixins.Annotations;

namespace Mixins
{
    /// <summary>
    /// <see cref="object.ReferenceEquals(object, object)"/> version.
    /// </summary>
    [Mixin]
    public struct NotifyPropertyChangedReferenceMixin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Protected]
        public void OnPropertyChanged([This] object @this, PropertyChangedEventArgs args)
            => PropertyChanged?.Invoke(@this, args);

        [Protected]
        public void OnPropertyChanged([This] object @this, [CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(@this, new PropertyChangedEventArgs(propertyName));

        [Protected]
        public void SetProperty<T>([This] object @this, ref T storage, T value, PropertyChangedEventArgs args)
        {
            if (!ReferenceEquals(storage, value))
            {
                storage = value;
                OnPropertyChanged(@this, args);
            }
        }

        [Protected]
        public void SetProperty<T>([This] object @this, ref T storage, T value, [CallerMemberName] string propertyName = null)
            => SetProperty(@this, ref storage, value, new PropertyChangedEventArgs(propertyName));
    }
}
