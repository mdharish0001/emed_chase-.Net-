namespace emedl_chase.Model
{
    public class ECWConfig
    {
        public string client_id { get; set; } = string.Empty;
        public string scope { get; set; }   
        public string auth_url { get; set; } = string.Empty;
        public string token_url { get; set; }
        public string private_key_path { get; set; } = string.Empty;
        public string kid { get; set; } = string.Empty;
        public string jku { get; set; } = string.Empty;
    }
}
