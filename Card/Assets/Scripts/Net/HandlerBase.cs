public abstract class HandlerBase
{

    public abstract void OnReceive(int subCode, object value);

    /// <summary>
    /// dispatch message
    /// </summary>
    /// <param name="areaCode"></param>
    /// <param name="eventCode"></param>
    /// <param name="message"></param>
    protected void Dispatch(int areaCode, int eventCode, object message)
    {
        MsgCenter.Instance.Dispatch(areaCode, eventCode, message);
    }
}