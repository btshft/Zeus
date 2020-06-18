using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Storage.Models.Alerts
{
    /// <summary>
    /// Alerts channel.
    /// </summary>
    public sealed class AlertsChannel
    {
        /// <summary>
        /// Unique channel name.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Channel name is not provided")]
        [RegularExpression("^[a-zA-Z0-9]{2,}(?:-?[a-zA-Z0-9])*$", ErrorMessage = "Channel name is invalid. Expected format '{1}'")]
        public string Name { get; set; }

        /// <summary>
        /// Is channel enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Channel description.
        /// </summary>
        public string Description { get; set; }

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