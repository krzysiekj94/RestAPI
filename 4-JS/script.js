"use strict";
var apiUrl = "http://localhost:8010/";
var collection = function() {
	var self = ko.observableArray();
	
	self.url = apiUrl + "students";
	self.postUrl = self.url;
	self.get = function(query) {
		var url = self.url;
		if(query){
			url = url + query;
		}
		$.ajax({
			url: url,
			method: "GET",
			dataType: "json",
			success: function(data) {
				console.log(data);
				self.removeAll();
				data.forEach(function(element, index, array) {
					var object = ko.mapping.fromJS(element);
					self.push(object);
				});
			}
			});
	}
	self.parseQuery = function() {
		self.get('?' + $.param(ko.mapping.toJS(self.queryParams)));
	}
	return self;
}
var StudentModel = function(){
	var self = this;
	self.students = new collection();
	
	self.students.queryParams = {
    	indeks: ko.observable(),
		imie: ko.observable(),
		nazwisko: ko.observable(),
		dataUrodzenia: ko.observable()
	}
	Object.keys(self.students.queryParams).forEach(function(key) {
		self.students.queryParams[key].subscribe(function() {
			self.students.parseQuery();
		});
	});
	self.students.get();
}
var model = new StudentModel();
$(document).ready(function() {
	
	ko.applyBindings(model);
});