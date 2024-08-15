using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace TTGamesExplorerRebirthHook.Utils
{
    public class Logger : IDisposable
    {
        private const int Size = 0x2000;

        private readonly Semaphore _semaphoreRead;
        private readonly Semaphore _semaphoreWrite;

        private readonly MemoryMappedFile _mmFile;
        private readonly MemoryMappedViewStream _mmStream;

        private static Logger _instance;

        public static Logger Instance
        {
            get
            {
                _instance = _instance ?? new Logger();

                return _instance;
            }
        }

        private Logger()
        {
            _mmFile = MemoryMappedFile.CreateOrOpen("TTGamesExplorerRebirthLauncherSharedMem", Size);
            _mmStream = _mmFile.CreateViewStream(0, Size, MemoryMappedFileAccess.ReadWrite);

            _semaphoreRead = Semaphore.OpenExisting("TTGamesExplorerRebirthLoggerSemaphoreRead");
            _semaphoreWrite = Semaphore.OpenExisting("TTGamesExplorerRebirthLoggerSemaphoreWrite");
        }

        public void Log(string message)
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name.Replace("_", "::");

            byte[] bytes = Encoding.ASCII.GetBytes($"{(methodName == ".ctor" ? "" : $"{methodName}() -> ")}{message}");

            _semaphoreWrite.WaitOne();

            _mmStream.Seek(0, SeekOrigin.Begin);
            _mmStream.Write(new byte[Size], 0, Size);

            _mmStream.Seek(0, SeekOrigin.Begin);
            _mmStream.Write(bytes, 0, bytes.Length);

            _semaphoreRead.Release();
        }

        public void Dispose()
        {
            _mmStream.Dispose();
            _mmFile.Dispose();
        }
    }
}