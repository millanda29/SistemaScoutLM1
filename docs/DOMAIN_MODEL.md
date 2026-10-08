# Modelo de Dominio
## Sistema de Gestión, Control y Trazabilidad de Recursos
### Oficina Scout de Leonardo Murialdo N.° 1 - Archidona

---

## 1. Información del Documento

| Campo | Descripción |
|---|---|
| **Proyecto** | Sistema de Gestión, Control y Trazabilidad de Recursos |
| **Documento** | Modelo de Dominio y Diseño de Base de Datos |
| **Versión** | 1.0 |
| **Fecha** | 2026-07-25 |
| **Motor de BD** | SQL Server 2022 |

---

## 2. Diagrama de Entidades y Relaciones (Conceptual)

```
                    ┌───────────────────────────────────────────────────────────────────────────────┐
                    │                                    CATEGORY                                    │
                    │                                    (Categoría)                                  │
                    └───────────────────────────────────────────────────────────────────────────────┘
                                                                  │ 1
                                                                  │
                                                                  │ *
                    ┌───────────────────────────────────────────────────────────────────────────────┐
                    │                                    LOCATION                                    │
                    │                                    (Ubicación)                                  │
                    └───────────────────────────────────────────────────────────────────────────────┘
                                                                  │ 1
                                                                  │
                                                                  │ *
                    ┌───────────────────────────────────────────────────────────────────────────────┐
                    │                                    RESOURCE                                    │──────────┐
                    │                                    (Recurso)                                   │          │ 1
                    └───────────────────────────────────────────────────────────────────────────────┘          │
                                │ 1                          │ 1                          │ 1                  │ *
                                │                            │                            │                    │
                                │ *                          │ *                          │ *                  ▼
                    ┌───────────────┐              ┌───────────────┐              ┌───────────────┐  ┌───────────────┐
                    │  MAINTENANCE  │              │     LOAN      │              │   MOVEMENT    │  │ PHYSICAL_INV  │
                    │ (Mantenimiento)│             │   (Préstamo)   │              │  (Movimiento)  │  │   (Inventario)│
                    └───────────────┘              └───────────────┘              └───────────────┘  └───────────────┘
                                                           │ 1                                                │ 1
                                                           │                                                  │
                                                           │ *                                                │ *
                                                           ▼                                                  ▼
                                                    ┌───────────────┐                                ┌───────────────┐
                                                    │  LOAN_ITEM    │                                │  INV_ITEM     │
                                                    │ (ItemPréstamo)│                                │ (ItemInvent.) │
                                                    └───────────────┘                                └───────────────┘

    ┌───────────────┐      ┌───────────────┐      ┌───────────────┐      ┌───────────────┐      ┌───────────────┐
    │     LOSS      │      │  RETIREMENT   │      │   DOCUMENT    │      │  AUDIT_LOG    │      │RESP_ASSIGN   │
    │   (Pérdida)    │      │    (Baja)     │      │  (Documento)   │      │ (Auditoría)    │      │(Asignación)  │
    └───────────────┘      └───────────────┘      └───────────────┘      └───────────────┘      └───────────────┘
```

---

## 3. Entidades del Dominio

### 3.1 User (Identity) — Usuario

Hereda de `IdentityUser` de ASP.NET Core Identity.

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | string (PK) | GENERATED | Id Identity |
| `UserName` | string | UNIQUE, REQUIRED | Nombre de usuario |
| `Email` | string | REQUIRED | Correo electrónico |
| `PhoneNumber` | string | — | Teléfono |
| `FullName` | string | REQUIRED | Nombre completo del usuario |
| `IsActive` | bool | DEFAULT true | Si el usuario está activo |
| `CreatedAt` | DateTime | REQUIRED | Fecha de creación |
| `LastLoginAt` | DateTime? | — | Último inicio de sesión |

### 3.2 Category — Categoría

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `Name` | string | UNIQUE, REQUIRED (100) | Nombre de la categoría |
| `Description` | string? | — (500) | Descripción |
| `Prefix` | string | UNIQUE, REQUIRED (10) | Prefijo para código único (MOB, TEC, CAM, etc.) |
| `NextNumber` | int | DEFAULT 1 | Siguiente número correlativo |
| `IsActive` | bool | DEFAULT true | Si la categoría está activa |
| `CreatedAt` | DateTime | REQUIRED | Fecha de creación |
| `UpdatedAt` | DateTime? | — | Fecha de última modificación |

**Reglas:**
- El prefijo debe ser único y en mayúsculas (ej. MOB, TEC, CAM, SCOUT, DOC, LIM, REC).
- `NextNumber` se incrementa automáticamente al crear un recurso en esta categoría.

### 3.3 Location — Ubicación

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `Name` | string | UNIQUE, REQUIRED (150) | Nombre de la ubicación |
| `Description` | string? | — (500) | Descripción |
| `IsActive` | bool | DEFAULT true | Si la ubicación está activa |
| `CreatedAt` | DateTime | REQUIRED | Fecha de creación |
| `UpdatedAt` | DateTime? | — | Fecha de última modificación |

