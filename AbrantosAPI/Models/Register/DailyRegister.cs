using System;

namespace AbrantosAPI.Models.Register
{
    public class DailyRegister
    {
        public long Id { get; set; }
        public long Abrantos { get; set; }
        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public DailyRegister(long abrantos, DateTime date, string userId)
        {
            Abrantos = abrantos;
            Date = date;
            UserId = userId;
        }
    }
}