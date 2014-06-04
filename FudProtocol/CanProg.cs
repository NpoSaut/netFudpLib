using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Fudp.Messages;
using Fudp.Exceptions;

namespace Fudp
{
    /// <summary>Класс компанует, отправляет сообщение и получает ответ</summary>
    public class CanProg : IDisposable
    {
        private static readonly Dictionary<CanFlow, CanProg> ProgsOnFlows = new Dictionary<CanFlow, CanProg>();

        public event EventHandler<CanProgFilesystemEventArgs> FileCreated;
        protected virtual void OnFileCreated(DevFileInfo File)
        {
            var handler = FileCreated;
            if (handler != null) handler(this, new CanProgFilesystemEventArgs(File));
        }

        public event EventHandler<CanProgFilesystemEventArgs> FileRemoved;
        protected virtual void OnFileRemoved(String FileName)
        {
            var handler = FileRemoved;
            if (handler != null) handler(this, new CanProgFilesystemEventArgs(new DevFileInfo(FileName, 0, 0)));
        }

        public const int CurrentProtocolVersion = 8;
        public const int LastCompatibleProtocolVersion = 4;
        private const int ProtocolVersionKey = 195;
        private const int LastCompatibleProtocolVersionKey = 196;

        public int DeviceProtocolVersion
        {
            get { return Properties.ContainsKey(ProtocolVersionKey) ? Properties[ProtocolVersionKey] : 1; }
        }

        protected Timer PingTimer { get; private set; }

        private bool _disposeFlowOnExit = false;
        public static IList<ICanProgLog> Logs { get; set; }

        public enum CheckVersionResult
        {
            /// <summary>Версии идентичны</summary>
            Equals,
            /// <summary>Версии совместимы</summary>
            Compatible,
            /// <summary>Версии не совместимы</summary>
            UnCompatible
        }

        /// <summary>Проверяет совместимость версии загрузчика с версией программатора</summary>
        public CheckVersionResult CheckProtocolVersion()
        {
            int deviceCurrentProtocolVersion = Properties.ContainsKey(ProtocolVersionKey) ? Properties[ProtocolVersionKey] : 1;
            int deviceLastCompatibleProtocolVersion = Properties.ContainsKey(LastCompatibleProtocolVersionKey) ? Properties[LastCompatibleProtocolVersionKey] : 1;

            // Версии протокола идентичны
            if (deviceCurrentProtocolVersion == CurrentProtocolVersion) return CheckVersionResult.Equals;
            // Версия программатора устарела, но является совместимой с версией загрузчика
            else if (CurrentProtocolVersion < deviceCurrentProtocolVersion && CurrentProtocolVersion >= deviceLastCompatibleProtocolVersion) return CheckVersionResult.Compatible;
            // Версия загрузчика устарела, но является совместимой с версией программатора
            else if (CurrentProtocolVersion > deviceCurrentProtocolVersion && deviceCurrentProtocolVersion >= LastCompatibleProtocolVersion) return CheckVersionResult.Compatible;
            // Версии не совместимы
            else return CheckVersionResult.UnCompatible;
        }

