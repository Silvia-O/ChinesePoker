using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// network message
/// </summary>
public class SocketMsg
{
    /// <summary>
    /// operation code
    /// </summary>
    public int OpCode { get; set; }

    /// <summary>
    /// operation sub code
    /// </summary>
    public int SubCode { get; set; }

    /// <summary>
    /// parameter
    /// </summary>
    public object Value { get; set; }

    public SocketMsg()
    {
    }

    public SocketMsg(int opCode, int subCode, object value)
    {
        this.OpCode = opCode;
        this.SubCode = subCode;
        this.Value = value;
    }

    public void Change(int opCode, int subCode, object value)
    {
        this.OpCode = opCode;
        this.SubCode = subCode;
        this.Value = value;
    }
}