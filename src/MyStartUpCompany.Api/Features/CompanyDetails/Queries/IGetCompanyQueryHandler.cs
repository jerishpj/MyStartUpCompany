using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetCompanyQueryHandler
{
    Task<CompanyResponse> HandleAsync(int companyId, CancellationToken cancellationToken = default);
}
