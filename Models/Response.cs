using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Activity_center.Models
{
    [Table("responses")]
    public class Response
    {
        [Key]
        public int ResponseId {get;set;}
        public int UserId {get;set;}
        public int ShindigId {get;set;}
        public User Guest {get;set;}
    }
}