using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.ControllerClass
{
    internal class ER4SCommandSet : ICommandSet
    {
        public byte Header { get; set; } = 0x4C;
        public byte AllChannelWriteCommand { get; set; } = 0x15;
        public byte AllChannelReadCommand { get; set; } = 0x16;
        public byte ChannelWriteCommand { get; set; } = 0x12;
        public byte EEPROMLoadCommand { get; set; } = 0x1C;
        public byte EEPROMSaveCommand { get; set; } = 0x1B;
        public byte Tail1 { get; set; } = 0x0D;
        public byte Tail2 { get; set; } = 0x0A;
    }
}
