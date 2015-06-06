using Mixins;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MixinGenerator
{
    public class Sample : INotifyPropertyChanged
    {
        NotifyPropertyChangedMixin _npc;

        public event PropertyChangedEventHandler PropertyChanged { add { _npc.PropertyChanged += value; } remove { _npc.PropertyChanged -= value; } }

        protected void OnPropertyChanged(PropertyChangedEventArgs args) => _npc.OnPropertyChanged(this, args);

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => _npc.OnPropertyChanged(this, propertyName);

        protected void SetProperty<T>(ref T storage, T value, PropertyChangedEventArgs args) => _npc.SetProperty(this, ref storage, value, args);

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) => _npc.SetProperty(this, ref storage, value, propertyName);
    }
}
