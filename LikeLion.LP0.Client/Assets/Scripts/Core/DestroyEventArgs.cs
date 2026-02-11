using System;

namespace LikeLion.LP0.Client.Core
{
    /// <summary>
    /// Provides data for an event that occurs BEFORE the target object is destroyed.
    /// </summary>
    /// <typeparam name="T">The type of the target object that will be destroyed.</typeparam>
    public class DestroyEventArgs : EventArgs
    {
        public readonly IDestroyable TargetObject;

        public DestroyEventArgs(IDestroyable targetObject)
        {
            TargetObject = targetObject;
        }
    }
}
