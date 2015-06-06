using System;
using System.Collections.Generic;
using Mixins.Annotations;

namespace Mixins
{
    [Mixin]
    public struct CompositeDisposableMixin : IDisposable
    {
        [PrimaryProperty]
        [Protected]
        public ICollection<IDisposable> Disposables => _disposables ?? (_disposables = new List<IDisposable>());

        private List<IDisposable> _disposables;

        [Mergeable]
        public void Dispose()
        {
            foreach (var d in _disposables) d?.Dispose();
            _disposables.Clear();
        }
    }
}
