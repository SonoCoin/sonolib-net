﻿namespace Sonolib.Dtos.Extended
{
    public class WalletDto
    {
        public string Mnemonic { get; set; }
        public string Seed { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string Address { get; set; }
    }
}