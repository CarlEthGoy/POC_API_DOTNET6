namespace API.Models.V1
{
  public interface IPasswordModel
  {
    int Id { get; set; }
    string Application_name { get; set; }
    string Username { get; set; }
    string Encrypted_password { get; set; }
  }
  public interface IPasswordViewModel
  {
    string Application_name { get; set; }
    string Username { get; set; }
    string Password { get; set; }
  }

  public class PasswordModel : IPasswordModel
  {
    public int Id { get; set; }
    public string Application_name { get; set; }
    public string Username { get; set; }
    public string Encrypted_password { get; set; }

    public PasswordModel()
    {
      this.Id = 0;
      this.Application_name = "";
      this.Username = "";
      this.Encrypted_password = "";
    }

    public PasswordModel(int id, string application_name, string username, string encrypted_password)
    {
      this.Id = id;
      this.Application_name = application_name;
      this.Username = username;
      this.Encrypted_password = encrypted_password;
    }
  }

  public class PasswordViewModel : IPasswordViewModel
  {
    public string Application_name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public PasswordViewModel()
    {
      this.Application_name = "";
      this.Username = "";
      this.Password = "";
    }

    public PasswordViewModel(string application_name, string username, string password)
    {
      this.Application_name = application_name;
      this.Username = username;
      this.Password = password;
    }
  }
}
