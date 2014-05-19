using System;
using System.IO;
using Fudp.Model;
using Fudp.Protocol.Exceptions;
using Fudp.Protocol.Messages;

namespace Fudp.Operators
{
    public class FudpFileStream : Stream
    {
        private readonly bool _canSeek;
        private readonly long _length;

        public FudpFileStream(IFudpConnection Connection, string FileName, long Length, bool CanSeek = true)
        {
            this.Connection = Connection;
            _canSeek = CanSeek;
            this.FileName = FileName;
            _length = Length;
        }

        public String FileName { get; private set; }

        public IFudpConnection Connection { get; private set; }

        /// <summary>
        ///     При переопределении в производном классе возвращает значение, показывающее, поддерживает ли текущий поток
        ///     возможность чтения.
        /// </summary>
        /// <returns>Значение true, если поток поддерживает чтение; в противном случае — значение false.</returns>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        ///     При переопределении в производном классе возвращает значение, которое показывает, поддерживается ли в текущем
        ///     потоке возможность поиска.
        /// </summary>
        /// <returns>Значение true, если поток поддерживает поиск; в противном случае — значение false.</returns>
        public override bool CanSeek
        {
            get { return _canSeek; }
        }

        /// <summary>
        ///     При переопределении в производном классе возвращает значение, которое показывает, поддерживает ли текущий
        ///     поток возможность записи.
        /// </summary>
        /// <returns>Значение true, если поток поддерживает запись; в противном случае — false.</returns>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>При переопределении в производном классе получает длину потока в байтах.</summary>
        /// <returns>Длинное значение, представляющее длину потока в байтах.</returns>
        /// <exception cref="T:System.NotSupportedException">
        ///     Класс, созданный на основе класса Stream, не поддерживает возможность
        ///     поиска.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Методы были вызваны после закрытия потока. </exception>
        public override long Length
        {
            get { return _length; }
        }

        /// <summary>При переопределении в производном классе получает или задает позицию в текущем потоке.</summary>
        /// <returns>Текущее положение в потоке.</returns>
        /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода. </exception>
        /// <exception cref="T:System.NotSupportedException">Этот поток не поддерживает поиск. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Методы были вызваны после закрытия потока. </exception>
        public override long Position { get; set; }

        /// <summary>
        ///     При переопределении в производном классе очищает все буферы данного потока и вызывает запись данных буферов в
        ///     базовое устройство.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода. </exception>
        public override void Flush() { throw new NotImplementedException(); }

        /// <summary>При переопределении в производном классе задает позицию в текущем потоке.</summary>
        /// <returns>Новая позиция в текущем потоке.</returns>
        /// <param name="offset">Смещение в байтах относительно параметра <paramref name="origin" />. </param>
        /// <param name="origin">
        ///     Значение типа <see cref="T:System.IO.SeekOrigin" /> определяет точку ссылки, которая используется
        ///     для получения новой позиции.
        /// </param>
        /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     Поток не поддерживает поиск, если поток создан на основе канала или
        ///     вывода консоли.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Методы были вызваны после закрытия потока. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
            }
            return Position;
        }

        /// <summary>При переопределении в производном классе задает длину текущего потока.</summary>
        /// <param name="value">Необходимая длина текущего потока в байтах. </param>
        /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода. </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     Поток не поддерживает ни поиск, ни запись, например, если поток создан
        ///     на основе канала или вывода консоли.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">Методы были вызваны после закрытия потока. </exception>
        public override void SetLength(long value) { throw new NotImplementedException(); }

        /// <summary>
        ///     При переопределении в производном классе считывает последовательность байтов из текущего потока и перемещает
        ///     позицию в потоке на число считанных байтов.
        /// </summary>
        /// <returns>
        ///     Общее количество байтов, считанных в буфер.Это число может быть меньше количества запрошенных байтов, если
        ///     столько байтов в настоящее время недоступно, а также равняться нулю (0), если был достигнут конец потока.
        /// </returns>
        /// <param name="buffer">
        ///     Массив байтов.После завершения выполнения данного метода буфер содержит указанный массив байтов, в
        ///     котором значения в интервале между <paramref name="offset" /> и (<paramref name="offset" /> +
        ///     <paramref name="count" /> - 1) заменены байтами, считанными из текущего источника.
        /// </param>
        /// <param name="offset">
        ///     Смещение байтов (начиная с нуля) в <paramref name="buffer" />, с которого начинается сохранение
        ///     данных, считанных из текущего потока.
        /// </param>
        /// <param name="count">Максимальное количество байтов, которое должно быть считано из текущего потока. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     Сумма значений параметров <paramref name="offset" /> и
        ///     <paramref name="count" /> больше длины буфера.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> имеет значение null; </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> или <paramref name="count" />
        ///     являются отрицательными.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода. </exception>
        /// <exception cref="T:System.NotSupportedException">Поток не поддерживает чтение. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Методы были вызваны после закрытия потока. </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var requestedLength = (int)
                                  Math.Min(count,
                                           Math.Min(Length - Position,
                                                    ProgRead.GetPayload(FileName, Connection.MaximumPacketLength)));

            var request = new ProgReadRq(FileName, (int)Position, requestedLength);
            var response = Connection.Request<ProgRead>(request);

            if (response.ErrorCode == 0)
            {
                Buffer.BlockCopy(response.ReadData, 0, buffer, offset, response.ReadData.Length);
                Position += response.ReadData.Length;
            }
            else
            {
                switch (response.ErrorCode)
                {
                    case 1:
                        throw new FileNotFoundException(response.ErrorMessage);
                    case 2:
                        throw new IndexOutOfRangeException(response.ErrorMessage);
                    case 3:
                        throw new CanProgReadException(response.ErrorMessage);
                    default:
                        throw new CanProgException();
                }
            }
            return response.ReadData.Length;
        }

        /// <summary>
        ///     При переопределении в производном классе записывает последовательность байтов в текущий поток и перемещает
        ///     текущую позицию в нем вперед на число записанных байтов.
        /// </summary>
        /// <param name="buffer">
        ///     Массив байтов.Этот метод копирует байты <paramref name="count" /> из <paramref name="buffer" /> в
        ///     текущий поток.
        /// </param>
        /// <param name="offset">
        ///     Смещение байтов (начиная с нуля) в <paramref name="buffer" />, с которого начинается копирование
        ///     байтов в текущий поток.
        /// </param>
        /// <param name="count">Количество байтов, которое необходимо записать в текущий поток. </param>
        /// <exception cref="T:System.ArgumentException">
        ///     Сумма значений параметров <paramref name="offset" /> и
        ///     <paramref name="count" /> больше длины буфера.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> имеет значение null; </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="offset" /> или <paramref name="count" />
        ///     являются отрицательными.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">Ошибка ввода-вывода. </exception>
        /// <exception cref="T:System.NotSupportedException">Этот поток не поддерживает запись. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Методы были вызваны после закрытия потока. </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            
        }
    }
}
