using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace ResumeTrackingSystem.Model
{
    [Table("employee")]
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("employeeid")]
        public int employeeid { get; set; }

        [Column("firstname")]
        public string firstname { get; set; }

        [Column("lastname")]
        public string lastname { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        [Column("email")]
        public string email { get; set; }

        [Column("phonenumber")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string phonenumber { get; set; }

        [Column("address")]
        public string address { get; set; }

        [Column("city")]
        public string city { get; set; }

        [Column("country")]
        public string country { get; set; }

        [Column("yearsofexperience")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 - 50")]
        public int yearsofexperience { get; set; }

        [Column("profilepicture")]
        public byte[]? profilepicture { get; set; }

        [NotMapped]
        public IFormFile profilepicturefile { get; set; }

        [DataType(DataType.Date)]
        [Column("dateofbirth")]
        public DateOnly dateofbirth { get; set; }

        [Column("skills")]
        public string skills { get; set; }
    }
}
