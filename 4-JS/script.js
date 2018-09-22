"use strict";

const apiUrl = "http://localhost:8010/";
const studentsEndpoint = "students";

const QueryInvoker = function () {
    const self = ko.observableArray();
    self.getStudents = function (query, cb) {
        const url = apiUrl + studentsEndpoint;
        if (query) {
            url = url + query;
        }
        $.ajax({
            url: url,
            method: "GET",
            dataType: "json",
            success: function (data) {
                cb(data, null);
            },
        });
    }
    self.addStudent = function (student, cb) {
        const url = apiUrl + studentsEndpoint;
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(student),
            success: cb,
            contentType: "application/json"
        });
    }
    self.updateStudent = function (student, cb) {
        const url = apiUrl + studentsEndpoint + "/" + student.indeks;
        $.ajax({
            type: "PUT",
            url: url,
            data: JSON.stringify(student),
            success: cb,
            contentType: "application/json"
        });
    }
    self.removeStudent = function (student, cb) {
        const url = apiUrl + studentsEndpoint + "/" + student.indeks;
        $.ajax({
            type: "DELETE",
            url: url,
            success: cb,
            contentType: "application/json"
        });
    }
    return self;
}
const q = new QueryInvoker();

const AppViewModel = function () {
    const self = this;
    self.students = new ko.observableArray();
    self.queryParams = {
        imie: ko.observable(),
        nazwisko: ko.observable()
    };
    self.current = {};
    self.current.student = {
        indeks: ko.observable(),
        imie: ko.observable(),
        nazwisko: ko.observable(),
        dataUrodzenia: ko.observable(),
    };
    self.refreshStudents = function () {
        q.getStudents(null, function (students) {
            appModel.students.removeAll();
            students.forEach(s => {
                appModel.students.push(s);
            });
        })
    };
    self.addStudent = function () {
        const plain = ko.mapping.toJS(self.current.student);
        q.addStudent(plain, function() {
            self.refreshStudents();
        });
    };
    self.updateStudent = function (student) {
        const plain = ko.mapping.toJS(student);
        q.updateStudent(plain, function() {
            self.refreshStudents();
        });
    };
    self.removeStudent = function (student) {
        const plain = ko.mapping.toJS(student);
        q.removeStudent(plain, function() {
            self.refreshStudents();
        });
    };
    self.showStudentGrades = function (student) {
        const plain = ko.mapping.toJS(student);
        console.log(plain);
        window.location = '#grades' 
    };
}

const appModel = new AppViewModel();

$(document).ready(function () {
    $.support.cors = true;
    ko.applyBindings(appModel);
    appModel.refreshStudents();
});