namespace API.Models.V2
{
  public class User
  {
    public Guid? uuid { get; set; }
    public string name { get; set; }
    public string hash { get; set; }
  }
}
