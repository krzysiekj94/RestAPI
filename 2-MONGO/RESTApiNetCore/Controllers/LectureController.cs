﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTApiNetCore.Models;

namespace RESTApiNetCore.Controllers
{
    [Route("lectures")]
    public class LectureController : Controller
    {
        private readonly IEducationSystem _educationSystemData;

        public LectureController(IEducationSystem educationSystem)
        {
            _educationSystemData = educationSystem;
        }

        // GET  /lectures 
        [HttpGet]
        public IActionResult GetAllLectures()
        {
            IEnumerable<Przedmiot> lecturesList = _educationSystemData.GetLectures();

            if (lecturesList == null || lecturesList.Count() <= 0)
            {
                return NotFound();
            }

            return Ok(lecturesList);
        }

        // GET  /lectures 
        [HttpGet("{indexLecture}")]
        public IActionResult GetOneLectures([FromRoute] int indexLecture)
        {
            Przedmiot lecture = _educationSystemData.GetLectures()
                .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == indexLecture);

            if (lecture == null )
            {
                return NotFound();
            }
           
            return Ok(lecture);
        }

        // GET  /lectures/notes/{idNote} 
        [HttpGet("{lectureIndex}/notes/{idNote}")]
        public IActionResult GetNotesFromLecture([FromRoute] int lectureIndex, [FromRoute] int idNote)
        {
            Przedmiot przedmiot = _educationSystemData.GetLectures()
                                    .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.IdOceny == idNote 
                                                    && noteObj.IdPrzedmiot == przedmiot.Id);

            if( note == null )
            {
                return NotFound();
            }

            Student student = _educationSystemData.GetStudents()
             .FirstOrDefault(studentObj => studentObj.Oceny.Any(ocenyObj => ocenyObj == note.Id));

            if( student == null )
            {
                return NotFound();
            }

            return Ok(note);
        }

        // usunięcie oceny z wykładu http://localhost:8004/lectures/1/notes/10
        [HttpDelete("{lectureIndex}/notes/{idNote}")]
        public IActionResult DeleteNotesFromLecture([FromRoute] int lectureIndex, [FromRoute] int idNote)
        {
            Przedmiot lecture = _educationSystemData.GetLectures()
                                    .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.IdOceny == idNote 
                                            && noteObj.IdPrzedmiot == lecture.Id);

            if (lecture == null || note == null)
            {
                return NotFound();
            }

            Student student = _educationSystemData.GetStudents()
             .FirstOrDefault(studentObj => studentObj.Oceny.Any(ocenyObj => ocenyObj == note.Id));

            if( student == null )
            {
                return NotFound();
            }

            _educationSystemData.DeleteNote(student,lecture,note);

            return Ok();
        }

        // modyfikacja oceny z wykładu http://localhost:8004/lectures/1/notes/10
        // PUT  /lectures/{studentIndex} 
        [HttpPut("{lectureIndex}/notes/{idNote}")]
        public IActionResult UpdateNoteFromLecture([FromRoute] int lectureIndex, [FromRoute] int idNote, [FromBody] Ocena noteFromBody )
        {
            Przedmiot lecture = _educationSystemData.GetLectures()
                                    .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                .FirstOrDefault(noteObj => noteObj.IdOceny == idNote 
                                && noteObj.IdPrzedmiot == lecture.Id);

            if (lecture == null || note == null)
            {
                return NotFound();
            }

            Student student = _educationSystemData.GetStudents()
            .FirstOrDefault(studentObj => studentObj.Oceny.Any(ocenyObj => ocenyObj == note.Id));

            if (student == null)
            {
                return NotFound();
            }

            _educationSystemData.UpdateNote(lecture, student, note, noteFromBody);

            return Ok();
        }

        // GET  /lectures/{studentIndex} 
        [HttpGet("{lectureIndex}/notes")]
        public IActionResult GetNotesFromLecture([FromRoute] int lectureIndex)
        {
            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            List<Ocena> oceny = _educationSystemData.GetNotes()
                            .FindAll(lectureObj => lectureObj.IdPrzedmiot == lecture.Id);

            if (lecture == null || oceny == null || oceny.Count <= 0)
            {
                return NotFound();
            }

            return Ok(oceny);
        }

        // POST  /lectures
        [HttpPost]
        public IActionResult AddNewLectureFromBody([FromBody] Przedmiot lecture)
        {
            if(lecture == null)
            {
                return BadRequest();
            }

            _educationSystemData.AddLecture(lecture);

            return CreatedAtAction("lectures", new { indexLecture = lecture.Id } , lecture);
        }

        [HttpPut("{lectureIndex}")]
        public IActionResult UpdateLecture([FromRoute] int lectureIndex, [FromBody] Przedmiot lecture)
        {
            Przedmiot lectureExist = _educationSystemData.GetLectures()
                             .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex );

            if(lectureExist == null)
            {
                return NotFound();
            }

            lecture.Id = lectureExist.Id;

            if ( !_educationSystemData.UpdateLecture(lecture) )
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{lectureIndex}")]
        public IActionResult UpdateLecture([FromRoute] int lectureIndex)
        {
            Przedmiot lectureExisted = _educationSystemData.GetLectures()
                             .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            if (lectureExisted == null)
            {
                return NotFound();
            }

            _educationSystemData.DeleteLecture(lectureExisted);

            return Ok();
        }
    }
}