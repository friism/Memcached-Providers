using System;
using System.IO;

namespace MemcachedProviders.Util
{
    [Serializable]
    internal class CachingFilter : Stream
    {
        private readonly MemoryStream _objMemoryStream;
        private readonly Stream _objOriginalStream;

        public CachingFilter(Stream originalStream)
        {
            _objMemoryStream = new MemoryStream();
            this._objOriginalStream = originalStream;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return _objOriginalStream.Length; }
        }

        public override long Position
        {
            get { return _objOriginalStream.Position; }
            set { throw new InvalidOperationException(); }
        }

        public override void Flush()
        {
            _objOriginalStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public byte[] GetData()
        {
            return _objMemoryStream.ToArray();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _objOriginalStream.Write(buffer, offset, count);
            _objMemoryStream.Write(buffer, offset, count);
        }
    }
}