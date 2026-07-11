namespace CommonLibrary.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(
                 string to,
                 string subject,
                 string body,
                 string? filePathOrUrl = null,
                 string? attachmentName = null,
                 byte[]? attachmentBytes = null);
    }
}
