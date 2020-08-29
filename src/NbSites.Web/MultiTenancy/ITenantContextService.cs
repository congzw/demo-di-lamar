namespace NbSites.Web.MultiTenancy
{
    public interface ITenantContextService
    {
        TenantContext GetCurrentTenantContext();
    }
}