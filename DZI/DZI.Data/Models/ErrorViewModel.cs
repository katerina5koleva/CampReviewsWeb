namespace CampRating.Data.Models;

/// <summary>
/// Represents an error view model used to display error information in the application.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the request that caused the error.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the RequestId should be displayed.
    /// </summary>
    /// <remarks>
    /// Returns true if the RequestId is not null or empty; otherwise, false.
    /// </remarks>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
