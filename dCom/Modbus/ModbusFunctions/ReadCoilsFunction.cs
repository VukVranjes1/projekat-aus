using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
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
            ushort counter = 0;
            int flag = 0;

            for (int i = 0; i<numberOfBytes; i++)
            {

                byte mask = 1;
                byte currentByte = response[9 + i];

                for (int j = 0; j < 8; j++)
                {

                    ushort value;
                    if (counter >= param.Quantity)
                    {
                        flag = 1;
                        break;
                    }


                    if ((currentByte & mask) != 0)
                    {
                        value = 1;
                    }
                    else
                    {
                        value = 0;
                    }

                    result.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, (ushort)(param.StartAddress + counter)), value);
                    mask <<= 1;
                    counter++;
                  
                }

                if (flag==1) break;
            }


            return result;
        }
    }
}