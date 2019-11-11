using System.Collections.Generic;

namespace GuiaTuristicaManager.Models
{
    public class ModelView
    {
        public int ModelId { get; set; }
        public string Name { get; set; }

        public List<MediaVIew> Media { get; set; }
    }
}
