using Mixins;

namespace MixinGenerator
{
    public class Sample
    {
        public NotifyPropertyChangedMixin Mixin1 { get; }

        public CompositeDisposableMixin Mixin2 { get; private set; }

        public LazyMixin<string> Mixin3 => _x;
        private LazyMixin<string> _x;

        public LazyMixin<string> Mixin4 { get { return _y; } }
        private LazyMixin<string> _y;
    }
}
