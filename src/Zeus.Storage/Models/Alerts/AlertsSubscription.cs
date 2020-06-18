using System;

namespace Zeus.Storage.Models.Alerts
{
    /// <summary>
    /// Alerts subscription.
    /// </summary>
    public sealed class AlertsSubscription : IEquatable<AlertsSubscription>
    {
        /// <summary>
        /// Chat id to send messages.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// User subscribed to channel.
        /// </summary>
        public string ChatName { get; set; }

        /// <summary>
        /// Channel name.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Disable notifications about alerts.
        /// </summary>
        public bool DisableNotifications { get; set; }

        /// <inheritdoc />
        public bool Equals(AlertsSubscription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ChatId == other.ChatId && Channel == other.Channel;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AlertsSubscription)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(ChatId, Channel);
        }
    }
}