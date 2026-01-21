namespace TravelTechApi.DTOs.Quota;

public class QuotaResponse
{
    public int Limit { get; set; }
    public int CurrentUsage { get; set; }
    public bool HasRemainingQuota { get; set; }
}