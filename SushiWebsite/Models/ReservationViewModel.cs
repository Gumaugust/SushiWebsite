using System.ComponentModel.DataAnnotations;

namespace SushiWebsite.Models
{
    public class ReservationViewModel
    {
        [Required]
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        [Range(1, 8, ErrorMessage = "Please select between 1 and 8 people.")]
        public int NumberOfPeople { get; set; }

        [Required(ErrorMessage = "Please select a valid date.")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Please select a time.")]
        public string Time { get; set; }

        public string Note { get; set; }
    }
}
