using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_chat
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    public interface IServiceChat
    {
        [OperationContract(IsOneWay = true)]
        void Registration(string name, string password);

        [OperationContract]
        int Connect(string name, string password);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract]
        List<string> UsersOnline();

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract(IsOneWay = true)]
        void PrivateSendMsg(string msg, int id, string name);

        [OperationContract]
        List<string> GetMessageStory(int send_id, string getterName);
    }

    public interface IServerChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg);

        [OperationContract(IsOneWay = true)]
        void PrivateMsgCallback(string msg, string name);

        [OperationContract(IsOneWay = true)]
        void MessageStoryCallback();
    }
}
