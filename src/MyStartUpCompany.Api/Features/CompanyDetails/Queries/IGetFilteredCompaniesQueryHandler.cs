using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetFilteredCompaniesQueryHandler
{
    Task<PagedResult<CompanyResponse>> HandleAsync(
        SearchCompanyRequest request,
        CancellationToken cancellationToken = default);
}