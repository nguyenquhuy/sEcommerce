using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.AdminUsers.ListUsers;

/// <summary>Query: paged user list with keyword (email/name) and role filter (UC-33).</summary>
public record ListUsersQuery(string? Keyword = null, string? Role = null, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<UserSummaryDto>>;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PagedResult<UserSummaryDto>>
{
    private readonly IAppDbContext _db;

    public ListUsersQueryHandler(IAppDbContext db) => _db = db;

    public async Task<PagedResult<UserSummaryDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var query = _db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Role))
            query = query.Where(u => u.Role == request.Role);
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var kw = request.Keyword.Trim();
            query = query.Where(u => u.Email.Contains(kw) || u.FullName.Contains(kw));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role,
                IsActive = u.IsActive,
                IsEmailVerified = u.IsEmailVerified,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<UserSummaryDto>(items, page, pageSize, total);
    }
}
