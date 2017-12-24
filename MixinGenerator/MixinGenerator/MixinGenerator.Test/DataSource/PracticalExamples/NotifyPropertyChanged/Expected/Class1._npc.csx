using System.ComponentModel;
partial class Class1 : System.ComponentModel.INotifyPropertyChanged
{
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add => _npc.PropertyChanged += value; remove => _npc.PropertyChanged -= value; }
    protected void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args) => _npc.OnPropertyChanged(this, args);
    protected void OnPropertyChanged(string propertyName) => _npc.OnPropertyChanged(this, propertyName);
    protected void SetProperty<T>(ref T storage, T value, System.ComponentModel.PropertyChangedEventArgs args) => _npc.SetProperty<T>(ref storage, value, this, args);
    protected void SetProperty<T>(ref T storage, T value, string propertyName) => _npc.SetProperty<T>(ref storage, value, this, propertyName);
}
