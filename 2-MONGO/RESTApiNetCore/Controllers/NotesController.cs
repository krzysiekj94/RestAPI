using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTApiNetCore.Models;

namespace RESTApiNetCore.Controllers
{
    [Route("notes")]
    public class NotesController : Controller
    {
        private readonly IEducationSystem _educationSystemData;

        public NotesController(IEducationSystem educationSystem)
        {
            _educationSystemData = educationSystem;
        }

        [HttpGet]
        public IActionResult GetOceny()
        {
            List<Ocena> oceny = _educationSystemData.GetNotes().ToList();
            return Ok(oceny);
        }

        [HttpGet("{idOceny}")]
        public IActionResult GetOcena([FromRoute] int idOceny)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ocena ocena =  _educationSystemData.GetNotes().FirstOrDefault( ocenaObj => ocenaObj.IdOceny == idOceny);

            if (ocena == null)
            {
                return NotFound();
            }

            return Ok(ocena);
        }

        [HttpPut("{idOceny}")]
        public IActionResult PutOcena([FromRoute] int idOceny, [FromBody] Ocena ocena)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == ocena.Indeks);

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == ocena.IdPrzedmiotu);

            if (student == null || lecture == null || ocena == null)
            {
                return NotFound();
            }

            Ocena noteTemp = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.IdOceny == ocena.IdOceny
                                            && noteObj.IdPrzedmiot == lecture.Id
                                            && noteObj.IdStudent == student.Id);

            if (noteTemp == null)
            {
                return NotFound();
            }

            if (ocena.Wartosc != 2.0 && ocena.Wartosc != 2.5
                && ocena.Wartosc != 3.0 && ocena.Wartosc != 3.5
                && ocena.Wartosc != 4.0 && ocena.Wartosc != 4.5
                && ocena.Wartosc != 5.0)
            {
                return BadRequest();
            }

            _educationSystemData.UpdateStudentNote(student, lecture, noteTemp, ocena);

            return Ok();
        }

        [HttpPost]
        public IActionResult PostOcena([FromBody] Ocena ocena)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Student student = _educationSystemData.GetStudents()
                             .FirstOrDefault(studentObj => studentObj.Indeks == ocena.Indeks);

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == ocena.IdPrzedmiotu);

            if (student == null || lecture == null || _educationSystemData.GetNotes() == null)
            {
                return BadRequest();
            }

            var studentExistInLecture = lecture.ZapisaniStudenci.FirstOrDefault(studentObj => studentObj == student.Id);

            if (studentExistInLecture == null || ocena.Wartosc < 2.0 || ocena.Wartosc > 5.0)
            {
                return BadRequest();
            }

            if (ocena.Wartosc != 2.0 && ocena.Wartosc != 2.5
                && ocena.Wartosc != 3.0 && ocena.Wartosc != 3.5
                && ocena.Wartosc != 4.0 && ocena.Wartosc != 4.5
                && ocena.Wartosc != 5.0)
            {
                return BadRequest();
            }

            _educationSystemData.AddNoteStudentFromLecture(student, lecture, ocena);

            return CreatedAtAction("Ocena", new { id = ocena.IdOceny }, ocena);
        }

        [HttpDelete("{idOceny}")]
        public IActionResult DeleteOcena([FromRoute] int idOceny)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ocena ocena = _educationSystemData.GetNotes()
                .FirstOrDefault(ocenaObj => ocenaObj.IdOceny== idOceny);

            if (ocena == null)
            {
                return NotFound();
            }

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == ocena.IdPrzedmiotu);

            Student student = _educationSystemData.GetStudents()
                .FirstOrDefault(studentObj => studentObj.Indeks == ocena.Indeks);

            if (lecture == null || student == null)
            {
                return NotFound();
            }

            _educationSystemData.DeleteStudentNote(student, lecture, ocena);

            return Ok();
        }
    }
}