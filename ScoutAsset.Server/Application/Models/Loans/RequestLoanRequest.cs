namespace ScoutAsset.Server.Application.Models.Loans;

public record RequestLoanRequest(
    string Reason, 
    DateTime? ExpectedExitDate, 
    DateTime ExpectedReturnDate, 
    List<int> ResourceIds,
    string RequesterType,
    DateTime? LoanDate,
    string? PdfDocumentPath
);
