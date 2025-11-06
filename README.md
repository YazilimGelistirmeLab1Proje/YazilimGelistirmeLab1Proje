# Operation Black Dawn: Proje Raporu
## Kocaeli Üniversitesi Teknoloji Fakültesi - Bilişim Sistemleri Mühendisliği
### 2025-2026 Güz Dönemi Yazılım Geliştirme Laboratuvar - I

---

### Ekip Üyeleri ve Görev Dağılımı
Bu proje, 3 kişilik bir ekip tarafından, net bir görev paylaşımı ve GitHub üzerinden versiyon kontrolü ile gerçekleştirilmiştir.

* **[Fatih Bilgin](https://github.com/fatihbilgin1)** - **Proje Yönetimi & Harita Tasarımı (Level Design)**
  * *Görevler:* Projenin Git/GitHub kurulumu ve yönetimi (Git LFS, .gitignore), ana oyun haritasının (askeri üs) "low-poly" modüler asset'ler kullanılarak tasarlanması, üs dışı sızma arazisinin (Terrain) oluşturulması, atmosfer ve ışıklandırmanın (URP, Gece & Sis) ayarlanması.

* **[Furkan Demirci](https://github.com/dmrcfurkan)** - **Oyuncu Mekanikleri (Player Mechanics)**
  * *Görevler:* Üçüncü şahıs bakış açılı (TPS) oyuncu karakterinin (Asker) hareket (`PlayerMovement.cs`), nişan alma ve silah (`WeaponController.cs`) mekaniklerinin kodlanması. Mermi toplama ve rehine kurtarma gibi 'E' tuşu etkileşim sisteminin geliştirilmesi.

* **[Yekta Cengiz](https://github.com/YEKTA34)** - **Yapay Zeka Mekanikleri (NPC Mechanics)**
  * *Görevler:* Düşman NPC'lerinin (Milisler) yapay zekasının "State Machine" (Durum Makinesi) deseni kullanılarak kodlanması. 'Patrol' (Devriye), 'Chase' (Takip) ve 'Attack' (Saldırı) durumlarının Unity NavMesh sistemi ile entegrasyonu. Rehinelerin oyuncuyu takip etme (`HostageAI.cs`) davranışının geliştirilmesi.

---

## İçindekiler
1.  [Giriş](#1-giriş)
    * [Projenin Amacı ve Kapsamı](#projenin-amacı-ve-kapsamı)
    * [Proje Senaryosu ve Akışı](#proje-senaryosu-ve-akışı)
2.  [Literatür Taraması](#2-literatür-taraması)
    * [Örnek Çalışma 1: Metal Gear Solid V](#örnek-çalışma-1-metal-gear-solid-v)
    * [Örnek Çalışma 2: Tom Clancy's Splinter Cell](#örnek-çalışma-2-tom-clancys-splinter-cell)
3.  [Sistem Mimarisi, Yöntemler ve Teknikler](#3-sistem-mimarisi-yöntemler-ve-teknikler)
    * [Geliştirme Ortamı ve Teknolojiler](#geliştirme-ortamı-ve-teknolojiler)
    * [Yazılımsal Mimariler ve Tasarım Desenleri](#yazılımsal-mimariler-ve-tasarım-desenleri)
    * [Sistem Şeması](#sistem-şeması)
4.  [Geliştirilen Oyun Mekanikleri (Blok Diyagramlar)](#4-geliştirilen-oyun-mekanikleri-blok-diyagramlar)
    * [Düşman Yapay Zeka Mekaniği (Devriye, Takip, Saldırı)](#düşman-yapay-zeka-mekaniği-devriye-takip-saldırı)
    * [Etkileşim Mekaniği (Mermi Toplama & Rehine Kurtarma)](#etkileşim-mekaniği-mermi-toplama--rehine-kurtarma)
    * [Görev ve Kaçış Mekaniği (2 Rehine)](#görev-ve-kaçış-mekaniği-2-rehine)
5.  [Tasarlanan Sayfalar (Harita Bölgeleri)](#5-tasarlanan-sayfalar-harita-bölgeleri)
6.  [Karşılaşılan Zorluklar ve Çözümler](#6-karşılaşılan-zorluklar-ve-çözümler)
7.  [Sonuç ve Kazanımlar](#7-sonuç-ve-kazanımlar)

---

## 1. Giriş

### Projenin Amacı ve Kapsamı
Bu projenin temel amacı, Unity oyun motoru ve Universal Render Pipeline (URP) teknolojisi kullanılarak, "Operation Black Dawn" adında, üçüncü şahıs bakış açısına (TPS) sahip, low-poly estetiğinde bir gizlilik-aksiyon oyunu prototipi geliştirmektir. Proje, Kocaeli Üniversitesi Yazılım Geliştirme Laboratuvarı I dersi kapsamında, 3 kişilik bir ekip çalışmasıyla ve Git/GitHub versiyon kontrol pratikleri uygulanarak gerçekleştirilmiştir.

### Proje Senaryosu ve Akışı
Oyun, Kuzey sınırındaki stratejik bir askeri araştırma üssünde geçmektedir. Üs, düşman milisler tarafından ele geçirilmiş ve **2** rehine üssün farklı noktalarında (örn: Hangar, Reaktör Binası) tutulmaktadır.

Oyuncu, özel bir birlik üyesi olarak üssün dışındaki bir arazide (Sızma Noktası) başlar. Rehinelerin konumları, oyuncunun taktiksel arayüzünde (HUD) **işaretli olarak görünmektedir.** Oyuncunun amacı, üsse gizlice sızmak, devriye gezen milisleri atlatmak veya etkisiz hale getirmek, iki rehineye de ulaşıp ('E' tuşu ile) onları kurtarmak ve son olarak her iki rehineyle birlikte haritada belirlenmiş **özel Çıkış Kapısı'na** ulaşmaktır. Çıkış kapısındaki tetikleyiciye (trigger) ulaşıldığında, ekranda "Tebrikler, rehineleri kurtardınız!" mesajı belirir ve oyun tamamlanır.

---

## 2. Literatür Taraması

Projemiz, TPS-Gizlilik türünde yer almaktadır. Bu türdeki referans çalışmalar incelenmiş ve kendi çalışmamızla karşılaştırılmıştır.

* **Örnek Çalışma 1: Metal Gear Solid V**
    * **Analiz:** MGSV, oyuncuya görevleri tamamlamak için muazzam bir özgürlük sunan, dinamik yapay zekaya sahip bir açık dünya oyunudur. Düşmanların görüş konileri ve alarm durumları, gizlilik türünün zirve noktalarındandır.
    * **Karşılaştırma:** "Operation Black Dawn", MGSV'nin aksine açık bir dünya yerine, tek ve odaklanmış bir askeri üs haritası sunar. Yapay zeka, MGSV'deki gibi dinamik ve karmaşık olmak yerine, "low-poly" estetiğine uygun olarak daha net kurallara (Patrol/Chase/Attack durumları) dayalı bir "State Machine" (Durum Makinesi) deseni ile tasarlanmıştır.

* **Örnek Çalışma 2: Tom Clancy's Splinter Cell**
    * **Analiz:** Splinter Cell serisi, ışık ve gölgeyi temel bir gizlilik mekaniği olarak kullanır. Oyuncunun karanlıkta saklanması oynanışın temelini oluşturur.
    * **Karşılaştırma:** Bizim projemiz, low-poly stilin doğası gereği karmaşık ışık-gölge hesaplamaları yerine, düşmanların sabit bir "görüş alanı" (vision cone) trigger'ı ile oyuncuyu algılamasına odaklanır. Atmosfer, URP'nin sis ve gece efektleri ile sağlanmıştır.

---

## 3. Sistem Mimarisi, Yöntemler ve Teknikler

### Geliştirme Ortamı ve Teknolojiler
* **Oyun Motoru:** Unity (Ekip Standardı Sürüm: `6000.2.7f2`)
* **Grafik Hattı (Renderer):** Universal Render Pipeline (URP)
* **Kodlama Dili:** C#
* **Versiyon Kontrol:** Git, GitHub ve **Git LFS** (Büyük proje dosyalarını yönetmek için zorunlu olarak kullanılmıştır).
* **AI Navigasyonu:** Unity NavMesh Sistemi

### Yazılımsal Mimariler ve Tasarım Desenleri

* **1. Bileşen Tabanlı Mimari (Component-Based):** Unity'nin temel felsefesine uygun olarak, her oyun nesnesi (GameObject) bağımsız işlevlere sahip bileşenlerin (script'lerin) birleşimi olarak tasarlanmıştır. (Örn: Oyuncu için `PlayerMovement.cs` ve Düşman için `EnemyAI.cs` gibi).
* **2. Durum Makinesi Deseni (State Machine Pattern):** Düşman yapay zekasının (AI) davranışlarını yönetmek için kullanılmıştır. AI'ın `PATROL` (Devriye), `CHASE` (Takip) ve `ATTACK` (Saldırı) gibi 3 net durumu ve bu durumlar arası geçiş mantığı kodlanmıştır.
* **3. Singleton (Tekil Nesne) Deseni:** Oyundaki genel durumu (kurtarılan rehine sayısı: **2**) yöneten bir `GameManager` script'i için kullanılmıştır. Bu sayede sahnedeki herhangi bir script, `GameManager.Instance` üzerinden oyunun genel durumuna erişebilmiştir.

### Sistem Şeması

Aşağıdaki şema, projedeki ana sistemlerin ve script'lerin birbiriyle olan ilişkisini özetlemektedir:

`![Proje Sistem Şeması](./images/sistem-semasi.png)`

* **Şema Açıklaması:** `Input System` girdileri `PlayerController`'ı tetikler. `PlayerController` ise `WeaponController` (ateş etme) ve `InteractionManager` ('E' tuşu) ile konuşur. `GameManager`, kurtarılan rehine sayısını takip eder ve `UIManager`'a bilgi vererek arayüzü günceller. `EnemyAI` ve `HostageAI`, `NavMesh System`'i kullanarak hareket eder.

---

## 4. Geliştirilen Oyun Mekanikleri (Blok Diyagramlar)

### Düşman Yapay Zeka Mekaniği (Devriye, Takip, Saldırı)
Bu diyagram, bir düşman NPC'sinin (Milis) oyuncuyu algıladığında hangi durumlara girdiğini göstermektedir:

`![Düşman AI Akış Şeması](./images/blok-diyagrami.png)`

* **Açıklama:** Düşman, "Patrol" durumunda sakince devriye gezer. Oyuncuyu "Görüş Konisinde" algılarsa, "Chase" durumuna geçer ve oyuncuyu kovalar. Oyuncu "Ateş Etme Menziline" girerse, "Attack" durumuna geçerek ateş eder. Eğer oyuncu menzilden veya görüş alanından çıkarsa, AI tekrar "Chase" veya "Patrol" durumlarına geri döner.

### Etkileşim Mekaniği (Mermi Toplama)
* **Açıklama:** `Projedeki temel etkileşim ('E' tuşu kullanımı), mermi toplama mekaniği için yapılandırılmıştır. InteractionManager script'i, oyuncu 'E' tuşuna bastığında tetiklenir. Eğer oyuncu, etkileşim menzilinde bir "Mermi Kutusu" objesi algılarsa (OnTriggerStay ve Input.GetKeyDown('E') kontrolleri ile), WeaponController'a mermi eklenir ve mermi kutusu sahneden yok edilir. Rehine kurtarma, bu 'E' tuşu sisteminden bağımsız çalışmaktadır.

### Görev ve Kaçış Mekaniği (2 Rehine)
* **Açıklama:** `Oyunun ana görev akışı GameManager (Singleton) tarafından yönetilir. Kurtarılması gereken rehine sayısı 2 olarak belirlenmiştir (GameManager.rehineSayisi = 2). Oyuncu bir rehinenin yanına yaklaştığında (rehinenin etrafındaki Collider'a bir trigger olarak girdiğinde), 'E' tuşuna basmaya gerek kalmadan rehine otomatik olarak kurtarılır. Kurtarma anında, GameManager'daki kurtarılan rehine sayısı +1 artar ve rehinenin HostageAI script'i "Takip" durumuna geçerek oyuncuyu takip etmeye başlar. Kurtarılan rehine sayısı 2'ye ulaştığında (GameManager.rehineSayisi == 2), "Çıkış Kapısına Git" görevi arayüzde belirir. Oyuncu (ve onu takip eden 2 rehine) haritadaki Çıkış Kapısı'nda bulunan tetikleyiciye (trigger) girdiğinde, UIManager "Tebrikler, rehineleri kurtardınız!" mesajını gösterir ve oyun tamamlanır.

---

## 5. Tasarlanan Sayfalar (Harita Bölgeleri)

Projemizdeki tüm oyun, "OperationBlackDawn" adlı tek bir Unity Sahnesi (Scene) içinde geçmektedir. Bu sahne, oyuncuya farklı deneyimler sunmak ve görev akışını desteklemek için **Harita Tasarımcısı (Level Designer)** tarafından aşağıdaki tematik bölgelere ayrılmıştır:

* **Bölge A: Sızma Alanı (Üs Dışı):** Unity Terrain aracıyla tasarlanmış, oyuncunun oyuna başladığı engebeli, kayalık ve sisli dış arazi. Üsse giriş için siper ve gizlilik imkanı sunar.
* **Bölge B: Ana Üs (Garaj, Koğuş ve Konteyner Alanı):** Modüler asset'ler kullanılarak tasarlanan, devriyelerin (NPC) yoğun olduğu ana avlu. Siper alınabilecek çok sayıda konteyner, askeri araç, koğuş, garaj ve mühimmat deposu (Mermi Toplama) içerir.
* **Bölge C: Hangarlar ve Reaktör Binası (Rehine Mekanları):** **2 Rehinemizin** tutulduğu ve kurtarma operasyonunun odak noktası olan kilitli iç mekanlar. Bu bölgeler, oyuncunun çatışmaya girmesini veya gizlice ilerlemesini gerektirecek şekilde tasarlanmıştır.
* **Bölge D: Çıkış Kapısı (Kaçış Noktası):** Haritanın özel bir noktasında bulunan ve 2 rehine kurtarıldıktan sonra aktif hale gelen son bölge. Oyuncunun buraya ulaşmasıyla görev tamamlanır.

---

## 6. Karşılaşılan Zorluklar ve Çözümler

Geliştirme sürecinde, özellikle ekip çalışması ve proje yönetimi konularında kritik zorluklarla karşılaşılmıştır:

* **Zorluk 1: Proje Dosyalarının Bozulması (OneDrive Senkronizasyonu)**
    * **Problem:** Projenin başlangıçta bir `OneDrive` klasörü içinde geliştirilmesi, bulut eşitlemesinin Unity'nin `Library` ve `.asset` dosyalarını kilitleyerek bozmasına (corrupt) neden oldu. `Failed to load... File may be corrupted` hataları alınmaya başlandı.
    * **Çözüm:** Sorunun kaynağının bulut eşitleme olduğu tespit edildi. Proje, OneDrive ile senkronize **olmayan** (`C:\...` gibi) lokal bir dizine taşındı. Bozuk proje dosyaları terk edilerek, sağlam bir yedek üzerinden bu güvenli alanda çalışmaya devam edildi.

* **Zorluk 2: Ekip İçi Unity Sürüm Uyuşmazlığı**
    * **Problem:** Ekip üyelerinden birinin projeyi, projenin ana sürümünden (`6000.0.58f2`) daha yeni bir sürümle (`6000.2.7f2`) açması, proje ayar dosyalarının (`.asset`) geri dönülemez şekilde yükseltilmesine neden oldu. Bu dosyalar GitHub'a yüklendiğinde, eski sürümü kullanan diğer ekip üyeleri `...serialized with a newer version` hatası alarak projeyi açamadı.
    * **Çözüm:** Ekip içi iletişimin kritik önemi anlaşıldı. Tüm ekibin **birebir aynı editör sürümünü** (`6000.2.7f2`) kullanması kararlaştırıldı. Herkes Unity Hub üzerinden bu ortak sürümü yükleyerek sürüm çakışması kalıcı olarak çözüldü.

* **Zorluk 3: GitHub 100MB Dosya Boyutu Sınırı (Git LFS)**
    * **Problem:** Projenin GitHub'a yüklenmesi sırasında, yüksek çözünürlüklü Doku (`.png`) ve Aydınlatma Verisi (`.asset`) dosyalarının GitHub'ın 100MB'lık dosya boyutu sınırını aşması (`GH001: Large files detected` hatası).
    * **Çözüm:** Projeyi Git LFS (Large File Storage) kullanacak şekilde yeniden yapılandırma kararı alındı. Proje kök dizinine, Unity'nin geçici dosyalarını (örn: `Library/`) görmezden gelmesi için bir **`.gitignore`** dosyası eklendi. Ayrıca, büyük dosya uzantılarını (`*.png`, `*.asset`, `*.fbx` vb.) LFS'e yönlendirmek için bir **`.gitattributes`** dosyası oluşturuldu. Bu sayede proje sorunsuz bir şekilde versiyon kontrolüne alındı.

* **Zorluk 4: Render Pipeline Uyumsuzluğu (Pembe Hata)**
    * **Problem:** Asset Store'dan indirilen bazı 3D modellerin (Built-in) projemizin URP (Universal Render Pipeline) kullanması nedeniyle materyallerinin tanınmaması ve pembe/mor görünmesi.
    * **Çözüm:** Unity'nin `Window > Rendering > Render Pipeline Converter` aracı kullanılarak, proje genelindeki tüm Built-in materyallerin otomatik olarak URP'ye yükseltilmesi (upgrade) sağlandı.

---

## 7. Sonuç ve Kazanımlar

Bu proje, Yazılım Geliştirme Laboratuvarı I dersinin hedefleri doğrultusunda başarıyla tamamlanmış ve 2 rehineli görev akışına sahip fonksiyonel bir TPS-Gizlilik oyunu prototipi ortaya çıkarılmıştır.

Projenin bize kattığı temel faydalar (kazanımlar) şunlardır:
* **Teknik Kazanımlar:**
    * Unity oyun motoru ve URP üzerinde yetkinlik kazanma.
    * C# dili ile Bileşen Tabanlı mimari ve yaygın Tasarım Desenleri'ni (State Machine, Singleton) kullanarak oyun mekanikleri (AI, Player Controller, Interaction) kodlama becerisi.
    * Low-poly harita tasarımı (Terrain, modüler asset yerleşimi) ve atmosfer (ışık, sis) oluşturma.
* **Profesyonel Kazanımlar:**
    * Bir yazılım projesinde **versiyon kontrolünün (Git/GitHub)** ne kadar hayati olduğunu yaşayarak öğrenme.
    * **Git LFS** gibi araçları kullanarak büyük boyutlu projeleri yönetebilme.
    * Ekip içinde **sürüm standardizasyonunun** (herkesin aynı Unity sürümünü kullanması) ve **görev dağılımının** önemini kavrama.
    * Karşılaşılan teknik sorunlara (dosya bozulması, sürüm çakışması) karşı sistematik hata ayıklama (debugging) ve çözüm üretme yeteneği geliştirme.
