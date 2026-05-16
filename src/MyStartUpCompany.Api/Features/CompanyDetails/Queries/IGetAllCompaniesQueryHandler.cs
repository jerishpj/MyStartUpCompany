using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetAllCompaniesQueryHandler
{
    Task<IEnumerable<Company>> HandleAsync(CancellationToken cancellationToken = default);
}
