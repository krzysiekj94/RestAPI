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
        [Key]
        [Required]
        public int IdOceny { get; set; }

        [DataMember]
        [Required]
        public int IndexStudent { get; set; }

        [DataMember]
        [Required]
        [Range(2.0, 5.0)]
        public float Wartosc { get; set; }

        [DataMember]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DataWystawienia { get; set; }

        [Required]
        [ForeignKey(nameof(Student))]
        public ObjectId IdStudent { get; set; }

        [Required]
        [ForeignKey(nameof(Przedmiot))]
        public ObjectId IdPrzedmiot { get; set; }
    }
}