namespace MyStartUpCompany.Worker.Configuration
{
    /// <summary>
    /// Configuration settings for Azure Service Bus connection and topic/subscription details.
    /// </summary>
    public class AzureServiceBusSettings
    {
        public const string SectionName = "AzureServiceBus";

        /// <summary>
        /// Connection string for Azure Service Bus namespace.
        /// Format: Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=[key-name];SharedAccessKey=[key]
        /// </summary>
        public required string ConnectionString { get; set; }

        /// <summary>
        /// Name of the topic to subscribe to.
        /// </summary>
        public required string TopicName { get; set; }

        /// <summary>
        /// Name of the subscription to receive messages from.
        /// </summary>
        public required string SubscriptionName { get; set; }

        /// <summary>
        /// Maximum number of concurrent message processing operations. Default: 1
        /// </summary>
        public int MaxConcurrentCalls { get; set; } = 1;

        /// <summary>
        /// Maximum number of messages to load into memory before processing. Default: 1
        /// </summary>
        public int MaxAutoLockRenewalDuration { get; set; } = 300;

        /// <summary>
        /// Enable automatic completion of messages after processing. Default: false (manual completion recommended)
        /// </summary>
        public bool AutoCompleteMessages { get; set; } = false;
    }
}
