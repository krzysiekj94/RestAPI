using MongoDB.Bson;
using MongoDB.Driver;
using RESTApiNetCore.MongoDB;
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
            MongoDBContext = new MongoDBContext();
            PrepareStudents();
            //PrepareLectures();
            //Students = new List<Student>(PrepareStudents());
            //Lectures = new List<Przedmiot>(PrepareLectures());
            //Notes = new List<Ocena>();
        }

        public MongoDBContext MongoDBContext { get; set; }

        public List<Student> GetStudents()
        {
            return MongoDBContext.Studenci.Find(_ => true).ToList();
        }

        public List<Przedmiot> GetLectures()
        {
            return MongoDBContext.Przedmioty.Find(_ => true).ToList();
        }

        public List<Ocena> GetNotes()
        {
            return MongoDBContext.Oceny.Find(_ => true).ToList();
        }

        private void PrepareStudents()
        {
            MongoDBContext.Studenci.DeleteMany(_ => true);
            MongoDBContext.Przedmioty.DeleteMany(_ => true);
            MongoDBContext.Oceny.DeleteMany( _ => true);

            Student studentPawel = new Student()
            {
                Imie = "Paweł",
                Nazwisko = "Nowak",
                Indeks = 112513,
                DataUrodzenia = new DateTime(1980, 2, 12)
            };

            Student studentTymek = new Student()
            {
                Imie = "Tymoteusz",
                Nazwisko = "Borowiak",
                Indeks = 153623,
                DataUrodzenia = new DateTime(1983, 5, 20)
            };

            Przedmiot przedmiotJezykPolski = new Przedmiot()
            {
                IdPrzedmiotu = 1,
                Nazwa = "Polski",
                Nauczyciel = "Bogdan Marecki",
            };

            Przedmiot przedmiotMatematyka = new Przedmiot()
            {
                IdPrzedmiotu = 2,
                Nazwa = "Matematyka",
                Nauczyciel = "Roman Bielecki",
            };

            MongoDBContext.Przedmioty.InsertOneAsync(przedmiotJezykPolski);
            MongoDBContext.Przedmioty.InsertOneAsync(przedmiotMatematyka);

            MongoDBContext.Studenci.InsertOneAsync(studentPawel);
            MongoDBContext.Studenci.InsertOneAsync(studentTymek);

            var przedmioty = MongoDBContext.Przedmioty.Find(_ => true).ToListAsync();
            var studenci =   MongoDBContext.Studenci.Find(_ => true).ToListAsync();

            var przedmiot = przedmioty.Result.FindAll(przedmiotTemp => przedmiotTemp.Nazwa == przedmiotJezykPolski.Nazwa).FirstOrDefault();
            var student =   studenci.Result.FindAll(studentTemp => studentTemp.Indeks == studentPawel.Indeks).FirstOrDefault();
            przedmiotJezykPolski.zapisaniStudenci.Add(student.Id);

            przedmiot = przedmioty.Result.FindAll(przedmiotTemp => przedmiotTemp.Nazwa == przedmiotMatematyka.Nazwa).FirstOrDefault();
            student = studenci.Result.FindAll(studentTemp => studentTemp.Indeks == studentTymek.Indeks).FirstOrDefault();
            przedmiotMatematyka.zapisaniStudenci.Add(student.Id);

            MongoDBContext.Przedmioty.ReplaceOneAsync(x => x.IdPrzedmiotu == 1, przedmiotJezykPolski, new UpdateOptions { IsUpsert = true });
            MongoDBContext.Przedmioty.ReplaceOneAsync(x => x.IdPrzedmiotu == 2, przedmiotMatematyka, new UpdateOptions { IsUpsert = true });
        }

        public void AddStudent(Student student)
        {
            if(student != null)
            {
                student.Oceny = new List<ObjectId>();
                MongoDBContext.Studenci.InsertOneAsync(student);
            }
        }

        public void UpdateStudent(Student updatedStudent)
        {
            if (updatedStudent != null)
            {
                MongoDBContext.Studenci.ReplaceOneAsync(x => x.Id == updatedStudent.Id, 
                    updatedStudent, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteStudent(Student student)
        {
            if(student != null )
            {
                MongoDBContext.Studenci.DeleteOneAsync( x => x.Id == student.Id);

                var przedmioty = MongoDBContext.Przedmioty.Find(_ => true).ToListAsync()
                    .Result.FindAll(przedmiotTemp => przedmiotTemp.zapisaniStudenci.Any( studentObj => studentObj == student.Id ));

                foreach(var przedmiot in przedmioty)
                {
                    przedmiot.zapisaniStudenci.RemoveAll(studentObj => studentObj == student.Id);
                    MongoDBContext.Przedmioty.ReplaceOneAsync(x => x.Id == przedmiot.Id,
                    przedmiot, new UpdateOptions { IsUpsert = true });
                }
            }
        }

        public void AddLecture(Przedmiot lecture)
        {
            if(lecture != null )
            {
                lecture.IdPrzedmiotu = generateUniqueLectureID();
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
            Przedmiot przedmiotRef = Lectures.Find(lectureObj => lectureObj.IdPrzedmiotu == indexLecture);

            try
            {
                przedmiotRef = Lectures.Find(lectureObj => lectureObj.IdPrzedmiotu == indexLecture);
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
                    Students[indexStudent].Oceny.Add(note.Id);
                }

                //note.Id = generateUniqueNoteID();
                //Notes.Add(note);
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
                lectureExisted.zapisaniStudenci.Add(studentExisted.Id);
                MongoDBContext.Przedmioty.ReplaceOneAsync(lectureObj => lectureObj.Id == lectureExisted.Id, lectureExisted, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted)
        {
            if (studentExisted != null && lectureExisted != null)
            {
                lectureExisted.zapisaniStudenci.RemoveAll(studentObj => studentObj == studentExisted.Id);

                MongoDBContext.Przedmioty.ReplaceOneAsync(przedmiotObj => przedmiotObj.Id == lectureExisted.Id,
                lectureExisted, new UpdateOptions { IsUpsert = true });
            }
        }

        public void AddNoteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted, Ocena note)
        {
            if (studentExisted != null && lectureExisted != null && note != null)
            {
                //Find last added Ocena
                var lastAddedOceny = MongoDBContext.Oceny.Find(_ => true).ToListAsync().Result.OrderByDescending(ocenaObj => ocenaObj.IdOceny).FirstOrDefault();
                int ocenyIndex = 0;

                if(lastAddedOceny != null)
                {
                    ocenyIndex++;
                }

                //Prepare Ocena field
                note.IdOceny = ocenyIndex;
                note.IdStudent = studentExisted.Id;
                note.IdPrzedmiot = lectureExisted.Id;
                note.DataWystawienia = DateTime.Now;

                //Add to Oceny
                MongoDBContext.Oceny.InsertOneAsync(note);

                //Add to concrete Student Oceny
                var ocenaLastAdded = MongoDBContext.Oceny.Find(_ => true).ToListAsync().Result.OrderByDescending(ocenaObj => ocenaObj.IdOceny).FirstOrDefault();
                studentExisted.Oceny.Add(ocenaLastAdded.Id);

                //Replace old Student data to new
                MongoDBContext.Studenci.ReplaceOneAsync(studentObj => studentObj.Id == studentExisted.Id, studentExisted, new UpdateOptions { IsUpsert = true });
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

                note.IdStudent = student.Id;
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
                && student.Id == note.IdStudent
                && lecture.IdPrzedmiotu == note.IdOceny);

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
