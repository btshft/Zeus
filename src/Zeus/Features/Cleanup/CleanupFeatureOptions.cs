namespace Zeus.Features.Cleanup
{
    public class CleanupFeatureOptions
    {
        public OrphanNotificationsOptions OrphanNotifications { get; set; }

        public class OrphanNotificationsOptions
        {
            public string Schedule { get; set; }
        }
    }
}