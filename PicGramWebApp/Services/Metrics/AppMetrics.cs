using System.Collections.Concurrent;

namespace PicGramWebApp.Services.Metrics
{
    public class AppMetrics
    {
        private readonly ConcurrentDictionary<string, int> _actionCounts = new();
        private readonly ConcurrentDictionary<string, List<long>> _executionTimes = new();

        public void IncrementAction(string actionName)
        {
            _actionCounts.AddOrUpdate(actionName, 1, (_, current) => current + 1);
        }

        public int GetActionCount(string actionName)
        {
            return _actionCounts.TryGetValue(actionName, out var count) ? count : 0;
        }

        public IReadOnlyDictionary<string, int> GetAllActionCounts()
        {
            return new Dictionary<string, int>(_actionCounts);
        }

        public void RecordExecutionTime(string actionName, long milliseconds)
        {
            _executionTimes.AddOrUpdate(
                actionName,
                _ => new List<long> { milliseconds },
                (_, existing) =>
                {
                    lock (existing)
                    {
                        existing.Add(milliseconds);
                    }

                    return existing;
                });
        }

        public double GetAverageExecutionTime(string actionName)
        {
            if (!_executionTimes.TryGetValue(actionName, out var values))
            {
                return 0;
            }

            lock (values)
            {
                return values.Count == 0 ? 0 : values.Average();
            }
        }

        public IReadOnlyDictionary<string, double> GetAllAverageExecutionTimes()
        {
            var result = new Dictionary<string, double>();

            foreach (var pair in _executionTimes)
            {
                lock (pair.Value)
                {
                    result[pair.Key] = pair.Value.Count == 0 ? 0 : pair.Value.Average();
                }
            }

            return result;
        }
    }
}