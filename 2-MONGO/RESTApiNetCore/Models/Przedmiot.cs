using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RESTApiNetCore.Models
{
    [DataContract(Namespace = "")]
    public class Przedmiot
    {
        public Przedmiot()
        {
            zapisaniStudenci = new List<BsonObjectId>();
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [DataMember]
        [Key]
        [Required]
        public int IdPrzedmiotu { get; set; }

        [DataMember]
        [Required]
        public string Nazwa { get; set; }

        [DataMember]
        [Required]
        public string Nauczyciel { get; set; }

        public List<BsonObjectId> zapisaniStudenci { get; set; }
    }
}
