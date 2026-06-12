namespace emedl_chase.Service
{
    public interface IClinicalPdfService
    {
        Task<byte[]> GenerateClinicalPdfAsync(string xmlContent, string outputPath, string dos, string encounterReason);


    }
}
