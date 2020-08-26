namespace Sonolib.Dtos.Extended
{
    public class HeadersRequestDto
    {
        public int Offset { get; set; } = 0;
        
        public int Limit { get; set; } = 0;

        /// <summary>
        /// Network name
        /// </summary>
        /// <remarks>Optional field. By default is Main</remarks>
        public string NetworkName { get; set; } = "Main";
    }
}