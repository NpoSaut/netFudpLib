using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Fudp.Model;
using Fudp.Protocol;
using Fudp.Protocol.Exceptions;
using Fudp.Protocol.Messages;

namespace Fudp.Operators
{
    /// <summary>Класс, выполняющий поиск устройств на CAN-линии</summary>
    public class FudpDeviceLocator : IDeviceLocator
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        /// <param name="Port">CAN-порт для поиска устройств</param>
        public FudpDeviceLocator(CanPort Port)
        {
            this.Port = Port;
        }

        /// <summary>CAN-порт, по которому выполняется поиск</summary>
        public CanPort Port { get; private set; }

        /// <summary>Выполняет поиск устройств на CAN-линии по заданному шаблону</summary>
        /// <param name="Pattern">Шаблон для поиска устройств</param>
        /// <param name="Timeout">Таймаут ожидания ответа от устройств</param>
        /// <returns>Список устройсв, удовлетворяющих условиям поиска</returns>
        public IList<DeviceTicket> LocateDevices(DeviceTicket Pattern, int Timeout = 100)
        {
            using (var flow = new CanFlow(Port, FudpOptions.FuDev, FudpOptions.FuInit, FudpOptions.FuProg))
            {
                var helloMessage = new ProgInit(Pattern);
                IsoTp.Send(flow, FudpOptions.FuInit, FudpOptions.FuDev, helloMessage.Encode());

                var res = new List<DeviceTicket>();
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < Timeout)
                {
                    try
                    {
                        TpReceiveTransaction tr = IsoTp.Receive(flow, FudpOptions.FuDev, FudpOptions.FuProg,
                                                                TimeSpan.FromMilliseconds(Timeout - sw.ElapsedMilliseconds));
                        Message msg = Message.DecodeMessage(tr.Data);
                        if (msg is ProgBCastResponse) res.Add((msg as ProgBCastResponse).Ticket);
                    }
                    catch (IsoTpReceiveTimeoutException)
                    {
                        break;
                    }
                }
                return res.Distinct().ToList();
            }
        }

        public FudpOverIsoTpConnection Connect(DeviceTicket Ticket)
        {
            if (Ticket.IsBroadcast) throw new ArgumentException("Билет является броадкастовым и не может использвоаться для подключения к конкретному устройству", "Ticket");

            Debug.Print("Пробуем подключиться к {0}", Ticket);

            using (var flow = new CanFlow(Port, FudpOptions.FuInit, FudpOptions.FuDev))
            {
                int i = 0;
                while (true)
                {
                    Debug.Print("Попытка {0}", i + 1);
                    if (i >= 10) throw new CanProgLimitConnectException("Превышен лимит попыток подключения");

                    flow.Clear();

                    Debug.Print("Отправляем ProgInit");
                    SendMsg(flow, new ProgInit(device), 100, WithTransmitDescriptor: FuInit);
                    i++;
                    try
                    {
                        Debug.Print("Ждём ответа на ProgInit");
                        var xxx = GetMsg<ProgStatus>(res.Flow, 100);
                        Debug.Print("Получили ответ на ProgInit");
                        res.Properties = xxx.Properties;
                        res.ResetPingTimer();

                        Debug.Print("PROPERTIES:");
                        Debug.Indent();
                        foreach (var property in res.Properties)
                            Debug.Print("  {0} : {1}", property.Key, property.Value);
                        Debug.Unindent();

                        break;
                    }
                    catch (IsoTpProtocolException) { }
                    catch (FudpReceiveTimeoutException) { }
                }
            }

            // Проверка версии
            if (res.CheckProtocolVersion() == CheckVersionResult.UnCompatible)
                throw new CanProgUnCompatibleVersionException();

            return res;
        }
    }
}
