namespace TWP.Api.Application.DataTransferObjects
{
    public class ShootAndLootDto
    {
        public string WeaponType { get; set; }
        public string Damage { get; set; }
        public string BaseCost { get; set; }
        public string CostMultiplier { get; set; }
        public string Cost {  get; set; }
        public string Range { get; set; }
        public string Weight { get; set; }
        public string Properties { get; set; }
        public string Rarity { get; set; }
        public string Benefit { get; set; }
        public string CompanyName { get; set; }
        public string DamageType { get; set; }
        public string Magazines { get; set; }
        public string TechLevel { get; set; }
        public CompanyLineDataDto CompanyLineData { get; set; }
        public CompanyModelDataDto CompanyModelData { get; set; }
    }

    public class CompanyLineDataDto
    {
        public string lineName { get; set; }
        public string AdditionalProperty { get; set; }
    }

    public class CompanyModelDataDto
    {
        public string modelName { get; set; }
        public string Benefit { get; set; }
    }
}
