using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Oceny = new List<ObjectId>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [DataMember]
        [Key]
        [Required]
        public int Indeks { get; set; }

        [DataMember]
        [Required]
        public string Imie { get; set; }

        [DataMember]
        [Required]
        public string Nazwisko { get; set; }

        [DataMember]
        [Required]
        [DataType(DataType.Date)]
        [JsonConverter(typeof(JsonDateConverter))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DataUrodzenia { get; set; }

        public List<ObjectId> Oceny { get; set; }
    }
}