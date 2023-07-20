using ViAPI.Other;

namespace ViAPI.Entites.DTO
{
    public class ViDictionaryDto
    {
        public string Name { get; }
        public Guid Guid { get; }
        public ViDictionaryDto(Guid guid, string name)
        {
            InputChecker.CheckStringException(name);
            InputChecker.CheckGuidException(guid);
            Name = name;
            Guid = guid;
        }
    }
}
