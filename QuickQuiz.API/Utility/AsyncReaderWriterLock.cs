namespace QuickQuiz.API.Utility
{
    public class AsyncReaderWriterLock
    {
        private readonly SemaphoreSlim _readerGate = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _writerGate = new SemaphoreSlim(1, 1);
        private int _readerCount = 0;

        public async Task<IAsyncDisposable> ReaderLockAsync()
        {
            await _readerGate.WaitAsync().ConfigureAwait(false);

            try
            {
                if (Interlocked.Increment(ref _readerCount) == 1)
                    await _writerGate.WaitAsync().ConfigureAwait(false);
            }
            finally
            {
                _readerGate.Release();
            }

            return new Releaser(this, isWriter: false);
        }

        public async Task<IAsyncDisposable> WriterLockAsync()
        {
            await _readerGate.WaitAsync().ConfigureAwait(false);
            await _writerGate.WaitAsync().ConfigureAwait(false);
            return new Releaser(this, isWriter: true);
        }

        private async Task ReleaseReaderAsync()
        {
            await _readerGate.WaitAsync().ConfigureAwait(false);

            try
            {
                if (Interlocked.Decrement(ref _readerCount) == 0)
                    _writerGate.Release();
            }
            finally
            {
                _readerGate.Release();
            }
        }

        private void ReleaseWriter()
        {
            _writerGate.Release();
            _readerGate.Release();
        }

        private struct Releaser : IAsyncDisposable
        {
            private readonly AsyncReaderWriterLock _lock;
            private readonly bool _isWriter;

            internal Releaser(AsyncReaderWriterLock @lock, bool isWriter)
            {
                _lock = @lock;
                _isWriter = isWriter;
            }

            public async ValueTask DisposeAsync()
            {
                if (_isWriter)
                {
                    _lock.ReleaseWriter();
                }
                else
                {
                    await _lock.ReleaseReaderAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
