using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace RESTApiNetCore.Models
{
    [DataContract(Namespace = "")]
    public class Ocena
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [DataMember]
        [Required]
        public int _id { get; set; }

        [DataMember]
        [Required]
        [Range(2.0, 5.0)]
        public float Wartosc { get; set; }

        [DataMember]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DataWystawienia { get; set; }

        [DataMember]
        [Required]
        [ForeignKey(nameof(Student))]
        public ObjectId IdStudent { get; set; }

        [DataMember]
        [Required]
        [ForeignKey(nameof(Przedmiot))]
        public ObjectId IdPrzedmiot { get; set; }
    }
}