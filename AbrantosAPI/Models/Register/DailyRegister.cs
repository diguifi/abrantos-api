using System;
using System.ComponentModel.DataAnnotations;

namespace AbrantosAPI.Models.Register
{
    public class DailyRegister
    {
        public long Id { get; set; }
        public long Abrantos { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(140)]
        public string Post { get; set; }

        public string UserId { get; set; }

        public DailyRegister(long abrantos, DateTime date, string userId, string post)
        {
            Abrantos = abrantos;
            Date = date;
            UserId = userId;
            Post = post;
        }
    }
}