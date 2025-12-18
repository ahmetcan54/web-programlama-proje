using Microsoft.AspNetCore.Mvc;
using SporSalonuYonetim.Services;

namespace SporSalonuYonetimi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<AIController> _logger;

        public AIController(IAIService aiService, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Üye için AI destekli egzersiz önerisi al
        /// </summary>
        /// <param name="uyeId">Üye ID</param>
        /// <returns>Egzersiz önerisi</returns>
        [HttpGet("EgzersizOnerisi/{uyeId}")]
        [ProducesResponseType(typeof(AIResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AIResponseDto>> GetEgzersizOnerisi(int uyeId)
        {
            try
            {
                _logger.LogInformation("Üye {UyeId} için egzersiz önerisi alınıyor", uyeId);

                var oneri = await _aiService.EgzersizOnerisiAl(uyeId);

                return Ok(new AIResponseDto
                {
                    Success = true,
                    Content = oneri,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Egzersiz önerisi alınırken hata oluştu");
                return BadRequest(new AIResponseDto
                {
                    Success = false,
                    Content = "Öneri alınırken bir hata oluştu.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Üye için AI destekli beslenme tavsiyesi al
        /// </summary>
        /// <param name="uyeId">Üye ID</param>
        /// <returns>Beslenme tavsiyesi</returns>
        [HttpGet("BeslenmeTavsiyesi/{uyeId}")]
        [ProducesResponseType(typeof(AIResponseDto), 200)]
        public async Task<ActionResult<AIResponseDto>> GetBeslenmeTavsiyesi(int uyeId)
        {
            try
            {
                _logger.LogInformation("Üye {UyeId} için beslenme tavsiyesi alınıyor", uyeId);

                var tavsiye = await _aiService.BeslenmeTavsiyesiAl(uyeId);

                return Ok(new AIResponseDto
                {
                    Success = true,
                    Content = tavsiye,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beslenme tavsiyesi alınırken hata oluştu");
                return BadRequest(new AIResponseDto
                {
                    Success = false,
                    Content = "Tavsiye alınırken bir hata oluştu.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Üye için kişiselleştirilmiş antrenman planı oluştur
        /// </summary>
        /// <param name="request">Plan oluşturma isteği</param>
        /// <returns>Antrenman planı</returns>
        [HttpPost("AntrenmanPlani")]
        [ProducesResponseType(typeof(AIResponseDto), 200)]
        public async Task<ActionResult<AIResponseDto>> CreateAntrenmanPlani([FromBody] AntrenmanPlaniRequestDto request)
        {
            try
            {
                _logger.LogInformation("Üye {UyeId} için {GunSayisi} günlük plan oluşturuluyor",
                    request.UyeId, request.GunSayisi);

                var plan = await _aiService.AntrenmanPlaniOlustur(
                    request.UyeId,
                    request.Hedef,
                    request.GunSayisi);

                return Ok(new AIResponseDto
                {
                    Success = true,
                    Content = plan,
                    GeneratedAt = DateTime.UtcNow,
                    Metadata = new Dictionary<string, object>
                    {
                        { "hedef", request.Hedef },
                        { "gunSayisi", request.GunSayisi }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Antrenman planı oluşturulurken hata oluştu");
                return BadRequest(new AIResponseDto
                {
                    Success = false,
                    Content = "Plan oluşturulurken bir hata oluştu.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Üyenin sağlık durumunu AI ile analiz et
        /// </summary>
        /// <param name="uyeId">Üye ID</param>
        /// <returns>Sağlık analizi</returns>
        [HttpGet("SaglikAnalizi/{uyeId}")]
        [ProducesResponseType(typeof(AIResponseDto), 200)]
        public async Task<ActionResult<AIResponseDto>> GetSaglikAnalizi(int uyeId)
        {
            try
            {
                _logger.LogInformation("Üye {UyeId} için sağlık analizi yapılıyor", uyeId);

                var analiz = await _aiService.SaglikAnaliziYap(uyeId);

                return Ok(new AIResponseDto
                {
                    Success = true,
                    Content = analiz,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sağlık analizi yapılırken hata oluştu");
                return BadRequest(new AIResponseDto
                {
                    Success = false,
                    Content = "Analiz yapılırken bir hata oluştu.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Tüm AI özelliklerini tek seferde al (combo)
        /// </summary>
        /// <param name="uyeId">Üye ID</param>
        /// <returns>Tüm AI analizleri</returns>
        [HttpGet("TumAnalizler/{uyeId}")]
        [ProducesResponseType(typeof(ComboAIResponseDto), 200)]
        public async Task<ActionResult<ComboAIResponseDto>> GetTumAnalizler(int uyeId)
        {
            try
            {
                _logger.LogInformation("Üye {UyeId} için tüm analizler alınıyor", uyeId);

                var egzersizTask = _aiService.EgzersizOnerisiAl(uyeId);
                var beslenmeTask = _aiService.BeslenmeTavsiyesiAl(uyeId);
                var saglikTask = _aiService.SaglikAnaliziYap(uyeId);

                await Task.WhenAll(egzersizTask, beslenmeTask, saglikTask);

                return Ok(new ComboAIResponseDto
                {
                    Success = true,
                    EgzersizOnerisi = egzersizTask.Result,
                    BeslenmeTavsiyesi = beslenmeTask.Result,
                    SaglikAnalizi = saglikTask.Result,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analizler alınırken hata oluştu");
                return BadRequest(new ComboAIResponseDto
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }
    }

    #region DTOs

    public class AIResponseDto
    {
        public bool Success { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Error { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    public class AntrenmanPlaniRequestDto
    {
        public int UyeId { get; set; }
        public string Hedef { get; set; } = "Genel fitness";
        public int GunSayisi { get; set; } = 3;
    }

    public class ComboAIResponseDto
    {
        public bool Success { get; set; }
        public string EgzersizOnerisi { get; set; } = string.Empty;
        public string BeslenmeTavsiyesi { get; set; } = string.Empty;
        public string SaglikAnalizi { get; set; } = string.Empty;
        public string? Error { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    #endregion
}
