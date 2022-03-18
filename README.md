# Configuration Case

Asp.net Core 5 ile Configuration Kayıtlarının Read we Write Operasyonlarının Yapıldığı Solution 

# Projede Kullanılan Teknolojiler:

- .Net 5 Core Restful Web Servisleri
- EF Core 
- Hangfire Jobs
- Masstransit ve RabbitMq ile Message Broker
- AutoMapper
- Redis Cache
- MsSql Server 

# CommonService:

Okuma ve Yazma Operasyonlarının Yönetildiği Ortak Bir Servistir.
Listeleme, Ekleme, Güncelleme ve Silme İşlemleri Yönetilir.
GetValue methoduna da bu servis üzerinden erişebilir

# ConfigurationSource

Bu proje bağımsız bir web projesi olup, Service A ve Service B'den
publish olan eventları consume ederek, event consume edildiği anda, parametreden
gönderilen ConString, AppName ve Miliseconda göre belli aralıklarda Confgiuraton DB'ye istek atıp,
son halini distributed Redis Cachea kayıtları atar.

Bu projede Hangfire Job perform edilmiştir ve istenilen zaman aralıklarına göre Joblar execute edilir.
Dashboard üzerinden de joblar monitoring edilebilir.

# ServiceA ve ServiceB

Bu 2 proje de bağımsız projeler olup CommonService ile haberleşir ve bu servisler üzerinden Configuration CRUD operasyonlarını gerçekleştirir.
Sadece kendi ApplicationName isimlerine göre confgiuration kaydı atıp listeleyebilirler.
Ayrıca her iki proje de ConfigurationSource projesine event publish edip Job başlatabilir.




TDD ve Unit Testler Eklenecektir
