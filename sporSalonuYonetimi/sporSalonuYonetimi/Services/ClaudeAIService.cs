using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimi.Data;
using System.Text;
using System.Text.Json;

namespace SporSalonuYonetim.Services
{
    public class ClaudeAIService : IAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClaudeAIService> _logger;

        public ClaudeAIService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ApplicationDbContext context,
            ILogger<ClaudeAIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<string> EgzersizOnerisiAl(int uyeId)
        {
            var uye = await _context.Uyeler
                .Include(u => u.Randevular)
                .FirstOrDefaultAsync(u => u.Id == uyeId);

            if (uye == null)
            {
                return "Üye bulunamadı.";
            }

            var prompt = $@"Sen profesyonel bir fitness koçusun. Aşağıdaki bilgilere sahip bir üye için egzersiz önerisi hazırla:

Üye Bilgileri:
- Ad Soyad: {uye.AdSoyad}
- Yaş: {DateTime.UtcNow.Year - uye.DogumTarihi.Year}
- Cinsiyet: {uye.Cinsiyet}
- Boy: {uye.Boy} cm
- Kilo: {uye.Kilo} kg
- VKİ: {uye.VucutKitleIndeksi:F1}
- Vücut Tipi: {uye.VucutTipi ?? "Belirtilmemiş"}
- Sağlık Durumu: {uye.SaglikDurumu ?? "Normal"}
- Hedefler: {uye.Hedefler ?? "Genel fitness"}
- Geçmiş Randevu Sayısı: {uye.Randevular?.Count ?? 0}

Lütfen kısa ve öz bir şekilde (maksimum 200 kelime):
1. Haftada kaç gün antrenman yapması gerektiğini
2. Hangi egzersiz türlerine odaklanması gerektiğini
3. Dikkat etmesi gereken önemli noktaları belirt.";

            return await GetAIResponse(prompt);
        }

        public async Task<string> BeslenmeTavsiyesiAl(int uyeId)
        {
            var uye = await _context.Uyeler.FindAsync(uyeId);

            if (uye == null)
            {
                return "Üye bulunamadı.";
            }

            var bmr = HesaplaBMR(uye);
            var tdee = bmr * 1.55; // Orta aktivite seviyesi

            var prompt = $@"Sen profesyonel bir diyetisyensin. Aşağıdaki bilgilere sahip bir üye için beslenme tavsiyesi hazırla:

Üye Bilgileri:
- Ad Soyad: {uye.AdSoyad}
- Yaş: {DateTime.UtcNow.Year - uye.DogumTarihi.Year}
- Cinsiyet: {uye.Cinsiyet}
- Boy: {uye.Boy} cm
- Kilo: {uye.Kilo} kg
- VKİ: {uye.VucutKitleIndeksi:F1}
- Günlük Kalori İhtiyacı: {tdee:F0} kcal
- Hedefler: {uye.Hedefler ?? "Genel sağlık"}

Lütfen kısa ve öz bir şekilde (maksimum 200 kelime):
1. Günlük kalori alımı önerisi
2. Makro besin dağılımı (protein, karbonhidrat, yağ)
3. Öğün sayısı ve zamanlaması
4. Kaçınılması gereken yiyecekler
5. Su tüketimi önerisi";

            return await GetAIResponse(prompt);
        }

        public async Task<string> AntrenmanPlaniOlustur(int uyeId, string hedef, int gunSayisi)
        {
            var uye = await _context.Uyeler.FindAsync(uyeId);

            if (uye == null)
            {
                return "Üye bulunamadı.";
            }

            var prompt = $@"Sen profesyonel bir fitness koçusun. Aşağıdaki bilgilere göre detaylı {gunSayisi} günlük antrenman planı hazırla:

Üye Bilgileri:
- Ad Soyad: {uye.AdSoyad}
- Yaş: {DateTime.UtcNow.Year - uye.DogumTarihi.Year}
- Cinsiyet: {uye.Cinsiyet}
- Boy: {uye.Boy} cm, Kilo: {uye.Kilo} kg
- VKİ: {uye.VucutKitleIndeksi:F1}
- Vücut Tipi: {uye.VucutTipi ?? "Belirtilmemiş"}
- Hedef: {hedef}

Her gün için:
1. Hangi kas grupları çalışılacak
2. 4-5 egzersiz önerisi (set x tekrar)
3. Toplam süre
4. Isınma ve soğuma önerileri

Format: Markdown liste formatında, günlere göre organize et.";

            return await GetAIResponse(prompt);
        }

