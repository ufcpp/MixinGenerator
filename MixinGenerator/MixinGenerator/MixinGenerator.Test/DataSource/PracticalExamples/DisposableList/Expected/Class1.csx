﻿using System;
using System.Collections.Generic;

partial class Class1
{
    private DisposableList _dispose;
}

[Mixin]
public struct DisposableList : IDisposable
{
    private List<IDisposable> _list;

    [Accessibility(Accessibility.Protected)]
    public void Add(IDisposable d)
    {
        if (d == null) throw new ArgumentNullException(nameof(d));
        _list = _list ?? new List<IDisposable>();
        _list.Add(d);
    }

    public void Dispose()
    {
        var list = _list;
        _list = null;
        foreach (var d in list) d.Dispose();
    }
}
