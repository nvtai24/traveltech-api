namespace TravelTechApi.Entities
{
    public class FAQ
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int DestinationId { get; set; }
        public virtual Destination Destination { get; set; } = null!;
    }
}