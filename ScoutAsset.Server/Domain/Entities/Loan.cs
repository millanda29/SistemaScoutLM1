using ScoutAsset.Server.Domain.Common;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Domain.Entities;

public class Loan : BaseEntity
{
    public string RequestNumber { get; set; } = string.Empty;
    public string RequesterId { get; set; } = string.Empty;
    public string? ApproverId { get; set; }
    public string? DeliveredById { get; set; }
    public string? ReceivedById { get; set; }
    public string Status { get; set; } = nameof(LoanStatus.SOLICITADO);
    public string Reason { get; set; } = string.Empty;
    public string? ApprovalType { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovalObservations { get; set; }
    public DateTime? ExpectedExitDate { get; set; }
    public DateTime ExpectedReturnDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public string CreatedById { get; set; } = string.Empty;
    public string RequesterType { get; set; } = "INTERNO";
    public string? PdfDocumentPath { get; set; }
    public DateTime? LoanDate { get; set; }

    public ICollection<LoanItem> Items { get; set; } = new List<LoanItem>();

    public bool CanDeliver()
    {
        return Status == nameof(LoanStatus.APROBADO);
    }
}
