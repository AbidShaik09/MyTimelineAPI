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
        public string Category { get; set; } // Active, Passive, Casual, Urgent
        public int Priority { get; set; } // Urgent: 4, Active: 3, Passive: 2, Casual: 1
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsFlexible { get; set; }
        [Required]
        public int UserId { get; set; } // Foreign Key
        

        
    }

    public class TimeSlot
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } // Free, Busy
        [Required]
        public int UserId { get; set; } // Foreign Key

        public ICollection<Task> Tasks { get; set; } = new List<Task>(); // Navigation property


        
    }

}
