namespace VirginMaryCenter.Controllers
{
    public class ZipCodeInfo
    {
        public string ZipCode{ get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public short GMT { get; set; }
        public bool DAYLIGHT { get; set; } //if true, it has DAYLIGHT SAVINGS


        public string city { get; set; }
        public string state{ get; set; }
        public string county { get; set; }

    }
}