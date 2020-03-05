using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Activity_center.Models
{
    public class NoPastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((DateTime)value <= DateTime.Now)
                return new ValidationResult("Date must be in the future");
            return ValidationResult.Success;
        }
    }
    
    [Table("shindig")]

    public class Shindig
    {
        [Key]
        public int ShindigId {get;set;}

        [Required]
        [MinLength(5,ErrorMessage="please input an event name of at least 5 characters")]
        public string Title {get;set;}

        [Required]
        [MaxLength(255,ErrorMessage="yo chill no ones gunna read all that")]
        public string Description {get;set;}

        [Required]
        [NoPastDate(ErrorMessage="Date must be a future date")]
        public DateTime Date {get;set;}

        
        public string Time {get;set;}

        [Required]
        public int Duration {get;set;}

        public string dur {get;set;}

        public User Creator {get;set;}


        public int UserId {get;set;}


        public List<Response> Responses {get;set;}


    }






}    