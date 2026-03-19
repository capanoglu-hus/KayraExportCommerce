# 1.Auth Service
### Uygulamanın güvenlik katmanı
  ####  Güvenli giriş için Register , login  işlemleri oluşturuldu.
  #### JWT ve refresh token oluşturuldu. 
  #### Role-based işlemler içi rol oluşturma ve rol atama servisleri yazıldı. 
  #### Güvenli çıkış için revoke işlemi oluşturuldu.
  #### İdentity Kütüphanesi kullanıldı.
  #### İdentity sayesinde direkt olarak Email, password kontrolleri yapıldı.
  #### Proje küçük yapılı olduğu tek bir alanla uğraştığı için web api olarak oluşturuldu.

# 2.Production Service
### Uygulamanın işlem katmanı
  #### Onion mimari ve CQRS tasarım deseni kullanıldı.
  #### Onion ile bağımlılık yönetimi için ve daha ölçeklenebilir, test edelebilir bir yapı oluşturuldu.
  ####  CQRS tasarımı ile veri okuma-yazma işlmeleri servislerde ayrıldı.
  #### Mediator davranışsal tasarım deseni ile birleştirildi, Servis içinde karar verme işlemleri için kullanıldı. MediatR kütüphanesi kullanıldı.
  #### Ürün listeleri ve ilgili ürün rediste cache kaydedildi.
  #### Ürün ekleme, güncelleme, silme gibi işlemlerden sonra rediste cache silme işlemleri uygulandı.
  #### Diğer mikroservisler için önemli işlemlerde event mesajı gönderildi.

# 3.Log Service
### Uygulamanın log katmanı
  #### Redis ile diğer mikroservislerin event ve log mesajlarını dinliyerek bir merkeze topladı.
  #### Serilog kütüphanesi ile Dosya'ya , konsola , seq servisine log bildirimi yapıldı. 

# 4. ApiGateway
### Uygulamayı merkezileştirme 
   #### 3 farklı mikroservisin endpointlerini bir merkezi proxy uygulayarak yönlendirme yapıldı.
   #### Auth service de oluşturulan tokenları production serviste kullanmak için oluşturuldu.
   #### Api limitleri için AddRateLimiter kullanıldı.
    

Local(Geliştirme) -> http://localhost:5125 


Docker (Prod) -> http://localhost:5000
