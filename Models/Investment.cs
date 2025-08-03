namespace Tokero.Models
{
    public class Investment
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string CryptoSymbol { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal MonthRoi { get; set; }
        public decimal TotalRoi { get; set; }
        public Color MonthRoiColor { get; set; }
        public Color TotalRoiColor { get; set; }
        public Color RowBackgroundColor { get; set; }
    }
}
