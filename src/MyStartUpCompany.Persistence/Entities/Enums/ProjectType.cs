namespace MyStartUpCompany.Persistence.Entities.Enums;

/// <summary>
/// Defines the types or categories of projects.
/// Each type represents a broad classification of work based on domain and purpose.
/// </summary>
public enum ProjectType
{
    /// <summary>
    /// Game development projects - involves game design, engine development, art, audio
    /// </summary>
    GameDevelopment = 0,

    /// <summary>
    /// Cloud service projects - involves cloud infrastructure, migration, and management
    /// </summary>
    CloudService = 1,

    /// <summary>
    /// Customer support projects - involves support tools, ticketing, and customer engagement
    /// </summary>
    CustomerSupport = 2,

    /// <summary>
    /// Data analytics projects - involves data pipeline, analytics, reporting, and BI
    /// </summary>
    DataAnalytics = 3,

    /// <summary>
    /// Infrastructure and DevOps projects - involves CI/CD, monitoring, infrastructure
    /// </summary>
    Infrastructure = 4,

    /// <summary>
    /// Security projects - involves security implementation, compliance, and auditing
    /// </summary>
    Security = 5,

    /// <summary>
    /// Mobile app development projects - involves iOS, Android, or cross-platform development
    /// </summary>
    MobileApp = 6,

    /// <summary>
    /// Web application projects - involves frontend and backend web development
    /// </summary>
    WebApplication = 7,

    /// <summary>
    /// API development projects - involves REST, GraphQL, or other API implementations
    /// </summary>
    ApiDevelopment = 8,

    /// <summary>
    /// Machine learning and AI projects - involves ML models, training, and AI implementations
    /// </summary>
    MachineLearning = 9,

    /// <summary>
    /// Integration and middleware projects - involves system integration and data interchange
    /// </summary>
    Integration = 10,

    /// <summary>
    /// Research and POC (Proof of Concept) projects - involves experimentation and prototyping
    /// </summary>
    Research = 11,

    /// <summary>
    /// Other/miscellaneous projects that don't fit standard categories
    /// </summary>
    Other = 12
}
