using System;
namespace RITS.StrymonEditor.Messaging
{
    public interface IMediator
    {
        void NotifyColleagues(ViewModelMessages message, object args);
        void Register(ViewModelMessages message, Action<object> callback);
        void UnRegister(ViewModelMessages message, Action<object> callback);
    }
}
