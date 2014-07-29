using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Messaging
{
    
    /// <summary>
    /// Mediator for all view models
    /// Credit to Marlon Grech for the initial implementation and idea 
    /// - made some tweaks here and there, mostly around more regimented reg and dereg <see cref="ViewModels.ViewModelBase"/>
    /// http://marlongrech.wordpress.com/2008/03/20/more-than-just-mvc-for-wpf/
    /// </summary>
    public class Mediator : IMediator
    {
        #region Data members
        MultiDictionary<ViewModelMessages, Action<object>> internalList = new MultiDictionary<ViewModelMessages, Action<object>>();
        #endregion

        /// <summary>
        /// Registers a Colleague to a specific message
        /// Credit to Marlon Grech for the initial implementation and idea - made some tweaks here and there
        /// http://marlongrech.wordpress.com/2008/03/20/more-than-just-mvc-for-wpf/
        /// Look at updating to v2 with WeakAction etc
        /// </summary>
        /// <param name="message">The message to register</param>
        /// <param name="callback">The callback to invoke</param>
        public void Register(ViewModelMessages message, Action<object> callback)
        {
            internalList.AddValue(message, callback);
        }
        public void UnRegister(ViewModelMessages message, Action<object> callback)
        {
            if (internalList.ContainsKey(message))
            {
                internalList[message].Remove(callback);
            }
        }

        /// <summary>
        /// Notify all colleagues that are registed to the specific message
        /// </summary>
        /// <param name="message">The message for the notify by</param>
        /// <param name="args">The arguments for the message</param>
        public void NotifyColleagues(ViewModelMessages message, object args)
        {
            if (internalList.ContainsKey(message))
            {
                //forward the message to all listeners
                // Create new temp copy of list to allow registrations and dereg to happen - any callback could potentially register/dereg and 
                // the foreach would have a changed list
                foreach (Action<object> callback in internalList[message].ToList())
                {
                   callback(args);
                }
            }
        }


    }
}
