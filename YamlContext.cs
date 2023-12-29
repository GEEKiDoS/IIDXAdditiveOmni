using IIDXAdditiveOmni;
using YamlDotNet.Serialization;

namespace IIDXAdditiveOmni;

public record DummyDict
{
    public Dictionary<int, MusicEntry> Entries { get; set; }
}

[YamlStaticContext]
[YamlSerializable(typeof(DummyDict))]
[YamlSerializable(typeof(MusicEntry))]
[YamlSerializable(typeof(MusicTextures))]
public partial class StaticContext : YamlDotNet.Serialization.StaticContext
{
}
