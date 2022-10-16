namespace API.Models.V1
{
  public interface IUserModel
  {
    public string username { get; set; }
    public string name { get; set; }
    public byte[] hash { get; set; }
    public byte[] salt { get; set; }
  }

  public interface IUserViewModel
  {
    public string username { get; set; }
    public string name { get; set; }
    public string password { get; set; }
  }


  public class UserModel : IUserModel
  {
    public string username { get; set; }
    public string name { get; set; }
    public byte[] hash { get; set; }
    public byte[] salt { get; set; }

    public UserModel()
    {
      username = "";
      name = "";
      hash = new byte[16];
      salt = new byte[16];
    }
  }

  public class UserViewModel
  {
    public string username { get; set; }
    public string name { get; set; }
    public string password { get; set; }

    public UserViewModel()
    {
      username = "";
      name = "";
      password = "";
    }
  }
}
