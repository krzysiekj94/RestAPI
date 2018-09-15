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

        bool UpdateLecture(Przedmiot lecture);

        void DeleteLecture(Przedmiot lectureExisted);

        void AddNote(Ocena note, Student student);

        bool UpdateNote(Ocena note);

        Przedmiot getLecture(int indexLecture);

        void AddStudentToLecture(Student studentExisted, Przedmiot lectureExisted);

        void DeleteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted);

        void AddNoteStudentFromLecture(Student studentExisted, Przedmiot lectureExisted, Ocena note);
        void UpdateStudentNote(Student student, Przedmiot lecture, Ocena note);
        void DeleteStudentNote(Student student, Przedmiot lecture, Ocena note);
        void DeleteNote(Ocena note);
    }
}
