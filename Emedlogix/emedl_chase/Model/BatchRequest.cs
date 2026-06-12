namespace emedl_chase.Model
{
    public class BatchRequest
    {
        public string FileName { get; set; }  // e.g. "patients.xlsx"
    }

    public class PatientRow
    {
        public string Name { get; set; }
        public string DOS { get; set; }
    }

    public class BatchResult
    {
        public string PatientName { get; set; }
        public string DOS { get; set; }
        public bool Success { get; set; }
        public string PdfPath { get; set; }  // relative URL to download
        public string Error { get; set; }
    }

}
