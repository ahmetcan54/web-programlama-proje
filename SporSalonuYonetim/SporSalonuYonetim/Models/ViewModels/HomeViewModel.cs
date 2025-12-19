namespace SporSalonuYonetim.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Antrenor> Antrenorler { get; set; } = new();
        public List<SalonGaleri> GaleriResimleri { get; set; } = new();
        public int ToplamUyeSayisi { get; set; }
        public int AktifAntrenorSayisi { get; set; }
        public int ToplamHizmetSayisi { get; set; }
    }

    public class SalonGaleri
    {
        public string BaslikUrl { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
    }

    public class UyelikPaketi
    {
        public string Ad { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string Donem { get; set; } = string.Empty;
        public List<string> Ozellikler { get; set; } = new();
        public bool Populer { get; set; }
    }
}
