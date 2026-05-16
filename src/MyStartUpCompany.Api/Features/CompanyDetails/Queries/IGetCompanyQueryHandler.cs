using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetCompanyQueryHandler
{
    Task<Company> HandleAsync(int companyId, CancellationToken cancellationToken = default);
}
