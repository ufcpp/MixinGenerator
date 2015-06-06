using Mixins.Annotations;

namespace MixinGenerator
{
    [Mixin]
    public struct X
    {
        [Protected]
        [Private]
        public string Item1 => null;

        [Private, Protected]
        public string Item2 => null;
    }
}
