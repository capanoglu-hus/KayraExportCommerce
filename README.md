#  Kayra Export Commerce - Microservices Project

Bu proje, modern yazılım mimarileri kullanılarak geliştirilmiş; **Auth** , **Log** ve **Production** mikroservislerinden oluşan, Dockerize edilmiş bir e-ticaret altyapı simülasyonudur.

---

## 🛠 Kullanılan Teknolojiler & Mimari

* **Framework:** .NET Core (Microservices)
* **Identity:** ASP.NET Core Identity (Email & Password Validation)
* **Security:** JWT & Refresh Token Mekanizması
* **Database:** Entity Framework Core & SQL Server
* **Caching:** Redis (Ürün listeleme )
* **Event:** Redis ile mikroservisler arası iletişim
* **Containerization:** Docker & Docker Compose
* **CI/CD:** GitHub Actions (CI Check)

---

##  Docker ile Kurulum

Projeyi herhangi bir manuel veritabanı veya servis kurulumu yapmadan, Docker üzerinden tek komutla ayağa kaldırabilirsiniz.

### 1. Projeyi Klonlayın
```bash
git clone [https://github.com/capanoglu-hus/KayraExportCommerce.git](https://github.com/capanoglu-hus/KayraExportCommerce.git)

cd KayraExportCommerce
```
### 2. Sistemi Başlatın

    docker-compose up -d --build


### 3. İstek atın

    "http://localhost:5000" ile endpointlere istek atın


##### Dokümantasyon  
    
 Daha detaylı teknik analizler, veritabanı şemaları ve endpoint listeleri için lütfen doküman klasörünü inceleyin
