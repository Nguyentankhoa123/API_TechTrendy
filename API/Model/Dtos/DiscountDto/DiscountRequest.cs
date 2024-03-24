namespace API.Model.Dtos.DiscountDto
{
    public class DiscountRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Percent { get; set; }
        public double? MaxDiscountAmount { get; set; }
        public double? MinOrderValue { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int MaxUse { get; set; }
        public bool IsActive { get; set; }
    }
}
