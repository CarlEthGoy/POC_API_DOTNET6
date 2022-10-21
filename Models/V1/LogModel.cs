namespace API.Models.V1
{
  public interface ILogModel
  {
    int Id { get; set; }
    string Data_before { get; set; }
    string Data_after { get; set; }
  }

  public class LogModel : ILogModel
  {
    public int Id { get; set; }
    public string Data_before { get; set; }
    public string Data_after { get; set; }

    public LogModel()
    {
      Id = 0;
      Data_before = "";
      Data_after = "";
    }

    public LogModel(int id, string data_before, string data_after)
    {
      this.Id = id;
      this.Data_before = data_before;
      this.Data_after = data_after;
    }
  }
}
