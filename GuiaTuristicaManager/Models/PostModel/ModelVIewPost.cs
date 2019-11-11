using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuiaTuristicaManager.Models.PostModel
{
    public class ModelVIewPost
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
