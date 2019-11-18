using GuiaTuristicaManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GuiaTuristicaManager.Controllers
{
    public class DownloadController : Controller
    {
        private readonly string pathDiretory;
        private readonly CatalogContext _context;
        private readonly string PathZonesDatabase;

        public DownloadController(CatalogContext context)
        {
            _context = context;
            pathDiretory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/GuiaTuristicaAR";
            PathZonesDatabase = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/GuiaTuristicaAR/Zones/";
        }

        [HttpGet]
        public async Task<ActionResult> GetAllZones()
        {
            return Ok(await _context.Zones.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlaces()
        {
            return Ok(await _context.Places.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllModels()
        {
            return Ok(await _context.Models.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedia()
        {
            return Ok(await _context.Media.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPlace(int Id)
        {
            if (Id < 1)
                return BadRequest("filename not present");
            var place = await _context.Places.FindAsync(Id);
            if (place != null)
            {   
                try
                {
                    var temppath = pathDiretory + place.PathPattern;
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(temppath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetMimeType(temppath), Path.GetFileName(temppath));
                }
                catch
                {
                    return BadRequest("filename not present");
                }
            }
            else
            {
                return BadRequest("filename not present");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadModel(int Id)
        {
            if (Id < 1)
                return BadRequest("filename not present");
            var model = await _context.Models.FindAsync(Id);
            if (model != null)
            {
                try
                {
                    var temppath = pathDiretory + model.PathModel;
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(temppath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetMimeType(temppath), Path.GetFileName(temppath));
                }
                catch
                {
                    return BadRequest("filename not present");
                }
            }
            else
            {
                return BadRequest("filename not present");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadMedia(int Id)
        {
            if (Id < 1)
                return BadRequest("filename not present");
            var media = await _context.Media.FindAsync(Id);
            if (media != null)
            {
                try
                {
                    var temppath = pathDiretory + media.PathMedia;
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(temppath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetMimeType(temppath), Path.GetFileName(temppath));
                }
                catch
                {
                    return BadRequest("filename not present");
                }
            }
            else
            {
                return BadRequest("filename not present");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDataBaseZone(int Id)
        {
            if (Id < 1)
                return BadRequest();
            var zone = await _context.Zones.FindAsync(Id);
            if(zone != null)
            {
                try
                {
                    var temppath = pathDiretory + zone.PathDatabase;
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(temppath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetMimeType(temppath), Path.GetFileName(temppath));
                }
                catch
                {
                    return BadRequest("filename not present");
                }
            }
            else
            {
                return BadRequest("filename not present");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCoverZone(int Id)
        {
            if (Id < 1)
                return BadRequest();
            var zone = await _context.Zones.FindAsync(Id);
            if (zone != null)
            {
                try
                {
                    var temppath = pathDiretory + zone.PathCover;
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(temppath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetMimeType(temppath), Path.GetFileName(temppath));
                }
                catch
                {
                    return BadRequest("filename not present");
                }
            }
            else
            {
                return BadRequest("filename not present");
            }

        }

        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}