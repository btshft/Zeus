using System.IO;
using FASTER.core;

namespace Zeus.Storage.Faster.Serialization
{
    internal abstract class Serializer<THolder, TContent> : IObjectSerializer<THolder>
    {
        protected Stream Reader { get; private set; }

        protected Stream Writer { get; private set; }

        /// <inheritdoc />
        public void Serialize(ref THolder obj)
        {
            Serialize(ref GetContentReference(ref obj));
        }

        protected void Serialize(ref TContent content)
        {
            var payload = GetBytes(content);
            var size = payload.Length;

            Writer.WriteInt32(size);
            Writer.Write(payload, 0, payload.Length);
        }

        /// <inheritdoc />
        public void Deserialize(ref THolder obj)
        {
            Deserialize(ref GetContentReference(ref obj));
        }

        // ReSharper disable once RedundantAssignment
        protected void Deserialize(ref TContent content)
        {
            var size = Reader.ReadInt32();
            var payload = new byte[size];

            Reader.Read(payload, 0, payload.Length);

            content = GetContent(payload);
        }

        protected abstract ref TContent GetContentReference(ref THolder holder);

        protected abstract byte[] GetBytes(TContent content);

        protected abstract TContent GetContent(byte[] bytes);

        /// <inheritdoc />
        public void BeginSerialize(Stream stream)
        {
            Writer = stream;
        }

        /// <inheritdoc />
        public void BeginDeserialize(Stream stream)
        {
            Reader = stream;
        }

        /// <inheritdoc />
        public void EndSerialize()
        {
            Writer = null;
        }

        /// <inheritdoc />
        public void EndDeserialize()
        {
            Reader = null;
        }
    }
}
