# Interactive-Safe-System
Interactive Safe & Vault System
Oyun içi etkileşimli, şifre korumalı ve deneme haklarına (attempts) sahip kasa yönetim sistemi.

Özellikler:

Merkezi Raycast Uyumu: Kasa kendi başına bir Update döngüsü veya fiziksel ışın atma (Raycast) işlemi kullanmaz. Modüler ITargetable arayüzünü kullanarak oyunun ana oyuncu kontrolcüsü tarafından tetiklenir. Performans kaybı sıfırdır.

Event-Driven Tasarım: Doğru/yanlış şifre girilmesi durumunda koda gömülü ses veya ışık yönetimi kullanmak yerine UnityEvent altyapısı sunar. Böylece yanlış şifre girildiğinde odayı karanlığa gömmek (LightManager.TriggerErrorEffect) Inspector üzerinden tek tıkla ayarlanabilir.

Save/Load Hazır: Kasanın açık, kapalı veya kalıcı olarak kilitli (Bloke) olma durumları LoadState metodu ile dışarıdan doğrudan enjekte edilebilir.

Kurulum:

Kasanın 3D modeline SafeController ve SafeUIController scriptlerini atayın.

Kasanızın UI Canvas objesini SafeCanvas ismiyle CanvasManager'a tanıtın.

Inspector'da yer alan On Wrong Code eventine ortamdaki LightManager.TriggerErrorEffect fonksiyonunuzu sürükleyerek hata gerilimini artırın.
