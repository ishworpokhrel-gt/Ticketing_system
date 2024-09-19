namespace HEI.Support.Areas.Admin.Models
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public List<string> CurrentRoles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
}
