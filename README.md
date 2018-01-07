# MixinGenerator

- analyzer & code fix: [MixinGenerator](https://www.nuget.org/packages/MixinGenerator/)
- common mixins: [MixinGenerator.Mixins](https://www.nuget.org/packages/MixinGenerator.Mixins/)

## How to use

1. Composition

![Composition](docs/Composition.png)

2. Quick Action (C# Analyzer & Code Fix))

![Code Fix](docs/CodeFix.png)

3. Delegation (Generated Source Code)

![Delegation](docs/Delegation.png)

## Why composition over inheritance

### mixins VS base classes

A class can have multiple mixins.

```cs
// base class
// ❌ multiple inheritance (compilation error)
class Inheritance : BindableBase, DisposableBase
{
}

// mixin
// ⭕ multiple mixins
class Composition
{
    // MixinGenerator/MixinGenerator.Mixins/Mixins/NotifyPropertyChanged.cs
    NotifyPropertyChanged _npc;

    // MixinGenerator/MixinGenerator.Mixins/Mixins/DisposableList.cs
    DisposableList _d;
}

class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

class DisposableBase : IDisposable
{
    private List<IDisposable> _disposables = new List<IDisposable>();

    public void Dispose()
    {
        foreach (var d in _disposables) d?.Dispose();
        _disposables.Clear();
    }
}
```

### mixins VS interface default methods

Mixins can have state.

```cs
// interface
// (C# 8.0 allows interface to have default method implementation)
interface INotifyPropertyChanged
{
    //❌ This is an abstract declaration. There is no backing field in the interface
    event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

interface ICompositeDisposable : IDisposable
{
    //❌ An intarface can't have fields
    private List<IDisposable> _disposables = new List<IDisposable>();

    void Dispose()
    {
        foreach (var d in _disposables) d?.Dispose();
        _disposables.Clear();
    }
}

// mixin
// ⭕ state (fields)
[NonCopyable]
[Mixin]
public struct NotifyPropertyChanged : INotifyPropertyChanged
{
    // has a backing field for the event
    public event PropertyChangedEventHandler PropertyChanged;

    [Accessibility(Accessibility.Protected)]
    public void OnPropertyChanged([This] object @this, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(@this, args);
}

[NonCopyable]
[Mixin]
public struct DisposableList : IDisposable
{
    // has a field
    private List<IDisposable> _list;

    public void Dispose()
    {
        var list = _list;
        _list = null;
        foreach (var d in list) d.Dispose();
    }
}
```
