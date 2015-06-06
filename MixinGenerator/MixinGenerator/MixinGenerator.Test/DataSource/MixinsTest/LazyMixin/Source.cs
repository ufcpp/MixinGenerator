using Mixins;

namespace MixinGenerator
{
    public class Buffer
    {
        byte[] _data = new byte[100];

        public byte this[int i] => _data[i];
    }

    public class Sample
    {
        LazyMixin<Buffer> _buffer;
    }
}
