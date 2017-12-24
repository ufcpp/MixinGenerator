using System;
using System.Collections.Generic;
partial class Class1 : System.IDisposable
{
    protected void Add(System.IDisposable d) => _dispose.Add(d);
    public void Dispose() => _dispose.Dispose();
}
