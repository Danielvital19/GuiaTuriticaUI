using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GuiaTuristicaManager.Data;
using GuiaTuristicaManager.Models;
using GuiaTuristicaManager.Models.PostModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuiaTuristicaManager.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly string pathDiretory;
        private readonly string pathImages;
        private readonly string pathModels;
        private readonly string pathMedia;

        private readonly CatalogContext _context;

        public AdministrationController(CatalogContext context)
        {
            _context = context;
            pathDiretory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/GuiaTuristicaAR";
            pathImages = "/Images/";
            pathModels = "/Models/";
            pathMedia = "/Media/";
        }
        // GET: Administration
        [HttpGet]
        public async Task<ActionResult> GetAllZones()
        {
            return Ok(await _context.Zones.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlace(int Id)
        {
            if (Id < 1)
            {
                return BadRequest();
            }
            else
            {
                var Places = await _context.Places.Where(P => P.ZoneId == Id).OrderBy(P => P.Name).ToListAsync();
                if (Places.Count < 1)
                {
                    return NotFound();
                }
                else
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    path += "/";
                    List<PlaceVIew> PlacesVIew = new List<PlaceVIew>();
                    foreach (var place in Places)
                    {
                        var placeview = new PlaceVIew()
                        {
                            PlaceId = place.PlaceId,
                            Base64Image = Convert.ToBase64String(System.IO.File.ReadAllBytes(path + place.PathPattern)),
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
        public async Task<IActionResult> PostZone(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var zone = new Zone()
                {
                    Name = Name
                };
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
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostPlace(PlaceViewPost Place)
        {
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
                    if(Place.Image == null)
                    {
                        return BadRequest("No se publico el archivo de imagen");
                    }
                    var temppath = pathImages + $"{Guid.NewGuid()}_{Place.Name}.jpg";
                    var extension = Path.GetExtension(Place.Image.FileName);
                    if(extension == ".jpg" || extension == ".jpeg" || extension == ".png")
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

        public async Task<IActionResult> PostModel(ModelVIewPost Model)
        {
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
                    if(Model.File == null)
                    {
                        return BadRequest("No se publico el archivo fbx");
                    }
                    var exetension = Path.GetExtension(Model.File.FileName);
                    if (exetension != ".fbx")
                    {
                        return BadRequest("No es un archivo FBX");
                    }
                    var temppath = pathModels + $"{Guid.NewGuid()}_{Model.Name}.fbx";
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
                    catch
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

        public async Task<IActionResult> PostMedia(MediaViewPost Media)
        {
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
                    if(Media.File  == null)
                    {
                        return BadRequest("No se publico el archivo multimedia");
                    }
                    var exetension = Path.GetExtension(Media.File.FileName);
                    if ((exetension == ".mp3" || exetension == ".mp4" || exetension == ".txt"))
                    {
                        if (!Directory.Exists(pathDiretory + pathMedia))
                        {
                            Directory.CreateDirectory(pathDiretory + pathMedia);
                        }
                        string temppath = string.Empty;
                        switch (Media.Type)
                        {
                            case TypeMedia.Sound:
                                temppath = pathMedia + $"{Guid.NewGuid()}_{Media.Name}.mp3";
                                break;
                            case TypeMedia.Video:
                                temppath = pathMedia + $"{Guid.NewGuid()}_{Media.Name}.mp4";
                                break;
                            case TypeMedia.Text:
                                temppath = pathMedia + $"{Guid.NewGuid()}_{Media.Name}.txt";
                                break;
                        }
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
    }
}