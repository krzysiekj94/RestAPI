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

            MongoDBContext.Przedmioty.InsertOne(przedmiotJezykPolski);
            MongoDBContext.Przedmioty.InsertOne(przedmiotMatematyka);

            MongoDBContext.Studenci.InsertOne(studentPawel);
            MongoDBContext.Studenci.InsertOne(studentTymek);

            var przedmioty = MongoDBContext.Przedmioty.Find(_ => true).ToList();
            var studenci =   MongoDBContext.Studenci.Find(_ => true).ToList();

            var przedmiot = przedmioty.FindAll(przedmiotTemp => przedmiotTemp.Nazwa == przedmiotJezykPolski.Nazwa).FirstOrDefault();
            var student =   studenci.FindAll(studentTemp => studentTemp.Indeks == studentPawel.Indeks).FirstOrDefault();
            przedmiotJezykPolski.ZapisaniStudenci.Add(student.Id);

            przedmiot = przedmioty.FindAll(przedmiotTemp => przedmiotTemp.Nazwa == przedmiotMatematyka.Nazwa).FirstOrDefault();
            student = studenci.FindAll(studentTemp => studentTemp.Indeks == studentTymek.Indeks).FirstOrDefault();
            przedmiotMatematyka.ZapisaniStudenci.Add(student.Id);

            MongoDBContext.Przedmioty.ReplaceOne(x => x.IdPrzedmiotu == 1, przedmiotJezykPolski, new UpdateOptions { IsUpsert = true });
            MongoDBContext.Przedmioty.ReplaceOne(x => x.IdPrzedmiotu == 2, przedmiotMatematyka, new UpdateOptions { IsUpsert = true });
        }

        public void AddStudent(Student student)
        {
            if(student != null)
            {
                student.Oceny = new List<ObjectId>();
                MongoDBContext.Studenci.InsertOne(student);
            }
        }

        public void UpdateStudent(Student updatedStudent)
        {
            if (updatedStudent != null)
            {
                MongoDBContext.Studenci.ReplaceOne(x => x.Id == updatedStudent.Id, 
                    updatedStudent, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteStudent(Student student)
        {
            if(student != null )
            {
                MongoDBContext.Studenci.DeleteOne( x => x.Id == student.Id);

                var przedmioty = MongoDBContext.Przedmioty.Find(_ => true).ToList()
                    .FindAll(przedmiotTemp => przedmiotTemp.ZapisaniStudenci.Any( studentObj => studentObj == student.Id ));

                foreach(var przedmiot in przedmioty)
                {
                    przedmiot.ZapisaniStudenci.RemoveAll(studentObj => studentObj == student.Id);
                    MongoDBContext.Przedmioty.ReplaceOne(x => x.Id == przedmiot.Id,
                    przedmiot, new UpdateOptions { IsUpsert = true });
                }
            }
        }

        public void AddLecture(Przedmiot lecture)
        {
            long newIndexLecture = MongoDBContext.Przedmioty.Count(_ => true);

            if (lecture != null )
            {
                //Find last added Przedmiot
                int przedmiotIndex = 0;
                Przedmiot lastAddedPrzedmiot = MongoDBContext.Przedmioty.Find(_ => true).ToList().OrderByDescending(przedmiotObj => przedmiotObj.IdPrzedmiotu).FirstOrDefault();

                if(lastAddedPrzedmiot != null)
                {
                    przedmiotIndex = lastAddedPrzedmiot.IdPrzedmiotu;
                    przedmiotIndex++;
                }

                lecture.IdPrzedmiotu = przedmiotIndex;
                MongoDBContext.Przedmioty.InsertOne(lecture);
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

        public void UpdateNote(Przedmiot lecture, Student student, Ocena note, Ocena noteFromBody)
        {
            UpdateStudentNote(student, lecture, note, noteFromBody);
        }

        public void AddStudentToLecture(Student studentExisted, Przedmiot lectureExisted)
        {
            if( studentExisted != null && lectureExisted != null)
            {
                lectureExisted.ZapisaniStudenci.Add(studentExisted.Id);
                MongoDBContext.Przedmioty.ReplaceOne(lectureObj => lectureObj.Id == lectureExisted.Id, lectureExisted, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted)
        {
            if (studentExisted != null && lectureExisted != null)
            {
                lectureExisted.ZapisaniStudenci.RemoveAll(studentObj => studentObj == studentExisted.Id);

                MongoDBContext.Przedmioty.ReplaceOne(przedmiotObj => przedmiotObj.Id == lectureExisted.Id,
                lectureExisted, new UpdateOptions { IsUpsert = true });
            }
        }

        public void AddNoteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted, Ocena note)
        {
            if (studentExisted != null && lectureExisted != null && note != null)
            {
                //Find last added Ocena
                long ocenyIndex = 0;
                Ocena lastAddedNote = MongoDBContext.Oceny.Find(_ => true).ToList().OrderByDescending(ocenaObj => ocenaObj.IdOceny).FirstOrDefault();

                if( lastAddedNote != null )
                {
                    ocenyIndex = lastAddedNote.IdOceny;
                    ocenyIndex++;
                }

                //Prepare Ocena field
                note.IdOceny = (int)ocenyIndex;
                note.IndexStudent = studentExisted.Indeks;
                note.IdStudent = studentExisted.Id;
                note.IdPrzedmiot = lectureExisted.Id;
                note.DataWystawienia = DateTime.Now;

                //Add to Oceny
                MongoDBContext.Oceny.InsertOne(note);

                //Add to concrete Student Oceny
                var ocenaLastAdded = MongoDBContext.Oceny.Find(_ => true).ToList().OrderByDescending(ocenaObj => ocenaObj.IdOceny).FirstOrDefault();
                studentExisted.Oceny.Add(ocenaLastAdded.Id);

                //Replace old Student data to new
                MongoDBContext.Studenci.ReplaceOne(studentObj => studentObj.Id == studentExisted.Id, studentExisted, new UpdateOptions { IsUpsert = true });
            }
        }

        public void UpdateStudentNote(Student student, Przedmiot lecture, Ocena noteTemp, Ocena updatedNote)
        {
            if (student != null && lecture != null && noteTemp != null && updatedNote != null)
            {
                //Prepare Ocena field
                float wartoscOceny = updatedNote.Wartosc;

                updatedNote = noteTemp;
                updatedNote.DataWystawienia = DateTime.Now;
                updatedNote.Wartosc = wartoscOceny;

                //Replace in Oceny (only, student store objectid note )
                MongoDBContext.Oceny.ReplaceOne(ocenyObj => ocenyObj.Id == updatedNote.Id, updatedNote, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteStudentNote(Student student, Przedmiot lecture, Ocena note)
        {
            if (student != null && lecture != null && note != null)
            {
                //Delete from Oceny
                MongoDBContext.Oceny.DeleteMany(ocenaObj => ocenaObj.Id == note.Id 
                                                && ocenaObj.IdPrzedmiot == note.IdPrzedmiot
                                                && ocenaObj.IdStudent == note.IdStudent );

                //Delete from concrete student
                student.Oceny.RemoveAll(ocenaObj => ocenaObj == note.Id);

                //Replace modified student
                MongoDBContext.Studenci.ReplaceOne(studentObj => studentObj.Id == student.Id,
                student, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteNote(Student student, Przedmiot lecture, Ocena note)
        {
            DeleteStudentNote(student, lecture, note);
        }
    }
}
