
using System.Collections.Generic;

namespace GuiaTuristicaManager.Models
{
    public class Model
    {
        public int ModelId { get; set; }
        public string Name { get; set; }
        public string PathModel { get; set; }

        public int PlaceId { get; set; }
        public virtual Place Place { get; set; }

        public List<Media> Media { get; set; }
    }
}
