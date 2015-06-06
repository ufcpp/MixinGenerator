using Mixins;
using System;
using System.Collections.Generic;

namespace MixinGenerator
{
    public class Sample1 : IDisposable
    {
        CompositeDisposableMixin _disposables;

        protected ICollection<IDisposable> Disposables => _disposables.Disposables;

        public void Dispose() => _disposables.Dispose();
    }

    public class Sample1 : IDisposable
    {
        CompositeDisposableMixin _a;

        protected ICollection<IDisposable> A => _disposables.Disposables;

        public void Dispose()
        {
            _a.Dispose();
            _b.Dispose();
        }

        CompositeDisposableMixin _b;

        protected ICollection<IDisposable> B => _disposables.Disposables;

        protected void DisposeA() => _a.Dispose();

        protected void DisposeB() => _b.Dispose();
    }
}
