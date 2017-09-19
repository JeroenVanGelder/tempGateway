using System;
using System.ComponentModel.DataAnnotations;

namespace exampleWebAPI.Models
{
    public class Meting
    {
        [Key]
        public int id { get; set; }
        public string ipSender { get; set; }
        public DateTime tijdStip { get; set; }
        public double temperatuurCelsius { get; set; }
        public double temperatuurFahrenheit { get; set; }
        public double lichtIntensiteit { get; set; }
    }
}