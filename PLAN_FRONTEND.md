# Plan Frontend — Angular + PrimeNG

## Stack
- Angular 21 (standalone components, signals)
- PrimeNG v19 (Aura dark theme)
- CSS (bez SCSS)
- Lazy loading per feature
- BehaviorSubject w serwisach (state management)
- Język UI: polski
- Responsywność: mobile-first

## Fazy

### Phase 1: Fundament
**Cel:** Skonfigurować projekt, PrimeNG, dark theme, CORS w backendzie, strukturę folderów.

1. Zainstalować PrimeNG v19 + PrimeIcons + PrimeFlex
2. Skonfigurować Aura dark theme w `styles.css`
3. Ustawić globalny font (sans-serif)
4. Dodać CORS w backendzie (`Program.cs`) dla `http://localhost:4200`
5. Stworzyć strukturę folderów:
   ```
   src/app/
   ├── core/              (guards, interceptors, serwisy globalne)
   │   ├── guards/         (auth.guard.ts)
   │   ├── interceptors/   (auth.interceptor.ts, error.interceptor.ts)
   │   └── services/       (auth.service.ts, toast.service.ts)
   ├── shared/            (współdzielone komponenty)
   │   └── components/     (navbar, sidebar)
   ├── features/
   │   ├── auth/           (login, register)
   │   ├── flats/          (lista flatów, tworzenie)
   │   ├── flat-detail/    (dashboard, chores, payments, notes, tenants, settings)
   │   └── profile/        (profil usera)
   └── models/             (interfejsy TS — Flat, Tenant, Chore, Payment, Note)
   ```
6. Stworzyć environment files (`environment.ts`, `environment.prod.ts`) z `apiUrl`

---

### Phase 2: Auth (Login / Register)
**Cel:** Landing page z formularzem login/register, JWT w localStorage, auth guard.

1. Stworzyć layout komponent dla auth (`AuthLayoutComponent`):
   - Lewa strona: grafika domku (placeholder na razie)
   - Prawa strona: `<router-outlet>` dla login/register
2. `LoginComponent` — formularz: email, hasło. PrimeNG InputText + Password + Button
3. `RegisterComponent` — formularz: imię, nazwisko, email, hasło. Te same kontrolki
4. Pod formularzem link `routerLink` do przełączania login ↔ register
5. `AuthService`:
   - `login(email, password)` → POST `/api/auth/login` → zapisz token w localStorage
   - `register(...)` → POST `/api/auth/register` → zapisz token
   - `logout()` → usuń token, redirect do `/login`
   - `isAuthenticated()` → sprawdź czy token istnieje i nie wygasł
   - `getToken()` → zwróć token
6. `AuthInterceptor` — dodaje `Authorization: Bearer <token>` do każdego requestu
7. `ErrorInterceptor` — łapie 401 → logout + redirect do `/login`
8. `AuthGuard` — chroni route'y wymagające logowania
9. Toast (PrimeNG Toast) — przy błędzie logowania/rejestracji
10. Routing:
    - `/login` → LoginComponent (AuthLayout)
    - `/register` → RegisterComponent (AuthLayout)
    - `/` → redirect do `/login`

---

### Phase 3: Lista flatów
**Cel:** Po zalogowaniu user widzi swoje mieszkania i wszystkie mieszkania.

1. `MainLayoutComponent` — navbar na górze + `<router-outlet>` dla reszty apki
2. `NavbarComponent` (PrimeNG Toolbar):
   - Lewo: logo "Flat Flow"
   - Prawo: imię usera, przycisk "Profil", przycisk "Wyloguj"
3. `FlatsListComponent` (`/flats`):
   - Sekcja "Twoje mieszkania" — karty (PrimeNG Card) z nazwą i adresem
     - Kliknięcie → `/flats/:id`
   - Przycisk "Utwórz mieszkanie" → dialog z formularzem (CreateFlatDialog)
   - Sekcja "Wszystkie mieszkania" — karty wszystkich flatów (nazwa + miasto, bez wrażliwych danych)
     - Kliknięcie → dialog z polem na access code (JoinFlatDialog)
     - Po dołączeniu → flat przenosi się do "Twoje mieszkania"
4. `FlatService`:
   - `getMyFlats()` → GET `/api/flats/my`
   - `getAllFlats(page, pageSize)` → GET `/api/flats`
   - `createFlat(data)` → POST `/api/flats`
   - `joinFlat(accessCode)` → POST `/api/flats/join`
5. Routing:
    - `/flats` → FlatsListComponent (MainLayout, AuthGuard)

---

### Phase 4: Widok flatu — layout + dashboard
**Cel:** Sidebar nawigacja + dashboard z podsumowaniem.

