using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetFilteredCompaniesQueryHandler
{
    Task<PagedResult<Company>> HandleAsync(
        CompanyRequest request,
        CancellationToken cancellationToken = default);
}