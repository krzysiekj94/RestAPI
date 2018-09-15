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
    public class Ocena
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Range(2.0, 5.0)]
        public float Wartosc { get; set; }

        [DataMember]
        public DateTime DataWystawienia { get; set; }

        [DataMember]
        public int IdStudent { get; set; }

        [DataMember]
        public int IdPrzedmiot { get; set; }
    }
}