1. `FlatDetailLayoutComponent` (`/flats/:id`):
   - Sidebar po lewej (PrimeNG Menu/PanelMenu):
     - Dashboard, Obowiązki, Płatności, Notatki, Mieszkańcy, Ustawienia (owner only)
   - Na mobile: hamburger menu (PrimeNG Sidebar)
   - Content area: `<router-outlet>`
2. `FlatDashboardComponent` (`/flats/:id`):
   - Karty z podsumowaniem (PrimeNG Card):
     - Ile chores do zrobienia
     - Ile nieopłaconych płatności
     - Ostatnie notatki
3. `FlatService` (rozszerzenie):
   - `getFlatById(id)` → GET `/api/flats/:id`

---

### Phase 5: Obowiązki (Chores)
**Cel:** CRUD obowiązków + assignments.

1. `ChoresListComponent` (`/flats/:id/chores`):
   - Lista/tabela obowiązków (PrimeNG Table)
   - Przycisk "Dodaj obowiązek" → dialog
   - Kliknięcie w wiersz → rozwija detail z assignments
2. `AddChoreDialogComponent` — formularz: tytuł, opis, częstotliwość (dropdown z enum)
3. `EditChoreDialogComponent` — edycja
4. Assignments inline:
   - Lista przypisań w udetailu chore'a
   - "Dodaj przypisanie" → dialog (wybór tenanta z dropdown, data)
   - Przyciski: Wykonaj / Otwórz ponownie / Usuń
5. `ChoreService`:
   - CRUD chores + CRUD assignments
6. Routing:
   - `/flats/:id/chores` → ChoresListComponent

---

### Phase 6: Płatności (Payments)
**Cel:** CRUD płatności + shares.

1. `PaymentsListComponent` (`/flats/:id/payments`):
   - Tabela z paginacją (PrimeNG Table z paginator)
   - Przycisk "Dodaj płatność" → dialog
   - Kliknięcie → detail ze shares
2. `AddPaymentDialogComponent` — tytuł, kwota, termin
3. `EditPaymentDialogComponent`
4. Shares inline:
   - Lista udziałów w detalu płatności
   - "Dodaj udział" → dialog (wybór tenanta, kwota)
   - Przyciski: Oznacz jako opłacony / częściowy / Usuń
5. `PaymentService`:
   - CRUD payments + CRUD shares
6. Routing:
   - `/flats/:id/payments` → PaymentsListComponent

---

### Phase 7: Notatki (Notes)
**Cel:** CRUD notatek.

1. `NotesListComponent` (`/flats/:id/notes`):
   - Lista z paginacją (PrimeNG DataView lub Table)
   - Przycisk "Dodaj notatkę" → dialog
   - Kliknięcie → dialog edycji
2. `AddNoteDialogComponent` — tytuł, treść (textarea)
3. `EditNoteDialogComponent`
4. `NoteService`:
   - CRUD notes
5. Routing:
   - `/flats/:id/notes` → NotesListComponent

---

### Phase 8: Mieszkańcy (Tenants)
**Cel:** Lista mieszkańców, zarządzanie (owner).

1. `TenantsListComponent` (`/flats/:id/tenants`):
   - Lista (PrimeNG Table) — imię, email, rola (owner/member)
   - Owner widzi przyciski: Promuj / Odbierz ownership / Usuń
   - Potwierdzenie akcji przez PrimeNG ConfirmDialog
2. `TenantService`:
   - `getTenantsByFlatId(flatId)`
   - `promoteTenant(flatId, tenantId)`
   - `revokeOwnership(flatId, tenantId)`
   - `removeTenant(flatId, tenantId)`
3. Routing:
   - `/flats/:id/tenants` → TenantsListComponent

---

### Phase 9: Ustawienia flatu + Profil
**Cel:** Owner może edytować flat, user może edytować swój profil.

1. `FlatSettingsComponent` (`/flats/:id/settings`) — tylko owner:
   - Formularz edycji (nazwa, adres)
   - Przycisk "Odśwież kod dostępu" (wyświetla nowy kod)
   - Przycisk "Usuń mieszkanie" (ConfirmDialog)
2. `ProfileComponent` (`/profile`):
   - Formularz: imię, nazwisko, email
   - Przycisk "Zapisz"
3. Routing:
   - `/flats/:id/settings` → FlatSettingsComponent
   - `/profile` → ProfileComponent

---

### Phase 10: Polish
**Cel:** Dopracowanie detali.

1. Loading states — PrimeNG Skeleton/ProgressSpinner przy ładowaniu danych
2. Empty states — komunikaty "Brak obowiązków", "Brak płatności" itp.
3. Walidacja formularzy — error messages pod polami (Reactive Forms + PrimeNG Message)
4. Responsywność — przetestować na mobile
5. Grafika domku na landing page (podmienić placeholder)
6. Favicon + tytuł strony "Flat Flow"
