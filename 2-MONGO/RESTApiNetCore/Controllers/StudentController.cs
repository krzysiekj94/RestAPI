using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RESTApiNetCore.Models;

namespace RESTApiNetCore.Controllers
{
    [Route("students")]
    public class StudentController : Controller
    {
        private readonly IEducationSystem _educationSystemData;

        public StudentController( IEducationSystem educationSystem )
        {
            _educationSystemData = educationSystem;
        }

        // GET  /students 
        [HttpGet]
        public IActionResult GetAllStudent([FromQuery] string imie, [FromQuery] string nazwisko, 
            [FromQuery(Name = "urodzonypo")] DateTime? urodzonypo,
            [FromQuery(Name = "urodzonyprzed")] DateTime? urodzonyprzed)
        {
            IEnumerable<Student> studentsList = null;

            if (imie != null || nazwisko != null || urodzonypo != null || urodzonyprzed != null)
            {
                studentsList = _educationSystemData.GetStudentListByNameFilter(imie, nazwisko, urodzonypo, urodzonyprzed);
            }
            else
            {
                studentsList = _educationSystemData.GetStudents();
            }

            if ( studentsList == null || studentsList.Count() <= 0 )
            {
                return NotFound();
            }

           return Ok(studentsList);
        }

        // GET  /students/{studentIndex} 
        [HttpGet("{studentIndex}")]
        public IActionResult GetStudent( [FromRoute] int studentIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault( studentObj => studentObj.Indeks == studentIndex );

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST  /students
        [HttpPost]
        public IActionResult AddNewStudentFromBody( [FromBody] Student student )
        {
            Student studentExisted = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == student.Indeks || student.Indeks == studentObj.Indeks );

            if( student == null || studentExisted != null )
            {
                return BadRequest();
            }

            _educationSystemData.AddStudent(student);

            return CreatedAtAction("students", new { id = student.Indeks } , student );
        }

        [HttpPut("{studentIndex}")]
        public IActionResult UpdateStudent( [FromRoute] int studentIndex, [FromBody] Student student )
        {
           Student studentExisted = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            if(studentExisted == null )
            {
                return NotFound();
            }

            student.Id = studentExisted.Id;
            student.Indeks = studentExisted.Indeks;
            student.Oceny = studentExisted.Oceny;

            _educationSystemData.UpdateStudent(student);

            return Ok();
        }

        [HttpDelete("{studentIndex}")]
        public IActionResult DeleteStudent([FromRoute] int studentIndex)
        {
            Student studentExisted = _educationSystemData.GetStudents()
                             .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            if (studentExisted == null )
            {
                return NotFound();
            }

            _educationSystemData.DeleteStudent(studentExisted);

            return Ok();
        }

        // GET  /students/{studentIndex}/lectures - lista kursów na które jest zapisany student
        [HttpGet("{studentIndex}/lectures")]
        public IActionResult GetSaveLecturesStudent([FromRoute] int studentIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            List<Przedmiot> listLectures = null;

            if( student != null)
            {
                listLectures = _educationSystemData.GetLectures()
                .FindAll(przedmiotTemp => przedmiotTemp.ZapisaniStudenci.Any(studentObj => studentObj == student.Id));
            }

            if (student == null || listLectures == null || listLectures.Count <= 0)
            {
                return NotFound();
            }

            return Ok(listLectures);
        }

        // POST  /students/{studentIndex}/lectures - zapisanie studenta na dany kurs
        [HttpPost("{studentIndex}/lectures")]
        public IActionResult PostSaveLecturesStudent([FromRoute] int studentIndex, [FromBody] Przedmiot lecture)
        {
            Student studentExisted = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex );

            Przedmiot existedLecture = existedLecture = _educationSystemData.GetLectures()
                 .FirstOrDefault(przedmiotTemp => przedmiotTemp.IdPrzedmiotu == lecture.IdPrzedmiotu);

