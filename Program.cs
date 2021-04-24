using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqDynamicQuery
{
    /// <summary>
    /// QUESTO CODICE MOSTRA COME APPLICARE LINQ 
    /// AD UNA STRUTTURA DEFINENDO IL CAMPO MEDIANTE 
    /// UNA STRINGA.
    /// Esistono essenzialmente due modi. 
    /// In questo codice mostro il secondo. 
    ///  -> Usare gli alberi di espressioni
    ///  -> produrre una sturttura dinamica 
    ///     usando la sua serializzazione, per
    ///     ovviare il vincolo dei tipi.
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            Person[] people = GetPeople();
            // Query 1: OK BUT NO CORRECT -> Accesso puntato al campo "age"
            var attempt_1 = people.Where(x => x.age == 20);

            // Query 2: NOT OK, BREAKS -> Accesso mediante stringa al campo "age"
            // SPIEGAZIONE: funziona normalmente se accedo al campo di un oggetto mediante notazione puntata
            //var attempt_2 = people.Where(x => x["age"] == 20);

            // Query 3: NOT OK, DOESN'T BREAK but returns no data
            // SPIEGAZIONE: non posso convertire direttamente una struttura in dynamic per usarvi poi su LINQ
            IEnumerable<dynamic> attempt_3_ = people.Cast<dynamic>();
            var attempt_3 = attempt_3_.Where(x => x["age"] == 20);

            // Query 4: OK!
            // SPEIGAZIONE: per compiere query su una struttura 
            // usando linq potendo operare su un qualsiasi cmapo, 
            // occorre applicare linq alla stessa struttura dati ma 
            // serializzata, così da NON essere più vincolato 
            // dai tipi. Lo si fa: 
            //  -> serializzare la struttura in un JArray
            //  -> castare il JArray ad IEnumerable<dynamic>
            //  -> eseguo la query con LINQ
            //  -> mappo il risultato nelle istanze della classe che componeva la struttura originale

            JArray arr = JArray.FromObject(people);
            IEnumerable<dynamic> attempt_4_ = arr.Cast<dynamic>();
            var attempt_4a = attempt_4_.Where(x => x["age"] == 20); // OK
            Person[] people_filter_a = Person.DeserialPeople(attempt_4a);
            var attempt_4b = attempt_4_.Where(x => x["birth"] == new DateTime(1970, 1, 1) ); // OK
            Person[] people_filter_b = Person.DeserialPeople(attempt_4b);


        }

        public static Person[] GetPeople()
        {
            DateTime date = new DateTime(1970,1,1);
            List<Person> people = new List<Person>();
            people.Add(new Person("Luca", "Abete", 20, date));
            people.Add(new Person("Marco", "Gino", 20, date));
            people.Add(new Person("Giovanni", "Mucca", 21, date));
            people.Add(new Person("Luigi", "Albero", 23, date));
            people.Add(new Person("Gianni", "Toro", 24, date));
            return people.ToArray();
        }
    }

    public class Person
    {
        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }
        public DateTime birth { get; set; }
        public Person() { }
        public Person(string name, string surname, int age, DateTime birth)
        {
            this.name = name;
            this.surname = surname;
            this.age = age;
            this.birth = birth;
        }

        public static Person[] DeserialPeople(dynamic serial)
        {
            List<Person> people = new List<Person>();
            foreach (var item in serial)
            {
                Person p = Person.DeserialPerson(item);
                people.Add(p);
            } 
            return people.ToArray();
        }

        public static Person DeserialPerson(dynamic o)
        {
            Person p = new Person();
            p.name = o.name;
            p.surname = o.surname;
            p.age = o.age;
            p.birth = o.birth;
            return p;
        }


    }
}
