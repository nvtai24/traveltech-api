namespace TravelTechApi.Common.Settings
{
    public class SepaySettings
    {
        public string ApiToken { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = "https://my.sepay.vn/userapi";
        public string QRCodeBaseUrl { get; set; } = "https://img.vietqr.io/image";
    }
}