### 3.4 Resource — Recurso

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `Code` | string | UNIQUE, REQUIRED (20) | Código único (MOB-0001) |
| `Name` | string | REQUIRED (200) | Nombre del recurso |
| `Description` | string? | — (1000) | Descripción |
| `CategoryId` | int (FK) | REQUIRED | Categoría → Category.Id |
| `Brand` | string? | — (100) | Marca |
| `Model` | string? | — (100) | Modelo |
| `SerialNumber` | string? | — (100) | Número de serie |
| `AcquisitionDate` | DateTime? | — | Fecha de adquisición |
| `AcquisitionType` | string | REQUIRED (20) | Tipo: COMPRA, DONACION, TRANSFERENCIA, OTRO |
| `AcquisitionCost` | decimal(18,2)? | — | Costo de adquisición |
| `AppraisedValue` | decimal(18,2)? | — | Valor de avalúo |
| `PhysicalCondition` | string | REQUIRED (20) | Estado físico: BUENO, REGULAR, DANIADO |
| `AdministrativeStatus` | string | REQUIRED (30) | Estado adm: REGISTRADO, DISPONIBLE, ASIGNADO, PRESTADO, EN_MANTENIMIENTO, NO_LOCALIZADO, PERDIDO, DADO_DE_BAJA |
| `LocationId` | int (FK) | REQUIRED | Ubicación actual → Location.Id |
| `CurrentResponsibleId` | string? (FK) | — | Responsable actual → User.Id |
| `MaintenanceFrequencyDays` | int? | — | Frecuencia de mantenimiento en días |
| `LastMaintenanceDate` | DateTime? | — | Último mantenimiento |
| `NextMaintenanceDate` | DateTime? | — | Próximo mantenimiento |
| `Observations` | string? | — (2000) | Observaciones |
| `IsActive` | bool | DEFAULT true | Si el registro está activo |
| `CreatedAt` | DateTime | REQUIRED | Fecha de registro |
| `UpdatedAt` | DateTime? | — | Última modificación |
| `CreatedById` | string (FK) | REQUIRED | Usuario que registró → User.Id |

**Índices:**
- `IX_Resource_Code` UNIQUE sobre `Code`
- `IX_Resource_CategoryId` sobre `CategoryId`
- `IX_Resource_LocationId` sobre `LocationId`
- `IX_Resource_AdministrativeStatus` sobre `AdministrativeStatus`
- `IX_Resource_PhysicalCondition` sobre `PhysicalCondition`
- `IX_Resource_CurrentResponsibleId` sobre `CurrentResponsibleId`

**FKs:**
- `FK_Resource_Category` → Category.Id (NO ACTION)
- `FK_Resource_Location` → Location.Id (NO ACTION)
- `FK_Resource_Responsible` → User.Id (SET NULL)
- `FK_Resource_CreatedBy` → User.Id (NO ACTION)

### 3.5 ResourceAssignment — Asignación de Responsable

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `ResourceId` | int (FK) | REQUIRED | Recurso → Resource.Id |
| `UserId` | string (FK) | REQUIRED | Usuario asignado → User.Id |
| `AssignedById` | string (FK) | REQUIRED | Usuario que asignó → User.Id |
| `AssignmentType` | string | REQUIRED (30) | Tipo: CUSTODIO, RESPONSABLE, SUPERVISOR |
| `AssignedAt` | DateTime | REQUIRED | Fecha de asignación |
| `UnassignedAt` | DateTime? | — | Fecha de desasignación |
| `IsActive` | bool | DEFAULT true | Si la asignación está vigente |
| `Observations` | string? | — (500) | Observaciones |

**Índices:** `IX_ResourceAssignment_ResourceId`, `IX_ResourceAssignment_UserId`

### 3.6 Loan — Préstamo

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `RequestNumber` | string | UNIQUE, REQUIRED (30) | Número de solicitud (PREST-YYYYMMDD-XXXXX) |
| `RequesterId` | string (FK) | REQUIRED | Solicitante → User.Id |
| `ApproverId` | string? (FK) | — | Aprobador (Jefe de Grupo) → User.Id |
| `DeliveredById` | string? (FK) | — | Quien entregó → User.Id |
| `ReceivedById` | string? (FK) | — | Quien recibió la devolución → User.Id |
| `Status` | string | REQUIRED (20) | SOLICITADO, APROBADO, RECHAZADO, ENTREGADO, PRESTADO, DEVUELTO, FINALIZADO |
| `Reason` | string | REQUIRED (1000) | Motivo del préstamo |
| `ApprovalType` | string? | — (20) | DOCUMENTADA, VERBAL |
| `ApprovalDate` | DateTime? | — | Fecha de aprobación |
| `ApprovalObservations` | string? | — (500) | Observaciones de la aprobación |
| `ExpectedExitDate` | DateTime? | — | Fecha prevista de salida |
| `ExpectedReturnDate` | DateTime | REQUIRED | Fecha estimada de devolución |
| `ActualDeliveryDate` | DateTime? | — | Fecha real de entrega |
| `ActualReturnDate` | DateTime? | — | Fecha real de devolución |
| `RejectionReason` | string? | — (500) | Motivo de rechazo |
| `CreatedAt` | DateTime | REQUIRED | Fecha de solicitud |
| `UpdatedAt` | DateTime? | — | Última modificación |
| `CreatedById` | string (FK) | REQUIRED | Usuario que creó → User.Id |

**Índices:** `IX_Loan_RequestNumber` UNIQUE, `IX_Loan_RequesterId`, `IX_Loan_Status`, `IX_Loan_ExpectedReturnDate`

### 3.7 LoanItem — Items del Préstamo

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `LoanId` | int (FK) | REQUIRED | Préstamo → Loan.Id |
| `ResourceId` | int (FK) | REQUIRED | Recurso → Resource.Id |
| `ConditionAtDelivery` | string | REQUIRED (20) | Estado físico al entregar |
| `ConditionAtReturn` | string? | — (20) | Estado físico al devolver |
| `ReturnObservations` | string? | — (500) | Observaciones de devolución |
| `DamagesDetected` | string? | — (1000) | Daños detectados en devolución |

**Índice:** `IX_LoanItem_LoanId`, `IX_LoanItem_ResourceId`
**UK:** UNIQUE(`LoanId`, `ResourceId`)

### 3.8 Maintenance — Mantenimiento

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `ResourceId` | int (FK) | REQUIRED | Recurso → Resource.Id |
| `Type` | string | REQUIRED (20) | PREVENTIVO, CORRECTIVO |
| `Status` | string | REQUIRED (20) | PROGRAMADO, PENDIENTE, EN_PROCESO, COMPLETADO, VENCIDO, CANCELADO |
| `Description` | string | REQUIRED (1000) | Descripción del mantenimiento |
| `ScheduledDate` | DateTime | REQUIRED | Fecha programada |
| `CompletedDate` | DateTime? | — | Fecha realizada |
| `ResponsibleId` | string? (FK) | — | Responsable → User.Id |
| `Cost` | decimal(18,2)? | — | Costo del mantenimiento |
| `Result` | string? | — (500) | Resultado del mantenimiento |
| `Observations` | string? | — (1000) | Observaciones |
| `NextMaintenanceDate` | DateTime? | — | Próxima fecha sugerida |
| `CreatedAt` | DateTime | REQUIRED | Fecha de registro |
| `UpdatedAt` | DateTime? | — | Última modificación |
| `CreatedById` | string (FK) | REQUIRED | Usuario que registró → User.Id |

