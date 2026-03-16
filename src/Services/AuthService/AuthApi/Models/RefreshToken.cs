namespace AuthApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public DateTime? Revoked  { get; set; }
        /*geçerlilik tarihi için*/
        public bool isExpired => DateTime.UtcNow >= Expires;
        /*kullanıcı aktifliğini kontrol etme*/
        public bool isActive => Revoked == null && !isExpired;
        // foreign key olarak ıdentityden geleni kullanıyoruz
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}