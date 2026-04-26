using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters param = (ModbusWriteCommandParameters)CommandParameters;
            byte[] request = new byte[12];

            request[0] = (byte)(param.TransactionId >> 8);
            request[1] = (byte)(param.TransactionId & 0xFF);

            request[2] = (byte)(param.ProtocolId >> 8);
            request[3] = (byte)(param.ProtocolId & 0xFF);

            request[4] = (byte)(param.Length >> 8);
            request[5] = (byte)(param.Length & 0xFF);


            request[6] = param.UnitId;

            request[7] = param.FunctionCode;
            request[8] = (byte)(param.OutputAddress >> 8);
            request[9] = (byte)(param.OutputAddress & 0xFF);

            request[10] = (byte)(param.Value >> 8);
            request[11] = (byte)(param.Value & 0xFF);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusWriteCommandParameters param = (ModbusWriteCommandParameters)CommandParameters;

            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            byte address1 = response[8];
            byte address2 = response[9];

            byte value1 = response[10];
            byte value2 = response[11];



            ushort address = (ushort)((ushort)(address1 << 8) + address2);
            ushort value = (ushort)((ushort)(value1 << 8) + value2);



            result.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, address), value);

            return result;
        }
    }
}