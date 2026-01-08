public interface ILevelEditable
{
    // Name shown as section header in the properties panel
    string GetSectionTitle();

    // Fill 'output' with descriptors representing current values
    void GetProperties(System.Collections.Generic.List<LevelPropertyDescriptor> output);
    void ApplyProperty(LevelPropertyDescriptor property);

    // Optional: integration with your save data
    void WriteToLevelData(BaseObjectInstanceData data);
    void ReadFromLevelData(BaseObjectInstanceData data);
}