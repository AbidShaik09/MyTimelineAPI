using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
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


        //Get User
        
        
        // GET: api/GetTasks/{id}
        [HttpGet("GetTasks/{Userid}")]
        public async Task<IActionResult> GetTasks(int Userid)
        {
            int id = Userid;
            User user = await _db.Users.Include(u => u.Tasks).Include(u => u.TimeSlots).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound("UserNotFound");
            }


            List<Models.Task> tasks = user.Tasks.Where(e=>e.IsCompleted==false).ToList();
            foreach(Models.Task t in tasks)
            {
                if (t.EndTime < DateTime.Now)
                {
                    t.Missed = true;
                }

            }

            return Ok(tasks);
        }

        [HttpGet("GetSchedule/{Userid}")]
        public async Task<IActionResult> GetSchedule(int Userid)
        {
            int id = Userid;
            User user = _db.Users.Include(e => e.Tasks).Include(e => e.TimeSlots).FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            List<TimeSlot> timeSlots = user.TimeSlots.ToList();



            return Ok(timeSlots);
        }

        [HttpGet("GetTasksByPriority/{userId}/{priority}")]
        public async Task<IActionResult> GetTasksByPriority(int priority, int userId)
        {
            User user = _db.Users.Include(e => e.Tasks).FirstOrDefault(e => e.Id == userId);
            if (user == null)
            {
                return BadRequest("User Not Found");

            }
            List<Models.Task> tasks = user.Tasks.Where(e => e.Priority == priority).ToList();
            return Ok(tasks);

        }

        //mark tasks as completed
        [HttpPost("MarkTaskAsCompleted/{id}")]
        public async Task<IActionResult> MarkTaskAsCompleted([FromBody] Models.Task task)
        {
            Models.User user = _db.Users.Include(e => e.Tasks).FirstOrDefault(e => e.Id == task.UserId);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            Models.Task userTask = user.Tasks.FirstOrDefault(e => e.Id == task.Id);
            if (userTask == null)
            {
                return BadRequest("Task Not Found");
            }
            userTask.IsCompleted = true;
            int x = await _db.SaveChangesAsync();


            return Ok("Marked As Completed");
        }

        // POST: api/ScheduleTasks/
        [HttpPost("ScheduleTask/")]
        public async Task<IActionResult> ScheduleTask([FromBody] Models.Task t)
        {
            var user = await _db.Users.Include(u => u.Tasks).Include(u => u.TimeSlots).FirstOrDefaultAsync(e => e.Id == t.UserId);
            if (user == null)
            {
                return BadRequest();
            }
            if (user.TimeSlots == null)
            {
                user.TimeSlots = new List<TimeSlot>();
            }
            List<TimeSlot> UserTimeSlots = user.TimeSlots.ToList();
            TimeSlot thisTimeSlot = null;
            foreach (TimeSlot currSlot in UserTimeSlots)
            {
                if (currSlot.StartTime.Date == t.StartTime.Date && currSlot.EndTime.Date == t.EndTime.Date)
                {
                    thisTimeSlot = currSlot;
                }
            }

            if (thisTimeSlot == null)
            {
                thisTimeSlot = new TimeSlot();
                thisTimeSlot.UserId = t.UserId;
                thisTimeSlot.Status = "busy";
                thisTimeSlot.StartTime = t.StartTime;
                thisTimeSlot.EndTime = t.EndTime;
                thisTimeSlot.Tasks = new List<Models.Task>();

            }
            if (thisTimeSlot.Tasks == null)
                thisTimeSlot.Tasks = new List<Models.Task>();
            thisTimeSlot.Tasks.Add(t);
            if (user.Tasks == null)
                user.Tasks = new List<Models.Task>();
            user.Tasks.Add(t);
            UserTimeSlots.Add(thisTimeSlot);
            user.TimeSlots = UserTimeSlots;
            var res = await _db.SaveChangesAsync();
            return Ok(user);

        }

        [HttpPost("ChangePriorityOfTask/{setPriority}")]
        public async Task<IActionResult> ChangePriorityOfTask([FromBody] Models.Task gettask, int setPriority)
        {
            int UserId = gettask.UserId;
            int TaskId = gettask.Id;
            User u = _db.Users.Include(e => e.Tasks).FirstOrDefault(x => x.Id == UserId);
            if (u == null)
            {
                BadRequest("User Not Found");
            }
            Models.Task t = u.Tasks.FirstOrDefault(x => x.Id == TaskId);
            if (t == null || t.UserId != UserId)
            {
                return BadRequest("Task Not Found");

            }
            t.Priority = setPriority;
            int v = await _db.SaveChangesAsync();
            return Ok($"Priority set to {setPriority}");
        }

        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] Models.Task task)
        {
            int UserId= task.UserId;

            User u = _db.Users.Include(e => e.Tasks).FirstOrDefault(x => x.Id == UserId);
            if (u==null)
            {
                return BadRequest("Not Found");

            }
            Models.Task t = u.Tasks.FirstOrDefault(e=>e.Id==task.Id);
            if(t==null || t.UserId != UserId)
            {
                return BadRequest("Task Not Found");
            }
            t = task;
            int x=await _db.SaveChangesAsync();
            return Ok("Task Updated");
        }

        //Delete Schedule
        [HttpDelete("DeleteSchedule")]
        public async Task<IActionResult> DeleteSchedule([FromBody] TimeSlot GetTimeSlot )
        {
            int UserId= GetTimeSlot.UserId;
            int timeSlotId = GetTimeSlot.Id;
            User u = _db.Users.Include(e => e.TimeSlots).Include(e => e.Tasks).FirstOrDefault(e=>e.Id==UserId);
            if (u == null)
            {
                return BadRequest("User Not Found");
            }


            TimeSlot ts = u.TimeSlots.FirstOrDefault(t=>t.Id==timeSlotId);
            if (ts == null || ts.UserId !=UserId)
            {
                return Ok("No such TimeSlot Exists");
            }
            List<Models.Task> tasks = ts.Tasks.ToList();
            foreach (Models.Task task in tasks)
            {
                u.Tasks.Remove(task);
            }
            u.TimeSlots.Remove(ts);
            var x= await _db.SaveChangesAsync();
            return Ok("TimeSlotHasBeenDeleted");

        }

        //DeleteTask
        [HttpDelete("DeleteTask")]
        public async Task<IActionResult> DeleteTask([FromBody] Models.Task getTask)
        {

            int UserId=getTask.UserId;
            int TaskId = getTask.Id;
            User u = _db.Users.Include(e => e.Tasks).FirstOrDefault(w=>w.Id==UserId);
            if(u == null)
            {
                return BadRequest("User Not Found");
            }
            Models.Task task = u.Tasks.FirstOrDefault(e=>e.Id==TaskId);
            if(task == null || task.UserId != UserId)
            {
                return BadRequest("Task Not Found");
            }
            u.Tasks.Remove(task);


            int x= await _db.SaveChangesAsync();
            return Ok("Task Has Been Removed");


        }

       

    }
}
 