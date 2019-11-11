using Microsoft.AspNetCore.Http;

namespace GuiaTuristicaManager.Models.PostModel
{
    public class PlaceViewPost
    {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
