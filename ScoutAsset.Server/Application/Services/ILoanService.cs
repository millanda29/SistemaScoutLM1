using ScoutAsset.Server.Application.Models.Loans;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Application.Services;

public interface ILoanService
{
    Task<List<Loan>> GetAllAsync(string? status);
    Task<Loan?> GetByIdAsync(int id);
    Task<Loan> CreateRequestAsync(RequestLoanRequest request, string currentUserId);
    Task ApproveAsync(int id, ApproveLoanRequest request, string currentUserId);
    Task RejectAsync(int id, RejectLoanRequest request, string currentUserId);
    Task DeliverAsync(int id, string currentUserId);
    Task ReturnLoanAsync(int id, ReturnLoanRequest request, string currentUserId);
}