**Índices:** `IX_Maintenance_ResourceId`, `IX_Maintenance_Status`, `IX_Maintenance_ScheduledDate`

### 3.9 PhysicalInventory — Jornada de Inventario Físico

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `InventoryNumber` | string | UNIQUE, REQUIRED (30) | Número de jornada (INV-YYYYMMDD-XXXXX) |
| `Status` | string | REQUIRED (20) | PLANIFICADA, EN_CURSO, FINALIZADA, CONCILIADA |
| `Description` | string? | — (500) | Descripción de la jornada |
| `StartDate` | DateTime | REQUIRED | Fecha de inicio |
| `EndDate` | DateTime? | — | Fecha de finalización |
| `ResponsibleId` | string (FK) | REQUIRED | Responsable → User.Id |
| `Observations` | string? | — (2000) | Observaciones generales |
| `CreatedAt` | DateTime | REQUIRED | Fecha de creación |
| `UpdatedAt` | DateTime? | — | Última modificación |
| `CreatedById` | string (FK) | REQUIRED | Usuario que creó → User.Id |

### 3.10 PhysicalInventoryLocation — Ubicaciones del Inventario

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `PhysicalInventoryId` | int (FK) | REQUIRED | Jornada → PhysicalInventory.Id |
| `LocationId` | int (FK) | REQUIRED | Ubicación → Location.Id |
| `Status` | string | REQUIRED (20) | PENDIENTE, VERIFICADA |

**UK:** UNIQUE(`PhysicalInventoryId`, `LocationId`)

### 3.11 InventoryItem — Items de Inventario Físico

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `PhysicalInventoryId` | int (FK) | REQUIRED | Jornada → PhysicalInventory.Id |
| `ResourceId` | int? (FK) | — | Recurso encontrado → Resource.Id (null si es no registrado) |
| `LocationId` | int (FK) | REQUIRED | Ubicación donde se encontró → Location.Id |
| `Result` | string | REQUIRED (20) | ENCONTRADO, NO_LOCALIZADO, DANIADO, NUEVO, NO_IDENTIFICADO, NO_CORRESPONDE |
| `PhysicalCondition` | string? | — (20) | Estado físico encontrado |
| `FoundCode` | string? | — (20) | Código del recurso si se identificó |
| `FoundName` | string? | — (200) | Nombre del recurso si no está registrado |
| `Observations` | string? | — (1000) | Observaciones |
| `VerifiedById` | string (FK) | REQUIRED | Usuario que verificó → User.Id |
| `VerifiedAt` | DateTime | REQUIRED | Fecha de verificación |

**Índices:** `IX_InventoryItem_PhysicalInventoryId`, `IX_InventoryItem_ResourceId`

### 3.12 InventoryReconciliation — Conciliación de Inventario

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `PhysicalInventoryId` | int (FK) | REQUIRED, UNIQUE | Jornada → PhysicalInventory.Id |
| `TotalRegistered` | int | REQUIRED | Total de recursos registrados |
| `TotalFound` | int | REQUIRED | Total encontrados |
| `TotalNotFound` | int | REQUIRED | Total no localizados |
| `TotalDamaged` | int | REQUIRED | Total dañados |
| `TotalNew` | int | REQUIRED | Total nuevos no registrados |
| `TotalNotIdentified` | int | REQUIRED | Total no identificados |
| `ReconciledAt` | DateTime | REQUIRED | Fecha de conciliación |
| `ReconciledById` | string (FK) | REQUIRED | Usuario que concilió → User.Id |
| `Observations` | string? | — (2000) | Observaciones |
| `CreatedAt` | DateTime | REQUIRED | Fecha de registro |

### 3.13 Loss — Pérdida

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `ResourceId` | int (FK) | REQUIRED | Recurso → Resource.Id |
| `LossNumber` | string | UNIQUE, REQUIRED (30) | Número de pérdida (PERD-YYYYMMDD-XXXXX) |
| `Status` | string | REQUIRED (20) | EN_INVESTIGACION, CONFIRMADA, RECUPERADA |
| `Circumstances` | string | REQUIRED (1000) | Circunstancias de la pérdida |
| `Description` | string? | — (2000) | Descripción detallada |
| `ReportedById` | string (FK) | REQUIRED | Quien reporta → User.Id |
| `ConfirmedById` | string? (FK) | — | Quien confirma → User.Id |
| `ConfirmedAt` | DateTime? | — | Fecha de confirmación |
| `RecoveredAt` | DateTime? | — | Fecha de recuperación |
| `Observations` | string? | — (1000) | Observaciones |
| `CreatedAt` | DateTime | REQUIRED | Fecha de registro |
| `UpdatedAt` | DateTime? | — | Última modificación |

### 3.14 Retirement — Baja

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `ResourceId` | int (FK) | REQUIRED, UNIQUE | Recurso → Resource.Id |
| `RetirementNumber` | string | UNIQUE, REQUIRED (30) | Número de baja (BAJA-YYYYMMDD-XXXXX) |
| `Status` | string | REQUIRED (20) | SOLICITADA, REVISADA, AUTORIZADA, RECHAZADA, EJECUTADA |
| `Reason` | string | REQUIRED (1000) | Motivo de la baja |
| `RequestedById` | string (FK) | REQUIRED | Quien solicita → User.Id |
| `ReviewedById` | string? (FK) | — | Quien revisa → User.Id |
| `AuthorizedById` | string? (FK) | — | Quien autoriza (Jefe de Grupo) → User.Id |
| `AuthorizedAt` | DateTime? | — | Fecha de autorización |
| `ExecutedAt` | DateTime? | — | Fecha de ejecución |
| `ExecutedById` | string? (FK) | — | Quien ejecuta → User.Id |
| `AppraisedValueAtRetirement` | decimal(18,2)? | — | Valor de avalúo al momento de baja |
| `Observations` | string? | — (1000) | Observaciones |
| `RejectionReason` | string? | — (500) | Motivo de rechazo |
| `CreatedAt` | DateTime | REQUIRED | Fecha de solicitud |
| `UpdatedAt` | DateTime? | — | Última modificación |

