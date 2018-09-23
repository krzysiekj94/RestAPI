"use strict";

const apiUrl = "http://localhost:8010/";
const studentsEndpoint = "students";
const lecturesEndpoint = "lectures";
const gradesEndpoint = "notes";

const QueryInvoker = function () {
    const self = ko.observableArray();
    self.get = function (endpoint, cb) {
        const url = apiUrl + endpoint;
        $.ajax({
            url: url,
            method: "GET",
            success: cb,
            dataType: "json",
            contentType: "application/json"
        });
    }
    self.add = function (endpoint, item, cb) {
        const url = apiUrl + endpoint;
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(item),
            success: cb,
            dataType: "json",
            contentType: "application/json"
        });
    }
    self.update = function (endpoint, item, cb) {
        const url = apiUrl + endpoint;
        $.ajax({
            type: "PUT",
            url: url,
            data: JSON.stringify(item),
            success: cb,
            dataType: "json",
            contentType: "application/json"
        });
    }
    self.delete = function (endpoint, cb) {
        const url = apiUrl + endpoint;
        $.ajax({
            type: "DELETE",
            url: url,
            success: cb,
            dataType: "json",
            contentType: "application/json"
        });
    }
    return self;
}
const q = new QueryInvoker();
const urlParams = new URLSearchParams(window.location.search);

