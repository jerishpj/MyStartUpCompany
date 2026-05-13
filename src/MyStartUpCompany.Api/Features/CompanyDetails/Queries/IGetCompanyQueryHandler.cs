namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetCompanyQueryHandler
{
    Task<CompanyDto> HandleAsync(int companyId, CancellationToken cancellationToken = default);
}
