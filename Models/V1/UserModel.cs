namespace API.Models.V1
{
  public interface IUserModel
  {
    int Id { get; set; }
    string Username { get; set; }
    string Name { get; set; }
    byte[] Hash { get; set; }
    byte[] Salt { get; set; }

  }

  public interface IUserViewModel
  {
    string Username { get; set; }
    string Name { get; set; }
    string Password { get; set; }
  }


  public class UserModel : IUserModel
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }

    public UserModel()
    {
      this.Id = 0;
      this.Username = "";
      this.Name = "";
      this.Hash = new byte[16];
      this.Salt = new byte[16];
    }

    public UserModel(int id, string username, string name, byte[] hash, byte[] salt)
    {
      this.Id = id;
      this.Username = username;
      this.Name = name;
      this.Hash = hash;
      this.Salt = salt;
    }
  }

  public class UserViewModel : IUserViewModel
  {
    public string Username { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }

    public UserViewModel()
    {
      this.Username = "";
      this.Name = "";
      this.Password = "";
    }

    public UserViewModel(string username, string name, string password)
    {
      this.Username = username;
      this.Name = name;
      this.Password = password;
    }
  }
}
