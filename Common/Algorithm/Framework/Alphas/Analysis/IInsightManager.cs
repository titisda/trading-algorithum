using System;
using System.Collections.Generic;

namespace QuantConnect.Algorithm.Framework.Alphas.Analysis
{
    /// <summary>
    /// Encapsulates the storage and on-line scoring of insights.
    /// </summary>b
    public interface IInsightManager
    {
        /// <summary>
        /// Enumerable of insights still under analysis
        /// </summary>
        IEnumerable<Insight> OpenInsights { get; }

        /// <summary>
        /// Enumerable of insights who's analysis has been completed
        /// </summary>
        IEnumerable<Insight> ClosedInsights { get; }

        /// <summary>
        /// Enumerable of all internally maintained insights
        /// </summary>
        IEnumerable<Insight> AllInsights { get; }

        /// <summary>
        /// Gets the unique set of symbols from analysis contexts that will
        /// </summary>
        /// <param name="frontierTimeUtc"></param>
        /// <returns></returns>
        IEnumerable<InsightAnalysisContext> ContextsOpenAt(DateTime frontierTimeUtc);

        /// <summary>
        /// Add an extension to this manager
        /// </summary>
        /// <param name="extension">The extension to be added</param>
        void AddExtension(IInsightManagerExtension extension);

        /// <summary>
        /// Initializes any extensions for the specified backtesting range
        /// </summary>
        /// <param name="start">The start date of the backtest (current time in live mode)</param>
        /// <param name="end">The end date of the backtest (<see cref="Time.EndOfTime"/> in live mode)</param>
        /// <param name="current">The algorithm's current utc time</param>
        void InitializeExtensionsForRange(DateTime start, DateTime end, DateTime current);

        /// <summary>
        /// Steps the manager forward in time, accepting new state information and potentialy newly generated insights
        /// </summary>
        /// <param name="frontierTimeUtc">The frontier time of the insight analysis</param>
        /// <param name="securityValuesCollection">Snap shot of the securities at the frontier time</param>
        /// <param name="generatedInsights">Any insight generated by the algorithm at the frontier time</param>
        void Step(DateTime frontierTimeUtc, ReadOnlySecurityValuesCollection securityValuesCollection, GeneratedInsightsCollection generatedInsights);

        /// <summary>
        /// Removes insights from the manager with the specified ids
        /// </summary>
        /// <param name="insightIds">The insights ids to be removed</param>
        void RemoveInsights(IEnumerable<Guid> insightIds);

        /// <summary>
        /// Gets all insight analysis contexts that have been updated since this method's last invocation.
        /// Contexts are marked as not updated during the enumeration, so in order to remove a context from
        /// the updated set, the enumerable must be enumerated.
        /// </summary>
        /// <returns></returns>
        IEnumerable<InsightAnalysisContext> GetUpdatedContexts();
    }
}
