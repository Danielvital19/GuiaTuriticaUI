using Microsoft.AspNetCore.Http;

namespace GuiaTuristicaManager.Models.PostModel
{
    public class ZoneViewPost
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