### 3.15 Movement — Movimiento

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | bigint (PK) | AUTO INCREMENT | Identificador |
| `ResourceId` | int (FK) | REQUIRED | Recurso → Resource.Id |
| `Type` | string | REQUIRED (30) | REGISTRO, ASIGNACION, DESASIGNACION, PRESTAMO, DEVOLUCION, TRASLADO, MANTENIMIENTO, PERDIDA, RECUPERACION, BAJA |
| `Description` | string | REQUIRED (500) | Descripción del movimiento |
| `PreviousStatus` | string? | — (30) | Estado administrativo anterior |
| `NewStatus` | string? | — (30) | Estado administrativo nuevo |
| `PreviousLocationId` | int? (FK) | — | Ubicación anterior → Location.Id |
| `NewLocationId` | int? (FK) | — | Ubicación nueva → Location.Id |
| `PreviousResponsibleId` | string? (FK) | — | Responsable anterior → User.Id |
| `NewResponsibleId` | string? (FK) | — | Responsable nuevo → User.Id |
| `ReferenceType` | string? | — (30) | Tipo de entidad de referencia (LOAN, MAINTENANCE, etc.) |
| `ReferenceId` | int? | — | ID de la entidad de referencia |
| `PerformedById` | string (FK) | REQUIRED | Usuario que realizó → User.Id |
| `PerformedAt` | DateTime | REQUIRED | Fecha del movimiento |
| `Observations` | string? | — (500) | Observaciones |

**Índices:** `IX_Movement_ResourceId`, `IX_Movement_PerformedAt`, `IX_Movement_Type`

### 3.16 AuditLog — Auditoría

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | bigint (PK) | AUTO INCREMENT | Identificador |
| `UserId` | string (FK) | REQUIRED | Usuario → User.Id |
| `Action` | string | REQUIRED (50) | Acción: CREATE, UPDATE, DELETE, APPROVE, REJECT, etc. |
| `EntityType` | string | REQUIRED (50) | Tipo de entidad: RESOURCE, LOAN, MAINTENANCE, etc. |
| `EntityId` | string | REQUIRED (50) | ID de la entidad |
| `PreviousValues` | string? | — (MAX) | Valores anteriores (JSON) |
| `NewValues` | string? | — (MAX) | Valores nuevos (JSON) |
| `ChangedFields` | string? | — (MAX) | Campos modificados (JSON) |
| `IpAddress` | string? | — (50) | Dirección IP |
| `PerformedAt` | DateTime | REQUIRED | Fecha de la acción |
| `Observations` | string? | — (500) | Observaciones |

**Índices:** `IX_AuditLog_UserId`, `IX_AuditLog_Action`, `IX_AuditLog_EntityType`, `IX_AuditLog_PerformedAt`

### 3.17 Document — Documento

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| `Id` | int (PK) | AUTO INCREMENT | Identificador |
| `FileName` | string | REQUIRED (255) | Nombre del archivo |
| `OriginalName` | string | REQUIRED (255) | Nombre original |
| `ContentType` | string | REQUIRED (100) | Tipo MIME |
| `Size` | bigint | REQUIRED | Tamaño en bytes |
| `StoragePath` | string | REQUIRED (500) | Ruta de almacenamiento |
| `EntityType` | string | REQUIRED (50) | Tipo de entidad asociada |
| `EntityId` | int | REQUIRED | ID de la entidad asociada |
| `UploadedById` | string (FK) | REQUIRED | Usuario que subió → User.Id |
| `UploadedAt` | DateTime | REQUIRED | Fecha de subida |
| `IsActive` | bool | DEFAULT true | Si el documento está activo |

---

## 4. Diagrama Entidad-Relación (Texto)

```
CATEGORY 1───* RESOURCE
LOCATION 1───* RESOURCE
USER     1───* RESOURCE (created_by)
USER     1───* RESOURCE (current_responsible)

RESOURCE 1───* RESOURCE_ASSIGNMENT
USER     1───* RESOURCE_ASSIGNMENT (user_id)
USER     1───* RESOURCE_ASSIGNMENT (assigned_by)

RESOURCE 1───* LOAN_ITEM
LOAN     1───* LOAN_ITEM
USER     1───* LOAN (requester)
USER     1───* LOAN (approver)
USER     1───* LOAN (delivered_by)
USER     1───* LOAN (received_by)

RESOURCE 1───* MAINTENANCE
USER     1───* MAINTENANCE (responsible)
USER     1───* MAINTENANCE (created_by)

PHYSICAL_INVENTORY 1───* PHYSICAL_INVENTORY_LOCATION
LOCATION           1───* PHYSICAL_INVENTORY_LOCATION
PHYSICAL_INVENTORY 1───* INVENTORY_ITEM
RESOURCE                1───? INVENTORY_ITEM
LOCATION                1───* INVENTORY_ITEM
USER                    1───* INVENTORY_ITEM (verified_by)
PHYSICAL_INVENTORY 1───1 INVENTORY_RECONCILIATION

RESOURCE 1───? LOSS
USER     1───* LOSS (reported_by)
USER     1───* LOSS (confirmed_by)

RESOURCE 1───? RETIREMENT
USER     1───* RETIREMENT (requested_by)
USER     1───* RETIREMENT (reviewed_by)
USER     1───* RETIREMENT (authorized_by)

RESOURCE 1───* MOVEMENT
USER     1───* MOVEMENT (performed_by)

USER     1───* AUDIT_LOG

USER     1───* DOCUMENT (uploaded_by)
```

