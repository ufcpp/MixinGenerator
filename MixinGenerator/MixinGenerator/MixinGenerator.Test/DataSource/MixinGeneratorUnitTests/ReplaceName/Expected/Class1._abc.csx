partial class Class1
{
    public X Abc => _abc.Value;
    public void DisposeAbc() => _abc.DisposeValue();
    public bool IsAbcInitialized => _abc.IsValueInitialized;
}
