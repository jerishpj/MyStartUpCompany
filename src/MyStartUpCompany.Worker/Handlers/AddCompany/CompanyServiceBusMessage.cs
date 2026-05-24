namespace MyStartUpCompany.Worker.Handlers.AddCompany
{
    /// <summary>
    /// Message model for company data received from Azure Service Bus.
    /// This model represents the expected structure of messages on the Service Bus topic.
    /// </summary>
    public class CompanyServiceBusMessage
    {
        /// <summary>
        /// Unique identifier for this message (can be correlation ID or event ID).
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Timestamp when the message was created (ISO 8601 format).
        /// </summary>
        public string? CreatedAt { get; set; }

        /// <summary>
        /// Source system or identifier that sent the message.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The company details to be processed.
        /// </summary>
        public required CompanyInputDto Company { get; set; }
    }

    /// <summary>
    /// Alternative simpler message format where company data is sent directly.
    /// Consumers can use either CompanyServiceBusMessage or CompanyInputDto depending on their message format.
    /// </summary>
    public class CompanyMessageWrapper
    {
        /// <summary>
        /// Gets or sets the company data.
        /// Can be used to deserialize messages that contain company data directly.
        /// </summary>
        public CompanyInputDto? Data { get; set; }

        /// <summary>
        /// Gets or sets audit information about the message.
        /// </summary>
        public MessageMetadata? Metadata { get; set; }
    }

    /// <summary>
    /// Metadata associated with a message for audit and tracking purposes.
    /// </summary>
    public class MessageMetadata
    {
        /// <summary>
        /// Unique message identifier for tracking.
        /// </summary>
        public string? MessageId { get; set; }

        /// <summary>
        /// Correlation ID for tracing related messages and operations.
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// When the message was created (ISO 8601 format).
        /// </summary>
        public string? CreatedAt { get; set; }

        /// <summary>
        /// Source system that generated the message.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Optional message version for handling schema evolution.
        /// </summary>
        public string? Version { get; set; } = "1.0";
    }
}
