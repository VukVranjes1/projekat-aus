using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters param = (ModbusReadCommandParameters)CommandParameters;
            byte[] request = new byte[12];

            request[0] = (byte)(param.TransactionId >> 8);
            request[1] = (byte)(param.TransactionId & 0xFF);

            request[2] = (byte)(param.ProtocolId >> 8);
            request[3] = (byte)(param.ProtocolId & 0xFF);

            request[4] = (byte)(param.Length >> 8);
            request[5] = (byte)(param.Length & 0xFF);


            request[6] = param.UnitId;

            request[7] = param.FunctionCode;
            request[8] = (byte)(param.StartAddress >> 8);
            request[9] = (byte)(param.StartAddress & 0xFF);

            request[10] = (byte)(param.Quantity >> 8);
            request[11] = (byte)(param.Quantity & 0xFF);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters param = (ModbusReadCommandParameters)CommandParameters;

            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            byte numberOfBytes = response[8];

            int numberOfRegisters = numberOfBytes / 2;

            int requiredRegisters;

            int counter = 0; 
            if (numberOfRegisters > param.Quantity)
                  requiredRegisters = param.Quantity;

            else requiredRegisters = numberOfRegisters;


            for (int i = 0; i<requiredRegisters; i++)
            {
                byte firstByte = response[9+2*i];
                byte secondByte = response[10+2*i];
                ushort value = (ushort)((ushort)(firstByte) << 8 | (ushort)(secondByte) & 0xff);
                result.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)(param.StartAddress + counter)), value);
                counter++;
            }

            return result;
        }
    }
}