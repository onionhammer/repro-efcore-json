using System.Text.Json.Serialization;

namespace Repro.Worker.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Stage")]
[JsonDerivedType(typeof(HistoryMatched), "Matched")]
[JsonDerivedType(typeof(HistoryComplete), "Complete")]
public abstract record History
{
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
}

public record HistoryMatched : History
{
    public required string MatchedProperty { get; set; }
}

public record HistoryComplete : History
{
    public DateTimeOffset DateCompleted { get; set; }
}