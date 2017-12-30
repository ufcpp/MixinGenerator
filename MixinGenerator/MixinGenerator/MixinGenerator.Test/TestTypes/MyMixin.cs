using MixinGenerator.Annotations;

namespace MixinGenerator.Test.TestTypes
{
    [Mixin]
    public struct M1
    {
        public int A { get; }
    }

    [Mixin("PlaceHolder")]
    public struct M2
    {
        public int AbcPlaceHolderDef { get; }
    }
}
