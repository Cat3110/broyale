

namespace Scripts.Core.Events
{
    public interface ICustomEventSender
    {
        void SetOneListener( ICustomEventListener listener );
    }
}