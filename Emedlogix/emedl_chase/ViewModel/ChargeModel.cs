namespace emedl_chase.ViewModel
{
    public class ChargeModel
    {
    public int Id { get; set; }
    public string patient_name { get; set; }
    public string patient_id { get; set; }
    public string cpt_code { get; set; }
    public string practice { get; set; }
    public int? claim_id { get; set; }
    public int? encounter_id { get; set; }
    public DateTime? dos { get; set; }
    public DateTime? claim_date { get; set; }

    }
}
