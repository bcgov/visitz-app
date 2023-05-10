namespace hestiapi.Models
{
    public class NotesListEntity
    {
        public string EntityNumber { get; set; }
        public string EntityType { get; set; }
        public List<NoteEntity> Notes { get; set; }
    }
}
