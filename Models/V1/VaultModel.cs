namespace API.Models.V1
{
  public interface IVaultModel
  {
    int Id { get; set; }
    string Name { get; set; }
    int User_id { get; set; }
  }
  public interface IVaultViewModel
  {
    string Name { get; set; }
    int User_id { get; set; }
  }

  public class VaultModel : IVaultModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int User_id { get; set; }

    public VaultModel()
    {
      this.User_id = 0;
      this.Id = 0;
      this.Name = "";
    }
    public VaultModel(int id, string name, int user_id)
    {
      this.User_id = user_id;
      this.Id = id;
      this.Name = name;
    }
  }

  public class VaultViewModel : IVaultViewModel
  {
    public string Name { get; set; }
    public int User_id { get; set; }

    public VaultViewModel()
    {
      this.Name = "";
      this.User_id = 0;
    }
    public VaultViewModel(string name, int user_id)
    {
      this.Name = name;
      this.User_id = user_id;
    }
  }
}
