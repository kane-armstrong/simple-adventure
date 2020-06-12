namespace PetDoctor.Domain.Aggregates.Appointments
{
    public class Owner
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Phone { get; }
        public string Email { get; }

        public Owner(string firstName, string lastName, string phone, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
        }
    }
}
