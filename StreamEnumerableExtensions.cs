using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System
{
    public static class StreamEnumerableExtensions
    {
        // 1mb di buffer per file
        private const int BufferSize = 100*1024*1024;

        public static StreamEnumerable AsEnumerable(this Stream stream) 
        {
            return new StreamEnumerable(stream, BufferSize);
        }

        public static FileStream OpenReadForwardOnly(this FileInfo file)
        {
            return new FileStream(file.FullName, FileMode.Open, 
                System.Security.AccessControl.FileSystemRights.ReadData, 
                FileShare.Read, BufferSize, 
                FileOptions.SequentialScan | FileOptions.WriteThrough);
        }
    }

    public class StreamEnumerable : IEnumerable<char>, IDisposable
    {
        private Stream _stream;
        private StreamEnumerator _enumerator;
        public StreamEnumerable(Stream stream, int bufferSize)
        {
            _stream = stream;
            _enumerator = new StreamEnumerator(stream, bufferSize);
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public long Length { get { return _stream.Length; } }
        public long ReadBytes { get { return _enumerator.ReadBytes; } }
        public double PercentComplete { get { return (double) ReadBytes / Length; } }
        public double PercentCompletePrintable { get { return (int)(PercentComplete * 100); } }

        #region IEnumerable<char> Members

        public IEnumerator<char> GetEnumerator()
        {
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public class StreamEnumerator : IEnumerator<char>
        {
            private Stream _stream;
            private int _currentIndex = Int32.MaxValue;
            private byte[] _currentBuffer = null;
            private int _currentCapacity;
            public long ReadBytes { get; private set; }

            public StreamEnumerator(Stream stream, int bufferSize) 
            {
                _stream = stream;
                _currentBuffer = new byte[bufferSize];
            }

            #region IEnumerator<char> Members

            public char Current { get; private set; }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (HasNext())
                {
                    _currentIndex++;
                    this.ReadBytes++;
                }

                if (_currentIndex >= _currentBuffer.Length)
                {
                    _currentIndex = 0;
                    _currentCapacity = _stream.Read(_currentBuffer, 0, _currentBuffer.Length);
                }

                this.Current = (char)_currentBuffer[_currentIndex];

                return HasNext();
            }

            public bool HasNext()
            {
                return _currentCapacity > 0 && 
                    _currentIndex < _currentCapacity;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