---

## 5. Enumeradores

### 5.1 AcquisitionType — Tipo de Adquisición

```csharp
public enum AcquisitionType
{
    COMPRA,
    DONACION,
    TRANSFERENCIA,
    OTRO
}
```

### 5.2 PhysicalCondition — Estado Físico

```csharp
public enum PhysicalCondition
{
    BUENO,
    REGULAR,
    DANIADO
}
```

### 5.3 AdministrativeStatus — Estado Administrativo

```csharp
public enum AdministrativeStatus
{
    REGISTRADO,
    DISPONIBLE,
    ASIGNADO,
    PRESTADO,
    EN_MANTENIMIENTO,
    NO_LOCALIZADO,
    PERDIDO,
    DADO_DE_BAJA
}
```

### 5.4 LoanStatus — Estado del Préstamo

```csharp
public enum LoanStatus
{
    SOLICITADO,
    APROBADO,
    RECHAZADO,
    ENTREGADO,
    PRESTADO,
    DEVUELTO,
    FINALIZADO
}
```

### 5.5 ApprovalType — Tipo de Aprobación

```csharp
public enum ApprovalType
{
    DOCUMENTADA,
    VERBAL
}
```

### 5.6 MaintenanceType — Tipo de Mantenimiento

```csharp
public enum MaintenanceType
{
    PREVENTIVO,
    CORRECTIVO
}
```

### 5.7 MaintenanceStatus — Estado de Mantenimiento

```csharp
public enum MaintenanceStatus
{
    PROGRAMADO,
    PENDIENTE,
    EN_PROCESO,
    COMPLETADO,
    VENCIDO,
    CANCELADO
}
```

### 5.8 InventoryStatus — Estado de Jornada de Inventario

```csharp
public enum InventoryStatus
{
    PLANIFICADA,
    EN_CURSO,
    FINALIZADA,
    CONCILIADA
}
```

### 5.9 InventoryResult — Resultado de Item de Inventario

```csharp
public enum InventoryResult
{
    ENCONTRADO,
    NO_LOCALIZADO,
    DANIADO,
    NUEVO,
    NO_IDENTIFICADO,
    NO_CORRESPONDE
}
```

### 5.10 LossStatus — Estado de Pérdida

```csharp
public enum LossStatus
{
    EN_INVESTIGACION,
    CONFIRMADA,
    RECUPERADA
}
```

### 5.11 RetirementStatus — Estado de Baja

```csharp
public enum RetirementStatus
{
    SOLICITADA,
    REVISADA,
    AUTORIZADA,
    RECHAZADA,
    EJECUTADA
}
```

### 5.12 MovementType — Tipo de Movimiento

```csharp
public enum MovementType
{
    REGISTRO,
    ASIGNACION,
    DESASIGNACION,
    PRESTAMO,
    DEVOLUCION,
    TRASLADO,
    MANTENIMIENTO,
    PERDIDA,
    RECUPERACION,
    BAJA
}
```

### 5.13 AssignmentType — Tipo de Asignación

```csharp
public enum AssignmentType
{
    CUSTODIO,
    RESPONSABLE,
    SUPERVISOR
}
```

---

## 6. Value Objects

### 6.1 ResourceCode

```csharp
public class ResourceCode
{
    public string Prefix { get; }    // MOB, TEC, CAM, etc.
    public int Number { get; }       // 1, 2, 3, etc.

    public string Code => $"{Prefix}-{Number:D5}";
    // MOB-00001, TEC-00005, CAM-00012
}
```

### 6.2 MonetaryValue

```csharp
public class MonetaryValue
{
    public decimal Amount { get; }
    public string Currency { get; }  // USD, EUR, etc.

    public MonetaryValue(decimal amount, string currency = "USD")
    {
        Amount = amount;
        Currency = currency;
    }
}
```

---

## 7. Reglas de Dominio (Implementación)

### 7.1 Resource Rules

```csharp
// RN-01: Un recurso solo puede estar DISPONIBLE si cumple todas las condiciones
public bool CanBeAvailable()
{
    return PhysicalCondition != PhysicalCondition.DANIADO
        && AdministrativeStatus != AdministrativeStatus.PRESTADO
        && AdministrativeStatus != AdministrativeStatus.EN_MANTENIMIENTO
        && AdministrativeStatus != AdministrativeStatus.NO_LOCALIZADO
        && AdministrativeStatus != AdministrativeStatus.PERDIDO
        && AdministrativeStatus != AdministrativeStatus.DADO_DE_BAJA;
}

// RN-03: Un recurso dado de baja no puede volver a disponible
public bool CanChangeToAvailable()
{
    return AdministrativeStatus != AdministrativeStatus.DADO_DE_BAJA;
}

// RN-13: El código es inmutable
```

### 7.2 Loan Rules

```csharp
// RN-02: Un recurso no puede tener más de un préstamo activo
public bool CanAddResource(Resource resource, IEnumerable<LoanItem> activeItems)
{
    return activeItems.All(i => i.ResourceId != resource.Id);
}

// RN-07: Todo préstamo debe tener autorización antes de entregar
public bool CanDeliver()
{
    return Status == LoanStatus.APROBADO;
}
```

### 7.3 Loss Rules

```csharp
// RN-05: 30 días en NO_LOCALIZADO antes de marcar PERDIDO
public bool CanConfirmLoss(Resource resource)
{
    var daysSinceNoLocalizado = (DateTime.UtcNow - resource.UpdatedAt.Value).Days;
    return daysSinceNoLocalizado >= 30;
}
```

---

## 8. Configuración de Conexión SQL Server

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ScoutInventory;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

---

## 9. Script de Creación de Base de Datos

