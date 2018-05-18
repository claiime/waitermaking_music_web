using System;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace Music_Web.Models.Waitermak
{
    public enum WaveFormats
    {
        Pcm = 1,//autoformat=1
        Float = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public class WaveFormat
    {
        public short wFormatTag;//
        public short nChannels;//numberchanel 2byte
        public int nSamplesPerSec;//samplerate thuong 22050
        public int nAvgBytesPerSec;//ByteRate 
        public short nBlockAlign;//BlockAlign
        public short wBitsPerSample;//bitperSample 16
        public short cbSize;//

        public WaveFormat(int rate, int bits, int channels)//khởi tạo
        {
            wFormatTag = (short)WaveFormats.Pcm;//1
            nChannels = (short)channels;//2
            nSamplesPerSec = rate;//22050
            wBitsPerSample = (short)bits;//16
            cbSize = 0;
            nBlockAlign = (short)(channels * (bits / 8));//4
            nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;//
        }
    }

    public class WaveStream : Stream, IDisposable
    {
        private Stream m_Stream;
        private long m_DataPos;
        private int m_Length;

        private WaveFormat m_Format;

        public WaveFormat Format
        {
            get { return m_Format; }
        }

        private string ReadChunk(BinaryReader reader)//đọc khối 4byte trả về chuỗi
        {
            byte[] ch = new byte[4];
            reader.Read(ch, 0, ch.Length);
            return System.Text.Encoding.ASCII.GetString(ch);
        }

        private void ReadHeader()
        {
            //Kiểm tra file có đúng định dạng ko
            BinaryReader Reader = new BinaryReader(m_Stream);//nhận file từ luồng
            //đọc header file, kiểm tra đúng định dạng ko 
            if (ReadChunk(Reader) != "RIFF")//4 byte đầu phải có giá trị RIEF mới đúng định dạng
                throw new Exception("Invalid file format");
            Reader.ReadInt32(); //dọc tiep 4 byte kế
            if (ReadChunk(Reader) != "WAVE")//4 byte kế này phải là "WAVE"
                throw new Exception("Invalid file format");

            if (ReadChunk(Reader) != "fmt ")//4 byte kế tiếp có giá trị "fmt" mới đúng định dạng
                throw new Exception("Invalid file format");

            int len = Reader.ReadInt32();//subchunk1Size 4byte
            if (len < 16) // bad format chunk length
                throw new Exception("Invalid file format");
            
            m_Format = new WaveFormat(22050, 16, 2); // khởi tạo file wave, gán các giá trị SampleRate=22050,bitsPerSAmple=16,channel=2
            m_Format.wFormatTag = Reader.ReadInt16();//AudioFormat (pcm)
            m_Format.nChannels = Reader.ReadInt16();//NumChanels
            m_Format.nSamplesPerSec = Reader.ReadInt32();//SampeRate
            m_Format.nAvgBytesPerSec = Reader.ReadInt32();//ByteRate
            m_Format.nBlockAlign = Reader.ReadInt16();//blockAlign//blocksize
            m_Format.wBitsPerSample = Reader.ReadInt16();//bitsPerSample

            // advance in the stream to skip the wave format block 
            len -= 16; // minimum format size
            while (len > 0)
            {
                Reader.ReadByte();
                len--;
            }

            // assume the data chunk is aligned
            while (m_Stream.Position < m_Stream.Length && ReadChunk(Reader) != "data")
                ;

            if (m_Stream.Position >= m_Stream.Length)
                throw new Exception("Invalid file format");

            m_Length = Reader.ReadInt32();//4byte kế chứa độ dài data
            m_DataPos = m_Stream.Position;

            Position = 0;
        }

        /// <summary>ReadChunk(reader) - Changed to CopyChunk(reader, writer)</summary>
        /// <param name="reader">source stream</param>
        /// <returns>4 kí tự</returns>
        private string CopyChunk(BinaryReader reader, BinaryWriter writer)
        {
            byte[] ch = new byte[4];
            reader.Read(ch, 0, ch.Length);
            writer.Write(ch);

            return System.Text.Encoding.ASCII.GetString(ch);
        }

        /// <summary>ReadHeader() - Changed to CopyHeader(destination)</summary>
        private void CopyHeader(Stream destinationStream)
        {
            BinaryReader reader = new BinaryReader(m_Stream);
            BinaryWriter writer = new BinaryWriter(destinationStream);

            if (CopyChunk(reader, writer) != "RIFF")//4 byte đầu khác RIEF thì ko đúng định dạng
                throw new Exception("Invalid file format");

            writer.Write(reader.ReadInt32()); //trừ 8 byte đầu(mo tả rief và chứa độ dài)

            if (CopyChunk(reader, writer) != "WAVE")//tiep theo neu khac Wave thì sai định dạng file wav
                throw new Exception("Invalid file format");

            if (CopyChunk(reader, writer) != "fmt ")//tương tự 4 byte tiep theo phải có định dạng "fmt"
                throw new Exception("Invalid file format");

            int len = reader.ReadInt32();
            if (len < 16)
            { // SAi độ dài định dạng
                throw new Exception("Invalid file format");
            }
            else
            {
                writer.Write(len);//khởi tạo
            }

            m_Format = new WaveFormat(22050, 16, 2); // khởi tạo định dạng file. 22050 là sampleRate,bitpersample 16, channels 2
            m_Format.wFormatTag = reader.ReadInt16();//audio format pcm
            m_Format.nChannels = reader.ReadInt16();//numchanel 
            m_Format.nSamplesPerSec = reader.ReadInt32();//samplerate
            m_Format.nAvgBytesPerSec = reader.ReadInt32();//byterate 
            m_Format.nBlockAlign = reader.ReadInt16();//blockAlign 2byte
            m_Format.wBitsPerSample = reader.ReadInt16();//bitpersamp 2 byte

            //copy thông tin format
            writer.Write(m_Format.wFormatTag);
            writer.Write(m_Format.nChannels);
            writer.Write(m_Format.nSamplesPerSec);
            writer.Write(m_Format.nAvgBytesPerSec);
            writer.Write(m_Format.nBlockAlign);
            writer.Write(m_Format.wBitsPerSample);


            // advance in the stream to skip the wave format block 
            len -= 16; // minimum format size
            writer.Write(reader.ReadBytes(len));
            len = 0;
            while (m_Stream.Position < m_Stream.Length && CopyChunk(reader, writer) != "data")
                ;

            if (m_Stream.Position >= m_Stream.Length)
                throw new Exception("Invalid file format");

            m_Length = reader.ReadInt32();
            writer.Write(m_Length);

            m_DataPos = m_Stream.Position;
            Position = 0;
        }


        public WaveStream(Stream sourceStream, Stream destinationStream)
        {
            m_Stream = sourceStream;
            CopyHeader(destinationStream);
        }
        public void Dispose()
        {
            if (m_Stream != null)
                m_Stream.Close();
            GC.SuppressFinalize(this);
        }
        public WaveStream(Stream sourceStream)
        {
            m_Stream = sourceStream;
            ReadHeader();
        }

        ~WaveStream()
        {
            Dispose();
        }
        
        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override long Length
        {
            get { return m_Length; }
        }

        //đếm độ dài data. số samples
        public long CountSamples
        {
            get { return (long)((m_Length - m_DataPos) / (m_Format.wBitsPerSample / 8)); }
        }

        public override long Position
        {
            get { return m_Stream.Position - m_DataPos; }
            set { Seek(value, SeekOrigin.Begin); }
        }
        public override void Close()
        {
            Dispose();
        }
        public override void Flush()
        {
        }
        public override void SetLength(long len)
        {
            throw new InvalidOperationException();
        }
        public override long Seek(long pos, SeekOrigin o)
        {
            switch (o)
            {
                case SeekOrigin.Begin:
                    m_Stream.Position = pos + m_DataPos;
                    break;
                case SeekOrigin.Current:
                    m_Stream.Seek(pos, SeekOrigin.Current);
                    break;
                case SeekOrigin.End:
                    m_Stream.Position = m_DataPos + m_Length - pos;
                    break;
            }
            return this.Position;
        }

        public override int Read(byte[] buf, int ofs, int count)
        {
            int toread = (int)Math.Min(count, m_Length - Position);
            return m_Stream.Read(buf, ofs, toread);
        }

        /// <summary>Read - Changed to Copy</summary>
        /// <param name="buf">Buffer to receive the data</param>
        /// <param name="ofs">Offset</param>
        /// <param name="count">Count of bytes to read</param>
        /// <param name="destination">Where to copy the buffer</param>
        /// <returns>Count of bytes actually read</returns>
        public int Copy(byte[] buf, int ofs, int count, Stream destination)
        {
            int toread = (int)Math.Min(count, m_Length - Position);
            int read = m_Stream.Read(buf, ofs, toread);
            destination.Write(buf, ofs, read);

            if (m_Stream.Position != destination.Position)
            {
                Console.WriteLine();
            }

            return read;
        }

        public override void Write(byte[] buf, int ofs, int count)
        {
            throw new InvalidOperationException();
        }
    }
}
