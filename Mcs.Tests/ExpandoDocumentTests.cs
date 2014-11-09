namespace Mcs.Tests
{
    using System;
    using System.Dynamic;

    using Mcs.Helpers;
    using Mcs.Models;

    using MongoDB.Bson;

    using NUnit.Framework;

    [TestFixture]
    public class ExpandoDocumentTests
    {
        [TestCase]
        public void Foo()
        {
            var doc = new BsonDocument
                      {
                          { "firstName", "Homer" },
                          { "birthDate", DateTime.Parse("2000-01-01") },
                          { "address", new BsonDocument { { "line1", "10 Acacia Avenue" } } }
                      };

            dynamic d = doc.ToExpando();

            dynamic y = new ExpandoObject();

            y.arse = "hello";

            Console.WriteLine(y.arse);

            string x = d.firstName;

            
        }
    }
}
