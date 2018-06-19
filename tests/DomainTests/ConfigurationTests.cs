using System;
using Domain;
using Xunit;

namespace DomainTests
{
    public class ConfigurationTests
    {
        [Fact]
        public void Can_connect_to_databse()
        {
            var database = Initialize.Database();
            var session = database.OpenSession();
            session.Store(new TestDocument { Name = "Sample", Id = Guid.NewGuid() });
            session.SaveChanges();
        }

        public class TestDocument
        {
            public string Name { get; set;}
            public Guid Id { get; set; }
        }
    }
}
