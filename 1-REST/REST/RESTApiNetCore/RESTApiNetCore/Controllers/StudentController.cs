﻿using System;
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
        public IActionResult GetAllStudent()
        {
           IEnumerable<Student> studentsList = _educationSystemData.GetStudents();

           if( studentsList == null || studentsList.Count() <= 0 )
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

            student.Indeks = studentExisted.Indeks;
            student.Indeks = studentExisted.Indeks;

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

            List<Przedmiot> listLectures = new List<Przedmiot>();

            if (student == null || student.Przedmioty == null )
            {
                return NotFound();
            }

            if( student.Przedmioty.Count() <= 0 )
            {
                return NotFound();
            }

            foreach( var lectureObj in student.Przedmioty )
            {

                Przedmiot przedmiotRef = _educationSystemData.getLecture(lectureObj.Id);

                listLectures.Add(przedmiotRef);

            }

            return Ok(listLectures);
        }

        // POST  /students/{studentIndex}/lectures - zapisanie studenta na dany kurs
        [HttpPost("{studentIndex}/lectures")]
        public IActionResult PostSaveLecturesStudent([FromRoute] int studentIndex, [FromBody] Przedmiot lecture)
        {
            Student studentExisted = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex );

            Przedmiot lectureExisted = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.Id == lecture.Id);

            if(studentExisted == null || lectureExisted == null)
            {
                return BadRequest();
            }

            foreach( var przedmiot in studentExisted.Przedmioty)
            {
                if( przedmiot.Id == lectureExisted.Id)
                {
                    return BadRequest();
                }
            }

            _educationSystemData.AddStudentToLecture(studentExisted, lectureExisted);

            return CreatedAtAction( "students", new { idStudent = studentExisted.Indeks } , lectureExisted );
        }


        // GET  /students/{studentIndex}/lectures/{lectureIndex} - informacja o pojedynczym kursie na który jest zapisany student
        [HttpGet("{studentIndex}/lectures/{lectureIndex}")]
        public IActionResult GetSaveSingleLectureStudent([FromRoute] int studentIndex, [FromRoute] int lectureIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot przedmiot = _educationSystemData.GetLectures()
                .FirstOrDefault(przedmiotObj => przedmiotObj.Id == lectureIndex);

            Przedmiot przedmiotRef = null;

            if (student == null || przedmiot == null || student.Przedmioty == null)
            {
                return NotFound();
            }

            if (student.Przedmioty.Count() <= 0)
            {
                return NotFound();
            }

            bool bFoundLecture = false;

            foreach (var lectureObj in student.Przedmioty)
            {
                przedmiotRef = _educationSystemData.getLecture(lectureObj.Id);

                if (przedmiotRef != null && lectureIndex == przedmiotRef.Id)
                {
                    bFoundLecture = true;
                    break;
                }
            }

            if (!bFoundLecture) return NotFound();
 
            return Ok(przedmiotRef);
        }

        // Delete  /students/{studentIndex}/lectures - usunięcie studenta z danego kursu
        [HttpDelete("{studentIndex}/lectures/{lectureIndex}")]
        public IActionResult DeleteSavedLecturesStudent([FromRoute] int studentIndex, [FromRoute] int lectureIndex )
        {
            Student studentExisted = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot lectureExisted = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.Id == lectureIndex);

            if (studentExisted == null || lectureExisted == null)
            {
                return NotFound();
            }

            _educationSystemData.DeleteStudentFromLecture(studentExisted, lectureExisted);

            return Ok();
        }

        // GET  /students/{studentIndex}/lectures/notes - pobranie listy ocen studenta z danego kursu
        [HttpGet("{studentIndex}/lectures/{przedmiotIndex}/notes")]
        public IActionResult GetSaveNotesFromLecturesStudent([FromRoute] int studentIndex, [FromRoute] int przedmiotIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot przedmiot = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.Id == przedmiotIndex);

            List<Ocena> listNotes = new List<Ocena>();

            if (student == null || przedmiot == null)
            {
                return NotFound();
            }

            if (student.Oceny.Count() <= 0)
            {
                return NotFound();
            }

            bool bFoundPrzedmiot = false;

            foreach( var przedmiotTemp in student.Przedmioty)
            {
                if( przedmiotTemp.Id == przedmiotIndex )
                {
                    bFoundPrzedmiot = true;
                }
            }

            if (!bFoundPrzedmiot) return NotFound();

            foreach(var noteObj in _educationSystemData.GetNotes())
            {
                if(noteObj.IdPrzedmiot == przedmiot.Id && noteObj.IdStudent == student.Indeks)
                {
                    listNotes.Add(noteObj);
                }
            }

            return Ok(listNotes);
        }

        // GET  /students/{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex} - pobranie konkretnych informacji o ocenie studenta z danego kursu
        [HttpGet("{studentIndex}/lectures/{przedmiotIndex}/notes/{noteIndex}")]
        public IActionResult GetSaveConcreteNoteFromLecturesStudent([FromRoute] int studentIndex, [FromRoute] int przedmiotIndex, [FromRoute] int noteIndex)
        {
            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot przedmiot = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.Id == przedmiotIndex);

            if (student == null || przedmiot == null )
            {
                return NotFound();
            }

            if( student.Oceny == null || student.Oceny.Count() <= 0)
            {
                return NotFound();
            }

            Ocena ocena = _educationSystemData.GetNotes()
                .Find(ocenaObj => ocenaObj.Id == noteIndex 
                && ocenaObj.IdStudent == student.Indeks 
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
                            .FirstOrDefault(lectureObj => lectureObj.Id == lectureIndex);

            bool bStudentIsSavedToLecture = false;

            if (student == null || lecture == null)
            {
                return BadRequest();
            }

            if (_educationSystemData.GetNotes() == null )
            {
                return BadRequest();
            }

            foreach( var przedmioty in student.Przedmioty )
            {
                if( przedmioty.Id == lectureIndex)
                {
                    bStudentIsSavedToLecture = true;
                    break;
                }
            }

            double[] listNotesValidation = { 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0 };
            bool bIsNote = false;

            if ( !bStudentIsSavedToLecture || note.Wartosc < 2.0 || note.Wartosc > 5.0 /**TODO value validation*/)
            {
                return BadRequest();
            }

            if (note.Wartosc != 2.0 && note.Wartosc !=  2.5 && note.Wartosc != 3.0 && note.Wartosc != 3.5 && note.Wartosc != 4.0 && note.Wartosc !=  4.5 && note.Wartosc != 5.0 )
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
                            .FirstOrDefault(lectureObj => lectureObj.Id == lectureIndex);

            Ocena noteTemp = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.Id == noteIndex);

            if (student == null || lecture == null || note == null)
            {
                return NotFound();
            }

            if(note.Wartosc < 2.0 || note.Wartosc > 5.0)
            {
                return BadRequest();
            }

            noteTemp.Wartosc = note.Wartosc;

            _educationSystemData.UpdateStudentNote(student,lecture, noteTemp);

            return Ok(/*student*/);
        }

        // Delete  /students/{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex} - usunięcie oceny studenta z danego kursu
        [HttpDelete("{studentIndex}/lectures/{lectureIndex}/notes/{noteIndex}")]
        public IActionResult UpdateStudentNote([FromRoute] int studentIndex, [FromRoute] int lectureIndex, [FromRoute] int noteIndex)
        {
            Student student = _educationSystemData.GetStudents()
                .FirstOrDefault(studentObj => studentObj.Indeks == studentIndex);

            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.Id == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.Id == noteIndex);

            if (student == null || lecture == null || note == null)
            {
                return NotFound();
            }

            _educationSystemData.DeleteStudentNote(student, lecture, note);

            return Ok(/*student*/);
        }
    }
}
