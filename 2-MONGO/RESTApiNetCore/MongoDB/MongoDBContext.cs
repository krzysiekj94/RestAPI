using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RESTApiNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApiNetCore.MongoDB
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database = null;

        public MongoDBContext()
        {
            MongoClient client = new MongoClient("mongodb://localhost:8004");

            if(client != null)
                _database = client.GetDatabase("EducationSystem");
        }

        public IMongoCollection<Student> Studenci
        {
            get
            {
                return _database.GetCollection<Student>("Studenci");
            }
        }

        public IMongoCollection<Przedmiot> Przedmioty
        {
            get
            {
                return _database.GetCollection<Przedmiot>("Przedmioty");
            }
        }

        public IMongoCollection<Ocena> Oceny
        {
            get
            {
                return _database.GetCollection<Ocena>("Oceny");
            }
        }
    }
}
