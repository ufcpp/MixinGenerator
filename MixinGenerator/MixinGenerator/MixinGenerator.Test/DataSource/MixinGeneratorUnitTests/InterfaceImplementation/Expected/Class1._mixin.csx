using System;
using System.Collections.Generic;
partial class Class1 : System.IDisposable
{
    public void Add(System.IDisposable d) => _mixin.Add(d);
    public void Dispose() => _mixin.Dispose();
}