```sql
-- Crear base de datos
CREATE DATABASE ScoutInventory;
GO

USE ScoutInventory;
GO

-- Tablas de Identity (generadas por EF Core)
-- Las tablas AspNetUsers, AspNetRoles, AspNetRoleClaims, etc. se generan automáticamente

-- Tabla: Categories
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Prefix NVARCHAR(10) NOT NULL,
    NextNumber INT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Categories_Name UNIQUE (Name),
    CONSTRAINT UQ_Categories_Prefix UNIQUE (Prefix)
);

-- Tabla: Locations
CREATE TABLE Locations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Locations_Name UNIQUE (Name)
);

-- Tabla: Resources
CREATE TABLE Resources (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(20) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    CategoryId INT NOT NULL,
    Brand NVARCHAR(100) NULL,
    Model NVARCHAR(100) NULL,
    SerialNumber NVARCHAR(100) NULL,
    AcquisitionDate DATETIME2 NULL,
    AcquisitionType NVARCHAR(20) NOT NULL,
    AcquisitionCost DECIMAL(18,2) NULL,
    AppraisedValue DECIMAL(18,2) NULL,
    PhysicalCondition NVARCHAR(20) NOT NULL,
    AdministrativeStatus NVARCHAR(30) NOT NULL,
    LocationId INT NOT NULL,
    CurrentResponsibleId NVARCHAR(450) NULL,
    MaintenanceFrequencyDays INT NULL,
    LastMaintenanceDate DATETIME2 NULL,
    NextMaintenanceDate DATETIME2 NULL,
    Observations NVARCHAR(2000) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedById NVARCHAR(450) NOT NULL,
    CONSTRAINT UQ_Resources_Code UNIQUE (Code),
    CONSTRAINT FK_Resources_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT FK_Resources_Locations FOREIGN KEY (LocationId) REFERENCES Locations(Id),
    CONSTRAINT FK_Resources_Responsible FOREIGN KEY (CurrentResponsibleId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Resources_CreatedBy FOREIGN KEY (CreatedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Resources_AcquisitionType CHECK (AcquisitionType IN ('COMPRA', 'DONACION', 'TRANSFERENCIA', 'OTRO')),
    CONSTRAINT CK_Resources_PhysicalCondition CHECK (PhysicalCondition IN ('BUENO', 'REGULAR', 'DANIADO')),
    CONSTRAINT CK_Resources_AdministrativeStatus CHECK (AdministrativeStatus IN ('REGISTRADO', 'DISPONIBLE', 'ASIGNADO', 'PRESTADO', 'EN_MANTENIMIENTO', 'NO_LOCALIZADO', 'PERDIDO', 'DADO_DE_BAJA'))
);

CREATE INDEX IX_Resources_CategoryId ON Resources(CategoryId);
CREATE INDEX IX_Resources_LocationId ON Resources(LocationId);
CREATE INDEX IX_Resources_AdministrativeStatus ON Resources(AdministrativeStatus);
CREATE INDEX IX_Resources_CurrentResponsibleId ON Resources(CurrentResponsibleId);

-- Tabla: ResourceAssignments
CREATE TABLE ResourceAssignments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ResourceId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    AssignedById NVARCHAR(450) NOT NULL,
    AssignmentType NVARCHAR(30) NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UnassignedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    Observations NVARCHAR(500) NULL,
    CONSTRAINT FK_ResourceAssignments_Resources FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT FK_ResourceAssignments_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_ResourceAssignments_AssignedBy FOREIGN KEY (AssignedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_ResourceAssignments_Type CHECK (AssignmentType IN ('CUSTODIO', 'RESPONSABLE', 'SUPERVISOR'))
);

CREATE INDEX IX_ResourceAssignments_ResourceId ON ResourceAssignments(ResourceId);
CREATE INDEX IX_ResourceAssignments_UserId ON ResourceAssignments(UserId);

-- Tabla: Loans
CREATE TABLE Loans (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequestNumber NVARCHAR(30) NOT NULL,
    RequesterId NVARCHAR(450) NOT NULL,
    ApproverId NVARCHAR(450) NULL,
    DeliveredById NVARCHAR(450) NULL,
    ReceivedById NVARCHAR(450) NULL,
    Status NVARCHAR(20) NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    ApprovalType NVARCHAR(20) NULL,
    ApprovalDate DATETIME2 NULL,
    ApprovalObservations NVARCHAR(500) NULL,
    ExpectedExitDate DATETIME2 NULL,
    ExpectedReturnDate DATETIME2 NOT NULL,
    ActualDeliveryDate DATETIME2 NULL,
    ActualReturnDate DATETIME2 NULL,
    RejectionReason NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedById NVARCHAR(450) NOT NULL,
    CONSTRAINT UQ_Loans_RequestNumber UNIQUE (RequestNumber),
    CONSTRAINT FK_Loans_Requester FOREIGN KEY (RequesterId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Loans_Approver FOREIGN KEY (ApproverId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Loans_DeliveredBy FOREIGN KEY (DeliveredById) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Loans_ReceivedBy FOREIGN KEY (ReceivedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Loans_CreatedBy FOREIGN KEY (CreatedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Loans_Status CHECK (Status IN ('SOLICITADO', 'APROBADO', 'RECHAZADO', 'ENTREGADO', 'PRESTADO', 'DEVUELTO', 'FINALIZADO')),
    CONSTRAINT CK_Loans_ApprovalType CHECK (ApprovalType IN ('DOCUMENTADA', 'VERBAL'))
);

CREATE INDEX IX_Loans_RequesterId ON Loans(RequesterId);
CREATE INDEX IX_Loans_ApproverId ON Loans(ApproverId);
CREATE INDEX IX_Loans_Status ON Loans(Status);
CREATE INDEX IX_Loans_ExpectedReturnDate ON Loans(ExpectedReturnDate);

-- Tabla: LoanItems
CREATE TABLE LoanItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LoanId INT NOT NULL,
    ResourceId INT NOT NULL,
    ConditionAtDelivery NVARCHAR(20) NOT NULL,
    ConditionAtReturn NVARCHAR(20) NULL,
    ReturnObservations NVARCHAR(500) NULL,
    DamagesDetected NVARCHAR(1000) NULL,
    CONSTRAINT FK_LoanItems_Loans FOREIGN KEY (LoanId) REFERENCES Loans(Id),
    CONSTRAINT FK_LoanItems_Resources FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT UQ_LoanItems_LoanResource UNIQUE (LoanId, ResourceId),
    CONSTRAINT CK_LoanItems_Condition CHECK (ConditionAtDelivery IN ('BUENO', 'REGULAR', 'DANIADO'))
);

CREATE INDEX IX_LoanItems_LoanId ON LoanItems(LoanId);
CREATE INDEX IX_LoanItems_ResourceId ON LoanItems(ResourceId);

-- Tabla: Maintenances
CREATE TABLE Maintenances (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ResourceId INT NOT NULL,
    Type NVARCHAR(20) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    ScheduledDate DATETIME2 NOT NULL,
    CompletedDate DATETIME2 NULL,
    ResponsibleId NVARCHAR(450) NULL,
    Cost DECIMAL(18,2) NULL,
    Result NVARCHAR(500) NULL,
    Observations NVARCHAR(1000) NULL,
    NextMaintenanceDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedById NVARCHAR(450) NOT NULL,
    CONSTRAINT FK_Maintenances_Resources FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT FK_Maintenances_Responsible FOREIGN KEY (ResponsibleId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Maintenances_CreatedBy FOREIGN KEY (CreatedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Maintenances_Type CHECK (Type IN ('PREVENTIVO', 'CORRECTIVO')),
    CONSTRAINT CK_Maintenances_Status CHECK (Status IN ('PROGRAMADO', 'PENDIENTE', 'EN_PROCESO', 'COMPLETADO', 'VENCIDO', 'CANCELADO'))
);

CREATE INDEX IX_Maintenances_ResourceId ON Maintenances(ResourceId);
CREATE INDEX IX_Maintenances_Status ON Maintenances(Status);
CREATE INDEX IX_Maintenances_ScheduledDate ON Maintenances(ScheduledDate);

-- Tabla: PhysicalInventories
CREATE TABLE PhysicalInventories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventoryNumber NVARCHAR(30) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Description NVARCHAR(500) NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    ResponsibleId NVARCHAR(450) NOT NULL,
    Observations NVARCHAR(2000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedById NVARCHAR(450) NOT NULL,
    CONSTRAINT UQ_PhysicalInventories_Number UNIQUE (InventoryNumber),
    CONSTRAINT FK_PhysicalInventories_Responsible FOREIGN KEY (ResponsibleId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_PhysicalInventories_CreatedBy FOREIGN KEY (CreatedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_PhysicalInventories_Status CHECK (Status IN ('PLANIFICADA', 'EN_CURSO', 'FINALIZADA', 'CONCILIADA'))
);

-- Tabla: PhysicalInventoryLocations
CREATE TABLE PhysicalInventoryLocations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PhysicalInventoryId INT NOT NULL,
    LocationId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',
    CONSTRAINT FK_PIL_PhysicalInventory FOREIGN KEY (PhysicalInventoryId) REFERENCES PhysicalInventories(Id),
    CONSTRAINT FK_PIL_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id),
    CONSTRAINT UQ_PIL_InventoryLocation UNIQUE (PhysicalInventoryId, LocationId),
    CONSTRAINT CK_PIL_Status CHECK (Status IN ('PENDIENTE', 'VERIFICADA'))
);

-- Tabla: InventoryItems
CREATE TABLE InventoryItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PhysicalInventoryId INT NOT NULL,
    ResourceId INT NULL,
    LocationId INT NOT NULL,
    Result NVARCHAR(20) NOT NULL,
    PhysicalCondition NVARCHAR(20) NULL,
    FoundCode NVARCHAR(20) NULL,
    FoundName NVARCHAR(200) NULL,
    Observations NVARCHAR(1000) NULL,
    VerifiedById NVARCHAR(450) NOT NULL,
    VerifiedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_InventoryItems_PhysicalInventory FOREIGN KEY (PhysicalInventoryId) REFERENCES PhysicalInventories(Id),
    CONSTRAINT FK_InventoryItems_Resource FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT FK_InventoryItems_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id),
    CONSTRAINT FK_InventoryItems_VerifiedBy FOREIGN KEY (VerifiedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_InventoryItems_Result CHECK (Result IN ('ENCONTRADO', 'NO_LOCALIZADO', 'DANIADO', 'NUEVO', 'NO_IDENTIFICADO', 'NO_CORRESPONDE'))
);

CREATE INDEX IX_InventoryItems_PhysicalInventoryId ON InventoryItems(PhysicalInventoryId);
CREATE INDEX IX_InventoryItems_ResourceId ON InventoryItems(ResourceId);

-- Tabla: InventoryReconciliations
CREATE TABLE InventoryReconciliations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PhysicalInventoryId INT NOT NULL,
    TotalRegistered INT NOT NULL,
    TotalFound INT NOT NULL,
    TotalNotFound INT NOT NULL,
    TotalDamaged INT NOT NULL,
    TotalNew INT NOT NULL,
    TotalNotIdentified INT NOT NULL,
    ReconciledAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReconciledById NVARCHAR(450) NOT NULL,
    Observations NVARCHAR(2000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UQ_IR_PhysicalInventory UNIQUE (PhysicalInventoryId),
    CONSTRAINT FK_IR_PhysicalInventory FOREIGN KEY (PhysicalInventoryId) REFERENCES PhysicalInventories(Id),
    CONSTRAINT FK_IR_ReconciledBy FOREIGN KEY (ReconciledById) REFERENCES AspNetUsers(Id)
);

-- Tabla: Losses
CREATE TABLE Losses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ResourceId INT NOT NULL,
    LossNumber NVARCHAR(30) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Circumstances NVARCHAR(1000) NOT NULL,
    Description NVARCHAR(2000) NULL,
    ReportedById NVARCHAR(450) NOT NULL,
    ConfirmedById NVARCHAR(450) NULL,
    ConfirmedAt DATETIME2 NULL,
    RecoveredAt DATETIME2 NULL,
    Observations NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Losses_Number UNIQUE (LossNumber),
    CONSTRAINT FK_Losses_Resource FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT FK_Losses_ReportedBy FOREIGN KEY (ReportedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Losses_ConfirmedBy FOREIGN KEY (ConfirmedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Losses_Status CHECK (Status IN ('EN_INVESTIGACION', 'CONFIRMADA', 'RECUPERADA'))
);

-- Tabla: Retirements
CREATE TABLE Retirements (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ResourceId INT NOT NULL,
    RetirementNumber NVARCHAR(30) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    RequestedById NVARCHAR(450) NOT NULL,
    ReviewedById NVARCHAR(450) NULL,
    AuthorizedById NVARCHAR(450) NULL,
    AuthorizedAt DATETIME2 NULL,
    ExecutedAt DATETIME2 NULL,
    ExecutedById NVARCHAR(450) NULL,
    AppraisedValueAtRetirement DECIMAL(18,2) NULL,
    Observations NVARCHAR(1000) NULL,
    RejectionReason NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Retirements_Number UNIQUE (RetirementNumber),
    CONSTRAINT UQ_Retirements_Resource UNIQUE (ResourceId),
    CONSTRAINT FK_Retirements_Resource FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT FK_Retirements_RequestedBy FOREIGN KEY (RequestedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Retirements_ReviewedBy FOREIGN KEY (ReviewedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Retirements_AuthorizedBy FOREIGN KEY (AuthorizedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Retirements_Status CHECK (Status IN ('SOLICITADA', 'REVISADA', 'AUTORIZADA', 'RECHAZADA', 'EJECUTADA'))
);

-- Tabla: Movements
CREATE TABLE Movements (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ResourceId INT NOT NULL,
    Type NVARCHAR(30) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    PreviousStatus NVARCHAR(30) NULL,
    NewStatus NVARCHAR(30) NULL,
    PreviousLocationId INT NULL,
    NewLocationId INT NULL,
    PreviousResponsibleId NVARCHAR(450) NULL,
    NewResponsibleId NVARCHAR(450) NULL,
    ReferenceType NVARCHAR(30) NULL,
    ReferenceId INT NULL,
    PerformedById NVARCHAR(450) NOT NULL,
    PerformedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Observations NVARCHAR(500) NULL,
    CONSTRAINT FK_Movements_Resource FOREIGN KEY (ResourceId) REFERENCES Resources(Id),
    CONSTRAINT FK_Movements_PreviousLocation FOREIGN KEY (PreviousLocationId) REFERENCES Locations(Id),
    CONSTRAINT FK_Movements_NewLocation FOREIGN KEY (NewLocationId) REFERENCES Locations(Id),
    CONSTRAINT FK_Movements_PreviousResponsible FOREIGN KEY (PreviousResponsibleId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Movements_NewResponsible FOREIGN KEY (NewResponsibleId) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Movements_PerformedBy FOREIGN KEY (PerformedById) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Movements_Type CHECK (Type IN ('REGISTRO', 'ASIGNACION', 'DESASIGNACION', 'PRESTAMO', 'DEVOLUCION', 'TRASLADO', 'MANTENIMIENTO', 'PERDIDA', 'RECUPERACION', 'BAJA'))
);

CREATE INDEX IX_Movements_ResourceId ON Movements(ResourceId);
CREATE INDEX IX_Movements_PerformedAt ON Movements(PerformedAt);
CREATE INDEX IX_Movements_Type ON Movements(Type);

-- Tabla: AuditLogs
CREATE TABLE AuditLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(50) NOT NULL,
    EntityId NVARCHAR(50) NOT NULL,
    PreviousValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    ChangedFields NVARCHAR(MAX) NULL,
    IpAddress NVARCHAR(50) NULL,
    PerformedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Observations NVARCHAR(500) NULL,
    CONSTRAINT FK_AuditLogs_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_Action ON AuditLogs(Action);
CREATE INDEX IX_AuditLogs_EntityType ON AuditLogs(EntityType);
CREATE INDEX IX_AuditLogs_PerformedAt ON AuditLogs(PerformedAt);

-- Tabla: Documents
CREATE TABLE Documents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FileName NVARCHAR(255) NOT NULL,
    OriginalName NVARCHAR(255) NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,
    Size BIGINT NOT NULL,
    StoragePath NVARCHAR(500) NOT NULL,
    EntityType NVARCHAR(50) NOT NULL,
    EntityId INT NOT NULL,
    UploadedById NVARCHAR(450) NOT NULL,
    UploadedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Documents_UploadedBy FOREIGN KEY (UploadedById) REFERENCES AspNetUsers(Id)
);
```

