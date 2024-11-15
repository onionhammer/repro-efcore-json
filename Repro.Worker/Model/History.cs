using System.Text.Json.Serialization;

namespace Repro.Worker.Model;

[JsonPolymorphic]
[JsonDerivedType(typeof(HistoryMatched), "Matched")]
[JsonDerivedType(typeof(HistoryComplete), "Complete")]
public abstract record History
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeadId { get; init; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public virtual HistoryStage Stage { get; }
}

public record HistoryMatched : History
{
    public required string MatchedProperty { get; set; }

    [JsonIgnore]
    public override HistoryStage Stage => HistoryStage.Matched;
}

public record HistoryComplete : History
{
    public DateTimeOffset DateCompleted { get; set; }

    [JsonIgnore]
    public override HistoryStage Stage => HistoryStage.Complete;
}

public enum HistoryStage
{
    Matched,
    Complete
}