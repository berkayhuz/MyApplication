namespace Forum.Domain.ValueObjects
{
    public class EmailAddress
    {
        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email cannot be empty.");
            if (!IsValidEmail(value)) throw new ArgumentException("Invalid email format.");

            Value = value;
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        public override string ToString() => Value;
    }
}
