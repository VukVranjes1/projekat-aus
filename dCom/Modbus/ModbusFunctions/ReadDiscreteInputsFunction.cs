using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read discrete inputs functions/requests.
    /// </summary>
    public class ReadDiscreteInputsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadDiscreteInputsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadDiscreteInputsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters zaCitanje = (ModbusReadCommandParameters)CommandParameters;
            byte[] zNi = new byte[12];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaCitanje.TransactionId)), 0, zNi, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaCitanje.ProtocolId)), 0, zNi, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaCitanje.Length)), 0, zNi, 4, 2);
            zNi[6] = zaCitanje.UnitId;
            zNi[7] = zaCitanje.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaCitanje.StartAddress)), 0, zNi, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaCitanje.Quantity)), 0, zNi, 10, 2);

            return zNi;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters zaCitanje = (ModbusReadCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> recnik = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort byteCount = response[8];
            ushort adresa = zaCitanje.StartAddress;
            int cnt = 0;

            for (int i = 9; i < 9 + byteCount; i++)
            {
                byte tempByte = response[i];
                for (int j = 0; j < 8; j++)
                {
                    ushort value = (ushort)(tempByte & 1);
                    tempByte >>= 1;
                    recnik.Add(Tuple.Create(PointType.DIGITAL_INPUT, adresa), value);
                    adresa++;
                    cnt++;
                    if (cnt == zaCitanje.Quantity)
                        break;
                }
                if (cnt == zaCitanje.Quantity)
                    break;
            }

            return recnik;
        }
    }
}