# 📚 IT Support Knowledge Base

**Entwicklung einer lokalen, schlanken Wissensdatenbank für den IT-Support** zur effizienten Bearbeitung häufig auftretender Störungen und Tickets – komplett lokal, ohne Cloud-Abhängigkeiten.

---

## 🚀 Projektziele (project objectives)
-DE-
- **Strukturierung statt Excel-Chaos**: Ablösung unübersichtlicher Excel-Listen durch eine durchsuchbare SQLite-Datenbank.
- **Schneller Zugriff**: Sofortige Verfügbarkeit relevanter Lösungen für wiederkehrende IT-Störungen.
- **Proof-of-Concept & Showcase**: Demonstration technischer Eigeninitiative und Teamorientierung.
- **Kostenfrei & lokal**: Keine Azure/AWS-Kosten, rein lokal betreibbar.
-EN-
- **Structure instead of Excel chaos**: Replacement of confusing Excel lists with a searchable SQLite database.
- **Fast access**: Immediate availability of relevant solutions for recurring IT problems.
- **Proof of concept & showcase**: Demonstration of technical initiative and team orientation.
- **Free & local**: No Azure/AWS costs, can be operated purely locally.
---

## 🧱 Architekturüberblick (Architecture overview)

### ⚙️ Backend – ASP.NET Core Web API (.NET 8)
- **ORM**: Entity Framework Core mit SQLite-Provider
- **Datenbank**: Dateibasiertes SQLite mit optionaler FTS5-Volltextsuche
- **API**: RESTful Endpunkte für CRUD-Operationen (Articles)
- **CORS**: Standardmäßig für `http://localhost:3000` bzw. Vite-Port erlaubt
- **Authentifizierung**: Lokal deaktiviert (kann später per Microsoft Entra ID ergänzt werden)

### 🖥️ Frontend – React + Vite
- **Framework**: React (abzüglich CRA; Vite für schnellen Start)
- **Komponenten**:
  - Suchleiste mit Fetch-API
  - Artikelliste mit Auswahl
  - Detailansicht der ausgewählten Lösung
- **Styles**: Inline / CSS-Module – minimal gehalten, einfach erweiterbar

### 🐳 Container (optional)
- **Docker & Docker Compose**:
  - `backend/`-Service mit ASP.NET Core
  - `frontend/`-Service mit Vite Dev Server
  - Gemeinsamer Netzwerkzugriff via Docker Compose

---

## 🚀 Quick-Start

### 1️⃣ Repository klonen
```bash
git clone https://github.com/lazar-iliev-dev/IT-Knowledge-Database.git
cd KnowledgeDatabase
```

### 2️⃣ Backend lokal starten
```bash
cd backend
dotnet restore
dotnet ef migrations add InitialCreate    # nur einmalig
dotnet ef database update
dotnet run --urls "http://localhost:5249"
```

- **Swagger UI**: `http://localhost:5249/swagger

### 3️⃣ Frontend lokal starten (in pogress)
```bash
cd frontend
npm install
npm run dev
```

- **App im Browser**: http://localhost:5173

### 4️⃣ Docker-Variante (in progress)
```bash
docker-compose up --build
```

---

## 📂 Verzeichnisstruktur (directory structure)

```
KnowledgeDatabase/
├── backend/               # ASP.NET Core Web API
│   ├── Controllers/       # API-Endpoints
│   ├── Data/              # DbContext & Migrations
│   ├── Models/            # Domänenklassen (Article, User, ...)
│   ├── Program.cs         # Middleware & Startup
│   └── KnowledgeApi.csproj
├── frontend/              # React + Vite Projekt
│   ├── src/               # Quellcode: App.jsx, Components...
│   ├── public/            # statische Assets
│   ├── package.json
│   └── vite.config.js
├── docker-compose.yml     # Optionale Container-Konfiguration
└── README.md              # Dieses Dokument
```

---

## ✨ Geplante Erweiterungen (Planned expansions)

-DE-
- **Tags & Kategorien**: Filtern und Kategorisieren von Artikeln
- **Markdown-Editor**: Rich-Text-Bearbeitung und Vorschau
- **Benutzerverwaltung**: Rollenbasiertes System (Editor, Viewer)
- **Power Automate Demo**: Beispiel-Flow (Artikel-Publishing → Teams-Notification)
- **Export-Funktion**: Artikel als PDF oder CSV herunterladen
- EN -
- **Tags & categories**: Filter and categorize articles
- **Markdown editor**: Rich text editing and preview
- **User management**: Role-based system (editor, viewer)
- **Power Automate demo**: Sample flow (article publishing → Teams notification)
- **Export function**: Download articles as PDF or CSV