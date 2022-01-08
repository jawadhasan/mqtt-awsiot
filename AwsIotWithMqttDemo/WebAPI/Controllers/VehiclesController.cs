using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly TruckSensorRepo _truckSensorRepo;

        //ctor
        public VehiclesController(TruckSensorRepo truckSensorRepo)
        {
            _truckSensorRepo = truckSensorRepo;
        }

        [HttpGet]
        [Route("getVehiclesData")]
        public async Task<dynamic> GetVehiclesData()
        {
            var result = await _truckSensorRepo.GetAllItems();
            var count = result.Count();

            return new
            {
                Result = result,
                Count = count,
                GeneratedAt = DateTime.Now
            };            
        }


        [HttpGet]
        public dynamic Get()
        {
            return new
            {
                Guid = Guid.NewGuid().ToString(),
                GeneratedAt = DateTime.Now,
                Issuer = Environment.MachineName
            };
        }
    }
}
