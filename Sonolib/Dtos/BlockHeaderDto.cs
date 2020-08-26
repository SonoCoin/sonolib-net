namespace Sonolib.Dtos
{
    public class BlockHeaderDto
    {
        public BlockType Type { get; set; }
        
        public string Hash { get; set; }
        
        public int Height { get; set; }
        
        public int Size { get; set; }
        
        public int Version { get; set; }

        public string PrevBlock { get; set; }

        public int Timestamp { get; set; }

        public string Seed { get; set; }

        public int TxCount { get; set; }
        
        public int AdviceCount { get; set; }

        public string Sign { get; set; }
    }
}