using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuiaTuristicaManager.Models.PostModel
{
    public class MediaViewPost
    {
        public int ModelId { get; set; }
        public string Name { get; set; }
        public IFormFile File { get; set; }
        public string Text { get; set; }
        public TypeMedia Type { get; set; }
    }
}
