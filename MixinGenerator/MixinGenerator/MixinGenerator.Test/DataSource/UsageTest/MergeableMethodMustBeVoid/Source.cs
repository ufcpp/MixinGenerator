using Mixins;

namespace MixinGenerator
{
    public struct X
    {
        [Mergeable]
        void M1() { } // OK

        [Mergeable]
        int M2() => 0 // NG
    }
}
