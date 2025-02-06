namespace Forum.Domain.Models
{
    public interface IDocument
    {
        DocumentVersion Version { get; set; }
    }
    public class DocumentVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public string GetVersionString() => $"{Major}.{Minor}.{Patch}";
    }
}
