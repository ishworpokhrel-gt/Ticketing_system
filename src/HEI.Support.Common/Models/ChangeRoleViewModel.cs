namespace HEI.Support.Common.Models
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public List<string> CurrentRoles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
        public string SelectedRole { get; set; }
    }
}
