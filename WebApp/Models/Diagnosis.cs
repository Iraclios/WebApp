using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace WebApp.Models
{
    public class Diagnosis
    {
        public int DiagnosisId { get; set; }
        [Required (ErrorMessage = "Не указано название")]
        public string Name { get; set; }
        public ICollection<Patient> Patients { get; set; }
        public Diagnosis()
        {
            Patients = new List<Patient>();
        }

    }
}
