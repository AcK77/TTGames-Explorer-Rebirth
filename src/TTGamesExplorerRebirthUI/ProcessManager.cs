using System.Diagnostics;

namespace TTGamesExplorerRebirthUI
{
    public class ProcessManager: IDisposable
    {
        private bool _disposedValue;

        private readonly CancellationTokenSource _cancelTokenSource = new();
        private readonly CancellationToken       _cancelToken;

        private string _gameExePath;
        private string _loaderExePath;

        public event EventHandler GameStateChanged;

        public bool IsLoaderRunning { get; private set; }
        public bool IsGameRunning   { get; private set; }

        private bool _isGameRunningOld;

        public ProcessManager(string gameExePath, string loaderExePath)
        {
            _cancelToken = _cancelTokenSource.Token;

            _gameExePath   = gameExePath;
            _loaderExePath = loaderExePath;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                while (!_cancelToken.IsCancellationRequested)
                {
                    IsGameRunning   = Process.GetProcesses().Where(pr => pr.ProcessName == Path.GetFileNameWithoutExtension(_gameExePath)).FirstOrDefault() != null;
                    IsLoaderRunning = Process.GetProcesses().Where(pr => pr.ProcessName == Path.GetFileNameWithoutExtension(_loaderExePath)).FirstOrDefault() != null;

                    if (_isGameRunningOld != IsGameRunning)
                    {
                        OnGameStateChanged(EventArgs.Empty);

                        _isGameRunningOld = IsGameRunning;
                    }

                    Thread.Sleep(100);
                }
            }).Start();
        }

        public void KillAllProcesses()
        {
            foreach (var process in Process.GetProcesses().Where(pr => pr.ProcessName == Path.GetFileNameWithoutExtension(_gameExePath)))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcesses().Where(pr => pr.ProcessName == Path.GetFileNameWithoutExtension(_loaderExePath)))
            {
                process.Kill();
            }
        }

        protected virtual void OnGameStateChanged(EventArgs e)
        {
            GameStateChanged?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _cancelTokenSource.Cancel();
                    _cancelTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
