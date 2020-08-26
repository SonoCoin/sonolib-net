using System.Collections.Generic;

namespace Sonolib.Dtos.Extended
{
    public class VmDataLogDto
    {
        public string Address { get; set; }
        public ICollection<string> Topics { get; set; }
        public string Log { get; set; }
    }
}