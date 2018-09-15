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

        // GET  /notes 
        [HttpGet]
        public IActionResult GetAllNotes()
        {
            IEnumerable<Ocena> notesList = _educationSystemData.GetNotes();

            if (notesList == null || notesList.Count() <= 0)
            {
                return NotFound();
            }

            return Ok(notesList);
        }

        // GET  /notes/{noteIndex} 
        [HttpGet("{noteIndex}")]
        public IActionResult GetNote([FromRoute] int noteIndex)
        {
            Ocena note = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.Id == noteIndex);

            if (note == null)
            {
                return NotFound();
            }

            return Ok(note);
        }

        // POST  /notes
        [HttpPost]
        public IActionResult AddNewNoteFromBody([FromBody] Ocena note)
        {
            Ocena noteExisted = _educationSystemData.GetNotes()
                            .FirstOrDefault(noteObj => noteObj.Id == note.Id);

            Student student = _educationSystemData.GetStudents()
                            .FirstOrDefault(studentObj => studentObj.Indeks == note.IdStudent);

            if(note == null || noteExisted != null || student == null)
            {
                return BadRequest();
            }

            note.DataWystawienia = DateTime.Now;

            _educationSystemData.AddNote(note, student);

            return CreatedAtAction("notes", new { idNote = note.Id }, note);
        }

        [HttpPut("{noteIndex}")]
        public IActionResult UpdateNote([FromRoute] int noteIndex, [FromBody] Ocena note)
        {
            Ocena studentExisted = _educationSystemData.GetNotes()
                             .FirstOrDefault(noteObj => noteObj.Id == noteIndex);

            if (studentExisted == null || noteIndex != note.Id)
            {
                return NotFound();
            }

            _educationSystemData.UpdateNote(note);

            return Ok(/*note*/);
        }
    }
}