            if (studentExisted == null 
                || existedLecture == null )
            {
                return BadRequest();
            }

            var studentExistInLecture = existedLecture.ZapisaniStudenci.FirstOrDefault(studentObj => studentObj == studentExisted.Id);

            if (studentExistInLecture != null)
            {
                return BadRequest();
            }

            _educationSystemData.AddStudentToLecture(studentExisted, existedLecture);

            return CreatedAtAction( "students", new { idStudent = studentExisted.Indeks } , existedLecture);
        }


        // GET  /students/{studentIndex}/lectures/{lectureIndex} - informacja o pojedynczym kursie na który jest zapisany student
        [HttpGet("{studentIndex}/lectures/{lectureIndex}")]
        public IActionResult GetSaveSingleLectureStudent([FromRoute] int studentIndex, [FromRoute] int lectureIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot przedmiot = _educationSystemData.GetLectures()
                .FirstOrDefault(przedmiotObj => przedmiotObj.IdPrzedmiotu == lectureIndex);

            if (student == null || przedmiot == null)
            {
                return NotFound();
            }

            Przedmiot przedmiotRef = _educationSystemData.GetLectures()
             .FirstOrDefault(przedmiotTemp => przedmiotTemp.IdPrzedmiotu == przedmiot.IdPrzedmiotu
                                              && przedmiotTemp.ZapisaniStudenci.Any(studentObj => studentObj == student.Id));

            if(przedmiotRef == null)
            {
                return NotFound();
            }
 
            return Ok(przedmiotRef);
        }

        // Delete  /students/{studentIndex}/lectures - usunięcie studenta z danego kursu
        [HttpDelete("{studentIndex}/lectures/{lectureIndex}")]
        public IActionResult DeleteSavedLecturesStudent([FromRoute] int studentIndex, [FromRoute] int lectureIndex )
        {
            Student studentExisted = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot lectureExisted = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            if (studentExisted == null || lectureExisted == null)
            {
                return NotFound();
            }

            if( lectureExisted.ZapisaniStudenci.FirstOrDefault( studentObj => studentObj == studentExisted.Id ) == null )
            {
                return NotFound();
            }

            _educationSystemData.DeleteStudentFromLecture(studentExisted, lectureExisted);

            return Ok();
        }

