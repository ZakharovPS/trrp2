using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class DESKeyAndIV
    {
        public byte[] Key;
        public byte[] IV;

        public DESKeyAndIV(byte[] Key, byte[] IV)
        {
            this.Key = Key;
            this.IV = IV;
        }
    }
}
