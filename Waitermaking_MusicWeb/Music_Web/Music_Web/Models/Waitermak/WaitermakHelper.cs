using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;

namespace Music_Web.Models.Waitermak
{
    public class WaitermakHelper
    {     
        private WaveStream sourceStream;

        private Stream destinationStream;

        private int bytesPerSample;
        public WaitermakHelper(WaveStream sourceStream, Stream destinationStream)
        : this(sourceStream)
        {
            this.destinationStream = destinationStream;
        }

        public WaitermakHelper(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.bytesPerSample = sourceStream.Format.wBitsPerSample / 8;//2
        }
        public static Stream getStream(string text)//chuyển khóa dạng string sang stream
        {
            BinaryWriter messageWriter = new BinaryWriter(new MemoryStream());

            messageWriter.Write(text.Length);
            messageWriter.Write(Encoding.ASCII.GetBytes(text));

            messageWriter.Seek(0, SeekOrigin.Begin);
            return messageWriter.BaseStream;
        }
        public static void callHide(string pathSrc, string pathDes, string key, string message)
        {
            Stream sourceStream = null;//file nguồn
            FileStream destinationStream = null;//file sau khi waitermak
            WaveStream audioStream = null; 
            Stream messageStream = getStream(message);//chuyển message sang dạng stream 
            Stream keyStream = getStream(key);//chuyển key sang stream
            try
            {  //chưa kiem tra do dai key so voi message
                sourceStream = new FileStream(pathSrc, FileMode.Open);//đọc file gốc-file cần ẩn thông tin
                destinationStream = new FileStream(pathDes, FileMode.Create);//tạo file lưu
                //copy header file nguồn sang đích
                audioStream = new WaveStream(sourceStream, destinationStream);
                //hide the message vào file đã lưu
                WaitermakHelper utility = new WaitermakHelper(audioStream, destinationStream);
                utility.Hide(messageStream, keyStream);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //đóng các luồng đi

                if (keyStream != null) { keyStream.Close(); }
                if (messageStream != null) { messageStream.Close(); }
                if (audioStream != null) { audioStream.Close(); }
                if (sourceStream != null) { sourceStream.Close(); }
                if (destinationStream != null) { destinationStream.Close(); }
               
            }
        }

        public static string callExtract(string pathDes,string key)
        {
            string kq="khong tim duoc";
            FileStream sourceStream = null;//file cần extract
            WaveStream audioStream = null;
            MemoryStream messageStream = new MemoryStream();//byte stream lưu meaasge tạm
            Stream keyStream = getStream(key);
            try
            {
                sourceStream = new FileStream(pathDes, FileMode.Open);//mở file cần extract
                audioStream = new WaveStream(sourceStream);//
                WaitermakHelper utility = new WaitermakHelper(audioStream);
                utility.Extract(messageStream, keyStream);//thực hiện extract 
                messageStream.Seek(0, SeekOrigin.Begin);
                kq = new StreamReader(messageStream).ReadToEnd();
            }
            catch (Exception ex)
            {
                            }
            finally
            {   //đóng luồng 
                if (keyStream != null) { keyStream.Close(); }
                if (messageStream != null) { messageStream.Close(); }
                if (audioStream != null) { audioStream.Close(); }
                if (sourceStream != null) { sourceStream.Close(); }
            }
            
            return kq;//trả về kết quả
        }
       
        
        public void Hide(Stream messageStream, Stream keyStream)
        {

            byte[] waveBuffer = new byte[bytesPerSample];
            byte message, bit, waveByte;
            int messageBuffer; //byte message tạm
            int keyByte; //distance of the next carrier sample

            while ((messageBuffer = messageStream.ReadByte()) >= 0)
            {
                //đọc 1 byte message
                message = (byte)messageBuffer;
                //mỗi bit trong message
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    //đọc byte key
                    keyByte = GetKeyValue(keyStream);
                    //skip a couple of samples
                    for (int n = 0; n < keyByte - 1; n++)
                    {
                        //copy one sample from the clean stream to the carrier stream
                        sourceStream.Copy(waveBuffer, 0, waveBuffer.Length, destinationStream);
                    }
                    //đọc 1 sample(2byte) từ wave stream
                    sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
                    waveByte = waveBuffer[bytesPerSample - 1];
                    //lấy bit tiếp theo trong message byte hiện tại
                    bit = (byte)(((message & (byte)(1 << bitIndex)) > 0) ? 1 : 0);
                    //...đặt nó vào bit cuối cùng của sample
                    if ((bit == 1) && ((waveByte % 2) == 0))
                    {
                        waveByte += 1;
                    }
                    else if ((bit == 0) && ((waveByte % 2) == 1))
                    {
                        waveByte -= 1;
                    }
                    waveBuffer[bytesPerSample - 1] = waveByte;
                    
                    destinationStream.Write(waveBuffer, 0, bytesPerSample);
                }
            }
            //copy the rest of the wave without changes
            waveBuffer = new byte[sourceStream.Length - sourceStream.Position];
            sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
            destinationStream.Write(waveBuffer, 0, waveBuffer.Length);//lưu 
        }

        public void Extract(Stream messageStream, Stream keyStream)
        {

            byte[] waveBuffer = new byte[bytesPerSample];
            byte message, bit, waveByte;
            int messageLength = 0; //độ dài message
            int keyByte; //distance of the next carrier sample

            while ((messageLength == 0 || messageStream.Length < messageLength))
            {
               //khởi tạo message ban đầu rổng
                message = 0;
                //duyệt mỗi bit trong message
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {  //Đọc byte từ khóa
                    keyByte = GetKeyValue(keyStream);
                     
                    for (int n = 0; n < keyByte - 1; n++)
                    {
                        //đọc 1 sample thuộc data from the wave stream
                        sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
                    }
                    sourceStream.Read(waveBuffer, 0, waveBuffer.Length);
                    waveByte = waveBuffer[bytesPerSample - 1];

                    //get the last bit of the sample...
                    bit = (byte)(((waveByte % 2) == 0) ? 0 : 1);

                    //...viết lại byte message
                    message += (byte)(bit << bitIndex);
                }

                //thêm byte tìm đc vào message
                messageStream.WriteByte(message);

                if (messageLength == 0 && messageStream.Length == 4)//?
                {
                   //4byte đầu là độ dài message
                    messageStream.Seek(0, SeekOrigin.Begin);
                    messageLength = new BinaryReader(messageStream).ReadInt32();
                    messageStream.Seek(0, SeekOrigin.Begin);
                    messageStream.SetLength(0);
                }
            }

        }

        
     
        private static byte GetKeyValue(Stream keyStream)//chuyển key (string stream) sang dạng byte
        {
            int keyValue;
            if ((keyValue = keyStream.ReadByte()) < 0)
            {
                keyStream.Seek(0, SeekOrigin.Begin);
                keyValue = keyStream.ReadByte();
                if (keyValue == 0) { keyValue = 1; }
            }
            return (byte)keyValue;
        }

    }
}