using MongoDB.Bson;
using MongoDB.Driver;
using RESTApiNetCore.MongoDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApiNetCore.Models
{
    public class EducationSystem : IEducationSystem
    {
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

        public Przedmiot AddLecture(Przedmiot lecture)
        {
            Przedmiot lectureToInsert = new Przedmiot();

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

                lectureToInsert.IdPrzedmiotu = przedmiotIndex;
                lectureToInsert.Nauczyciel = lecture.Nauczyciel;
                lectureToInsert.Nazwa = lecture.Nazwa;

                MongoDBContext.Przedmioty.InsertOne(lectureToInsert);

                return lectureToInsert;
            }

            return null;
        }

        public void UpdateLecture(Przedmiot lectureFromDB, Przedmiot lectureFromBody)
        {
            if( lectureFromDB != null && lectureFromBody != null )
            {
                lectureFromDB.Nauczyciel = lectureFromBody.Nauczyciel;
                lectureFromDB.Nazwa = lectureFromBody.Nazwa;

                MongoDBContext.Przedmioty.ReplaceOne(przedmiotObj => przedmiotObj.Id == lectureFromDB.Id,
                  lectureFromDB, new UpdateOptions { IsUpsert = true });
            }
        }

        public void DeleteLecture(Przedmiot lectureExisted)
        {
            var noteContainDeleteLecture = MongoDBContext.Oceny.Find(_ => true).ToList().FindAll( ocenaObj => ocenaObj.IdPrzedmiot == lectureExisted.Id);
            List<ObjectId> listNoteObjectContainIdDeletedLecture = new List<ObjectId>(); 

            foreach( var note in noteContainDeleteLecture)
            {
                listNoteObjectContainIdDeletedLecture.Add(note.Id);
            }    
       
            //Delete Oceny which contains Id deleted lecture from concrete student and then replace student note list 
            var studenciContainDeleteLecture = MongoDBContext.Studenci.Find(_ => true).ToList()
                .FindAll(studentObj =>
                        studentObj.Oceny.Any(noteObj => listNoteObjectContainIdDeletedLecture.Any(noteTempObj => noteTempObj == noteObj)));

            foreach (var student in studenciContainDeleteLecture)
            {
                student.Oceny.RemoveAll(ocenaObj => listNoteObjectContainIdDeletedLecture.Contains(ocenaObj));
                MongoDBContext.Studenci.ReplaceOne(studentObj => studentObj.Id == student.Id,
                student, new UpdateOptions { IsUpsert = true });
            }

            //Remove all Oceny, which contains Id deleted lecture
            foreach( var deleteNote in noteContainDeleteLecture)
            {
                MongoDBContext.Oceny.DeleteOne(ocenaObj => ocenaObj.Id == deleteNote.Id);
            }

            //Delete Przedmiot
            MongoDBContext.Przedmioty.DeleteOne( przedmiotObj => przedmiotObj.Id == lectureExisted.Id);

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

        //FILTERS

        public IEnumerable<Student> GetStudentListByNameFilter(string imie, string nazwisko)
        {
            try
            {
                List<FilterDefinition<Student>> filters =
                    new List<FilterDefinition<Student>>();

                if( imie != null)
                {
                    filters.Add(Builders<Student>.Filter
                                .Regex("Imie", ".*" + imie + ".*"));
                }

                if(nazwisko != null )
                {
                    filters.Add(Builders<Student>.Filter
                                .Regex("Nazwisko", ".*" + nazwisko + ".*"));
                }

                FilterDefinition<Student> filter = null;

                if (imie == null || nazwisko == null)
                {
                    filter = Builders<Student>.Filter.Or(filters);
                }
                else
                {
                    filter = Builders<Student>.Filter.And(filters);
                }

                return MongoDBContext.Studenci.Find(filter).ToList();
            }
            catch (Exception studenciException)
            {
                throw studenciException;
            }
        }

        public IEnumerable<Student> GetStudentListByDateFilter(string dataW, string dataPrzed, string dataPo)
        {
            DateModes dateMode = DateModes.UnknownMode;
            DateTime dateTime = DateTime.Now;

            try
            {
                if( dataW != null )
                {
                    if( DateTime.TryParse(dataW, out dateTime) )
                    {
                        dateMode = DateModes.FilterDateEqual;
                    }
                }
                else if( dataPrzed != null )
                {
                    if (DateTime.TryParse(dataPrzed, out dateTime))
                    {
                        dateMode = DateModes.FilterDateBefore;
                    }
                }
                else if( dataPo != null)
                {
                    if (DateTime.TryParse(dataPo, out dateTime))
                    {
                        dateMode = DateModes.FilterDateAfter;
                    }
                }

                FilterDefinition<Student> filter = null;

                switch( dateMode )
                {
                    case DateModes.FilterDateEqual:

                        filter = Builders<Student>.Filter
                                .Eq("DataUrodzenia", dateTime);
                        break;

                    case DateModes.FilterDateBefore:

                        filter = Builders<Student>.Filter
                                .Lt<DateTime>("DataUrodzenia", dateTime);
                        break;

                    case DateModes.FilterDateAfter:

                        filter = Builders<Student>.Filter
                                .Gt<DateTime>("DataUrodzenia", dateTime);
                        break;

                    case DateModes.UnknownMode:
                        break;
                }
                
                return MongoDBContext.Studenci.Find(filter).ToList();
            }
            catch (Exception studenciException)
            {
                throw studenciException;
            }
        }

        public List<Ocena> GetNotesStudentsByFilter(Student student, Przedmiot przedmiot, string mniejszaNiz, string wiekszaNiz)
        {
            double mniejszaNizDouble = 0.0;
            double wiekszaNizDouble = 0.0;

            try
            {
                List<FilterDefinition<Ocena>> filters =
                    new List<FilterDefinition<Ocena>>()
                    {
                        Builders<Ocena>.Filter
                            .Eq("IdStudent", student.Id),
                        Builders<Ocena>.Filter
                            .Eq("IdPrzedmiot", przedmiot.Id),
                    };


                if (mniejszaNiz != null)
                {
                    if (Double.TryParse(mniejszaNiz, NumberStyles.Number, CultureInfo.InvariantCulture, out mniejszaNizDouble))
                    {
                        filters.Add(
                            Builders<Ocena>.Filter
                            .Lt("Wartosc", (float)mniejszaNizDouble)
                        );
                    }
                }
                else if (wiekszaNiz != null)
                {
                    if (Double.TryParse(wiekszaNiz, NumberStyles.Number, CultureInfo.InvariantCulture, out wiekszaNizDouble))
                    {
                        filters.Add(
                            Builders<Ocena>.Filter
                            .Gt("Wartosc", (float)wiekszaNizDouble)
                        );
                    }
                }

                FilterDefinition<Ocena> filter = Builders<Ocena>.Filter.And(filters);

                return MongoDBContext.Oceny.Find(filter).ToList();
            }
            catch (Exception studenciException)
            {
                throw studenciException;
            }
        }

        public IEnumerable<Przedmiot> GetLecturesByTeacherName(string prowadzacy)
        {
            try
            {

                FilterDefinition<Przedmiot> filter = Builders<Przedmiot>.Filter
                                .Eq("Nauczyciel", prowadzacy);

                return MongoDBContext.Przedmioty.Find(filter).ToList();
            }
            catch (Exception studenciException)
            {
                throw studenciException;
            }
        }
    }
}
