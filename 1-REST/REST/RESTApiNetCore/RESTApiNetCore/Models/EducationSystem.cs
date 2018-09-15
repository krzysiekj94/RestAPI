using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApiNetCore.Models
{
    public class EducationSystem : IEducationSystem
    {
        private List<Student> Students;
        private List<Ocena> Notes;
        private List<Przedmiot> Lectures;

        static int counterStudentId = 10;
        static int counterNotesId = 10;
        static int counterLectures = 10;

        public EducationSystem()
        {
            Students = new List<Student>(PrepareStudents());
            Lectures = new List<Przedmiot>(PrepareLectures());
            Notes = new List<Ocena>();
        }

        public List<Student> GetStudents()
        {
            return Students;
        }

        public List<Przedmiot> GetLectures()
        {
            return Lectures;
        }

        public List<Ocena> GetNotes()
        {
            return Notes;
        }

        List<Przedmiot> PrepareLectures()
        {
            List<Przedmiot> lectures = new List<Przedmiot>();

            Przedmiot lecture = new Przedmiot()
            {
                Id = 1,
                Nazwa = "Język polski",
                Nauczyciel = "Mikołaj Nowacki",
            };

            lectures.Add(lecture);

            lecture = new Przedmiot()
            {
                Id = 2,
                Nazwa = "Matematyka",
                Nauczyciel = "Marta Polak",
            };

            lectures.Add(lecture);

            return lectures;
        }

        private List<Student> PrepareStudents()
        {
            List<Student> students = new List<Student>();

            Student student = new Student()
            {
                Imie = "Paweł",
                Nazwisko = "Nowak",
                Indeks = 112513,
                DataUrodzenia = new DateTime(1980, 2, 12)
            };

            Przedmiot przedmiotRef = new Przedmiot();
            przedmiotRef.Id = 1;
            student.Przedmioty.Add( przedmiotRef );

            students.Add(student);

            student = new Student()
            {
                Imie = "Tymoteusz",
                Nazwisko = "Borowiak",
                Indeks = 153623,
                DataUrodzenia = new DateTime(1983, 5, 20)
            };

            przedmiotRef = new Przedmiot();
            przedmiotRef.Id = 2;
            student.Przedmioty.Add(przedmiotRef);

            students.Add(student);

            return students;
        }

        public void AddStudent(Student student)
        {
            if(student != null)
            {
                //student.Id = generateUniqueStudentID();
                Students.Add(student);
            }
        }

        public void UpdateStudent(Student updatedStudent)
        {
            var studentToReplace = Students.Find(studentObj => studentObj.Indeks == updatedStudent.Indeks);

            if (studentToReplace != null)
            {
                var indexOfReplacedStudent = Students.IndexOf(studentToReplace);

                if (indexOfReplacedStudent >= 0)
                {
                    Students[indexOfReplacedStudent] = updatedStudent;
                }
            }
        }

        public void DeleteStudent(Student student)
        {
            var studentToDelete = Students.Find(studentObj => studentObj.Indeks == student.Indeks);

            if(studentToDelete != null )
            {
                var indexDeleteStudent = Students.IndexOf(studentToDelete);

                if(indexDeleteStudent >= 0)
                {
                    Students.RemoveAt(indexDeleteStudent);
                }
            }
        }

        public void AddLecture(Przedmiot lecture)
        {
            if(lecture != null )
            {
                lecture.Id = generateUniqueLectureID();
                Lectures.Add(lecture);
            }
        }

        private int generateUniqueLectureID()
        {
            return counterLectures++; //(Lectures.Count < 0) ? 0 : Lectures.Count + 1;
        }

        private int generateUniqueStudentID()
        {
            return counterStudentId++; //(Students.Count < 0) ? 0 : Students.Count + 1;
        }

        private int generateUniqueNoteID()
        {
            return counterNotesId++; //(Notes.Count < 0) ? 0 : Notes.Count + 1;
        }

        public Przedmiot getLecture( int indexLecture )
        {
            Przedmiot przedmiotRef = Lectures.Find(lectureObj => lectureObj.Id == indexLecture);

            try
            {
                przedmiotRef = Lectures.Find(lectureObj => lectureObj.Id == indexLecture);
            }
            catch
            {
                przedmiotRef = null;
            }

            return przedmiotRef;
        }

        public bool UpdateLecture(Przedmiot updateLecture)
        {
            bool IsLectureUpdated = false;
            var lectureToReplace = Lectures.Find(lectureObj => lectureObj.Id == updateLecture.Id);

            if (lectureToReplace != null)
            {
                var indexOfReplacedStudent = Lectures.IndexOf(lectureToReplace);

                if (indexOfReplacedStudent >= 0)
                {
                    Lectures[indexOfReplacedStudent] = updateLecture;
                    IsLectureUpdated = true;
                }
            }

            return IsLectureUpdated;
        }

        public void DeleteLecture(Przedmiot lectureExisted)
        {
            var lectureToDelete = Lectures.Find(studentObj => studentObj.Id == lectureExisted.Id);
            int indexDeleteLecture = -1;

            if (lectureToDelete != null)
            {
                indexDeleteLecture = Lectures.IndexOf(lectureToDelete);

                if (indexDeleteLecture >= 0)
                {
                    Lectures.RemoveAt(indexDeleteLecture);
                }
            }
        }

        public void AddNote(Ocena note, Student student)
        {
            int indexStudent = -1;

            if (note != null && student != null)
            {
                indexStudent = Students.IndexOf(student);

                if (indexStudent > -1)
                {
                    Students[indexStudent].Oceny.Add(note);
                }

                note.Id = generateUniqueNoteID();
                Notes.Add(note);
            }
        }

        public bool UpdateNote(Ocena updatedNote)
        {
            bool IsNoteUpdated = false;
            var noteToReplace = Notes.Find(noteObj => noteObj.Id == updatedNote.Id);

            if (noteToReplace != null)
            {
                var indexOfReplacedNote = Notes.IndexOf(noteToReplace);

                if (indexOfReplacedNote >= 0)
                {
                    Notes[indexOfReplacedNote] = updatedNote;
                    IsNoteUpdated = true;
                }
            }

            return IsNoteUpdated;
        }

        public void AddStudentToLecture(Student studentExisted, Przedmiot lectureExisted)
        {
            if( studentExisted != null && lectureExisted != null)
            {
                var studentFounded = Students.Find(studentObj => studentObj.Indeks == studentExisted.Indeks);
                int indexFoundedStudent = Students.IndexOf(studentFounded);
                Students[indexFoundedStudent].Przedmioty.Add(lectureExisted);
                //studentExisted.Przedmioty.Add(lectureExisted);
            }
        }

        public void DeleteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted)
        {
            int indexFoundedStudent = -1;
            int indexFoundedLecture = -1;

            if (studentExisted != null && lectureExisted != null)
            {
                indexFoundedStudent = Students.IndexOf(studentExisted);
                var lectureFounded = Students[indexFoundedStudent].Przedmioty.Find(lectureObj => lectureObj.Id == lectureExisted.Id);
                indexFoundedLecture = Students[indexFoundedStudent].Przedmioty.IndexOf(lectureFounded);

                if (indexFoundedLecture >= 0)
                {
                    Students[indexFoundedStudent].Przedmioty.RemoveAt(indexFoundedLecture);
                }
            }
        }

        public void AddNoteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted, Ocena note)
        {
            var studentFounded = Students.Find(studentObj => studentObj.Indeks == studentExisted.Indeks);
            var lectureFounded = Lectures.Find(lectureObj => lectureObj.Id == lectureExisted.Id);

            if (studentExisted != null && lectureExisted != null && note != null)
            {
                int indexStudent = Students.IndexOf(studentFounded);
                int indexLecture = Lectures.IndexOf(lectureFounded);

                note.Id = generateUniqueNoteID();
                note.IdStudent = studentExisted.Indeks;
                note.IdPrzedmiot = lectureExisted.Id;
                note.DataWystawienia = DateTime.Now;

                Students[indexStudent].Oceny.Add(note);
                Notes.Add(note);
                //studentExisted.Przedmioty.Add(lectureExisted);
            }
        }

        public void UpdateStudentNote(Student student, Przedmiot lecture, Ocena note)
        {
            var studentFounded = Students.Find(studentObj => studentObj.Indeks == student.Indeks);
            var lectureFounded = Lectures.Find(lectureObj => lectureObj.Id == lecture.Id);
            var noteFounded = Notes.Find(lectureObj => lectureObj.Id == note.Id);

            if (studentFounded != null && lectureFounded != null && noteFounded != null)
            {
                int indexStudent = Students.IndexOf(studentFounded);
                int indexLecture = Lectures.IndexOf(lectureFounded);
                int indexNote = Notes.IndexOf(noteFounded);

                note.IdStudent = student.Indeks;
                note.IdPrzedmiot = lecture.Id;
                note.DataWystawienia = DateTime.Now;

                //Students[indexStudent].Oceny.Add(note);
                Notes.RemoveAt(indexNote);
                Notes.Insert(indexNote, note);
                //studentExisted.Przedmioty.Add(lectureExisted);
            }
        }

        public void DeleteStudentNote(Student student, Przedmiot lecture, Ocena note)
        {
            if (student != null && lecture != null && note != null)
            {
                Ocena ocena = Notes.Find(noteObj => noteObj.Id == note.Id 
                && student.Indeks == note.IdStudent
                && lecture.Id == note.IdPrzedmiot );

                if( ocena != null )
                {
                    var indexFoundedNotes = Notes.IndexOf(ocena);
                    Notes.RemoveAt(indexFoundedNotes);
                }
            }
        }

        public void DeleteNote(Ocena note)
        {
            if (note != null)
            {
                Ocena ocena = Notes.Find(noteObj => noteObj.Id == note.Id);

                if (ocena != null)
                {
                    var indexFoundedNotes = Notes.IndexOf(ocena);
                    Notes.RemoveAt(indexFoundedNotes);
                }
            }
        }
    }
}
