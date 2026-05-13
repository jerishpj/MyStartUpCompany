namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public interface IGetAllCompaniesQueryHandler
{
    Task<IEnumerable<CompanyDto>> HandleAsync(CancellationToken cancellationToken = default);
}
