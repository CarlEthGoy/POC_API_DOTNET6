namespace API.Models.V1
{
  public class UserModel
  {
    public string username { get; set; }
    public string name { get; set; }
    public byte[] hash { get; set; }
    public byte[] salt { get; set; }
  }

  public class UserViewModel
  {
    public string username { get; set; }
    public string name { get; set; }
    public string password { get; set; }
  }
}