const AppViewModel = function () {
    const self = this;
    self.students = new ko.observableArray();
    self.studentsQuery = {
        imie: ko.observable(""),
        nazwisko: ko.observable(""),
        dataUrodzeniaOd: ko.observable(""),
        dataUrodzeniaDo: ko.observable("")
    };
    self.studentsFiltered = ko.computed(function () {
        const imie = ko.mapping.toJS(self.studentsQuery.imie);
        const nazwisko = ko.mapping.toJS(self.studentsQuery.nazwisko);
        let filtered = self.students().filter(function (i) {
            const regImie = new RegExp(imie.toLowerCase());
            const regNazwisko = new RegExp(nazwisko.toLowerCase());
            return regImie.test(i.imie.toLowerCase()) === true && regNazwisko.test(i.nazwisko.toLowerCase()) === true;
        });
        filtered = filtered.filter(function(i) {
            let dataUrodzenia = self.studentsQuery.dataUrodzeniaOd();
            if (dataUrodzenia === "") {
                return true;
            }
            let dataUrodzeniaDate = new Date(i.dataUrodzenia);
            let dataUrodzeniaOd = new Date(dataUrodzenia);
            return dataUrodzeniaDate >= dataUrodzeniaOd;
        })
        filtered = filtered.filter(function(i) {
            let dataUrodzenia = self.studentsQuery.dataUrodzeniaDo();
            if (dataUrodzenia === "") {
                return true;
            }
            let dataUrodzeniaDate = new Date(i.dataUrodzenia);
            let dataUrodzeniaDo = new Date(dataUrodzenia);
            return dataUrodzeniaDate <= dataUrodzeniaDo;
        })
        return filtered;
    });
    self.studentsForLecture = new ko.observableArray();
    self.selectedLectureForStudents = ko.observable("");
    self.selectedStudentToAssign = ko.observable();
    self.lectures = new ko.observableArray();
    self.lecturesQuery = {
        nazwa: ko.observable(""),
        nauczyciel: ko.observable(""),
    };
    self.lecturesFiltered = ko.computed(function () {
        const nazwa = ko.mapping.toJS(self.lecturesQuery.nazwa);
        const nauczyciel = ko.mapping.toJS(self.lecturesQuery.nauczyciel);
        const filtered = self.lectures().filter(function (i) {
            const regNazwa = new RegExp(nazwa.toLowerCase());
            const regNauczyciel = new RegExp(nauczyciel.toLowerCase());
            return regNazwa.test(i.nazwa.toLowerCase()) === true && regNauczyciel.test(i.nauczyciel.toLowerCase()) === true;
        });
        return filtered;
    });
    self.grades = new ko.observableArray();
    self.gradesQuery = {
        idPrzedmiotu: ko.observable(""),
        indeks: ko.observable(""),
    };
    self.gradesFiltered = ko.computed(function () {
        const idPrzedmiotu = ko.mapping.toJS(self.gradesQuery.idPrzedmiotu);
        const indeks = ko.mapping.toJS(self.gradesQuery.indeks);
        const filtered = self.grades().filter(function (i) {
            if (idPrzedmiotu == null & indeks == null) {
                return true;
            }
            if (idPrzedmiotu == null & indeks != null) {
                return i.indeks == indeks;
            }
            if (idPrzedmiotu != null & indeks == null) {
                return i.idPrzedmiotu == idPrzedmiotu;
            }
            return i.idPrzedmiotu == idPrzedmiotu && i.indeks == indeks;
        });
        return filtered;
    });
    self.current = {};
    self.current.student = {
        indeks: ko.observable(),
        imie: ko.observable(),
        nazwisko: ko.observable(),
        dataUrodzenia: ko.observable(),
    };
    self.current.lecture = {
        nazwa: ko.observable(),
        nauczyciel: ko.observable(),
    };
    self.current.grade = {
        idPrzedmiotu: ko.observable(),
        indeks: ko.observable(),
        wartosc: ko.observable(),
        dataWystawienia: ko.observable(),
    };
    self.getStudentFullName = function (student) {
        return student.imie + ' ' + student.nazwisko;
    };
    self.getLectureName = function (lecture) {
        return lecture.nazwa;
    };
    self.refreshStudents = function (cb) {
        q.get(studentsEndpoint, function (students) {
            appModel.students.removeAll();
            students.forEach(s => {
                appModel.students.push(s);
            });
            if (cb) {
                cb();
            }
        })
    };
    self.refreshLectures = function (cb) {
        q.get(lecturesEndpoint, function (lectures) {
            appModel.lectures.removeAll();
            lectures.forEach(s => {
                appModel.lectures.push(s);
            });
            if (cb) {
                cb();
            }
        })
    };
    self.refreshGrades = function (cb) {
        let endpoint = gradesEndpoint;
        q.get(endpoint, function (grades) {
            appModel.grades.removeAll();
            grades.forEach(g => {
                appModel.grades.push(g);
            });
            if (cb) {
                cb();
            }
        })
    };
    self.refreshAll = function () {
        self.refreshStudents(function () {
            self.refreshLectures(function () {
                self.refreshGrades();
            });
        });
    };
    self.clearQueries = function () {
        self.studentsQuery.imie("");
        self.studentsQuery.nazwisko("");
        self.studentsQuery.dataUrodzeniaOd("");
        self.studentsQuery.dataUrodzeniaDo("");
        self.lecturesQuery.nazwa("");
        self.lecturesQuery.nauczyciel("");
        self.gradesQuery.idPrzedmiotu("");
        self.gradesQuery.indeks("");
    };
    self.addStudent = function () {
        const plain = ko.mapping.toJS(self.current.student);
        q.add(studentsEndpoint, plain, function () {
            self.refreshAll();
            self.clearQueries();
        });
    };
    self.updateStudent = function (student) {
        const plain = ko.mapping.toJS(student);
        q.update(studentsEndpoint + "/" + plain.indeks, plain, function () {
            self.refreshAll();
        });
    };
    self.removeStudent = function (student) {
        const plain = ko.mapping.toJS(student);
        q.delete(studentsEndpoint + "/" + plain.indeks, function () {
            self.refreshAll();
        });
    };
    self.addLecture = function () {
        const plain = ko.mapping.toJS(self.current.lecture);
        q.add(lecturesEndpoint, plain, function () {
            self.refreshAll();
            self.clearQueries();
        });
    };
    self.updateLecture = function (lecture) {
        const plain = ko.mapping.toJS(lecture);
        q.update(lecturesEndpoint + "/" + plain.idPrzedmiotu, plain, function () {
            self.refreshAll();
        });
    };
    self.removeLecture = function (lecture) {
        const plain = ko.mapping.toJS(lecture);
        q.delete(lecturesEndpoint + "/" + plain.idPrzedmiotu, function () {
            self.refreshAll();
        });
    };
    self.addGrade = function () {
        const plain = ko.mapping.toJS(self.current.grade);
        q.add(gradesEndpoint, plain, function () {
            self.refreshAll();
            self.clearQueries();
        });
        console.log(plain);
    };
    self.updateGrade = function (grade) {
        const plain = ko.mapping.toJS(grade);
        q.update(gradesEndpoint + "/" + plain.idOceny, plain, function () {
            self.refreshAll();
        });
    };
    self.removeGrade = function (grade) {
        const plain = ko.mapping.toJS(grade);
        q.delete(gradesEndpoint + "/" + plain.idOceny, function () {
            self.refreshAll();
        });
    };
    self.showStudentGrades = function (student) {
        self.refreshGrades(function() {
            self.clearQueries();
            const plain = ko.mapping.toJS(student);
            self.gradesQuery.indeks(plain.indeks)
        });
        window.location = "#grades";
    };
    self.showLectureGrades = function (lecture) {
        self.refreshGrades(function() {
            self.clearQueries();
            const plain = ko.mapping.toJS(lecture);
            self.gradesQuery.idPrzedmiotu(plain.idPrzedmiotu)
            window.location = "#grades";
        });
    };
    self.showStudentsForLecture = function(lecture) {
        self.selectedLectureForStudents(lecture);
        q.get(lecturesEndpoint + "/" + lecture.idPrzedmiotu + "/students", function (students) {
            self.studentsForLecture.removeAll();
            students.forEach(s => {
                self.studentsForLecture.push(s);
            });
            window.location = "#studentsForLecture";
        })
    };
    self.unassignStudentFromLecture = function(student) {
        const plainLecture = ko.mapping.toJS(self.selectedLectureForStudents);
        const unassignEndpoint = studentsEndpoint + "/" + student.indeks + "/lectures/" + plainLecture.idPrzedmiotu;
        q.delete(unassignEndpoint);
        self.studentsForLecture.remove(function(storedStudent) {
            return student.indeks == storedStudent.indeks;
        });
    };
    self.assignStudentToLecture = function() {
        const plainLecture = self.selectedLectureForStudents();
        const plainStudent = self.selectedStudentToAssign();
        if (plainLecture == null ) return;
        if (plainStudent == null ) return;
        const alreadyAssigned = self.studentsForLecture().find(function(s) {
            return s.numerIndeksu == plainStudent.indeks;
        });
        if (alreadyAssigned != null) {
            return;
        }
        const assignEndpoint = studentsEndpoint + "/" + plainStudent.indeks + "/lectures/" + plainLecture.idPrzedmiotu;
        q.add(assignEndpoint);
        self.studentsForLecture.push(plainStudent);
    };
}

const appModel = new AppViewModel();

$(document).ready(function () {
    $.support.cors = true;
    ko.applyBindings(appModel);
    appModel.refreshAll();
});