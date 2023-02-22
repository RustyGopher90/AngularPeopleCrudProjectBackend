using System.Reflection.PortableExecutable;

namespace AngularLearningProjectBackEnd.Models
{
    public class People
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DOB { get; set; } = DateTime.MinValue;
        public string Email { get; set; } = string.Empty;
        public int ID { get; set; } = int.MinValue;

    }
}
