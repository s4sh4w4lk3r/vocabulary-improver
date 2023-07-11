namespace ViAPI.Entities
{
    public class ViDictionary
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public IEnumerable<Word> Words { get; set; }
        public ViDictionary(Guid guid, string name, User user, IEnumerable<Word> words) 
        {
            Guid = guid;
            Name = name;
            User = user;
            Words = words;
        }
    }
}
