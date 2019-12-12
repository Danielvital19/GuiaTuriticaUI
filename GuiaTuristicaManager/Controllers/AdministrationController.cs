using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GuiaTuristicaManager.Data;
using GuiaTuristicaManager.Models;
using GuiaTuristicaManager.Models.PostModel;
using GuiaTuristicaManager.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace GuiaTuristicaManager.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly string pathDiretory;
        private readonly string PathZonesDatabase;
        private readonly string PathZoneCover;
        private readonly string pathImages;
        private readonly string pathModels;
        private readonly string pathMedia;
        private readonly string pathWtc;

        private readonly CatalogContext _context;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(CatalogContext context, ILogger<AdministrationController> logger)
        {
            _context = context;
            pathDiretory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/GuiaTuristicaAR";
            pathImages = "/Images/";
            pathModels = "/Models/";
            pathMedia = "/Media/";
            PathZonesDatabase = "/Zones/";
            PathZoneCover = "/Cover/";
            pathWtc = "/WtcDataBase/";
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        // GET: Administration
        [HttpGet]
        public async Task<ActionResult> GetAllZones()
        {
            return Ok(await _context.Zones.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlace(int id)
        {
            int Id = id;
            if (Id < 1)
            {
                return BadRequest();
            }
            else
            {
                var Places = await _context.Places.Where(P => P.ZoneId == Id).OrderBy(P => P.Name).ToListAsync();
                if (Places.Count < 1)
                {
                    return Ok(new List<string>());
                }
                else
                {
                    List<PlaceVIew> PlacesVIew = new List<PlaceVIew>();
                    foreach (var place in Places)
                    {
                        var placeview = new PlaceVIew()
                        {
                            PlaceId = place.PlaceId,
                            Base64Image = Convert.ToBase64String(System.IO.File.ReadAllBytes(pathDiretory + place.PathPattern)),
                            Name = place.Name
                        };
                        PlacesVIew.Add(placeview);
                    }
                    return Ok(PlacesVIew);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetModel(int Id)
        {
            if (Id < 1)
            {
                return BadRequest();
            }
            else
            {
                var model = await _context.Models.Include(m => m.Media).FirstOrDefaultAsync(m => m.PlaceId == Id);
                if (model != null)
                {
                    var modelView = new ModelView()
                    {
                        ModelId = model.ModelId,
                        Name = model.Name,
                        Media = new List<MediaVIew>()
                    };
                    foreach (var media in model.Media)
                    {
                        var mediaView = new MediaVIew()
                        {
                            MediaId = media.MediaId,
                            Name = media.Name,
                            Type = Enum.GetName(typeof(TypeMedia), media.Type)
                        };
                        modelView.Media.Add(mediaView);
                    }
                    return Ok(modelView);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostZone(IFormFile file, string zonename, IFormFile WtcFile)
        {
            ZoneViewPost Zone = new ZoneViewPost
            {
                Name = zonename,
                Image = file,
                WtcFile = WtcFile
            };

            try
            {
                if (!Directory.Exists(pathDiretory + PathZoneCover))
                {
                    Directory.CreateDirectory(pathDiretory + PathZoneCover);
                }
                if (Zone.Image == null)
                {
                    return BadRequest("No se publico el archivo de imagen");
                }
                var temppath = PathZoneCover + $"{Guid.NewGuid()}.jpg";
                temppath = temppath.Replace(" ", "");
                var extension = Path.GetExtension(Zone.Image.FileName);
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    using (var stream = new FileStream(pathDiretory + temppath, FileMode.Create))
                    {
                        await Zone.Image.CopyToAsync(stream);
                    }
                    if (!Directory.Exists(pathDiretory + pathWtc))
                    {
                        Directory.CreateDirectory(pathDiretory + pathWtc);
                    }
                    if (Zone.WtcFile == null)
                    {
                        System.IO.File.Delete(pathDiretory + temppath);
                        return BadRequest("No se publico el archivo de imagen");
                    }
                    extension = Path.GetExtension(Zone.WtcFile.FileName);
                    if (extension == ".wtc")
                    {
                        try
                        {
                            var temppathwtc = pathWtc + $"{Guid.NewGuid()}.wtc";
                            using (var stream = new FileStream(pathDiretory + temppathwtc, FileMode.Create))
                            {
                                await Zone.WtcFile.CopyToAsync(stream);
                            }

                            var zone = new Zone()
                            {
                                Name = Zone.Name,
                                PathCover = temppath,
                                PathWtc = temppathwtc
                            };
                            try
                            {
                                _context.Zones.Add(zone);
                                var result = await _context.SaveChangesAsync();
                                if (result > 0)
                                {
                                    return Ok(zone);
                                }
                                else
                                {
                                    return BadRequest();
                                }
                            }
                            catch
                            {
                                System.IO.File.Delete(pathDiretory + temppath);
                                System.IO.File.Delete(pathDiretory + temppathwtc);
                                return BadRequest();
                            }
                        }
                        catch
                        {
                            System.IO.File.Delete(pathDiretory + temppath);
                            return BadRequest("No se publico el archivo wtc");
                        }
                    }
                    else
                    {
                        System.IO.File.Delete(pathDiretory + temppath);
                        return BadRequest("No se publico el archivo de imagen");
                    }
                }
                else
                {
                    return BadRequest("Tipo de imagen no admitido");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostPlace(IFormFile file, string placename, int zoneId)
        {

            PlaceViewPost Place = new PlaceViewPost
            {
                ZoneId = zoneId,
                Name = placename,
                Image = file
            };


            if (Place.ZoneId < 1)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    if (!Directory.Exists(pathDiretory + pathImages))
                    {
                        Directory.CreateDirectory(pathDiretory + pathImages);
                    }
                    if (Place.Image == null)
                    {
                        return BadRequest("No se publico el archivo de imagen");
                    }
                    var temppath = pathImages + $"{Guid.NewGuid()}_{Place.ZoneId}.jpg";
                    temppath = temppath.Replace(" ", "");
                    var extension = Path.GetExtension(Place.Image.FileName);
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        using (var stream = new FileStream(pathDiretory + temppath, FileMode.Create))
                        {
                            await Place.Image.CopyToAsync(stream);
                        }
                        var place = new Place()
                        {
                            Name = Place.Name,
                            PathPattern = temppath,
                            ZoneId = Place.ZoneId
                        };
                        try
                        {
                            _context.Places.Add(place);
                            var result = await _context.SaveChangesAsync();
                            if (result > 0)
                            {
                                return Ok(place);
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        catch
                        {
                            System.IO.File.Delete(pathDiretory + temppath);
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return BadRequest("Tipo de imagen no admitido");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostModel(IFormFile file, string name, int placeid)
        {

            ModelVIewPost Model = new ModelVIewPost
            {
                PlaceId = placeid,
                Name = name,
                File = file
            };

            if (Model.PlaceId < 1)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    if (!Directory.Exists(pathDiretory + pathModels))
                    {
                        Directory.CreateDirectory(pathDiretory + pathModels);
                    }
                    if (Model.File == null)
                    {
                        return BadRequest("No se publico el archivo fbx");
                    }
                    var exetension = Path.GetExtension(Model.File.FileName);
                    if (exetension != ".fbx")
                    {
                        return BadRequest("No es un archivo FBX");
                    }
                    var temppath = pathModels + $"{Guid.NewGuid()}_{Model.PlaceId}.fbx";
                    temppath = temppath.Replace(" ", "");
                    using (var stream = new FileStream(pathDiretory + temppath, FileMode.Create))
                    {
                        await Model.File.CopyToAsync(stream);
                    }
                    var model = new Model()
                    {
                        PlaceId = Model.PlaceId,
                        PathModel = temppath,
                        Name = Model.Name
                    };
                    try
                    {
                        _context.Models.Add(model);
                        var result = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            return Ok(model);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    catch (Exception e)
                    {
                        System.IO.File.Delete(pathDiretory + temppath);
                        return BadRequest();
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostMedia(IFormFile file,int modelId, string name, string text, TypeMedia type )
        {
            MediaViewPost Media = new MediaViewPost
            {
                ModelId = modelId,  
                Name = name,
                File = file,
                Text = text,
                Type = type

            };

            if (Media.ModelId < 1)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    if (!Directory.Exists(pathDiretory + pathMedia))
                    {
                        Directory.CreateDirectory(pathDiretory + pathMedia);
                    }
                    if (Media.File == null)
                    {
                        return BadRequest("No se publico el archivo multimedia");
                    }
                    var exetension = Path.GetExtension(Media.File.FileName);
                    if ((exetension == ".jpg" || exetension == ".mp4" || exetension == ".txt"))
                    {
                        if (!Directory.Exists(pathDiretory + pathMedia))
                        {
                            Directory.CreateDirectory(pathDiretory + pathMedia);
                        }
                        string temppath = string.Empty;
                        switch (Media.Type)
                        {
                            case TypeMedia.Image:
                                temppath = pathMedia + $"{Guid.NewGuid()}_{Media.ModelId}.jpg";
                                break;
                            case TypeMedia.Video:
                                temppath = pathMedia + $"{Guid.NewGuid()}_{Media.ModelId}.mp4";
                                break;
                            case TypeMedia.Text:
                                temppath = pathMedia + $"{Guid.NewGuid()}_{Media.ModelId}.txt";
                                break;
                        }
                        temppath = temppath.Replace(" ", "");
                        using (var stream = new FileStream(pathDiretory + temppath, FileMode.Create))
                        {
                            await Media.File.CopyToAsync(stream);
                        }
                        var media = new Media()
                        {
                            ModelId = Media.ModelId,
                            Name = Media.Name,
                            Type = Media.Type,
                            PathMedia = temppath
                        };
                        try
                        {
                            _context.Media.Add(media);
                            var result = await _context.SaveChangesAsync();
                            if (result > 0)
                            {
                                return Ok(media);
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        catch
                        {
                            System.IO.File.Delete(pathDiretory + temppath);
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return BadRequest("No es un archivo valido, solo se admite .mp3, .mp4, .txt");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.ToString());
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteZone(int Id)
        {
            if (Id < 1)
                return BadRequest("La Zona no es la correcta");
            var Zone = await _context.Zones.FindAsync(Id);
            if (Zone != null)
            {
                var Places = await _context.Places.Where(P => P.ZoneId == Zone.ZoneId).ToListAsync();
                foreach (var place in Places)
                {
                    if (System.IO.File.Exists(place.PathPattern))
                    {
                        System.IO.File.Delete(place.PathPattern);
                    }
                    var model = await _context.Models.FirstOrDefaultAsync(m => m.PlaceId == place.PlaceId);
                    if (model != null)
                    {
                        if (System.IO.File.Exists(model.PathModel))
                        {
                            System.IO.File.Delete(model.PathModel);
                        }
                        var medias = await _context.Media.Where(ME => ME.ModelId == model.ModelId).ToListAsync();
                        if (medias.Count > 0)
                        {
                            foreach (var media in medias)
                            {
                                if (System.IO.File.Exists(media.PathMedia))
                                {
                                    System.IO.File.Delete(media.PathMedia);
                                }
                            }
                        }
                    }
                }
                _context.Zones.Remove(Zone);
                await _context.SaveChangesAsync();
                return Ok("Se Elimino correctamente");
            }
            else
            {
                return BadRequest("No se encontro la Zona");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlace(int Id)
        {
            if (Id < 1)
                return BadRequest("El Sitio no es el correcto");
            var place = await _context.Places.FindAsync(Id);
            if (place != null)
            {
                if (System.IO.File.Exists(place.PathPattern))
                {
                    System.IO.File.Delete(place.PathPattern);
                }
                var model = await _context.Models.FirstOrDefaultAsync(M => M.PlaceId == place.PlaceId);
                if (model != null)
                {
                    if (System.IO.File.Exists(model.PathModel))
                    {
                        System.IO.File.Delete(model.PathModel);
                    }
                    var medias = await _context.Media.Where(m => m.ModelId == model.ModelId).ToListAsync();
                    if (medias.Count > 0)
                    {
                        foreach (var media in medias)
                        {
                            if (System.IO.File.Exists(media.PathMedia))
                            {
                                System.IO.File.Delete(media.PathMedia);
                            }
                        }
                    }
                }
                _context.Places.Remove(place);
                await _context.SaveChangesAsync();
                return Ok("Se elimino correctamente");
            }
            else
            {
                return BadRequest("No se encontro el Sitio");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModel(int id)
        {
            if (id < 1)
                return BadRequest("El Modelo no es el correcto");
            var model = await _context.Models.FindAsync(id);
            if (model != null)
            {
                if (System.IO.File.Exists(model.PathModel))
                {
                    System.IO.File.Delete(model.PathModel);
                }
                var medias = await _context.Media.Where(M => M.ModelId == model.ModelId).ToListAsync();
                if (medias.Count > 0)
                {
                    foreach (var media in medias)
                    {
                        if (System.IO.File.Exists(media.PathMedia))
                        {
                            System.IO.File.Delete(media.PathMedia);
                        }
                    }
                }
                _context.Models.Remove(model);
                await _context.SaveChangesAsync();
                return Ok("Se elimino correctamente");
            }
            else
            {
                return BadRequest("No se encontro el Modelo");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedia(int Id)
        {
            if (Id < 1)
                return BadRequest("El Archivo Multimedia no es el correcto");
            var media = await _context.Media.FindAsync(Id);
            if (media != null)
            {
                if (System.IO.File.Exists(media.PathMedia))
                {
                    System.IO.File.Delete(media.PathMedia);
                }
                _context.Media.Remove(media);
                await _context.SaveChangesAsync();
                return Ok("Se elimino correctamente");
            }
            else
            {
                return BadRequest("No se encontro el Archivo Multimedia");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> RunRutime(int Id)
        //{
        //    var places = await _context.Places.Where(p => p.ZoneId == Id).ToListAsync();
        //    if(places.Count > 0)
        //    {
        //        if(!Directory.Exists(pathDiretory + PathZonesDatabase))
        //        {
        //            Directory.CreateDirectory(pathDiretory + PathZonesDatabase);
        //        }
        //        var text = string.Empty;
        //        foreach(var place in places)
        //        {
        //            text += $"{place.PlaceId}|" + pathDiretory + place.PathPattern + Environment.NewLine;
        //        }
        //        var fileText = pathDiretory +  PathZonesDatabase + Id + ".txt";
        //        var fileoutput = pathDiretory + PathZonesDatabase + Id + ".imgdb";
        //        if (System.IO.File.Exists(fileText))
        //        {
        //            System.IO.File.Delete(fileText);
        //        }
        //        if (System.IO.File.Exists(pathDiretory + PathZonesDatabase + Id + ".imgdb-imglist.txt"))
        //        {
        //            System.IO.File.Delete(pathDiretory + PathZonesDatabase + Id +".imgdb-imglist.txt");
        //        }
        //        if (System.IO.File.Exists(pathDiretory + PathZonesDatabase + Id + ".imgdb"))
        //        {
        //            System.IO.File.Delete(pathDiretory + PathZonesDatabase + Id + ".imgdb");
        //        }
        //        System.IO.File.WriteAllText(fileText, text);
        //        try
        //        {
        //            await $"/app/wwwroot/lib/augmented_image_cli_linux build-db --input_image_list_path={fileText} --output_db_path={fileoutput}".Bash(_logger);
        //        }
        //        catch(Exception e)
        //        {
        //            return BadRequest(e);
        //        }
        //        try
        //        {
        //            var zone = await _context.Zones.FindAsync(Id);
        //            zone.PathDatabase = PathZonesDatabase + Id + ".imgdb";
        //            zone.IsBuild = true;
        //            _context.Zones.Update(zone);
        //            await _context.SaveChangesAsync();
        //            return Ok("Construido con exito");
        //        }
        //        catch
        //        {
        //            if (System.IO.File.Exists(fileoutput))
        //            {
        //                System.IO.File.Delete(fileoutput);
        //            }
        //        }
        //    }
        //    return BadRequest("No se encontraron imagenes");
        //}
    }
}