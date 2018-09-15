using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RESTApiNetCore.Models
{
    [DataContract(Namespace = "")]
    public class Student
    {
        public Student()
        {
            Oceny = new List<Ocena>();
            Przedmioty = new List<Przedmiot>();
        }

        [DataMember]
        public int Indeks { get; set; }
        [DataMember]
        public string Imie { get; set; }
        [DataMember]
        public string Nazwisko { get; set; }
        [DataMember]
        public DateTime DataUrodzenia { get; set; }
        public List<Ocena> Oceny { get; set; }
        public List<Przedmiot> Przedmioty { get; set; }
    }
}