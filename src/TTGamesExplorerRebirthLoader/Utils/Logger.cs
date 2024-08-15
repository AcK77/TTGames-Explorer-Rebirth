using System.IO.MemoryMappedFiles;
using System.Text;

namespace TTGamesExplorerRebirthLoader.Utils
{
    public class Logger : IDisposable
    {
        private const int Size = 0x2000;

        private readonly Semaphore _semaphoreRead;
        private readonly Semaphore _semaphoreWrite;

        private readonly MemoryMappedFile _mmFile;
        private readonly MemoryMappedViewStream _mmStream;

        private readonly CancellationTokenSource _cancelTokenSource;

        public event EventHandler<string> ReadingCompleted;

#pragma warning disable CA1416
        public Logger()
        {
            _cancelTokenSource = new CancellationTokenSource();

            _mmFile = MemoryMappedFile.CreateOrOpen("TTGamesExplorerRebirthLauncherSharedMem", Size);
            _mmStream = _mmFile.CreateViewStream(0, Size, MemoryMappedFileAccess.ReadWrite);

            _semaphoreRead = new Semaphore(0, 1, "TTGamesExplorerRebirthLoggerSemaphoreRead");
            _semaphoreWrite = new Semaphore(0, 1, "TTGamesExplorerRebirthLoggerSemaphoreWrite");

            _semaphoreWrite.Release();

            StartReading();
        }
#pragma warning restore CA1416

        public void StartReading()
        {
            CancellationToken token = _cancelTokenSource.Token;

            new Thread(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    _semaphoreRead.WaitOne();

                    byte[] buffer = new byte[Size];

                    _mmStream.Seek(0, SeekOrigin.Begin);
                    _mmStream.Read(buffer, 0, buffer.Length);

                    string text = Encoding.ASCII.GetString(buffer).TrimEnd('\0');

                    _mmStream.Seek(0, SeekOrigin.Begin);
                    _mmStream.Write(new byte[Size], 0, Size);

                    _semaphoreWrite.Release();

                    if (text != "")
                    {
                        OnReadingCompleted(text);
                    }
                }
            }).Start();
        }

        protected virtual void OnReadingCompleted(string message)
        {
            ReadingCompleted?.Invoke(this, message);
        }

        public void Dispose()
        {
            _cancelTokenSource.Dispose();
            _mmStream.Dispose();
            _mmFile.Dispose();
        }
    }
}