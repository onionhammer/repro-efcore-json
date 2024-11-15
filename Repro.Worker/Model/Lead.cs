namespace Repro.Worker.Model;

public class Lead
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public History? LastHistory { get; set; }
    public ICollection<History> History { get; init; } = [];
}