using System;

namespace AbrantosAPI.Models.Register
{
    public class DailyRegister
    {
        public long Id { get; set; }
        public long Abrantos { get; set; }
        public DateTime Date { get; set; }

        public DailyRegister(long abrantos, DateTime date)
        {
            Abrantos = abrantos;
            Date = date;
        }
    }
}