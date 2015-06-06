using Mixins.Annotations;

namespace MixinGenerator
{
    [Mixin]
    public struct X
    {
        public string PublicValue => null;

        [Protected]
        public string ProtectedValue => null;

        [Private]
        public string PrivateValue => null;
    }

    public class Sample
    {
        X _x;

        public string PublicValue => _x.PublicValue;

        protected string ProtectedValue => _x.ProtectedValue;
    }
}
