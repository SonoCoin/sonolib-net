using System.Collections.Generic;

namespace Sonolib.Dtos.Extended
{
    public class VmDataDto
    {
        public int Index { get; set; }
        public IEnumerable<VmDataLogDto> Logs { get; set; }
        public string Result { get; set; }
    }
}