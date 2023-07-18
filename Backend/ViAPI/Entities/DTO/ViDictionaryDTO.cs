using ViAPI.StaticMethods;

namespace ViAPI.Entites.DTO
{
    public class ViDictionaryDto
    {
        public string Name { get; }
        public Guid Guid { get; }
        public ViDictionaryDto(string name, Guid guid)
        {
            InputChecker.CheckStringException(name);
            InputChecker.CheckGuidException(guid);
            Name = name;
            Guid = guid;
        }
    }
}
