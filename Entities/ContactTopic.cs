namespace TravelTechApi.Entities
{
    public class ContactTopic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
    }
}