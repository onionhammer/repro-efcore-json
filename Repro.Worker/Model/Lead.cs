namespace Repro.Worker.Model;

public class Lead
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public List<History> HistoryJson { get; set; } = [];
    public List<History> HistoryJsonB { get; set; } = [];
}