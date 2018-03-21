using System;
using System.Collections.Generic;
using System.Text;

namespace Playground.Core.Interfaces
{
    public interface INotificationService
    {
        void RaiseLocalNotification(string message);
    }
}
