using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Storage.Models.Alerts
{
    /// <summary>
    /// Alerts channel.
    /// </summary>
    public class AlertsChannel
    {
        /// <summary>
        /// Unique channel name.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Channel name is not provided")]
        public string Name { get; set; }

        /// <summary>
        /// Is channel enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        public sealed class NameEqualityComparer : IEqualityComparer<AlertsChannel>
        {
            /// <inheritdoc />
            public bool Equals(AlertsChannel x, AlertsChannel y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
            }

            /// <inheritdoc />
            public int GetHashCode(AlertsChannel obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }
    }
}