using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace WebApp.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        public string? Patronymic { get; set; }
        [Required(ErrorMessage = "Не указан возраст")]
        [Range(0, 111, ErrorMessage = "Возраст должен быть в диапазоне {1}-{2}")]
        public int Age { get; set; }
        public ICollection<Patient> Patients { get; set; }
        public Doctor()
        {
            Patients = new List<Patient>();
        }
        public string Fullname
        {
            get
            {
                if (Patronymic == null)
                {
                    return string.Format("{0} {1}", Surname, Name);
                }
                return string.Format("{0} {1} {2}", Surname, Name, Patronymic);
            }
        }
        public string Shortname
        {
            get
            {
                if (Patronymic == null)
                {
                    return string.Format("{0} {1}.", Surname, Name.ElementAt(0));
                }
                return string.Format("{0} {1}.{2}.", Surname, Name.ElementAt(0), Patronymic.ElementAt(0));
            }
        }
    }
}
