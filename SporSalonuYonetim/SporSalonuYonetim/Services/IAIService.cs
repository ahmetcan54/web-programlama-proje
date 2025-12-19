namespace SporSalonuYonetim.Services
{
    public interface IAIService
    {
        Task<string> EgzersizOnerisiAl(int uyeId);
        Task<string> BeslenmeTavsiyesiAl(int uyeId);
        Task<string> AntrenmanPlaniOlustur(int uyeId, string hedef, int gunSayisi);
        Task<string> SaglikAnaliziYap(int uyeId);
    }
}
