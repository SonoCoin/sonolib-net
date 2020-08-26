﻿namespace Sonolib.Helpers
{
    public class Constants
    {
        /// <summary>
        /// Network type: MainNet
        /// </summary>
        public const string NetworkTypeMainNet = "Main";
        
        /// <summary>
        /// Network type: TestNet
        /// </summary>
        public const string NetworkTypeTestNet = "Test";
        
        /// <summary>
        /// Network type: RegTest
        /// </summary>
        public const string NetworkTypeRegTest = "RegTest";

        /// <summary>
        /// Commission
        /// </summary>
        public const ulong Commission = 1000000;
        
        /// <summary>
        /// Satoshi in 1
        /// </summary>
        public const decimal CurrencyDivider = 100000000;

        /// <summary>
        /// Transaction version
        /// </summary>
        public static int TxVersion = 1;
    }
}