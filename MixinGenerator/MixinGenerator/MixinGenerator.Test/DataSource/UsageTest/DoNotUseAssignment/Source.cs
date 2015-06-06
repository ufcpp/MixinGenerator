using Mixins;

namespace MixinGenerator
{
    public class Sample1
    {
        private CompositeDisposableMixin _mixin;

        public void X()
        {
            _mixin = new CompositeDisposableMixin();
        }

        public void X()
        {
            var x = _mixin;
            x.Dispose();
        }
    }
}
