## Instrukcja uruchomienia 

### Jak uruchomić?
1. Otwórz projekt w IDE
2. Uruchom profil: **TransportApi: https** (lub http)
3. Wejdź w przeglądarce na: <br>
https://localhost:{PORT}/swagger <br>
W moim przypadku -> https://localhost:7187/swagger/index.html <br>

## Opis funkcji

| Funkcja                      | Metoda | Endpoint                    | Opis                                          |
| ---------------------------- | ------ | --------------------------- | --------------------------------------------- |
| Pobierz listę pojazdów       | GET    | `/api/vehicles`             | Zwraca wszystkie pojazdy                      |
| Dodaj nowy pojazd            | POST   | `/api/vehicles`             | Dodaje pojazd do floty                        |
| Pobierz listę kierowców      | GET    | `/api/drivers`              | Zwraca wszystkich kierowców                   |
| Dodaj kierowcę               | POST   | `/api/drivers`              | Rejestruje kierowcę                           |
| Utwórz zlecenie transportowe | POST   | `/api/orders`               | Tworzy zlecenie (pojazd + kierowca + ładunek) |
| Pobierz zlecenia             | GET    | `/api/orders`               | Zwraca wszystkie aktywne zlecenia             |
| Zakończ zlecenie             | PUT    | `/api/orders/{id}/complete` | Oznacza zlecenie jako zakończone              |

## Przykładowe Posty
### Dodawanie pojazdu

**POST** `/api/vehicles`

```json
{
  "type": "truck",
  "registrationNumber": "KRA 34241",
  "model": "Renault",
  "trailerLength": 15
}
```
### Dodawanie kierowcy

**POST** `/api/drivers`
```json
{
"firstName": "Szymon",
"lastName": "Bastaa"
}
```

### Dodawanie zlecenia

**POST** `/api/orders`
```json
{
  "vehicleId": 1,
  "driverId": 1,
  "cargoDescription": "Elektronarzędzia"
}
```
