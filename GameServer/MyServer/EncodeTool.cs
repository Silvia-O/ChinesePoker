using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    /// <summary>
    /// helps encode & decode msg
    /// </summary>
    public class EncodeTool
    {
        #region  encapsulate msg packet
        /// <summary>
        /// construct packet: head + tail
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] data)
        {
            // using: release resources once finishing
            // byte[] buffer
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    // first write packet head
                    bw.Write(data.Length);
                    // then write packet tail
                    bw.Write(data);

                    byte[] byteArray = new byte[(int)ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);
                    return byteArray;
                }
            }
            
        }
           
        public static byte[] DecodePacket(ref List<byte> dataCache)
        {
            //int 4 bytes
            if (dataCache.Count < 4)
            {
                return null;
                // throw new Exception("Packet data length is less than 4 bytes. " +
                //    "It's not a complete packet");
            }
                

            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    // packet head defined length
                    int length = br.ReadInt32();
                    int dataRemainLength = (int)(ms.Length - ms.Position);
                    if (length > dataRemainLength)
                    {
                        return null;
                        // throw new Exception("Packet data length is less than actual length defined in packet head. " +
                        //   "It's not a complete packet");
                    }
                       

                    byte[] data = br.ReadBytes(length);
                    // refresh the data cache
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dataRemainLength));

                    return data;
                }
            }

        }


        #endregion

        #region construct sending SocketMsg

        /// <summary>
        /// convert SocketMsg into byte array to send
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(SocketMsg msg)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(msg.OpCode);
            bw.Write(msg.SubCode);
            if (msg.Value != null)
            {
                byte[] valueBytes = EncodeObject(msg.Value);
                bw.Write(valueBytes);
            }
            byte[] data = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return data;

        }

        /// <summary>
        /// convert received byte array into SocketMsg
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SocketMsg DecodeMsg(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            SocketMsg msg = new SocketMsg();
            msg.OpCode = br.ReadInt32();
            msg.SubCode = br.ReadInt32();
            if (ms.Length > ms.Position)
            {
                byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                object value = DecodeObject(valueBytes);
                msg.Value = value;
            }
            br.Close();
            ms.Close();
            return msg;

        }

        #endregion

        #region convert object into byte array

        /// <summary>
        /// serialize object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodeObject(object value)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                byte[] valueBytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, valueBytes, 0, (int)ms.Length);
                return valueBytes;
            }
        }

        /// <summary>
        /// deserialize object
        /// </summary>
        /// <param name="valueBytes"></param>
        /// <returns></returns>
        public static object DecodeObject(byte[] valueBytes)
        {
            using(MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }
        #endregion
    }

}