---

## 10. Resumen de Tablas

| # | Tabla | Descripción |
|---|---|---|
| 1 | `AspNetUsers` | Usuarios (Identity) |
| 2 | `AspNetRoles` | Roles (Identity) |
| 3 | `AspNetUserRoles` | Usuario-Rol (Identity) |
| 4 | `AspNetRoleClaims` | Claims de roles (Identity) |
| 5 | `AspNetUserClaims` | Claims de usuarios (Identity) |
| 6 | `AspNetUserLogins` | Logins externos (Identity) |
| 7 | `AspNetUserTokens` | Tokens (Identity) |
| 8 | `Categories` | Categorías de recursos |
| 9 | `Locations` | Ubicaciones físicas |
| 10 | `Resources` | Recursos institucionales |
| 11 | `ResourceAssignments` | Asignaciones de responsables |
| 12 | `Loans` | Préstamos |
| 13 | `LoanItems` | Items de préstamos |
| 14 | `Maintenances` | Mantenimientos |
| 15 | `PhysicalInventories` | Jornadas de inventario |
| 16 | `PhysicalInventoryLocations` | Ubicaciones por jornada |
| 17 | `InventoryItems` | Items de inventario |
| 18 | `InventoryReconciliations` | Conciliaciones |
| 19 | `Losses` | Pérdidas |
| 20 | `Retirements` | Bajas |
| 21 | `Movements` | Movimientos |
| 22 | `AuditLogs` | Bitácora de auditoría |
| 23 | `Documents` | Documentos adjuntos |
