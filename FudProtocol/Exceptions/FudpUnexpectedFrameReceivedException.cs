using System;
using System.Runtime.Serialization;
using Fudp.Messages;

namespace Fudp.Exceptions
{
    /// <Summary>Было принято неожиданное сообщение</Summary>
    [Serializable]
    public class FudpUnexpectedFrameReceivedException : FudpException
    {
        public Type ExpectedFrameType { get; private set; }
        public Message ReceivedFrame { get; private set; }
 
        public FudpUnexpectedFrameReceivedException(Type ExpectedFrameType, Message ReceivedFrame)
            : base(String.Format("Было принято неожиданное сообщение ({0} в то время, как ожидалось {1})", ReceivedFrame.GetType().Name, ExpectedFrameType.Name))
        {
            this.ReceivedFrame = ReceivedFrame;
            this.ExpectedFrameType = ExpectedFrameType;
        }

        protected FudpUnexpectedFrameReceivedException(
            SerializationInfo info,
            StreamingContext context, Type ExpectedFrameType, Message ReceivedFrame) : base(info, context)
        {
            this.ReceivedFrame = ReceivedFrame;
            this.ExpectedFrameType = ExpectedFrameType;
        }
    }
}