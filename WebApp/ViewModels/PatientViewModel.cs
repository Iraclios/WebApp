using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
namespace WebApp.ViewModels
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Не указана фамилия")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        public string? Patronymic { get; set; }
        [Required(ErrorMessage = "Не указан возраст")]
        [Range(0, 111, ErrorMessage = "Возраст должен быть в диапазоне {1}-{2}")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Не указан лечащий врач")]
        public int DoctorId { get; set; }
    }
}
