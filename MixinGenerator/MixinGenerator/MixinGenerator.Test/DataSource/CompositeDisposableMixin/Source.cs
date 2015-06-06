using Mixins;

namespace MixinGenerator
{
    public class Sample1
    {
        CompositeDisposableMixin _disposables;
    }

    public class Sample1
    {
        CompositeDisposableMixin _a;
        CompositeDisposableMixin _b;

        protected void DisposeA() => _a.Dispose();

        protected void DisposeB() => _b.Dispose();
    }
}
