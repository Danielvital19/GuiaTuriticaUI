
namespace GuiaTuristicaManager.Models
{
    public class Place
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string PathPattern { get; set; }

        public int ZoneId { get; set; }
        public virtual Zone Zone { get; set; }
    }
}
