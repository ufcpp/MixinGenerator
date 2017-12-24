partial class Class1
{
    public void Log(string message) => _mixin.Log(this, message);
    public void Log() => _mixin.Log(this, this);
}
