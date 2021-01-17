using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects
                .FirstOrDefault(o => o.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context
                .CelestialObjects
                .Where(o => o.OrbitedObjectId == id)
                .ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(o => o.Name == name);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach(var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context
                    .CelestialObjects
                    .Where(o => o.OrbitedObjectId == celestialObject.Id)
                    .ToList();
            }        

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context
                    .CelestialObjects
                    .Where(o => o.OrbitedObjectId == celestialObject.Id)
                    .ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { Id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject inputCelestialObject)
        {
            var dbCelestialObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);

            if (dbCelestialObject == null)
            {
                return NotFound();
            }

            dbCelestialObject.Name = inputCelestialObject.Name;
            dbCelestialObject.OrbitalPeriod = inputCelestialObject.OrbitalPeriod;
            dbCelestialObject.OrbitedObjectId = inputCelestialObject.OrbitedObjectId;
            
            _context.CelestialObjects.Update(dbCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var dbCelestialObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);

            if (dbCelestialObject == null)
            {
                return NotFound();
            }

            dbCelestialObject.Name = name;

            _context.CelestialObjects.Update(dbCelestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(o => o.Id == id);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
