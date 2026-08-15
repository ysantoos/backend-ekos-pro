namespace Infrastructure.Entity.Constants;

/// <summary>
/// Database configuration constants for Entity Framework
/// </summary>
public static class DatabaseConstants
{
    /// <summary>
    /// Default string length for names and titles
    /// </summary>
    public const int NameLength = 100;

    /// <summary>
    /// Default string length for descriptions
    /// </summary>
    public const int DescriptionLength = 500;

    /// <summary>
    /// Default string length for long text fields
    /// </summary>
    public const int LongTextLength = 2000;

    /// <summary>
    /// Default string length for email addresses
    /// </summary>
    public const int EmailLength = 256;

    /// <summary>
    /// Default string length for phone numbers
    /// </summary>
    public const int PhoneLength = 20;

    /// <summary>
    /// Default string length for codes
    /// </summary>
    public const int CodeLength = 50;

    /// <summary>
    /// Default precision for decimal fields (money)
    /// </summary>
    public const int DecimalPrecision = 18;

    /// <summary>
    /// Default scale for decimal fields (money)
    /// </summary>
    public const int DecimalScale = 2;

    /// <summary>
    /// Schema name for application tables
    /// </summary>
    public const string DefaultSchema = "dbo";
}
