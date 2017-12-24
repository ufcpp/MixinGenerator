partial class Class1
{
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add => _mixin.PropertyChanged += value; remove => _mixin.PropertyChanged -= value; }
}
