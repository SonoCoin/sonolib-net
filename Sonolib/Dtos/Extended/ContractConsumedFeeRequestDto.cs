﻿namespace Sonolib.Dtos.Extended
{
    public class ContractConsumedFeeRequestDto
    {
        public string Sender { get; set; }
        public string Address { get; set; }
        public string Payload { get; set; }
        public ulong Value { get; set; }
        public ulong Commission { get; set; }
    }
}