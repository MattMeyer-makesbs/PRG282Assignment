using System;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    // Holds the one and only classification rule set for the centre.
    public static class AnimalClassifier
    {
        public const string StatusCritical = "Critical";
        public const string StatusSerious = "Serious";
        public const string StatusStable = "Stable";
        public const string StatusRecovering = "Recovering";
        public const string StatusReleaseReady = "Release-Ready";

        // All five statuses in report order.
        public static readonly string[] AllStatuses =
        {
            StatusCritical,
            StatusSerious,
            StatusStable,
            StatusRecovering,
            StatusReleaseReady
        };

        // Works out the Status and Housing Unit that belong to a Recovery Score.
        public static void Classify(int recoveryScore, out string status, out string housingUnit)
        {
            if (recoveryScore < 0 || recoveryScore > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(recoveryScore), "Recovery Score must be between 0 and 100.");
            }

            if (recoveryScore <= 19)
            {
                status = StatusCritical;
                housingUnit = "Intensive Care Unit";
            }
            else if (recoveryScore <= 39)
            {
                status = StatusSerious;
                housingUnit = "High-Dependency Ward";
            }
            else if (recoveryScore <= 59)
            {
                status = StatusStable;
                housingUnit = "Recovery Ward";
            }
            else if (recoveryScore <= 79)
            {
                status = StatusRecovering;
                housingUnit = "Outdoor Enclosure";
            }
            else
            {
                status = StatusReleaseReady;
                housingUnit = "Pre-Release Camp";
            }
        }
    }
}
