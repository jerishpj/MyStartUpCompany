using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetAllCompaniesQueryHandler
{
    Task<IEnumerable<CompanyResponse>> HandleAsync(CancellationToken cancellationToken = default);
}
