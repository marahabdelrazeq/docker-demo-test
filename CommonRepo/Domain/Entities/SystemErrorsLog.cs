using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonRepo.Domain.Entities;
public class SystemErrorsLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string ErrorDetails { get; set; }
    public string ErrorTagContext { get; set; }
    public string Args { get; set; }
    public string ControllerName { get; set; }
    public string RequestAPI { get; set; }
    public string LocalIpAddress { get; set; }
    public string RemoteIpAddress { get; set; }
    public Int64 UserId { get; set; }
    public string Browser { get; set; }
    public string ErrorSource { get; set; }
}

public class SystemErrorsLogFile
{
    public string InnerErrorDetails { get; set; }
    public string InnerErrorTagContext { get; set; }
    public string OuterErrorDetails { get; set; }
    public string OuterErrorTagContext { get; set; }
    public string Args { get; set; }
    public string ControllerName { get; set; }
    public string RequestAPI { get; set; }
    public string LocalIpAddress { get; set; }
    public string RemoteIpAddress { get; set; }
    public Int64 UserId { get; set; }
    public string Browser { get; set; }
    public string ErrorSource { get; set; }
}

public class SystemErrorsLogDTO
{
    public Exception InnerEx { get; set; }
    public string ControllerName { get; set; }
    public string RequestAPI { get; set; }
    public object Args { get; set; }
}