        public CanProg(CanFlow Flow)
        {
            lock (ProgsOnFlows)
            {
                if (ProgsOnFlows.ContainsKey(Flow)) throw new CanProgFlowOccupiedException();
                ProgsOnFlows.Add(Flow, this);
            }
            this.Flow = Flow;
            Properties = new Dictionary<int, int>();
            SubmitAction = SubmitStatus.Cancel;
            PingTimer = new Timer(PingTimer_Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private Byte _pingCounter = 0;
        private void PingTimer_Callback(object State)
        {
            Debug.Print("PING!");
            var pingMessage = new ProgPing(_pingCounter);
            var pongMessage = Request<ProgPong>(Flow, pingMessage, 200);
            //SendMsg(Flow, pingMessage, 200);
            _pingCounter++;
        }

        public const UInt16 FuInit = 0x66a8;
        public const UInt16 FuProg = 0x66c8;
        public const UInt16 FuDev =  0x66e8;
        /// <summary>
        /// Словарь свойств файлов
        /// </summary>
        public Dictionary<int, int> Properties { get; private set; }
        /// <summary>
        /// Порт
        /// </summary>
        public CanFlow Flow { get; private set; }

        public DeviceTicket Device { get; private set; }

        const int DefaultMaximumSendAttempts = 7;
        private const int DefaultFudpTimeout = 700;

        /// <summary>Отправляет сообщение</summary>
        /// <param name="flow">CAN-поток</param>
        /// <param name="msg">Отправляемое сообщение</param>
        /// <param name="TimeOut">Таймаут на ожидание ответа</param>
        /// <param name="WithTransmitDescriptor">Дескриптор, с которым передаётся сообщение</param>
        /// <param name="WithAcknowledgmentDescriptor">Дескриптор, с которым передаются подтверждения на сообщение</param>
        public static void SendMsg(CanFlow flow, Message msg, int TimeOut = DefaultFudpTimeout, UInt16 WithTransmitDescriptor = FuProg, UInt16 WithAcknowledgmentDescriptor = FuDev, int MaxAttempts = DefaultMaximumSendAttempts)
        {
            lock (flow)
            {
                SuspendPingTimer(flow);
                flow.Clear();
                for (int attempt = 0; attempt < MaxAttempts; attempt ++)
                {
                    Logs.PushFormatTextEvent("--> {0}", msg);
                    try
                    {
                        flow.Clear();
                        IsoTp.Send(flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor, msg.Encode(), TimeSpan.FromMilliseconds(TimeOut));
                        break;
                    }
                    catch(IsoTpProtocolException istoProtocolException)
                    {
                        Logs.PushFormatTextEvent("Исключение во время передачи: {0}", istoProtocolException.Message);
                        Thread.Sleep(100);
                        if (attempt >= MaxAttempts-1) throw new CanProgTransportException(istoProtocolException);
                    }
                }
            }
            ResetPingTimer(flow);
        }

        public static TAnswer Request<TAnswer>(CanFlow flow, Message RequestMessage, int TimeOut = DefaultFudpTimeout, UInt16 ThisSideDescriptor = FuProg, UInt16 TheirSideDescriptor = FuDev, int MaxAttempts = DefaultMaximumSendAttempts)
            where TAnswer : Message
        {
            lock (flow)
            {
                Exception lastException = null;
                for (int attempt = 0; attempt < MaxAttempts; attempt++)
                {
                    try
                    {
                        SendMsg(flow, RequestMessage, TimeOut, ThisSideDescriptor, TheirSideDescriptor, MaxAttempts);
                        return GetMsg<TAnswer>(flow, TimeOut, TheirSideDescriptor, ThisSideDescriptor);
                    }
                    catch (IsoTpProtocolException ex) { lastException = ex; }
                    catch (FudpReceiveTimeoutException ex) { lastException = ex; }
                    Thread.Sleep(200);
                }
                Logs.PushFormatTextEvent("Исключение во время передачи: {0}", lastException);
                throw new CanProgTransportException(lastException);
            }
        }

        public static TMessage GetMsg<TMessage>(CanFlow flow, int TimeOut = DefaultFudpTimeout, UInt16 WithTransmitDescriptor = FuDev, UInt16 WithAcknowledgmentDescriptor = FuProg)
            where TMessage : Message
        {
            lock (flow)
            {
                SuspendPingTimer(flow);
                var sw = new Stopwatch();
                sw.Start();
                while (10 * sw.ElapsedMilliseconds < TimeOut)
                {
                    try
                    {
                        var tr = IsoTp.Receive(flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor, TimeSpan.FromMilliseconds(TimeOut));
                        var mes = Message.DecodeMessage(tr.Data);
                        var typedMes = mes as TMessage;
                        if (typedMes != null)
                        {
#if DEBUG
                            Logs.PushFormatTextEvent("<-- {0}", typedMes);
#endif
                            ResetPingTimer(flow);
                            return typedMes;
                        }
                        else
                        {
#if DEBUG
                            Logs.PushFormatTextEvent("<-- {0} - игнорируем (ожидали {1})", mes, typeof (TMessage));
#endif
                        }
                    }
                    catch (IsoTpProtocolException) { }
                }
                ResetPingTimer(flow);
                throw new FudpReceiveTimeoutException(string.Format("Превышено время ожидания FUDP-сообщения (ожидали сообщения {0})", typeof(TMessage)));
            }
        }

        /// <summary>Устанавливает соединение</summary>
        /// <param name="Port">Can-порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        public static CanProg Connect(CanPort Port, DeviceTicket device)
        {
            var session = Connect(new CanFlow(Port, FuDev, FuInit, FuProg), device);
            session._disposeFlowOnExit = true;
            return session;
        }

        /// <summary>Устанавливает соединение</summary>
        /// <param name="Flow">Пото Can-сообщений</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns></returns>
        public static CanProg Connect(CanFlow Flow, DeviceTicket device)
        {
            Logs.PushFormatTextEvent("Пробуем подключиться к {0}", device);
            var res = new CanProg(Flow) { Device = device };
            int i = 0;
            while(true)
            {
                Logs.PushFormatTextEvent("Попытка {0}", i+1);
                if (i >= 10) throw new CanProgLimitConnectException("Превышен лимит попыток подключения");
                
                Flow.Clear();

                Logs.PushFormatTextEvent("Отправляем ProgInit");
                SendMsg(Flow, new ProgInit(device), 100, WithTransmitDescriptor: FuInit);
                i++;
                try
                {
                    Logs.PushFormatTextEvent("Ждём ответа на ProgInit");
                    var xxx = GetMsg<ProgStatus>(res.Flow, 100);
                    Logs.PushFormatTextEvent("Получили ответ на ProgInit");
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

            // Проверка версии
            if (res.CheckProtocolVersion() == CheckVersionResult.UnCompatible)
                throw new CanProgUnCompatibleVersionException();

            return res;
        }

        #region Сброс и приостановка PING-таймера

        private void ResetPingTimer()
        {
            if (DeviceProtocolVersion >= 6) // В шестой версии протокола появилась поддержка Ping-Pong поддержания соединения
                PingTimer.Change(1000, Timeout.Infinite);
        }

        private static void ResetPingTimer(CanProg Session) { Session.ResetPingTimer(); }

        private static void ResetPingTimer(CanFlow Flow)
        {
            CanProg session;
            lock (ProgsOnFlows)
            {
                session = ProgsOnFlows[Flow];
            }
            session.ResetPingTimer();
        }

        private void SuspendPingTimer() { PingTimer.Change(Timeout.Infinite, Timeout.Infinite); }
        private static void SuspendPingTimer(CanProg Session) { Session.SuspendPingTimer(); }

        private static void SuspendPingTimer(CanFlow Flow)
        {
            CanProg session;
            lock (ProgsOnFlows)
            {
                session = ProgsOnFlows[Flow];
            }
            SuspendPingTimer(session);
        }

        #endregion


        /// <summary>Запрос списка файлов</summary>
        public List<DevFileInfo> ListFiles() { return RequestFiles().ToList(); }

        private IEnumerable<DevFileInfo> RequestFiles(int Offset = 0)
        {
            var listRq = new ProgListRq((ushort)Offset);
            int counter = 0;
            foreach (var file in Request<ProgList>(Flow, listRq).Files)
            {
                if (file is DevFileInfo)
                {
                    counter++;
                    yield return (DevFileInfo)file;
                }
                else
                {
                    var appendix = RequestFiles(Offset + counter);
                    foreach (var subfile in appendix) yield return subfile;
                }
            }
        }

        /// <summary>Запрос на чтение</summary>
        public Byte[] ReadFile(DevFileInfo fileInfo, IProgressAcceptor ProgressAcceptor = null, CancellationToken CancelToken = default(CancellationToken))
        {
            var buff = new Byte[fileInfo.FileSize];

            int pointer = 0;
            const int maximumReadSize = 4000;

            if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(0);
            while (pointer < buff.Length)
            {
                CancelToken.ThrowIfCancellationRequested();

                var request = new ProgReadRq(fileInfo.FileName, pointer, Math.Min(fileInfo.FileSize - pointer, maximumReadSize));
                var response = Request<ProgRead>(Flow, request);

                if (response.ErrorCode == 0)
                {
                    Buffer.BlockCopy(response.ReadData, 0, buff, pointer, response.ReadData.Length);
                    pointer += response.ReadData.Length;
                }
                else
                    switch (response.ErrorCode)
                    {
                        case 1: throw new FileNotFoundException(response.ErrorMessage);
                        case 2: throw new IndexOutOfRangeException(response.ErrorMessage);
                        case 3: throw new CanProgReadException(response.ErrorMessage);
                        default: throw new CanProgException();
                    }

                if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(Math.Min(1, ((double)pointer / fileInfo.FileSize)));
            }

            return buff;
        }
        /// <summary>
        /// Команда на удаление
        /// </summary>
        /// <param name="FileName">Имя фала</param>
        public int DeleteFile(String FileName)
        {
            var removeRequest = new ProgRm(FileName);
            var removeResponse = Request<ProgRmAck>(Flow, removeRequest);
            OnFileRemoved(FileName);
            return removeResponse.ErrorCode;
        }
        /// <summary>
        /// Команда на очистку памяти
        /// </summary>
        public void Erase()
        {
            ProgMrPropper MrPropper = new ProgMrPropper();
            SendMsg(Flow, MrPropper);
        }

        /// <summary>
        /// Команда на создание файла
        /// </summary>
        /// <param name="fileInfo">Информация о создаваемом файле</param>
        /// <param name="ProgressAcceptor">Приёмник прогресса выполнения файла</param>
        /// <param name="CancelToken">Токен отмены</param>
        /// <returns></returns>
        public void CreateFile(DevFileInfo fileInfo, IProgressAcceptor ProgressAcceptor = null, CancellationToken CancelToken = default(CancellationToken))
        {
            var create = new ProgCreate(fileInfo.FileName, fileInfo.FileSize, FudpCrc.CalcCrc(fileInfo.Data));

            var createAck = Request<ProgCreateAck>(Flow, create);
            if (createAck.ErrorCode != 0)
                switch (createAck.ErrorCode)
                {
                    case 1: throw new CanProgFileAlreadyExistsException(createAck.ErrorMsg[createAck.ErrorCode]);
                    case 2: throw new CanProgMaximumFilesCountAchivedException(createAck.ErrorMsg[createAck.ErrorCode]);
                    case 3: throw new CanProgMemoryIsOutException(createAck.ErrorMsg[createAck.ErrorCode]);
                    case 4: throw new CanProgCreateException(createAck.ErrorMsg[createAck.ErrorCode]);
                    default: throw new CanProgException();
                }

            int pointer = 0;
            while (pointer < fileInfo.FileSize)
            {
                CancelToken.ThrowIfCancellationRequested();
                pointer += Write(fileInfo, pointer);

                if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(Math.Min(1, ((double)pointer / fileInfo.FileSize)));
            }

            OnFileCreated(fileInfo);
        }

        private int Write(DevFileInfo fileInfo, int offset)
        {
            ProgWrite WriteMessage = new ProgWrite(fileInfo, offset);

            var result = Request<ProgWriteAck>(Flow, WriteMessage);
            if (result.Status != ProgWriteAck.WriteStatusKind.OK)
                throw new CanProgWriteException(result.Status);

            return WriteMessage.Data.Length;
        }
        /// <summary>
        /// Команда на содание или изменение записи в словаре свойств
        /// </summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="paramValue">Значение свойства</param>
        public void SetParam(byte paramKey, int paramValue)
        {
            ParamSetRq psr = new ParamSetRq(paramKey, paramValue);
            ParamSetAck psa = Request<ParamSetAck>(Flow, psr);

            if (psa.ErrorCode != 0)
                throw new CanProgCreateException(psa.ErrorMessage);
            else
            {
                if (!Properties.ContainsKey(paramKey)) Properties.Add(paramKey, paramValue);
                else Properties[paramKey] = paramValue;
            }
        }
        /// <summary>
        /// Удаление записи из словаря свойств
        /// </summary>
        /// <param name="paramKey">Ключ</param>
        public void DeleteProperty(byte paramKey)
        {
            ParamRmRq prr = new ParamRmRq(paramKey);
            ParamRmAck pra = Request<ParamRmAck>(Flow, prr);
            if (pra.ErrorCode == 0)
                Console.WriteLine(pra.ErrorMessage);
            else
                throw new CanProgCreateException(pra.ErrorMessage);
        }

        public SubmitStatus SubmitAction { get; set; }
        private bool _submited = false;

        /// <summary>
        /// Разрыв соединения
        /// </summary>
        public void Dispose()
        {
            SuspendPingTimer();
            lock (ProgsOnFlows)
            {
                ProgsOnFlows.Remove(Flow);
            }
            if (!_submited)
                try
                {
                    // TODO: Убедиться в безопасности Dispose()
                    // Убедиться в том, что если Dispose() возникает из-за отключения АППИ, мы не зависнем на этом месте из-за того,
                    // что будем долго пытаться отправить Submit();
                    Submit(SubmitAction);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            if (_disposeFlowOnExit) Flow.Dispose();
        }

        /// <summary>
        /// Отправляет запрос на завершение сеанса программирования
        /// </summary>
        public SubmitAckStatus Submit(SubmitStatus Status)
        {
            var submitMessage = new ProgSubmit(Status);
            var submitAnswer = Request<ProgSubmitAck>(Flow, submitMessage,
                                                      Status == SubmitStatus.Submit ? 15000 : DefaultFudpTimeout,
                                                      MaxAttempts: Status == SubmitStatus.Submit ? DefaultMaximumSendAttempts : 3);
            var status = submitAnswer.Status;
            Console.WriteLine("SUBMIT STATUS: {0}", status);
            _submited = true;
            return status;
        }
    }

    public class CanProgFilesystemEventArgs : EventArgs
    {
        public CanProgFilesystemEventArgs(DevFileInfo File) { this.File = File; }
        public DevFileInfo File { get; private set; }
    }
}
