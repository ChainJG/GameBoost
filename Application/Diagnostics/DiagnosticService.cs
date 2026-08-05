using GameBoost.Core.Interfaces;
using GameBoost.Shared.Helpers;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Application.Diagnostics
{
    public sealed class DiagnosticService
    {
        public static DiagnosticService Disabled { get; } = new DiagnosticService(new DiagnosticOptions { Enabled = false});

        private readonly DiagnosticOptions _options;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private readonly string _sessionDirectory;
        private readonly string _textPath;

        public bool Enabled => _options.Enabled;

        public string SessionId { get; } =
            DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        public DiagnosticService(DiagnosticOptions options)
        {
            _options = options;

            _sessionDirectory = Path.Combine(
                _options.RootDirectory,
                SessionId);

            _textPath = Path.Combine(
                _sessionDirectory,
                "diagnostics.txt");

            if (_options.Enabled)
                Directory.CreateDirectory(_sessionDirectory);
        }

        public async Task<T> TrackAsync<T>(
            string category,
            DiagnosticOperationType operationType,
            string name,
            string source,
            Func<CancellationToken, Task<T>> operation,
            CancellationToken token = default,
            IReadOnlyDictionary<string, string?>? metadata = null)
        {
            if (!Enabled)
                return await operation(token);

            var startedAtUtc = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await operation(token);

                stopwatch.Stop();

                var success = GetSuccess(result);
                var state = success == false
                    ? ResultType.Failed
                    : ResultType.Successful;

                if (_options.IncludeSuccessfulOperations || state != ResultType.Successful)
                {
                    await WriteEntryAsync(new DiagnosticEntry
                    {
                        SessionId = SessionId,
                        StartedAtUtc = startedAtUtc,
                        CompletedAtUtc = DateTimeOffset.UtcNow,
                        Category = category,
                        OperationType = operationType,
                        Name = name,
                        Source = source,
                        State = state,
                        DurationMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
                        Success = success,
                        ResultStatus = GetStatus(result),
                        Message = GetMessage(result),
                    }, token);
                }

                return result;
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();

                await WriteEntryAsync(new DiagnosticEntry
                {
                    SessionId = SessionId,
                    StartedAtUtc = startedAtUtc,
                    CompletedAtUtc = DateTimeOffset.UtcNow,
                    Category = category,
                    OperationType = operationType,
                    Name = name,
                    Source = source,
                    State = ResultType.Cancelled,
                    DurationMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
                    Success = false,
                    Message = "Operation cancelled",
                    ExceptionType = nameof(OperationCanceledException),
                    ExceptionMessage = "Operation cancelled",
                }, CancellationToken.None);

                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                await WriteEntryAsync(new DiagnosticEntry
                {
                    SessionId = SessionId,
                    StartedAtUtc = startedAtUtc,
                    CompletedAtUtc = DateTimeOffset.UtcNow,
                    Category = category,
                    OperationType = operationType,
                    Name = name,
                    Source = source,
                    State = ResultType.Failed,
                    DurationMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
                    Success = false,
                    Message = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message,
                }, CancellationToken.None);

                throw;
            }
        }

        public async Task TrackAsync(
            string category,
            DiagnosticOperationType operationType,
            string name,
            string source,
            Func<CancellationToken, Task> operation,
            CancellationToken token = default,
            IReadOnlyDictionary<string, string?>? metadata = null)
        {
            await TrackAsync<object?>(
                category,
                operationType,
                name,
                source,
                async innerToken =>
                {
                    await operation(innerToken);
                    return null;
                },
                token,
                metadata);
        }

        /// <summary>
        /// Records a one-off error that is not wrapped in a tracked operation.
        /// Fire-and-forget so logging never blocks or throws into the caller.
        /// </summary>
        public void RecordError(string name, string source, string message)
        {
            if (!Enabled)
                return;

            var now = DateTimeOffset.UtcNow;

            _ = WriteEntryAsync(new DiagnosticEntry
            {
                SessionId = SessionId,
                StartedAtUtc = now,
                CompletedAtUtc = now,
                Category = "Error",
                OperationType = DiagnosticOperationType.Custom,
                Name = name,
                Source = source,
                State = ResultType.Failed,
                Success = false,
                Message = message
            }, CancellationToken.None);
        }

        private async Task WriteEntryAsync(DiagnosticEntry entry, CancellationToken token)
        {
            try
            {
                await _writeLock.WaitAsync(token);
            }
            catch (OperationCanceledException)
            {
                // Lock was never acquired, so there is nothing to release.
                return;
            }

            try
            {
                await File.AppendAllTextAsync(
                    _textPath,
                    FormatTextEntry(entry) + Environment.NewLine,
                    token);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(
                    $"Diagnostics write failed: {ex.Message}");
#endif
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private static string FormatTextEntry(DiagnosticEntry entry)
        {
            string timestamp = $"[{entry.CompletedAtUtc:yyyy-MM-dd HH:mm:ss.fff} UTC]";
            string state = FormatState(entry.State).PadRight(12);
            string duration = MathHelper.FormatMilliseconds(entry.DurationMilliseconds).PadLeft(8);
            string type = $"{entry.Category}/{entry.OperationType}".PadRight(28);
            string name = entry.Name.PadRight(28);

            string output = string.IsNullOrWhiteSpace(entry.Message)
                ? string.Empty
                : $" Output: {entry.Message}";

            return $"{timestamp}  {state}  {duration}  {type}  {name}{output}";
        }

        private static string FormatState(ResultType state)
        {
            return state switch
            {
                ResultType.Successful => "✅ SUCCESS",
                ResultType.Failed => "❌ FAILED",
                ResultType.Cancelled => "⏹ CANCELLED",
                _ => $"❔ {state.ToString().ToUpperInvariant()}"
            };
        }

        private static bool? GetSuccess<T>(T result)
        {
            return result switch
            {
                IResult appResult => appResult.Success,
                _ => null
            };
        }

        private static string? GetStatus<T>(T result)
        {
            return result switch
            {
                IResult appResult => appResult.Status.ToString(),
                _ => null
            };
        }

        private static string? GetMessage<T>(T result)
        {
            return result switch
            {
                IResult appResult => appResult.Message,
                _ => null
            };
        }
    }
}