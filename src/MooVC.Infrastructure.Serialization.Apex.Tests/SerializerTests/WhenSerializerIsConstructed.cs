namespace MooVC.Infrastructure.Serialization.Apex.SerializerTests
{
    using global::Apex.Serialization;
    using Xunit;

    public sealed class WhenSerializerIsConstructed
    {
        [Fact]
        public void GivenNoSettingsThenADefaultSerializerIsCreated()
        {
            _ = new Serializer();
        }

        [Fact]
        public void GivenSettingsThenASerializerIsCreatedWithTheSettingsApplied()
        {
            var settings = new Settings
            {
                AllowFunctionSerialization = false,
                FlattenClassHierarchy = true,
                InliningMaxDepth = 4,
                SerializationMode = Mode.Graph,
                SupportSerializationHooks = true,
                UseSerializedVersionId = true,
            };

            _ = new Serializer(settings: settings);
        }
    }
}