        // GET  /students/{studentIndex}/lectures/notes - pobranie listy ocen studenta z danego kursu
        [HttpGet("{studentIndex}/lectures/{przedmiotIndex}/notes")]
        public IActionResult GetSaveNotesFromLecturesStudent([FromRoute] int studentIndex, [FromRoute] int przedmiotIndex,
            [FromQuery(Name = "WiekszeLubRowne")] double? wiekszeLubRowne, [FromQuery(Name = "MniejszeLubRowne")] double? mniejszeLubRowne)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot przedmiot = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == przedmiotIndex);

            List<Ocena> listNotes = new List<Ocena>();

            if (student == null || przedmiot == null)
            {
                return NotFound();
            }

            if (student.Oceny.Count() <= 0)
            {
                return NotFound();
            }

            if (przedmiot.ZapisaniStudenci.FirstOrDefault(studentObj => studentObj == student.Id) == null)
            {
                return NotFound();
            }

            if (wiekszeLubRowne != null || mniejszeLubRowne != null)
            {
                listNotes = _educationSystemData.GetNotesStudentsByFilter(student, przedmiot, wiekszeLubRowne, mniejszeLubRowne);
                return Ok(listNotes);
            }

            listNotes = _educationSystemData.GetNotes()
                .FindAll(noteObj => noteObj.IdPrzedmiot == przedmiot.Id
                    && noteObj.IdStudent == student.Id);

            return Ok(listNotes);
        }

        // GET  /students/{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex} - pobranie konkretnych informacji o ocenie studenta z danego kursu
        [HttpGet("{studentIndex}/lectures/{przedmiotIndex}/notes/{noteIndex}")]
        public IActionResult GetSaveConcreteNoteFromLecturesStudent([FromRoute] int studentIndex, [FromRoute] int przedmiotIndex, [FromRoute] int noteIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot przedmiot = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == przedmiotIndex);

            if (student == null || przedmiot == null )
            {
                return NotFound();
            }

            if( student.Oceny == null || student.Oceny.Count() <= 0)
            {
                return NotFound();
            }

                Ocena ocena = _educationSystemData.GetNotes()
                    .FirstOrDefault(ocenaObj => ocenaObj.IdOceny == noteIndex
                                     && ocenaObj.IdStudent == student.Id 
                                     && ocenaObj.IdPrzedmiot == przedmiot.Id);

            if( ocena == null )
            {
                return NotFound();
            }

            return Ok(ocena);
        }

        //POST /students/{studentIndex}/lectures/{lectureIndex}/notes - wstawienie konkretnemu studentowi oceny
        [HttpPost("{studentIndex}/lectures/{lectureIndex}/notes")]
        public IActionResult PostSaveLecturesStudent([FromRoute] int studentIndex, [FromRoute] int lectureIndex, [FromBody] Ocena note)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            if (student == null || lecture == null || _educationSystemData.GetNotes() == null)
            {
                return BadRequest();
            }

            var studentExistInLecture = lecture.ZapisaniStudenci.FirstOrDefault(studentObj => studentObj == student.Id);

            if ( studentExistInLecture == null || note.Wartosc < 2.0 || note.Wartosc > 5.0)
            {
                return BadRequest();
            }

            if (note.Wartosc != 2.0 && note.Wartosc !=  2.5 
                && note.Wartosc != 3.0 && note.Wartosc != 3.5 
                && note.Wartosc != 4.0 && note.Wartosc !=  4.5 
                && note.Wartosc != 5.0 )
            {
                return BadRequest();
            }

            _educationSystemData.AddNoteStudentFromLecture(student, lecture, note);

            return CreatedAtAction("students", new { idLecture = lecture.Id, idStudent = student.Indeks }, note);
        }


        //PUT students/{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex} - edycja oceny konkretnego studenta
        [HttpPut("{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex}")]
        public IActionResult UpdateStudentNote([FromRoute] int studentIndex, [FromRoute] int lectureIndex, [FromRoute] int noteIndex, [FromBody] Ocena note)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            if (student == null || lecture == null || note == null)
            {
                return NotFound();
            }

            Ocena noteTemp = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.IdOceny == noteIndex
                                            && noteObj.IdPrzedmiot == lecture.Id
                                            && noteObj.IdStudent == student.Id);

            if( noteTemp == null )
            {
                return NotFound();
            }

            if (note.Wartosc != 2.0 && note.Wartosc != 2.5
                && note.Wartosc != 3.0 && note.Wartosc != 3.5
                && note.Wartosc != 4.0 && note.Wartosc != 4.5
                && note.Wartosc != 5.0)
            {
                return BadRequest();
            }

            _educationSystemData.UpdateStudentNote(student,lecture, noteTemp, note);

            return Ok();
        }

        // Delete  /students/{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex} - usunięcie oceny studenta z danego kursu
        [HttpDelete("{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex}")]
        public IActionResult UpdateStudentNote([FromRoute] int studentIndex, [FromRoute] int lectureIndex, [FromRoute] int noteIndex)
        {
            Student student = _educationSystemData.GetStudents()
                .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            if (student == null || lecture == null)
            {
                return NotFound();
            }

            Ocena note = _educationSystemData.GetNotes()
                .FirstOrDefault(noteObj => noteObj.IdOceny == noteIndex
                && noteObj.IdPrzedmiot == lecture.Id
                && noteObj.IdStudent == student.Id);

            if(note == null)
            {
                return NotFound();
            }

            _educationSystemData.DeleteStudentNote(student, lecture, note);

            return Ok();
        }
    }
}
