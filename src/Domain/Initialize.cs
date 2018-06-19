using System;
using Marten;

namespace Domain
{
    public static class Initialize
    {
        public static IDocumentStore Database() => 
            DocumentStore.For(x =>
            {
                // Turn this off in production 
                x.AutoCreateSchemaObjects = AutoCreate.All; // AutoCreate.None;

                var connectionString = "host=localhost;database=eventstore;password=mysecretpassword;username=postgres";
                x.Connection(connectionString);
            });
    }
}
