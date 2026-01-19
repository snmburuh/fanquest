namespace FanQuest.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string PhoneNumber { get; private set; }
        public string DisplayName { get; private set; }
        public int TotalPoints { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected User() { } // EF Core

        public User(string phoneNumber, string displayName)
        {
            Id = Guid.NewGuid();
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            TotalPoints = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddPoints(int points)
        {
            if (points < 0) throw new ArgumentException("Points cannot be negative", nameof(points));
            TotalPoints += points;
        }
    }
}
