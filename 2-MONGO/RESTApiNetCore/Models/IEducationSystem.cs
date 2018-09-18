using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApiNetCore.Models
{
    public interface IEducationSystem
    {
        List<Student> GetStudents();
        List<Przedmiot> GetLectures();
        List<Ocena> GetNotes();
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(Student student);
        void AddLecture(Przedmiot lecture);
        void UpdateLecture(Przedmiot lectureFromDB, Przedmiot lectureFromBody);
        void DeleteLecture(Przedmiot lectureExisted);
        void UpdateNote(Przedmiot lecture, Student student, Ocena note, Ocena noteFromBody);
        void AddStudentToLecture(Student studentExisted, Przedmiot lectureExisted);
        void DeleteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted);
        void AddNoteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted, Ocena note);
        void UpdateStudentNote(Student student, Przedmiot lecture, Ocena noteTemp, Ocena note);
        void DeleteStudentNote(Student student, Przedmiot lecture, Ocena note);
        void DeleteNote(Student student, Przedmiot lecture, Ocena note);

        //FILTERS
        IEnumerable<Student> GetStudentListByNameFilter(string imie, string nazwisko);
        IEnumerable<Student> GetStudentListByDateFilter(string dataW, string dataPrzed, string dataPo );
        List<Ocena> GetNotesStudentsByFilter(Student student, Przedmiot przedmiot, string mniejszaNiz, string wiekszaNiz);
    }
}
