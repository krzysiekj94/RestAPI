using System;
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
            List<Przedmiot> lecturesListTemp = new List<Przedmiot>();

            if (lecturesList == null || lecturesList.Count() <= 0)
            {
                return NotFound();
            }

            foreach(var lecture in lecturesList)
            {
                lecturesListTemp.Add(lecture);
            }

            return Ok(lecturesListTemp);
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

        // POST  /lectures/{lectureIndex}/{notes}
        [HttpPost("{lectureIndex}/notes")]
        public IActionResult AddNewGradeFromLecture([FromRoute] int lectureIndex, [FromBody] Ocena grade)
        {
            Przedmiot lectureExisted = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Student studentExisted = _educationSystemData.GetStudents()
                .FirstOrDefault(studentObj => studentObj.Id ==  grade.IdStudent);

            Ocena noteExisted = _educationSystemData.GetNotes()
                .FirstOrDefault(noteObj => noteObj.Id == grade.Id);

            if(lectureExisted == null || noteExisted != null)
            {
                return BadRequest();
            }

            grade.DataWystawienia = DateTime.Now;
            grade.IdPrzedmiot = lectureExisted.Id;// lectureIndex;

            _educationSystemData.AddNote(grade,studentExisted);

            return CreatedAtAction("lectures", new { idLecture = lectureIndex, idNote = grade.Id } , grade );
        }

        // TODO1 pobranie oceny z wykładu http://localhost:8004/lectures/1/notes/10

        // GET  /lectures/notes/{idNote} 
        [HttpGet("{lectureIndex}/notes/{idNote}")]
        public IActionResult GetNotesFromLecture([FromRoute] int lectureIndex, [FromRoute] int idNote)
        {
            Przedmiot przedmiot = _educationSystemData.GetLectures()
                                    .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.IdOceny == idNote && noteObj.IdPrzedmiot == przedmiot.Id);

            if( note == null )
            {
                return NotFound();
            }

            return Ok(note);
        }

        // usunięcie oceny z wykładu http://localhost:8004/lectures/1/notes/10
        [HttpDelete("{lectureIndex}/notes/{idNote}")]
        public IActionResult DeleteNotesFromLecture([FromRoute] int lectureIndex, [FromRoute] int idNote)
        {
            Przedmiot przedmiot = _educationSystemData.GetLectures()
                                    .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.IdOceny == idNote && noteObj.IdPrzedmiot == przedmiot.Id);

            if (note == null)
            {
                return NotFound();
            }

            _educationSystemData.DeleteNote(note);

            return Ok();
        }

        // modyfikacja oceny z wykładu http://localhost:8004/lectures/1/notes/10
        // GET  /lectures/{studentIndex} 
        [HttpPut("{lectureIndex}/notes/{idNote}")]
        public IActionResult UpdateNoteFromLecture([FromRoute] int lectureIndex, [FromRoute] int idNote, [FromBody] Ocena noteFromBody )
        {
            Przedmiot przedmiot = _educationSystemData.GetLectures()
                                    .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            Ocena note = _educationSystemData.GetNotes()
                .FirstOrDefault(noteObj => noteObj.IdOceny == idNote && noteObj.IdPrzedmiot == przedmiot.Id);

            if( noteFromBody.Id != note.Id )
            {
                return NotFound();
            }

            if (note == null)
            {
                return NotFound();
            }

            _educationSystemData.UpdateNote(noteFromBody);

            return Ok();
        }

        // GET  /lectures/{studentIndex} 
        [HttpGet("{lectureIndex}/notes")]
        public IActionResult GetNotesFromLecture([FromRoute] int lectureIndex)
        {
            Przedmiot lecture = _educationSystemData.GetLectures()
                            .FirstOrDefault(lectureObj => lectureObj.IdPrzedmiotu == lectureIndex);

            List<Ocena> oceny = new List<Ocena>();

            if(lecture == null)
            {
                return NotFound();
            }

            foreach( var ocenaTemp in _educationSystemData.GetNotes() )
            {

                if( ocenaTemp.IdPrzedmiot == lecture.Id)
                {
                    oceny.Add(ocenaTemp);
                }
            }
            
            if(oceny == null || oceny.Count() <= 0)
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