using System;

namespace emedl_chase.Cure_Model
{
    public class article_lcd_user_assign
    {
        public long id { get; set; }
        public long? article_id { get; set; }
        public long? lcd_id { get; set; }
        public string? status { get; set; }
        public int? user_id { get; set; }
        public int? count { get; set; }
        public int? batch { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public DateTime? assigned_on { get; set; }
    }
}
