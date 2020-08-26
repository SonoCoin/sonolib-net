using System.Collections.Generic;

namespace Sonolib.Dtos.Extended
{
    public class BlocksRequestDto
    {
        /// <summary>
        /// Hashes of block
        /// </summary>
        public List<string> Hashes { get; set; }
    }
}