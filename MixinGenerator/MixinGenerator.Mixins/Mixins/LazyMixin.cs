using MixinGenerator.Annotations;
using System;

namespace MixinGenerator.Mixins
{
    [NonCopyable]
    [Mixin("Value")]
    public struct LazyMixin<T>
        where T : class, new()
    {
        private T _value;

        /// <summary>
        /// Gets the lazily initialized value.
        /// </summary>
        public T Value => _value ?? (_value = new T());

        /// <summary>
        /// Invokes value.Dispose if _value != null, then resets value to null.
        /// </summary>
        /// <remarks>
        /// This struct can be resused. If you use the Value after invoking the Dispose, the Value is re-initialized.
        /// </remarks>
        [Ignore]
        public void Clear()
        {
            if (_value != null)
            {
                (_value as IDisposable)?.Dispose();
                _value = null;
            }
        }

        [Ignore]
        public bool IsInitialized => _value != null;
    }
}
