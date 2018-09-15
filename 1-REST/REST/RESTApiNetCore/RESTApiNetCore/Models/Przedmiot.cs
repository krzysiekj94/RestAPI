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
    public class Przedmiot
    {
        public Przedmiot()
        {
            Oceny = new List<Ocena>();
            Studenci = new List<Student>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nazwa { get; set; }

        [DataMember]
        public string Nauczyciel { get; set; }

        public IEnumerable<Ocena> Oceny { get; set; }
        public IEnumerable<Student> Studenci { get; set; }
    }
}
