# Shipments System

Aplikacja webowa do zarządzania przesyłkami, zrealizowana w technologii **ASP.NET Core** w architekturze **API + MVC**.  
System obsługuje różne role użytkowników i pełny cykl życia przesyłki.

---

## Opis projektu

Celem projektu jest stworzenie systemu umożliwiającego zarządzanie przesyłkami pomiędzy klientami, kurierami oraz administratorem.  
Aplikacja została podzielona na dwie główne części:

- **Shipments.Api** – REST API odpowiedzialne za logikę biznesową i dostęp do bazy danych
- **Shipments.Mvc** – aplikacja MVC pełniąca rolę interfejsu użytkownika

Komunikacja pomiędzy MVC a API odbywa się za pomocą `HttpClient`.

---

## Architektura

Projekt wykorzystuje architekturę warstwową:

- warstwa prezentacji (MVC)
- warstwa aplikacyjna (API Controllers + Services)
- warstwa domenowa (Entities, Enums, Rules)
- warstwa danych (Entity Framework Core)

W projekcie zastosowano:
- REST API
- DTO (Data Transfer Objects)
- Dependency Injection
- Middleware do obsługi wyjątków

---

## Role użytkowników

### Client
- rejestracja i logowanie
- tworzenie przesyłek
- przegląd własnych przesyłek
- anulowanie przesyłki w statusie `Created`
- podgląd historii zdarzeń

### Courier
- dostęp tylko do przypisanych przesyłek
- zmiana statusów przesyłki zgodnie z regułami biznesowymi
- brak możliwości edycji przesyłek zakończonych (`Delivered`, `Canceled`)

### Admin
- dostęp do wszystkich przesyłek
- przypisywanie i zmiana kuriera
- zmiana statusów przesyłek
- brak możliwości zmiany kuriera po zakończeniu przesyłki
- wymuszona zmiana hasła przy pierwszym logowaniu

---

## Statusy przesyłek

Dostępne statusy:

- Created
- PickedUp
- OutForDelivery
- Delivered
- DeliveryFailed
- Canceled

Przejścia pomiędzy statusami są kontrolowane centralnie w logice biznesowej i nie mogą być omijane z poziomu UI.

---

## Baza danych i ORM

- SQLite
- Entity Framework Core
- Mapowanie relacyjne:
  - Shipment → ShipmentEvents (1:N)
  - Shipment → Client (N:1)
  - Shipment → Courier (N:1)

Projekt wykorzystuje ORM do:
- operacji CRUD
- filtrowania
- sortowania
- relacji pomiędzy encjami

---

## Bezpieczeństwo

- ASP.NET Identity
- JWT (API)
- Cookies Authentication (MVC)
- Autoryzacja oparta o role
- Ochrona dostępu do danych użytkowników
- Wymuszenie zmiany hasła administratora

---

## Seed administratora

Administrator tworzony jest automatycznie przy starcie API.  
Dane logowania nie są zapisane w kodzie źródłowym, lecz w **User Secrets**.

dotnet user-secrets init
dotnet user-secrets set "AdminSeed:Email" "admin@test.com"
dotnet user-secrets set "AdminSeed:Password" "TestAdmin123!"

Po pierwszym logowaniu admin musi zmienić hasło.


