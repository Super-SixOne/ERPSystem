namespace ERPSystem.Models
{
    public class Material
    {
        public string MaterialNo { get; set; }
        public string MaterialType { get; set; }
        public string? Description { get; set; }
        public string? QualityInstructions { get; set; }
        public string? TechnicalDrawingURL { get; set; }
    }
}
