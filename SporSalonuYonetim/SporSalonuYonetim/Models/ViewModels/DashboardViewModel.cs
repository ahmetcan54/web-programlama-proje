namespace SporSalonuYonetim.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Genel Ýstatistikler
        public int ToplamUyeSayisi { get; set; }
        public int AktifUyeSayisi { get; set; }
        public int ToplamAntrenorSayisi { get; set; }
        public int AktifAntrenorSayisi { get; set; }
        public int ToplamHizmetSayisi { get; set; }

        // Randevu Ýstatistikleri
        public int BugunkuRandevuSayisi { get; set; }
        public int BekleyenRandevuSayisi { get; set; }
        public int OnaylananRandevuSayisi { get; set; }
        public int IptalEdilenRandevuSayisi { get; set; }
        public int BuAyToplamRandevu { get; set; }

        // Finansal Ýstatistikler
        public decimal BuAyGelir { get; set; }
        public decimal BuHaftaGelir { get; set; }
        public decimal BugunGelir { get; set; }
        public decimal ToplamGelir { get; set; }

        // Listeler
        public List<Randevu>? SonRandevular { get; set; }
        public List<Uye>? YeniUyeler { get; set; }
        public List<AntrenorIstatistik>? AntrenorIstatistikleri { get; set; }
        public List<HizmetIstatistik>? PopulerHizmetler { get; set; }
        public List<AylikGelir>? AylikGelirGrafigi { get; set; }
    }

    public class AntrenorIstatistik
    {
        public int AntrenorId { get; set; }
        public string? AntrenorAdi { get; set; }
        public int ToplamRandevu { get; set; }
        public int TamamlananRandevu { get; set; }
        public decimal ToplamGelir { get; set; }
        public double OrtalamaPuan { get; set; }
    }

    public class HizmetIstatistik
    {
        public int HizmetId { get; set; }
        public string? HizmetAdi { get; set; }
        public int ToplamKullanim { get; set; }
        public decimal ToplamGelir { get; set; }
        public decimal OrtalamaPuan { get; set; }
    }

    public class AylikGelir
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string? AyAdi { get; set; }
        public decimal Gelir { get; set; }
        public int RandevuSayisi { get; set; }
    }
}