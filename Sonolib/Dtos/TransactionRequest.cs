using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sonolib.Crypto;
using Sonolib.Extensions;
using Sonolib.Helpers;

namespace Sonolib.Dtos
{
    public class TransactionRequest
    {
        public string Hash { get; set; }
        public TransactionType Type { get; set; }
        public int Version { get; set; }
        public ulong GasPrice { get; set; }
        public List<TransactionInputDto> Inputs { get; set; }
        public List<TransferDto> Transfers { get; set; }
        public List<ContractMessageDto> Messages { get; set; }
        public List<StakeDto> Stakes { get; set; }
        
        // Extended fields
        [JsonIgnore]
        private Dictionary<string, Key> Signers { get; set; }
        
        [JsonIgnore]
        private ulong _transferCommision { get; set; }
        
        public TransactionRequest()
        {
            Version = Constants.TxVersion;
            Inputs = new List<TransactionInputDto>();
            Signers = new Dictionary<string, Key>();
            Type = TransactionType.Account;
        }
        
        public IEnumerable<byte> GenerateHash()
        {
            var payload = ToBytes();
            return payload.ToArray().ToDoubleSha256();
        }

        public List<byte> ToBytes()
        {
             var payload = new List<byte>();
             
             // Step 1 add Type (4 bytes)
             payload.AddRange(BitConverter.GetBytes((int) Type));

             // Step 2: add Version (4 bytes)
             payload.AddRange(BitConverter.GetBytes(Version));
             
             // Step 3: add GasPrice (8 bytes)
             payload.AddRange(BitConverter.GetBytes(GasPrice));
             
             // Step 4: add Inputs
             if (Inputs != null)
             {
                 foreach (var item in Inputs)
                 {
                     payload.AddRange(item.ToBytes());
                 }
             }
             
             // Step 5: add Transfers
             if (Transfers != null)
             {
                 foreach (var item in Transfers)
                 {
                     payload.AddRange(item.ToBytes());
                 }   
             }
             
             // Step 6: add Messages
             if (Messages != null)
             {
                 foreach (var item in Messages)
                 {
                     payload.AddRange(item.ToBytes());
                 }
             }
             
             // Step 7: add Stakes
             if (Stakes != null)
             {
                 foreach (var item in Stakes)
                 {
                     payload.AddRange(item.ToBytes());
                 }
             }

             return payload;
        }

        public void ValidateValue(ulong commission)
        {
            var len = (Transfers?.Count ?? 0) + (Stakes?.Count ?? 0);
            var outValue = commission * GasPrice * (ulong) len;
            
            if (Transfers != null)
            {
                outValue = Transfers.Aggregate(outValue, (current, txOut) =>
                    current + txOut.Value);   
            }

            if (Stakes != null)
            {
                outValue = Stakes.Aggregate(outValue, (current, txOut) =>
                    current + txOut.Value);   
            }

            if (Messages != null)
            {
                outValue = Messages.Aggregate(outValue, (current, txOut) => 
                    current + txOut.Value + txOut.Gas * GasPrice);   
            }

            var inValue = Inputs.Aggregate((ulong) 0, (current, txIn) =>
                current + txIn.Value);

            if (inValue != outValue)
            {
                throw new Exception($"Wrong sum in transaction, inValue: {inValue}, outValue: {outValue}");
            }
        }

        public TransactionRequest AddCommission(ulong gasPrice, ulong commission)
        {
            GasPrice = gasPrice;
            _transferCommision = commission;
            return this;
        }
        
        public TransactionRequest AddSender(string address, Key key, ulong value, ulong nonce)
        {
            Inputs.Add(new TransactionInputDto
            {
                Address = address,
                Value = value,
                Nonce = nonce,
                PublicKey = key.PublicKey.ToHex(),
            });

            Signers.Add(address, key);
            return this;
        }

        public TransactionRequest AddTransfer(string address, ulong value)
        {
            Transfers ??= new List<TransferDto>();
            
            Transfers.Add(new TransferDto
            {
                Address = address,
                Value = value,
            });
            return this;
        }

        private void CheckContactsData()
        {
            Messages ??= new List<ContractMessageDto>();
        }

        public TransactionRequest AddContractCreation(string sender, string code, ulong value, ulong gas)
        {
            CheckContactsData();
            Messages.Add(new ContractMessageDto
            {
                Sender = sender,
                Payload = code,
                Value = value,
                Gas = gas,
            });
            return this;
        }

        public TransactionRequest AddContractExecution(string sender, string address, string input, ulong value, ulong gas)
        {
            CheckContactsData();
            Messages.Add(new ContractMessageDto
            {
                Sender = sender,
                Address = address,
                Payload = input,
                Value = value,
                Gas = gas,
            });
            return this;
        }

        public TransactionRequest AddStake(string address, ulong value, string nodeId)
        {
            Stakes ??= new List<StakeDto>();
            Stakes.Add(new StakeDto
            {
                Address = address,
                Value = value,
                NodeId = nodeId,
            });
            return this;
        }

        public void Validate()
        {
            ValidateValue(_transferCommision);
        }

        public TransactionRequest Sign()
        {
            Validate();
            
            foreach (var item in Inputs)
            {
                var msg = MsgFormSignUser(item);
                var sig = Key.Sign(msg, Signers[item.Address].PrivateKey);
                item.Sign = sig.ToHex();
            }
            Hash = GenerateHash().ToHex();
            return this;
        }

        private byte[] MsgFormSignUser(TransactionInputDto input)
        {
            var payload = new List<byte>();

            // Step 1 add Type (4 bytes)
            payload.AddRange(BitConverter.GetBytes((int) Type));

            // Step 2: add Version (4 bytes)
            payload.AddRange(BitConverter.GetBytes(Version));
            
            // Step 3: add Version (8 bytes)
            payload.AddRange(BitConverter.GetBytes(GasPrice));

            // Step 4: add transaction input
            payload.AddRange(input.ToBytes());

            // Step 5: add Transfers
            if (Transfers != null)
            {
                foreach (var item in Transfers)
                {
                    payload.AddRange(item.ToBytes());
                }
            }
            
            // Step 6: add Messages
            if (Messages != null)
            {
                foreach (var item in Messages)
                {
                    payload.AddRange(item.ToBytes());
                }
            }
            
            // Step 7: add Stakes
            if (Stakes != null)
            {
                foreach (var item in Stakes)
                {
                    payload.AddRange(item.ToBytes());
                }   
            }

            return payload.ToArray();
        }

        /// <summary>
        /// Returns JSON representation of class
        /// </summary>
        /// <returns></returns>
        public string ToJson(bool prettify = true)
        {
            var opts = new JsonSerializerOptions
            {
                WriteIndented = prettify,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                //AllowTrailingCommas = false,
            };

            return JsonSerializer.Serialize(this, opts);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string ToHex()
        {
            var str = ToJson();
            var bytes = Encoding.UTF8.GetBytes(str);
            return bytes.ToHex();
        }
    }
}