using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using WebApp.Models;
using WebApp.ViewModels;
using System.Threading.Tasks;
namespace WebApp.Controllers
{
    public class DbController : Controller
    {
        private readonly AppDbContext db;
        public DbController(AppDbContext appDbContext)
        {
            db = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Diagnoses()
        {
            return View(await db.Diagnoses.ToListAsync());
        }
        public async Task<IActionResult> Doctors()
        {
            return View(await db.Doctors.ToListAsync());
        }
        public async Task<IActionResult> Patients()
        {
            db.Doctors.Load();
            return View(await db.Patients.ToListAsync());
        }
        public async Task<IActionResult> DiagnosisPatient()
        {
            db.Doctors.Load();
            return View(await db.Patients.Include(c => c.Diagnoses).ToListAsync());
        }
        public IActionResult CreateDiagnosis()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDiagnosis(Diagnosis diagnosis)
        {
            if (ModelState.IsValid)
            {
                db.Diagnoses.Add(diagnosis);
                await db.SaveChangesAsync();
                return RedirectToAction("Diagnoses");
            }
            return View(diagnosis);
        }
        public IActionResult CreatePatient()
        {
            SelectList doctors = new SelectList(db.Doctors, "DoctorId", "Fullname");
            ViewBag.Doctors = doctors;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePatient(PatientViewModel model)
        {
            if (db.Doctors.Find(model.DoctorId) == null)
            {
                ModelState.AddModelError("DoctorId", "Несуществующий Id врача");
            }
            if (ModelState.IsValid)
            {
                Patient patient = new Patient();
                patient.Surname = model.Surname;
                patient.Name = model.Name;
                patient.Patronymic = model.Patronymic;
                patient.Age = model.Age;
                patient.Doctor = db.Doctors.Find(model.DoctorId);
                db.Patients.Add(patient);
                await db.SaveChangesAsync();
                return RedirectToAction("Patients");
            }
            return View(model);
        }
        public IActionResult CreateDoctor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDoctor(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                await db.SaveChangesAsync();
                return RedirectToAction("Doctors");
            }
            return View(doctor);
        }
        public IActionResult CreateDiagnosisPatient()
        {
            SelectList patients = new SelectList(db.Patients, "PatientId", "Fullname");
            ViewBag.Patients = patients;
            SelectList diagnoses = new SelectList(db.Diagnoses, "DiagnosisId", "Name");
            ViewBag.Diagnoses = diagnoses;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDiagnosisPatient(DiagnosisPatient model)
        {
            if (db.Patients.Find(model.PatientId) == null)
            {
                ModelState.AddModelError("PatientId", "Несуществующий Id пациента");
            }
            if (db.Diagnoses.Find(model.DiagnosisId) == null)
            {
                ModelState.AddModelError("DiagnosisId", "Несуществующий Id недуга");
            }
            Patient patient0 = db.Patients.Find(model.PatientId);
            Diagnosis diagnosis0 = db.Diagnoses.Find(model.DiagnosisId);
            db.Entry(patient0).Collection(a => a.Diagnoses).Load();
            if (patient0.Diagnoses.Contains(diagnosis0))
            {
                ModelState.AddModelError("DiagnosisPatient", "Дублирование данных");
            }
            if (ModelState.IsValid)
            {
                db.Patients.Find(model.PatientId).Diagnoses.Add(db.Diagnoses.Find(model.DiagnosisId));
                await db.SaveChangesAsync();
                return RedirectToAction("DiagnosisPatient");
            }
            SelectList patients = new SelectList(db.Patients, "PatientId", "Fullname");
            ViewBag.Patients = patients;
            SelectList diagnoses = new SelectList(db.Diagnoses, "DiagnosisId", "Name");
            ViewBag.Diagnoses = diagnoses;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDiagnosis(int? id)
        {
            if (id != null)
            {
                Diagnosis diagnosis = new Diagnosis { DiagnosisId = id.Value };
                db.Entry(diagnosis).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Diagnoses");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeletePatient(int? id)
        {
            if (id != null)
            {
                Patient patient = new Patient { PatientId = id.Value };
                db.Entry(patient).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Patients");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int? id)
        {
            if (id != null)
            {
                Doctor doctor = new Doctor { DoctorId = id.Value };
                db.Entry(doctor).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Doctors");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDiagnosisPatient(int? id, int? id0)
        {
            if (id != null && id0 != null && id != 0 && id0 != 0)
            {
                if (db.Patients.Find(id) != null && db.Diagnoses.Find(id0) != null)
                {
                    db.Diagnoses.Load();
                    db.Patients.Load();
                    Patient patient = db.Patients.Find(id);
                    await db.Entry(patient).Collection(b => b.Diagnoses).LoadAsync();
                    Diagnosis diagnosis = db.Diagnoses.Find(id0);
                    patient.Diagnoses.Remove(diagnosis);
                    db.Patients.Update(patient);
                    await db.SaveChangesAsync();
                    return RedirectToAction("DiagnosisPatient");
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> EditDiagnosis(int? id)
        {
            if (id != null)
            {
                Diagnosis? diagnosis = await db.Diagnoses.FirstOrDefaultAsync(p => p.DiagnosisId == id);
                if (diagnosis != null) return View(diagnosis);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditDiagnosis(Diagnosis diagnosis)
        {
            if (ModelState.IsValid)
            {
                db.Diagnoses.Update(diagnosis);
                await db.SaveChangesAsync();
                return RedirectToAction("Diagnoses");
            }
            return View(diagnosis);
        }
        public async Task<IActionResult> EditDoctor(int? id)
        {
            if (id != null)
            {
                Doctor? doctor = await db.Doctors.FirstOrDefaultAsync(p => p.DoctorId == id);
                if (doctor != null) return View(doctor);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditDoctor(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Doctors.Update(doctor);
                await db.SaveChangesAsync();
                return RedirectToAction("Doctors");
            }
            return View(doctor);
        }
        public async Task<IActionResult> EditPatient(int? id)
        {
            SelectList doctors = new SelectList(db.Doctors, "DoctorId", "Fullname");
            ViewBag.Doctors = doctors;
            if (id != null)
            {
                Patient? patient = await db.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
                if (patient != null)
                {
                    db.Doctors.Load();
                    PatientViewModel model = new PatientViewModel();
                    model.PatientId = patient.PatientId;
                    model.Surname = patient.Surname;
                    model.Name = patient.Name;
                    model.Patronymic = patient.Patronymic;
                    model.Age = patient.Age;
                    model.DoctorId = patient.Doctor.DoctorId;
                    return View(model);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditPatient(PatientViewModel model)
        {
            if (db.Doctors.Find(model.DoctorId) == null)
            {
                ModelState.AddModelError("DoctorId", "Несуществующий Id врача");
            }
            if (ModelState.IsValid)
            {
                Patient patient = db.Patients.Find(model.PatientId);
                db.Entry(patient).Reference(b => b.Doctor).Load();
                patient.Surname = model.Surname;
                patient.Name = model.Name;
                patient.Patronymic = model.Patronymic;
                patient.Age = model.Age;
                patient.Doctor = db.Doctors.Find(model.DoctorId);
                db.Patients.Update(patient);
                await db.SaveChangesAsync();
                return RedirectToAction("Patients");
            }
            return View(model);
        }
        public async Task<IActionResult> EditDiagnosisPatient(int? id, int? id0)
        {
            SelectList patients = new SelectList(db.Patients, "PatientId", "Fullname");
            ViewBag.Patients = patients;
            SelectList diagnoses = new SelectList(db.Diagnoses, "DiagnosisId", "Name");
            ViewBag.Diagnoses = diagnoses;
            if (id != null && id0 != null)
            {
                db.Patients.Load();
                db.Diagnoses.Load();
                Patient? patient = await db.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
                Diagnosis? diagnosis = await db.Diagnoses.FirstOrDefaultAsync(p => p.DiagnosisId == id0);
                if (patient != null && diagnosis != null)
                {
                    DiagnosisPatient model = new DiagnosisPatient();
                    model.PatientId = patient.PatientId;
                    model.OldPatientId = patient.PatientId;
                    model.DiagnosisId = diagnosis.DiagnosisId;
                    model.OldDiagnosisId = diagnosis.DiagnosisId;
                    return View(model);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditDiagnosisPatient(DiagnosisPatient model)
        {
            if (db.Patients.Find(model.PatientId) == null)
            {
                ModelState.AddModelError("PatientId", "Несуществующий Id пациента");
            }
            if (db.Diagnoses.Find(model.DiagnosisId) == null)
            {
                ModelState.AddModelError("DiagnosisId", "Несуществующий Id недуга");
            }
            Patient patient0 = db.Patients.Find(model.PatientId);
            Diagnosis diagnosis0 = db.Diagnoses.Find(model.DiagnosisId);
            db.Entry(patient0).Collection(a => a.Diagnoses).Load();
            if (patient0.Diagnoses.Contains(diagnosis0))
            {
                ModelState.AddModelError("DiagnosisPatient", "Дублирование данных");
            }
            if (ModelState.IsValid)
            {
                Patient patient = db.Patients.Find(model.OldPatientId);
                Diagnosis diagnosis = db.Diagnoses.Find(model.OldDiagnosisId);
                db.Entry(patient).Collection(a => a.Diagnoses).Load();
                patient.Diagnoses.Remove(diagnosis);
                db.Patients.Update(patient);
                db.Patients.Find(model.PatientId).Diagnoses.Add(db.Diagnoses.Find(model.DiagnosisId));
                await db.SaveChangesAsync();
                return RedirectToAction("DiagnosisPatient");
            }
            SelectList patients = new SelectList(db.Patients, "PatientId", "Fullname");
            ViewBag.Patients = patients;
            SelectList diagnoses = new SelectList(db.Diagnoses, "DiagnosisId", "Name");
            ViewBag.Diagnoses = diagnoses;
            return View(model);
        }
    }
}
