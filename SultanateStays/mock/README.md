# Mock veriler

RapidAPI her çağrıyı abonelik kotasından düşer. Sayfayı her yenilediğimizde istek atarsak kota hızla biter.

Kota bitmesin diye `mock` klasörünü oluşturduk. Bu klasördeki dosyalar RapidAPI'ye istek atmaz; yalnızca sayfayı yenilediğimizde RapidAPI'ye istek atılmasını engellemek için kullanılır.

## Dosyalar

- `destination.json`: Search Hotel Destination yanıtı (`query=Milano`)
- `hotels.json`: Search Hotels yanıtı (Milano, EUR, 2 yetişkin)
- `details.json`: Get Hotel Details yanıtı (`hotel_id=191605`)

Bu dosyalar API yanıtının birebir kopyasıdır; içlerine yorum ya da ek metin eklenmez.
