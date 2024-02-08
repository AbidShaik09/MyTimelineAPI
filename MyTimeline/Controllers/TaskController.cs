using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTimeline.Models;

namespace MyTimeline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TaskController(ApplicationDbContext db)
        {
            _db = db;

        }

        // GET: api/GetTasks/{id}
        [HttpGet("GetTasks/{id}")]
        public async Task<IActionResult> GetTasks(int id)
        {
            User user = await _db.Users.Include(u => u.Tasks ).Include(u=>u.TimeSlots).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound("UserNotFound");
            }


            List<Models.Task> tasks = user.Tasks.ToList();
            
            return Ok(tasks);
        }


        [HttpGet("GetSchedule/{id}")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            User user= _db.Users.Include(e=>e.Tasks).Include(e=>e.TimeSlots).FirstOrDefault(x => x.Id == id);
            if(user == null)
            {
                return NotFound("User Not Found");
            }

            List<TimeSlot> timeSlots = user.TimeSlots.ToList();



            return Ok(timeSlots);
        }



        // POST: api/ScheduleTasks/
        [HttpPost("ScheduleTasks/")]
        public async Task<IActionResult> scheduleTasks([FromBody] Models.Task t)
        {
            var user =await _db.Users.Include(u => u.Tasks).Include(u => u.TimeSlots).FirstOrDefaultAsync(e => e.Id == t.UserId);
            if (user == null)
            {
                return BadRequest();
            }
            if (user.TimeSlots == null)
            {
                user.TimeSlots = new List<TimeSlot>();
            }
            List<TimeSlot> UserTimeSlots = user.TimeSlots.ToList();
            TimeSlot thisTimeSlot=null;
            foreach(TimeSlot currSlot in UserTimeSlots){
                    if(currSlot.StartTime<=t.StartTime && currSlot.EndTime >= t.EndTime)
                    {
                        thisTimeSlot = currSlot;
                    }
                }

            if(thisTimeSlot == null) 
            {
                thisTimeSlot = new TimeSlot();
                thisTimeSlot.UserId = t.UserId;
                thisTimeSlot.Status = "busy";
                thisTimeSlot.StartTime = t.StartTime;
                thisTimeSlot.EndTime = t.EndTime;
                thisTimeSlot.Tasks = new List<Models.Task>();
                
            }
            if(thisTimeSlot.Tasks==null)
                thisTimeSlot.Tasks=new List<Models.Task>();
            thisTimeSlot.Tasks.Add(t);
            if (user.Tasks == null)
                user.Tasks = new List<Models.Task>();
            user.Tasks.Add(t);


            UserTimeSlots.Add(thisTimeSlot);
            user.TimeSlots = UserTimeSlots;
            var res= await _db.SaveChangesAsync();
            return Ok(user);

        }
    }
}
