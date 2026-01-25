namespace TravelTechApi.Entities
{
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5,
        Expired = 6
    }

    // public enum PaymentMethod
    // {
    //     BankTransfer = 0,
    //     QRCode = 1,
    //     EWallet = 2
    // }
}
