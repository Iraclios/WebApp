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
    public class DiagnosisPatient
    {
        [Required(ErrorMessage = "Не указан больной")]
        public int OldPatientId { get; set; }
        [Required(ErrorMessage = "Не указана болезнь")]
        public int OldDiagnosisId { get; set; }
        [Required(ErrorMessage = "Не указан больной")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Не указана болезнь")]
        public int DiagnosisId { get; set; }
    }
}
