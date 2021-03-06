
namespace GuiaTuristicaManager.Models
{
    public class Media
    {
        public int MediaId { get; set; }
        public string Name { get; set; }
        public string PathMedia { get; set; }
        public TypeMedia Type { get; set; }


        public int ModelId { get; set; }
        public virtual Model Model { get; set; }
    }


    public enum TypeMedia
    {
        Text,
        Image,
        Video,
    }
}
