using Programmer.Input.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Programmer.Input
{
    // int
    // int64
    // int48
    // hex32
    // hex48
    // hex64
    // mac
    // ip
    // str

    class InputParser
    {
        List<InputType> Types = new List<InputType>();

        public InputParser()
        {
            Types.Add(new InputType("int", 
                s => { int val; return Int32.TryParse(s, out val); }, 
                s => s,
                s => (Int32.Parse(s) + 1).ToString()));
            Types.Add(new InputType("int64", 
                s => { Int64 val; return Int64.TryParse(s, out val); },
                s => s,
                s => (Int64.Parse(s) + 1).ToString()));
            Types.Add(new InputType("int48",
                s => { Int64 val; return Int64.TryParse(s, out val); },
                s => (Int64.Parse(s) & 0xFFFFFFFFFFFFL).ToString(),
                s => ((Int64.Parse(s) + 1) & 0xFFFFFFFFFFFFL).ToString()));
            Types.Add(new InputType("hex32",
                s => { UInt32 val; return UInt32.TryParse(s, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val); },
                s => (UInt32.Parse(s, System.Globalization.NumberStyles.HexNumber) & 0xFFFFFFFFFFFFL).ToString("X8"),
                s => ((UInt32.Parse(s, System.Globalization.NumberStyles.HexNumber) + 1) & 0xFFFFFFFFFFFFL).ToString("X8")));
            Types.Add(new InputType("hex48", 
                s => { UInt64 val; return UInt64.TryParse(s, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val); },
                s => (UInt64.Parse(s, System.Globalization.NumberStyles.HexNumber) & 0xFFFFFFFFFFFFL).ToString("X12"),
                s => ((UInt64.Parse(s, System.Globalization.NumberStyles.HexNumber) + 1) & 0xFFFFFFFFFFFFL).ToString("X12")));
            Types.Add(new InputType("hex64",
                s => { UInt64 val; return UInt64.TryParse(s, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val); },
                s => UInt64.Parse(s, System.Globalization.NumberStyles.HexNumber).ToString(),
                s => (UInt64.Parse(s, System.Globalization.NumberStyles.HexNumber) + 1).ToString("X16")));
            Types.Add(new InputType("ip",
                s => { IPAddress val; return IPAddress.TryParse(s, out val); },
                s => s,
                s => { var val = IPAddress.Parse(s).GetAddressBytes();
                       for (int i = 0; i < val.Length; i++ )
                       {
                           int Index = val.Length - 1 - i;
                           val[Index]++;
                           if (val[Index] != 0) break;
                       }
                    return new IPAddress(val).ToString();
                }));
            Types.Add(new InputType("mac",
                s => { MACAddress val; return MACAddress.TryParse(s, out val); },
                s => {
                    var val = MACAddress.Parse(s);
                    return val.Address.ToString();
                },
                s =>
                {
                    var val = MACAddress.Parse(s);
                    val.Increment();
                    return val.ToString();
                })); 
            Types.Add(new InputType("str",
                s => true,
                s => s,
                null));
        }

        /// <summary>
        /// Увеличить значение на 1, если возможно
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public string Increment(string Type, string Value)
        {
            foreach (var T in Types)
            {
                if (T.Name.CompareTo(Type) == 0) return T.Increment(Value);
            }

            return Value;
        }

        /// <summary>
        /// Проверка корректности ввода
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool Check(string Type, string Value)
        {
            foreach (var T in Types)
            {
                if (T.Name.CompareTo(Type) == 0) return T.Check(Value);
            }

            return false;
        }

        /// <summary>
        /// Преобразовать в выходное значение
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public string Convert(string Type, string Value)
        {
            foreach (var T in Types)
            {
                if (T.Name.CompareTo(Type) == 0) return T.Convert(Value);
            }

            return Value;
        }

        /// <summary>
        /// Может ли увеличиваться?
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public bool HasIncrement(string Type)
        {
            foreach(var T in Types)
            {
                if (T.Name.CompareTo(Type) == 0) return T.Increment != null;
            }

            return false;
        }
    }
}
