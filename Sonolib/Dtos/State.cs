namespace Sonolib.Dtos
{
    public class State
    {
        public string Address { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public string Requester { get; set; }
        public ulong Commission { get; set; }
        public ulong NonceDelta { get; set; }
        public int ParentIndex { get; set; }
    }
}