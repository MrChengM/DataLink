using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiBase.Helper;
using GuiBase.Common;
using DataServer;
using DataServer.Points;

namespace GuiBase.Services
{
    public interface ISignalService
    {
        event Action<bool> ConnectStatusChangeEvent;
        event Action<Tag>  ReceiveTagEvent;
        event Action<List<Tag>> ReceiveTagsEvent;

        bool Start();
        bool Stop();
        bool IsConnect { get; }

        void Subscribe(string tagName);
        void Unsubscribe(string tagName);

        void Subscribe(List<string> tagName);
        void Unsubscribe(List<string> tagName);

        Task<Tag> ReadAsync(string tagName);

        Task<List<Tag>> ReadAsync(List<string> tagNames);

        Task<WriteResult> WriteAsync(Tag tag);
        Task<WriteResult> WriteAsync(List<Tag> tags);

        Tag Read(string tagName);
        List<Tag> Read(List<string> tagNames);
        WriteResult Write(Tag tag);
        WriteResult Write(List<Tag> tags);
    }
}
