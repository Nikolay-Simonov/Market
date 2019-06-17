﻿using System.Collections.Generic;

namespace NToastNotify.MessageContainers
{
    public interface IMessageContainer<TMessage> where TMessage : class, IToastMessage
    {
        void Add(TMessage message);
        void RemoveAll();
        IList<TMessage> GetAll();
        IList<TMessage> ReadAll();
    }
}
