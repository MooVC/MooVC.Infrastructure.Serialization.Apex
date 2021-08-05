namespace MooVC.Infrastructure.Serialization.Apex
{
    using System;
    using System.IO;
    using global::Apex.Serialization;
    using MooVC.Compression;
    using MooVC.Serialization;
    using static global::Apex.Serialization.Binary;

    public sealed class Serializer
        : SynchronousSerializer,
          IDisposable
    {
        private readonly Lazy<IBinary> binary;
        private bool isDisposed;

        public Serializer(
            ICompressor? compressor = default,
            Settings? settings = default)
            : base(compressor: compressor)
        {
            settings ??= new Settings();

            binary = new(Create(settings));
        }

        public IBinary Binary => binary.Value;

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        protected override T PerformDeserialize<T>(Stream source)
        {
            return Binary.Read<T>(source);
        }

        protected override void PerformSerialize<T>(T instance, Stream target)
        {
            Binary.Write(instance, target);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing && binary.IsValueCreated)
                {
                    Binary.Dispose();
                }

                isDisposed = true;
            }
        }
    }
}