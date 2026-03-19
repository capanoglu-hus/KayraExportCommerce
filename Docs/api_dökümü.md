# 1. Auth Service (Kimlik Doğrulama ve Yetki)
### Kullanıcı yönetimi, JWT işlemleri ve rol atamaları bu servis üzerinden yürütülür.

#### [POST] /auth-service/auth/register
    Yeni bir kullanıcı hesabı oluşturur.
    Body (JSON): username, email, password
    Özellikler: Email formatı ve şifre karmaşıklığı kontrol edilir.
    Not: Başarılı kayıtta log sistemi bilgilendirilir.

#### [POST] /auth-service/auth/login
    Sisteme giriş yapar.
    Body (JSON): email, password
    Yanıt: accessToken, refreshToken
    Not: Docker ortamında bu işlem log servisine Event olarak bildirilir.

#### [POST] /auth-service/role/create
    Yeni bir kullanıcı rolü tanımlar.
    Body (JSON): roleName (örn: admin, junior, editor)

#### [POST] /auth-service/role/assign
    Bir kullanıcıya belirli bir rol atar.
    Body (JSON): roleName, email
    Not: Kullanıcı veya rol bulunamazsa hata logu oluşturulur.

#### [POST] /auth-service/auth/revoke
    Mevcut oturumu sonlandırır (Logout).
    Body (JSON): accessToken, refreshToken
    Amacı: Tokenları geçersiz kılarak güvenliği sağlar.

# 2. Production Service (Ürün Yönetimi)
## Ürünlerin CRUD işlemleri ve caching mekanizmaları bu servis üzerindedir.

### [GET] /production-service/product
    Tüm ürün listesini getirir.

    Auth: JWT Gerekmez.

    Cache: Eğer veri Redis'te yoksa, veritabanından okunur ve cache'e yüklenir.

### [GET] /production-service/product/GetProduct?id={id}
    ID bazlı tekil ürün detayını getirir.

    Auth: JWT Gerekmez.

    Cache: İlgili ürünü cache'e kaydeder.

### [POST] /production-service/product
    Sisteme yeni ürün ekler.

    Auth:JWT gereklidir.

    Body (JSON): name, price, description, StockQuantity, Status

    Cache: İşlem sonrası Cache temizlenir ve diğer mikroservislere Event gönderilir.

### [PUT] /production-service/product
    Mevcut ürün bilgilerini günceller.

    Auth: JWT gereklidir.

    Body (JSON): productId + Güncel veriler.

    Cache: Ürün listesi ve ilgili ürünün cache verileri temizlenir.

### [DELETE] /production-service/product?id={id}
    Sistemden ürün siler.

    Auth: JWT gereklidir.

    Cache: Ürün listesi cache'ten silinir.
