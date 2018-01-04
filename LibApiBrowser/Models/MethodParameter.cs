namespace LibApiBrowser.Models
{
    public class MethodParameter
    {
        public bool IsGeneric { get; set; }

        public string ParameterType { get; set; }

        public string[] Types { get; set; }

        public string Name { get; set; }
    }
}