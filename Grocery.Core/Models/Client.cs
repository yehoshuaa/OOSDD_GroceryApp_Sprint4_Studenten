
namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        // UC11: role creatie
        public Role Role { get; set; } = Role.None;
        public Client(int id, string name, string emailAddress, string password) : base(id, name)
        {
            EmailAddress=emailAddress;
            Password=password;
        }
    }
}
