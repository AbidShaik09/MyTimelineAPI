using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyTimeline.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; } // Remember to hash this!
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }

        DateTime Created { get; set; }= DateTime.UtcNow;



        [JsonIgnore]
        public ICollection<Task> Tasks { get; set; } = new List<Task>(); // Navigation property
        [JsonIgnore]
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>(); // Navigation property
    }

    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; } = 3; // Urgent: 4, Active: 3, Passive: 2, Casual: 1

        
        public DateTime StartTime { get; set; }=DateTime.UtcNow;
        
        public DateTime EndTime { get; set; }=DateTime.UtcNow.AddHours(1);
        public bool IsFlexible { get; set; }=false;

        public bool IsCompleted { get; set; } = false;
 
        public bool Missed { get; set; } = false;
        [Required]
        public int UserId { get; set; } // Foreign Key
        

        
    }

    public class TimeSlot
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow.Date;
        
        public DateTime EndTime { get; set; } = DateTime.UtcNow.AddDays(1);
        
        public string Status { get; set; } = "Free"; // Free, Busy
        [Required]
        public int UserId { get; set; } // Foreign Key
        [JsonIgnore]
        public ICollection<Task> Tasks { get; set; } = new List<Task>(); // Navigation property


        
    }

}
