using System;
using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace ReflectedTypes.Tests
{
    public class Tests
    {
        [Test]
        public void NewtonsoftEnumTest()
        {
            var formatting = new FormattingEnum(FormattingEnum.Options.Indented);
            formatting.Instance!.ToString().Should().Be("Indented");
        }

        [Test]
        public void NewtonsoftStaticTests()
        {
            var name = Guid.NewGuid().ToString();

            var formatting = new FormattingEnum(FormattingEnum.Options.Indented);
            
            var jsonConvert = new JsonConvert();
            var json = jsonConvert.SerializeObject(new Person() {Name = name}, formatting);

            json.Should().NotBeEmpty();
            Console.WriteLine(json);

            var person = jsonConvert.DeserializeObject<Person>(json);
            person.Name.Should().Be(name);
        }

        [Test]
        public void NewtonsoftInstanceTests()
        {
            var contractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serializer = new JsonSerializer
            {
                ContractResolver = contractResolver
            };

            var stream = new MemoryStream();

            var name = Guid.NewGuid().ToString();

            serializer.Serialize(stream, new Person
            {
                Name = name,
            });

            var json = Encoding.UTF8.GetString(stream.ToArray());

            Console.WriteLine(json);

            stream.Position = 0;
            var person = serializer.Deserialize<Person>(stream);
            person.Name.Should().Be(name);
        }

        [Test]
        public void StructTest()
        {
            var proxy = new MyStructProxy(35);
            proxy.Value = 55;

            dynamic instance = proxy.Instance;
            Assert.AreEqual(instance.Value, 55);
        }

        public class Person
        {
            public string Name { get; set; }
        }
    }
}
