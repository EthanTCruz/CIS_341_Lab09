using System.ComponentModel;

namespace Lab8.Models
{
    public class Manager
    {
        [DisplayName("Manager ID")]
        public int ManagerID { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }



    }
}
