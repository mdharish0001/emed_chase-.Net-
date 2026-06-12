namespace emedl_chase.Model
{
    public class BinaryDTO
    {
        public class Rootobject
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public string contentType { get; set; }
            public string data { get; set; }
        }

        public class Meta
        {
            public string[] profile { get; set; }
        }


        public class finalresponse
        {
            public string binaryxml { get; set; }

        }
    }

}