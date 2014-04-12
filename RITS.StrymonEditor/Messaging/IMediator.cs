using System;
namespace RITS.StrymonEditor.Messaging
{
    /// <summary>
    /// Medaitor that will dispatch messages to subscribers
    /// Credit to Marlon Grech for the initial implementation and idea 
    /// - made some tweaks here and there, mostly around more regimented reg and dereg <see cref="ViewModels.ViewModelBase"/>
    /// http://marlongrech.wordpress.com/2008/03/20/more-than-just-mvc-for-wpf/
    /// Look at updating to v2 with WeakAction etc
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Notify method indicating that all subscribers of the supplied <see cref="ViewModelMessages"/> should be notificed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void NotifyColleagues(ViewModelMessages message, object args);

        /// <summary>
        /// <see cref="IColleague"/> implementing classes can regsiter a callback for the specified <see cref="ViewModelMessages"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        void Register(ViewModelMessages message, Action<object> callback);

        /// <summary>
        /// <see cref="IColleague"/> implementing classes can unregsiter the callback for the specified <see cref="ViewModelMessages"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        void UnRegister(ViewModelMessages message, Action<object> callback);
    }
}
