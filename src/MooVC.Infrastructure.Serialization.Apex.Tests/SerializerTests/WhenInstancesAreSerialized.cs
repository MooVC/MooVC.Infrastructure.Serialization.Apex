﻿namespace MooVC.Infrastructure.Serialization.Apex.SerializerTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Apex.Serialization;
    using MooVC.Collections.Generic;
    using MooVC.Serialization;
    using Xunit;
    using Serializer = MooVC.Infrastructure.Serialization.Apex.Serializer;

    public sealed class WhenInstancesAreSerialized
        : IDisposable
    {
        private readonly ISerializer serializer;
        private bool isDisposed;

        public WhenInstancesAreSerialized()
        {
            Settings settings = new Settings()
                .MarkSerializable(type => true);

            serializer = new Serializer(settings: settings);
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);

            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GivenAnInstanceOfAClassThenACloneOfThatInstanceIsDeserializedAsync()
        {
            var original = new SerializableClass
            {
                Array = new ulong[] { 1, 2, 3 },
                Integer = 25,
                String = "Something something dark side...",
            };

            IEnumerable<byte> stream = await serializer.SerializeAsync(original);
            SerializableClass deserialized = await serializer.DeserializeAsync<SerializableClass>(stream);

            AssertEqual(original, deserialized);
        }

        [Fact]
        public async Task GivenAnInstanceOfAClassWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            var original = new SerializableClass
            {
                Array = new ulong[] { 1, 2, 3 },
                Integer = 25,
                String = "Something something dark side...",
            };

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(original, stream);

            stream.Position = 0;

            SerializableClass deserialized = await serializer.DeserializeAsync<SerializableClass>(stream);

            AssertEqual(original, deserialized);
        }

        [Fact]
        public async Task GivenAnInstanceOfAClassWithAReferencedObjectWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            var original = new SerializableClass
            {
                Array = new ulong[] { 1, 2, 3 },
                Integer = 25,
                Object = new SerializableClass(),
                String = "Something something dark side...",
            };

            using var stream = new MemoryStream();

            await serializer.SerializeAsync<ISerializableInstance>(original, stream);

            stream.Position = 0;

            ISerializableInstance deserialized = await serializer.DeserializeAsync<ISerializableInstance>(stream);

            AssertEqual(original, deserialized);
        }

        [Fact]
        public async Task GivenAnInstanceOfARecordWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            var original = new SerializableRecord(
                new ulong[] { 1, 2, 3 },
                25,
                default,
                "Something something dark side...");

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(original, stream);

            stream.Position = 0;

            SerializableRecord deserialized = await serializer.DeserializeAsync<SerializableRecord>(stream);

            AssertEqual(original, deserialized);
        }

        [Fact]
        public async Task GivenAnInstanceOfARecordThenACloneOfThatInstanceIsDeserializedAsync()
        {
            var original = new SerializableRecord(
                new ulong[] { 1, 2, 3 },
                25,
                default,
                "Something something dark side...");

            IEnumerable<byte> stream = await serializer.SerializeAsync(original);
            SerializableRecord deserialized = await serializer.DeserializeAsync<SerializableRecord>(stream);

            AssertEqual(original, deserialized);
        }

        [Fact]
        public async Task GivenAnInstanceOfARecordWithAReferencedObjectWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            var original = new SerializableRecord(
                new ulong[] { 1, 2, 3 },
                25,
                new SerializableRecord(
                    new ulong[] { 1, 5 },
                    3,
                    default,
                    "Something else..."),
                "Something something dark side...");

            using var stream = new MemoryStream();

            await serializer.SerializeAsync<ISerializableInstance>(original, stream);

            stream.Position = 0;

            ISerializableInstance deserialized = await serializer.DeserializeAsync<ISerializableInstance>(stream);

            AssertEqual(original, deserialized);
        }

        [Fact]
        public async Task GivenAnInstancesOfAClassThenACloneOfThatInstanceIsDeserializedAsync()
        {
            IEnumerable<SerializableClass> originals = new[]
            {
                new SerializableClass
                {
                    Array = new ulong[] { 1, 2, 3 },
                    Integer = 25,
                    String = "Something something",
                },
                new SerializableClass
                {
                    Array = new ulong[] { 4, 5, 6 },
                    Integer = 2,
                    String = "dark side...",
                },
            };

            IEnumerable<byte> stream = await serializer.SerializeAsync(originals);

            IEnumerable<SerializableClass> deserialized = await serializer
                .DeserializeAsync<IEnumerable<SerializableClass>>(stream);

            AssertEqual(originals, deserialized);
        }

        [Fact]
        public async Task GivenAnInstancesOfAClassWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            IEnumerable<SerializableClass> originals = new[]
            {
                new SerializableClass
                {
                    Array = new ulong[] { 1, 2, 3 },
                    Integer = 25,
                    String = "Something something",
                },
                new SerializableClass
                {
                    Array = new ulong[] { 4, 5, 6 },
                    Integer = 2,
                    String = "dark side...",
                },
            };

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(originals, stream);

            stream.Position = 0;

            IEnumerable<SerializableClass> deserialized = await serializer
                .DeserializeAsync<IEnumerable<SerializableClass>>(stream);

            AssertEqual(originals, deserialized);
        }

        [Fact]
        public async Task GivenAnInstancesOfAClassWithAReferencedObjectWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            IEnumerable<ISerializableInstance> originals = new[]
            {
                new SerializableClass
                {
                    Array = new ulong[] { 1, 2, 3 },
                    Integer = 25,
                    Object = new SerializableClass(),
                    String = "Something something",
                },
                new SerializableClass
                {
                    Array = new ulong[] { 4, 5, 6 },
                    Integer = 5,
                    Object = new SerializableClass(),
                    String = "dark side...",
                },
            };

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(originals, stream);

            stream.Position = 0;

            IEnumerable<ISerializableInstance> deserialized = await serializer
                .DeserializeAsync<IEnumerable<ISerializableInstance>>(stream);

            AssertEqual(originals, deserialized);
        }

        [Fact]
        public async Task GivenAnInstancesOfARecordWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            IEnumerable<SerializableRecord> originals = new[]
            {
                new SerializableRecord(
                    new ulong[] { 1, 2, 3 },
                    25,
                    default,
                    "Something something"),
                new SerializableRecord(
                    new ulong[] { 4, 5, 6 },
                    5,
                    default,
                    "dark side..."),
            };

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(originals, stream);

            stream.Position = 0;

            IEnumerable<SerializableRecord> deserialized = await serializer
                .DeserializeAsync<IEnumerable<SerializableRecord>>(stream);

            AssertEqual(originals, deserialized);
        }

        [Fact]
        public async Task GivenAnInstancesOfARecordThenACloneOfThatInstanceIsDeserializedAsync()
        {
            IEnumerable<SerializableRecord> originals = new[]
            {
                new SerializableRecord(
                    new ulong[] { 1, 2, 3 },
                    25,
                    default,
                    "Something something"),
                new SerializableRecord(
                    new ulong[] { 4, 5, 6 },
                    5,
                    default,
                    "dark side..."),
            };

            IEnumerable<byte> stream = await serializer.SerializeAsync(originals);

            IEnumerable<SerializableRecord> deserialized = await serializer
                .DeserializeAsync<IEnumerable<SerializableRecord>>(stream);

            AssertEqual(originals, deserialized);
        }

        [Fact]
        public async Task GivenAnInstancesOfARecordWithAReferencedObjectWhenSerializedToAStreamThenACloneOfThatInstanceIsDeserializedAsync()
        {
            IEnumerable<ISerializableInstance> originals = new[]
            {
                new SerializableRecord(
                    new ulong[] { 1, 2, 3 },
                    25,
                    new SerializableRecord(
                        new ulong[] { 1, 5 },
                        3,
                        default,
                        "Something else..."),
                    "Something something"),
                new SerializableRecord(
                    new ulong[] { 4, 5, 6 },
                    2,
                    new SerializableRecord(
                        new ulong[] { 2, 4 },
                        1,
                        default,
                        "Something else..."),
                    "dark side..."),
            };

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(originals, stream);

            stream.Position = 0;

            IEnumerable<ISerializableInstance> deserialized = await serializer
                .DeserializeAsync<IEnumerable<ISerializableInstance>>(stream);

            AssertEqual(originals, deserialized);
        }

        private static void AssertEqual(
            IEnumerable<ISerializableInstance> originals,
            IEnumerable<ISerializableInstance> deserialized)
        {
            Assert.Equal(originals.Count(), deserialized.Count());

            originals.For((index, original) =>
            {
                ISerializableInstance? pair = deserialized.ElementAt(index);

                AssertEqual(original, pair);
            });
        }

        private static void AssertEqual(ISerializableInstance? original, ISerializableInstance? deserialized)
        {
            if (original is { })
            {
                Assert.NotNull(deserialized);
                Assert.Equal(original.Array, deserialized!.Array);
                Assert.Equal(original.Integer, deserialized.Integer);
                Assert.Equal(original.String, deserialized.String);
                Assert.NotSame(original, deserialized);

                AssertEqual(original.Object, deserialized.Object);
            }
            else
            {
                Assert.Null(deserialized);
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    ((IDisposable)serializer).Dispose();
                }

                isDisposed = true;
            }
        }
    }
}