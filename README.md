# Sistema de Gestión, Control y Trazabilidad de Recursos
### Oficina Scout de Leonardo Murialdo N.° 1 - Archidona

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-22-DD0031?logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)
![Status](https://img.shields.io/badge/Status-En%20Desarrollo-success)

---

## 📌 1. Descripción del Proyecto

El **Sistema de Gestión, Control y Trazabilidad de Recursos** es una plataforma web integral diseñada para la **Oficina Scout de Leonardo Murialdo N.° 1 - Archidona**. Su propósito principal es administrar, supervisar y auditar todo el ciclo de vida de los bienes institucionales (mobiliario, equipos tecnológicos, material scout, equipo de campamento, papelería y suministros).

El sistema resuelve la problemática de inventarios desactualizados, pérdidas de equipos, falta de control en préstamos y ausencia de un historial detallado de mantenimientos y bajas.

---

## 🏗️ 2. Arquitectura y Stack Tecnológico

El proyecto está diseñado bajo una arquitectura limpia en capas (**Clean Architecture / Modular Monolith**) con separación total de responsabilidades:

- **Frontend**: [Angular](https://angular.dev/) + [Angular Material](https://material.angular.io/) (SPA reactiva, diseño moderno y responsivo).
- **Backend**: [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/) (.NET 10 en C#).
- **ORM & Persistencia**: [Entity Framework Core](https://learn.microsoft.com/ef/core/) con migraciones y soporte para SQL Server.
- **Base de Datos**: [Microsoft SQL Server 2022](https://www.microsoft.com/sql-server).
- **Autenticación y Seguridad**: ASP.NET Core Identity + Tokens JWT (JSON Web Tokens).
- **Contenedores**: Docker (compilación multi-etapa) y orquestación con [Docker Compose](https://docs.docker.com/compose/) (`compose.yaml`).
- **Integración Continua (CI)**: GitHub Actions (`.github/workflows/ci.yml`).

---

## 📦 3. Módulos y Funcionalidades

| Módulo | Descripción |
| :--- | :--- |
| **🔐 Autenticación y Usuarios** | Login con JWT, recuperación de contraseña, gestión de usuarios, asignación de roles y perfiles institucionales. |
| **📋 Catálogo de Recursos** | Registro de activos con código institucional, fotos, categoría, ubicación, custodio, estado físico y valor contable. |
| **🤝 Préstamos y Devoluciones** | Flujo de solicitudes, entrega, control de fechas de retorno y recepción de materiales. |
| **🔧 Mantenimientos** | Registro y seguimiento de mantenimientos preventivos y correctivos, historial de talleres y costos asociados. |
| **🔍 Inventario Físico** | Levantamiento de tomas físicas de inventario, auditoría ciega y conciliación contra existencias en sistema. |
| **📉 Pérdidas y Bajas** | Gestión de actas formales de desincorporación, reporte de pérdidas, hurtos o deterioro irreparable. |
| **📊 Trazabilidad y Kardex** | Registro inmutable de cada movimiento y auditoría (`AuditLogs`) para cada recurso. |
| **📈 Reportes y Exportación** | Reportes consolidados, métricas de inventario y exportación a formatos estándar (Excel/PDF). |

---

## 👥 4. Roles y Permisos

El sistema implementa control de acceso basado en roles (**RBAC**):

* **`ADMIN`**: Control total del sistema, configuración, usuarios, parámetros globales y auditoría.
* **`SUPERINTENDENTE`**: Supervisión general, consulta y aprobación de inventarios y reportes.
* **`JEFE_GRUPO`**: Gestión y aprobación de préstamos, eventos scouts y recursos asignados.
* **`CUSTODIO`**: Responsable del resguardo físico de bienes en ubicaciones específicas (bodega, salas).
* **`SOLICITANTE`**: Miembro scout o dirigente habilitado para solicitar préstamos de material.
* **`AUXILIAR`**: Apoyo en tomas físicas de inventario y registro de movimientos cotidianos.

---

## 📁 5. Estructura del Repositorio

```text
ScoutAsset/
├── .github/
│   └── workflows/
│       └── ci.yml                 # Pipeline CI en GitHub Actions
├── docs/                          # Documentación técnica, modelo de dominio y requisitos
│   ├── DOMAIN_MODEL.md
│   ├── PLAN_PROYECTO.md
│   └── REQUISITOS.md
├── ScoutAsset.Server/             # Proyecto Backend ASP.NET Core Web API
│   ├── Application/               # Lógica de aplicación, DTOs e interfaces
│   ├── Controllers/               # Controladores API REST
│   ├── Domain/                    # Entidades del dominio de negocio
│   ├── Infrastructure/            # Persistencia, EF Core, Identity y servicios
│   ├── appsettings.json           # Configuración base del backend
│   └── Program.cs                 # Punto de entrada e inicialización (Seeds)
├── scoutasset.client/             # Proyecto Frontend Angular
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/              # Guards, interceptores JWT y servicios base
│   │   │   ├── features/          # Módulos de negocio (recursos, préstamos, etc.)
│   │   │   ├── layout/            # Sidebar, header y navegación
│   │   │   └── shared/            # Componentes y diálogos reutilizables
│   ├── angular.json
│   └── package.json
├── .dockerignore                  # Reglas de exclusión para Docker
├── .env.example                   # Plantilla de variables de entorno
├── .gitignore                     # Reglas de exclusión para Git
├── compose.yaml                   # Orquestación de Docker Compose (normativa estándar)
├── Dockerfile                     # Construcción multi-etapa (Angular + ASP.NET Core)
├── update.sh                      # Script automatizado de actualización y merge
└── README.md                      # Documentación del proyecto
```

---

## ⚙️ 6. Variables de Entorno

Toda la configuración sensible se gestiona de forma segura a través de variables de entorno, evitando credenciales en el repositorio:

1. Crea tu archivo `.env` a partir de la plantilla:
   ```bash
   cp .env.example .env
   ```

2. Configura los parámetros en `.env`:
   ```env
   # ASP.NET Core
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_HTTP_PORTS=8080

   # Conexión SQL Server (desde red infra: sqlserver22_db,1433)
   ConnectionStrings__DefaultConnection=Server=sqlserver22_db,1433;Database=ScoutInventory;User Id=sa;Password=TuPasswordSeguro!;TrustServerCertificate=True;MultipleActiveResultSets=true

   # Autenticación JWT (mínimo 32 caracteres)
   Jwt__Key=TuClaveSecretaDeAlMenos32CaracteresDeLongitud!
   Jwt__Issuer=ScoutInventory.Api
   Jwt__Audience=ScoutInventory.Client

   # Correo SMTP (Gmail / Servidor de Correo)
   Smtp__Host=smtp.gmail.com
   Smtp__Port=587
   Smtp__EnableSsl=true
   Smtp__Username=tu-correo@gmail.com
   Smtp__Password=tu-password-de-aplicacion
   Smtp__FromAddress=tu-correo@gmail.com
   Smtp__FromName=Scout Inventory System

   # Semilla del Usuario Administrador Inicial
   AdminSeed__UserName=admin
   AdminSeed__Email=admin@scoutinventory.com
   AdminSeed__Password=TuClaveAdmin123!
   ```

---

## 🚀 7. Despliegue con Docker en Servidor

El despliegue en producción se realiza con Docker Compose utilizando la red interna **`infra`** para conectar la aplicación con la base de datos `sqlserver22_db`:

### Paso a paso:

```bash
# 1. Clonar el repositorio (rama develop o main)
git clone -b develop git@github.com:millanda29/SistemaScoutLM1.git
cd SistemaScoutLM1

# 2. Configurar variables de entorno
cp .env.example .env
nano .env

# 3. Asegurar la red Docker 'infra'
docker network inspect infra >/dev/null 2>&1 || docker network create infra

# 4. Compilar y levantar la aplicación
docker compose up -d --build
```

La aplicación estará disponible inmediatamente en el puerto `8080`:
- **Web App (Frontend)**: `http://tu-servidor:8080`
- **API REST**: `http://tu-servidor:8080/api`

---

## 🔀 8. Flujo de Trabajo y Política de Pull Requests (PRs)

> ⚠️ **Regla de Oro**: Ningún cambio se sube directamente a `main`. Todo código nuevo, corrección o funcionalidad debe pasar obligatoriamente por un **Pull Request (PR)** con revisión previa.

### Estrategia de Ramas:
1. **`feature/nombre-tarea` o `fix/nombre-bug`**: Ramas de trabajo creadas a partir de `develop`.
2. **`develop`**: Rama de integración continua y pruebas (staging/QA). Los desarrolladores abren un PR hacia `develop`.
3. **`main`**: Rama protegida de producción. Solo recibe cambios mediante PR aprobado desde `develop`.

```text
[feature/mi-funcionalidad] ──(PR)──> [develop] ──(PR)──> [main] (Producción)
```

---

## 🔄 9. Automatización de Actualizaciones en Servidor (`update.sh`)

Una vez que un Pull Request ha sido revisado y fusionado en GitHub hacia la rama correspondiente, puedes actualizar el servidor y redesplegar los contenedores ejecutando:

```bash
# Desplegar la rama actual (o main por defecto en producción):
./update.sh

# O desplegar una rama específica (por ejemplo, para pruebas en develop):
./update.sh develop
```

El script ejecuta automáticamente el siguiente flujo seguro:
1. Verifica que no haya modificaciones locales sin commitear.
2. Descarga las referencias del repositorio (`git fetch origin`).
3. Sincroniza la rama destino con el código aprobado (`git pull origin <rama>`).
4. Valida la existencia de `.env` y de la red Docker `infra`.
5. Reconstruye y levanta los servicios en segundo plano con `docker compose up -d --build`.

---

## 💻 10. Ejecución en Entorno Local de Desarrollo

Si deseas trabajar en desarrollo local sin Docker:

### Requisitos Previos:
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js 20+ y npm](https://nodejs.org/)
- [Angular CLI](https://angular.dev/tools/cli): `npm install -g @angular/cli`
- Instancia de SQL Server accesible

### 1. Backend:
```bash
cd ScoutAsset.Server
dotnet restore
dotnet run
```
*El backend escuchará en `http://localhost:5003` y `https://localhost:7100`.*

### 2. Frontend:
```bash
cd scoutasset.client
npm install
npm start
```
*El cliente Angular se levantará en `https://localhost:49698` con proxy configurado hacia la API.*

---

## 🌱 11. Datos Semilla Iniciales (Seed Data)

Al iniciar la aplicación por primera vez, el sistema inicializa automáticamente:

* **Usuario Administrador**:
  - **Usuario**: `admin` (o el valor de `AdminSeed__UserName`)
  - **Email**: `admin@scoutinventory.com` (o el valor de `AdminSeed__Email`)
  - **Contraseña inicial**: `Admin123!` (o el valor de `AdminSeed__Password`)
  - **Rol**: `ADMIN`
* **Roles del sistema**: `ADMIN`, `SUPERINTENDENTE`, `JEFE_GRUPO`, `CUSTODIO`, `SOLICITANTE`, `AUXILIAR`.
* **Categorías por defecto**: Mobiliario (`MOB`), Tecnología (`TEC`), Campamento (`CAM`), Material Scout (`SCOUT`), Documentación (`DOC`), Limpieza (`LIM`), Recreación (`REC`).
* **Ubicaciones por defecto**: Oficina Principal, Bodega, Sala de Reuniones.

---

## 👥 Equipo y Créditos

- **Organización**: Oficina Scout de Leonardo Murialdo N.° 1 - Archidona
- **Repositorio**: [millanda29/SistemaScoutLM1](https://github.com/millanda29/SistemaScoutLM1)
- **Desarrollador principal**: Maikol Isaac Llanda Huatatoca