        public async Task<string> SaglikAnaliziYap(int uyeId)
        {
            var uye = await _context.Uyeler
                .Include(u => u.Randevular)
                    .ThenInclude(r => r.Hizmet)
                .FirstOrDefaultAsync(u => u.Id == uyeId);

            if (uye == null)
            {
                return "Üye bulunamadı.";
            }

            var tamamlananRandevular = uye.Randevular?
                .Where(r => r.Durum == Models.RandevuDurumu.Tamamlandi)
                .Count() ?? 0;

            var prompt = $@"Sen bir sağlık danışmanısın. Aşağıdaki üyenin sağlık durumunu analiz et:

Üye Bilgileri:
- Ad Soyad: {uye.AdSoyad}
- Yaş: {DateTime.UtcNow.Year - uye.DogumTarihi.Year}
- Cinsiyet: {uye.Cinsiyet}
- Boy: {uye.Boy} cm, Kilo: {uye.Kilo} kg
- VKİ: {uye.VucutKitleIndeksi:F1}
- Vücut Tipi: {uye.VucutTipi ?? "Belirtilmemiş"}
- Sağlık Durumu: {uye.SaglikDurumu ?? "Normal"}
- Üyelik Süresi: {(DateTime.UtcNow - uye.UyelikBaslangic).Days} gün
- Tamamlanan Antrenman: {tamamlananRandevular} seans

VKİ Değerlendirmesi:
{GetVKIAciklama(uye.VucutKitleIndeksi ?? 0)}

Lütfen:
1. Genel sağlık durumunu değerlendir
2. Potansiyel risk faktörlerini belirt
3. İyileştirme önerilerinde bulun
4. Motivasyon mesajı ver

Maksimum 250 kelime, empatik ve profesyonel bir dille yaz.";

            return await GetAIResponse(prompt);
        }

        private async Task<string> GetAIResponse(string prompt)
        {
            try
            {
                var apiKey = _configuration["AnthropicApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("Anthropic API key bulunamadı. Varsayılan yanıt döndürülüyor.");
                    return GetDefaultResponse(prompt);
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var requestBody = new
                {
                    model = "claude-3-5-sonnet-20241022",
                    max_tokens = 1024,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(
                    "https://api.anthropic.com/v1/messages",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    var text = jsonDoc.RootElement
                        .GetProperty("content")[0]
                        .GetProperty("text")
                        .GetString();

                    return text ?? "Yanıt alınamadı.";
                }
                else
                {
                    _logger.LogError($"API hatası: {response.StatusCode}");
                    return GetDefaultResponse(prompt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI servisi hatası");
                return GetDefaultResponse(prompt);
            }
        }

        private string GetDefaultResponse(string prompt)
        {
            if (prompt.Contains("egzersiz"))
            {
                return @"**Egzersiz Önerisi**

• Haftada 3-4 gün antrenman yapmanızı öneriyoruz
• Kardiyo ve kuvvet antrenmanını dengeli bir şekilde kombine edin
• Her antrenmana 5-10 dakika ısınma ile başlayın
• Vücudunuzu dinleyin ve aşırı zorlamayın
• Düzenli dinlenme günleri planlayın

Detaylı program için antrenörünüzle görüşebilirsiniz.";
            }
            else if (prompt.Contains("beslenme"))
            {
                return @"**Beslenme Tavsiyesi**

• Dengeli ve düzenli beslenmeye özen gösterin
• Günde 2-2.5 litre su tüketin
• Protein, karbonhidrat ve sağlıklı yağları dengeli alın
• Günde 4-5 öğün halinde beslenin
• İşlenmiş gıdalardan kaçının
• Yeşil sebze ve meyve tüketiminizi artırın

Kişiselleştirilmiş plan için diyetisyenimizle görüşebilirsiniz.";
            }
            else if (prompt.Contains("antrenman planı"))
            {
                return @"**Antrenman Planı**

**Gün 1 - Üst Vücut:**
• Bench Press: 3x10
• Dumbbell Row: 3x12
• Shoulder Press: 3x10
• Biceps Curl: 3x15

**Gün 2 - Alt Vücut:**
• Squat: 3x10
• Leg Press: 3x12
• Lunges: 3x10
• Calf Raises: 3x15

**Gün 3 - Kardiyo & Core:**
• 20-30 dakika kardiyo
• Plank: 3x30 saniye
• Crunches: 3x20

Her antrenman 45-60 dakika sürmeli.";
            }
            else
            {
                return @"**Sağlık Analizi**

Düzenli antrenman ve sağlıklı beslenme ile hedeflerinize ulaşabilirsiniz. 

• Mevcut durumunuz genel olarak iyi görünüyor
• Düzenli antrenman rutininizi sürdürün
• Beslenme alışkanlıklarınıza dikkat edin
• Yeterli dinlenme ve uyku önemli
• İlerlemenizi düzenli takip edin

Daha detaylı analiz için antrenörünüzle görüşebilirsiniz.";
            }
        }

        private double HesaplaBMR(Models.Uye uye)
        {
            if (!uye.Kilo.HasValue || !uye.Boy.HasValue)
                return 2000;

            var yas = DateTime.UtcNow.Year - uye.DogumTarihi.Year;

            // Mifflin-St Jeor denklemi
            if (uye.Cinsiyet == "Erkek")
            {
                return (10 * uye.Kilo.Value) + (6.25 * uye.Boy.Value) - (5 * yas) + 5;
            }
            else
            {
                return (10 * uye.Kilo.Value) + (6.25 * uye.Boy.Value) - (5 * yas) - 161;
            }
        }

        private string GetVKIAciklama(double vki)
        {
            if (vki < 18.5)
                return "Düşük kilolu - Kilo almaya odaklanın";
            else if (vki < 25)
                return "Normal kilolu - Mevcut kiloyu koruyun";
            else if (vki < 30)
                return "Fazla kilolu - Kilo vermeye odaklanın";
            else
                return "Obez - Sağlık profesyoneli ile görüşün";
        }
    }
}
