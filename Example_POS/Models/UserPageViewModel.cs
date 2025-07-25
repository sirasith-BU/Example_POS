using Example_POS.DTOs;

namespace Example_POS.Models
{
    public class UserPageViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public UpdateUser UpdateForm { get; set; }
    }
}
