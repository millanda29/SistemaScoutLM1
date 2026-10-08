# Sistema de Gestión, Control y Trazabilidad de Recursos
## Oficina Scout de Leonardo Murialdo N.° 1 - Archidona

---

## 1. Información General

| Campo | Descripción |
|---|---|
| Nombre del proyecto | Sistema de Gestión, Control y Trazabilidad de Recursos |
| Organización | Oficina Scout de Leonardo Murialdo N.° 1 - Archidona |
| Tipo de sistema | Sistema web de gestión institucional |
| Propósito principal | Gestionar, controlar y dar trazabilidad a los recursos y bienes institucionales |
| Frontend | Angular |
| Backend | ASP.NET Core Web API |
| Lenguaje backend | C# |
| Lenguaje frontend | TypeScript |
| Base de datos | SQL Server 2022 |
| ORM | Entity Framework Core |
| Arquitectura | Arquitectura en Capas |
| Patrón arquitectónico | Monolito Modular |
| Comunicación | API REST |
| Autenticación | ASP.NET Core Identity + JWT |
| Autorización | Roles y Policies |
| Documentación API | OpenAPI / Swagger |
| Contenedores | Docker |
| UI | Angular Material |

---

# 2. Contexto del Proyecto

La Oficina Scout de Leonardo Murialdo N.° 1 - Archidona actualmente no dispone de un inventario institucional actualizado y estructurado que permita conocer con exactitud los bienes y recursos que posee la organización.

Actualmente se desconoce con precisión:

- La cantidad total de recursos existentes.
- La ubicación actual de cada recurso.
- El estado físico de los bienes.
- Los recursos que se encuentran perdidos.
- Los recursos que se encuentran dañados.
- Los recursos que han sido prestados.
- Los recursos que han sido devueltos.
- Los responsables de los bienes.
- El historial de movimientos.
- Los recursos que requieren mantenimiento.
- Los bienes que deberían ser dados de baja.

Esta situación dificulta la administración y custodia de los bienes institucionales y genera riesgos relacionados con:

- Pérdida de recursos.
- Falta de trazabilidad.
- Falta de control sobre préstamos.
- Desconocimiento de la ubicación de los bienes.
- Ausencia de historial de movimientos.
- Falta de seguimiento de mantenimiento.
- Ausencia de un proceso formal de baja.
- Dificultad para realizar procesos de inventario físico.

Por esta razón, se propone desarrollar un sistema web que permita realizar el levantamiento inicial del inventario y posteriormente gestionar de manera continua el ciclo de vida de los recursos institucionales.

El sistema no se limitará a administrar un catálogo de bienes, sino que permitirá controlar su existencia física, ubicación, estado, responsable, préstamos, devoluciones, mantenimiento, pérdidas, bajas y movimientos históricos.

---

# 3. Problema

La Oficina Scout de Leonardo Murialdo N.° 1 - Archidona no cuenta actualmente con un mecanismo sistematizado para registrar y controlar sus recursos institucionales.

La inexistencia de un inventario actualizado impide determinar con precisión qué bienes posee la organización, dónde se encuentran, en qué estado están y quién es responsable de su custodia.

Adicionalmente, los préstamos de bienes se realizan ocasionalmente sin disponer de un registro formal que permita identificar al solicitante, la autorización correspondiente, la fecha de entrega, la fecha prevista de devolución y el estado del recurso al momento de ser devuelto.

La ausencia de procesos formalizados para inventarios físicos, mantenimiento, pérdidas y bajas genera una falta de trazabilidad sobre el ciclo de vida de los bienes.

---

# 4. Objetivo General

Desarrollar un sistema web de gestión, control y trazabilidad de recursos institucionales para la Oficina Scout de Leonardo Murialdo N.° 1 - Archidona, que permita realizar el levantamiento inicial del inventario y gestionar posteriormente la ubicación, estado, responsables, préstamos, devoluciones, mantenimiento, pérdidas, bajas y movimientos de los bienes institucionales.

---

# 5. Objetivos Específicos

1. Realizar un levantamiento físico inicial de los recursos existentes en la organización.
2. Registrar y clasificar los bienes institucionales según su categoría.
3. Asignar un identificador único a cada recurso.
4. Registrar la ubicación física de los recursos.
5. Registrar el responsable de la custodia de los bienes.
6. Controlar el estado físico y administrativo de cada recurso.
7. Gestionar solicitudes y procesos de préstamo.
8. Registrar la aprobación de préstamos por parte del Jefe de Grupo.
9. Registrar la entrega y devolución de recursos.
10. Registrar el estado físico del recurso al momento de la devolución.
11. Gestionar mantenimientos periódicos y correctivos.
12. Registrar recursos no localizados y pérdidas.
13. Gestionar procesos de baja de bienes.
14. Realizar inventarios físicos periódicos.
15. Comparar el inventario registrado con la existencia física encontrada.
16. Generar procesos de conciliación de inventario.
17. Mantener un historial de movimientos y cambios.
18. Implementar auditoría sobre las operaciones relevantes del sistema.
19. Generar reportes para apoyar la gestión y toma de decisiones.

---

# 6. Alcance del Sistema

El sistema estará orientado a la gestión de los recursos institucionales pertenecientes a la Oficina Scout de Leonardo Murialdo N.° 1 - Archidona.

Las categorías iniciales consideradas son:

- Mobiliario.
- Recursos tecnológicos.
- Recursos de campamento.
- Material Scout.
- Documentación.
- Recursos de limpieza.
- Recursos de recreación.

El sistema permitirá administrar el ciclo de vida de los recursos desde su registro inicial hasta su baja.

El alcance contempla:

```text
Registro
    ↓
Clasificación
    ↓
Identificación
    ↓
Ubicación
    ↓
Asignación
    ↓
Préstamo
    ↓
Devolución
    ↓
Mantenimiento
    ↓
Inventario físico
    ↓
Conciliación
    ↓
Pérdida
    ↓
Baja
```

---

# 7. Situación Actual

| Aspecto | Situación |
|---|---|
| Tipos de recursos | Mobiliario, tecnológicos, campamento, material Scout, documentación, limpieza y recreación |
| Cantidad existente | Desconocida |
| Ubicación | Principalmente en la oficina; existen recursos perdidos o dañados |
| Responsable general | Superintendente |
| Préstamos | Se realizan ocasionalmente |
| Aprobación | Requiere aprobación documentada y verbal del Jefe de Grupo |
| Registro de préstamos | No existe |
| Registro de devoluciones | No existe |
| Recursos perdidos | Desconocidos |
| Recursos dañados | Desconocidos |
| Mantenimiento | Requerido para determinados recursos |
| Frecuencia de mantenimiento | Periódica según el tipo de recurso |
| Adquisición | Compra y donación |
| Valor económico | Costo de adquisición y valor de avalúo |
| Disponibilidad | Recurso en buen estado y físicamente disponible en la oficina |
| Bajas | No existe un proceso formal |
| Inventario inicial | Debe realizarse mediante levantamiento físico |

---

# 8. Inventario Inicial

El sistema debe considerar que al inicio del proyecto no existe una cantidad confiable de recursos.

Por lo tanto, la base de datos no debe considerarse inicialmente como la representación definitiva del inventario real.

El inventario inicial deberá construirse mediante un proceso de levantamiento físico.

```text
INVENTARIO INICIAL
        │
        ▼
LEVANTAMIENTO FÍSICO
        │
        ▼
IDENTIFICACIÓN DE RECURSOS
        │
        ▼
REGISTRO EN EL SISTEMA
        │
        ▼
VERIFICACIÓN
        │
        ▼
CONCILIACIÓN
        │
        ├── Encontrado
        ├── Dañado
        ├── No localizado
        ├── Perdido
        ├── No identificado
        └── No corresponde
```

El levantamiento inicial debe permitir registrar recursos que:

* Ya se encontraban registrados.
* No estaban registrados.
* Se encuentran dañados.
* No pudieron ser localizados.
* Se encuentran en ubicaciones diferentes a las esperadas.
* No tienen información completa.

---

# 9. Inventario Registrado vs Existencia Física

El sistema debe diferenciar entre:

## Inventario Registrado

Representa los recursos que la organización tiene registrados en el sistema.

## Existencia Física

Representa los recursos encontrados físicamente durante una jornada de inventario.

Ejemplo:

```text
Inventario registrado:

Código: CAM-0001
Nombre: Carpa Scout
Estado administrativo: DISPONIBLE
```

Durante el inventario físico:

```text
Código: CAM-0001
Resultado: ENCONTRADO
Ubicación: Bodega
Estado físico: DAÑADO
```

Resultado de la conciliación:

```text
Estado físico: DAÑADO
Estado administrativo: EN MANTENIMIENTO
```

Otro caso:

```text
Código: CAM-0002
Nombre: Carpa Scout

Inventario:
Registrado

Inventario físico:
No encontrado

Resultado:
NO LOCALIZADO
```

El estado `NO_LOCALIZADO` debe permitir realizar una investigación antes de determinar si el recurso realmente fue perdido.

---

# 10. Categorías de Recursos

El sistema deberá permitir administrar categorías de recursos.

Categorías iniciales:

```text
MOBILIARIO
TECNOLOGÍA
CAMPAMENTO
MATERIAL_SCOUT
DOCUMENTACIÓN
LIMPIEZA
RECREACIÓN
```

Las categorías deben poder ser administradas por usuarios autorizados.

Cada categoría podrá contener múltiples recursos.

---

# 11. Información del Recurso

Cada recurso deberá contar, según corresponda, con información como:

```text
Código único
Nombre
Descripción
Categoría
Marca
Modelo
Número de serie
Fecha de adquisición
Tipo de adquisición
Costo de adquisición
Valor de avalúo
Estado físico
Estado administrativo
Ubicación
Responsable
Fecha de registro
Observaciones
```

El sistema deberá permitir registrar el origen de adquisición.

Tipos de adquisición iniciales:

```text
COMPRA
DONACIÓN
TRANSFERENCIA
OTRO
```

Esto permitirá generar información como:

* Recursos adquiridos mediante compra.
* Recursos recibidos mediante donación.
* Recursos transferidos.
* Recursos de otro origen.

---

# 12. Identificación de Recursos

Cada recurso deberá contar con un código único.

Ejemplos:

```text
MOB-0001
TEC-0001
CAM-0001
SCOUT-0001
DOC-0001
LIM-0001
REC-0001
```

Se recomienda que el sistema permita posteriormente asociar un código QR a cada recurso.

El QR permitirá identificar rápidamente el recurso y consultar información como:

* Código.
* Nombre.
* Categoría.
* Estado.
* Ubicación.
* Responsable.
* Historial.

---

# 13. Estados de los Recursos

Se deben separar dos conceptos:

## Estado físico

Representa la condición física del recurso.

```text
BUENO
REGULAR
DAÑADO
```

## Estado administrativo

Representa la situación administrativa del recurso.

```text
REGISTRADO
DISPONIBLE
ASIGNADO
PRESTADO
EN_MANTENIMIENTO
NO_LOCALIZADO
PERDIDO
DADO_DE_BAJA
```

Ejemplo:

```text
Recurso: CAM-0001

Estado físico:
REGULAR

Estado administrativo:
DISPONIBLE
```

Otro ejemplo:

```text
Recurso: TEC-0001

Estado físico:
BUENO

Estado administrativo:
PRESTADO
```

---

# 14. Regla de Disponibilidad

Un recurso será considerado disponible cuando:

1. Se encuentre físicamente localizado.
2. Se encuentre dentro de una ubicación válida.
3. Su estado físico permita su utilización.
4. No esté prestado.
5. No esté en mantenimiento.
6. No esté perdido.
7. No esté dado de baja.

Por lo tanto:

```text
DISPONIBLE
    =
LOCALIZADO
    +
BUEN ESTADO / ESTADO UTILIZABLE
    +
NO PRESTADO
    +
NO MANTENIMIENTO
    +
NO PERDIDO
    +
NO DADO DE BAJA
```

---

# 15. Responsables y Custodia

El responsable general de la administración y custodia de los bienes es el Superintendente.

El sistema deberá permitir registrar responsables asociados a recursos o procesos específicos.

La gestión de responsables debe permitir conocer:

```text
Recurso
    ↓
Responsable
    ↓
Fecha de asignación
    ↓
Tipo de responsabilidad
```

El sistema debe mantener historial de cambios de responsables.

---

# 16. Gestión de Préstamos

Los préstamos constituyen uno de los procesos principales del sistema.

Actualmente no existe un registro formal de:

* Solicitante.
* Recurso prestado.
* Motivo.
* Autorización.
* Fecha de entrega.
* Fecha prevista de devolución.
* Fecha real de devolución.
* Estado del recurso al regresar.

El sistema deberá controlar el ciclo:

```text
SOLICITUD
    ↓
REVISIÓN
    ↓
APROBACIÓN
    ↓
AUTORIZADO
    ↓
ENTREGA
    ↓
PRESTADO
    ↓
DEVOLUCIÓN
    ↓
INSPECCIÓN
    ↓
FINALIZADO
```

---

# 17. Aprobación de Préstamos

La aprobación del préstamo requiere la intervención del Jefe de Grupo.

El sistema deberá registrar:

```text
Solicitante
Recurso
Motivo
Fecha de solicitud
Fecha prevista de salida
Fecha estimada de devolución
Aprobador
Fecha de aprobación
Tipo de aprobación
Observaciones
```

Tipos de aprobación:

```text
APROBACIÓN_DOCUMENTADA
APROBACIÓN_VERBAL
```

La aprobación verbal puede registrarse como antecedente, pero el sistema deberá permitir formalizar posteriormente la autorización correspondiente.

---

# 18. Entrega de Recursos

La entrega deberá registrar:

```text
Recurso
Solicitante
Responsable de entrega
Fecha de entrega
Estado físico de salida
Observaciones
```

Una vez registrada la entrega:

```text
Estado administrativo:
PRESTADO
```

El sistema deberá evitar que un mismo recurso sea entregado simultáneamente a diferentes préstamos activos.

---

# 19. Devolución de Recursos

La devolución deberá registrar:

```text
Fecha de devolución
Responsable que recibe
Estado físico de devolución
Observaciones
Daños detectados
```

Después de la devolución, el sistema deberá determinar el siguiente estado del recurso.

Ejemplo:

```text
DEVOLUCIÓN
    │
    ├── Buen estado
    │       ↓
    │   DISPONIBLE
    │
    ├── Daño detectado
    │       ↓
    │   EN_MANTENIMIENTO
    │
    └── No devolución
            ↓
        NO_LOCALIZADO
```

---

# 20. Gestión de Mantenimiento

El sistema deberá gestionar mantenimiento preventivo y correctivo.

Cada registro de mantenimiento podrá contener:

```text
Recurso
Tipo de mantenimiento
Descripción
Fecha programada
Fecha realizada
Responsable
Costo
Resultado
Observaciones
Próxima fecha de mantenimiento
```

Estados de mantenimiento:

```text
PROGRAMADO
PENDIENTE
EN_PROCESO
COMPLETADO
VENCIDO
CANCELADO
```

El sistema deberá permitir visualizar:

```text
Mantenimiento próximo
Mantenimiento vencido
Mantenimiento en proceso
Mantenimiento completado
```

Los mantenimientos pueden ser periódicos según el recurso.

Ejemplo:

```text
Carpa CAM-0001

Frecuencia:
6 meses

Último mantenimiento:
01/01/2026

Próximo mantenimiento:
01/07/2026
```

---

# 21. Gestión de Recursos No Localizados

Cuando un recurso registrado no sea encontrado durante un inventario físico, deberá pasar temporalmente a:

```text
NO_LOCALIZADO
```

Flujo:

```text
INVENTARIO FÍSICO
       ↓
NO ENCONTRADO
       ↓
NO_LOCALIZADO
       ↓
INVESTIGACIÓN
       │
       ├── ENCONTRADO
       │
       ├── CONFIRMADO PERDIDO
       │
       └── PENDIENTE
```

No se deberá establecer inmediatamente el estado `PERDIDO`.

El sistema debe conservar el historial del proceso.

---

# 22. Gestión de Pérdidas

Cuando un recurso sea confirmado como perdido, deberá registrarse:

```text
Recurso
Fecha
Responsable
Circunstancias
Descripción
Observaciones
Documento de respaldo
```

El sistema deberá conservar el historial de la pérdida.

---

# 23. Gestión de Bajas

Los recursos dañados, obsoletos o inutilizables podrán iniciar un proceso de baja.

Flujo:

```text
RECURSO
    ↓
DAÑADO / OBSOLETO
    ↓
SOLICITUD DE BAJA
    ↓
REVISIÓN
    ↓
AUTORIZACIÓN
    ↓
DADO_DE_BAJA
```

El proceso deberá registrar:

```text
Recurso
Fecha
Motivo
Responsable
Autorizador
Valor de avalúo
Observaciones
Documento de respaldo
```

La baja requiere autorización.

Una vez dado de baja, el recurso no podrá volver a estar disponible mediante una operación ordinaria.

---

# 24. Inventario Físico

El inventario físico será un módulo central del sistema.

Una jornada de inventario deberá permitir:

1. Crear una jornada de inventario.
2. Definir la fecha.
3. Seleccionar una o varias ubicaciones.
4. Asignar responsables.
5. Consultar recursos esperados.
6. Escanear códigos QR.
7. Confirmar existencia.
8. Registrar estado físico.
9. Registrar ubicación real.
10. Registrar observaciones.
11. Finalizar el inventario.
12. Generar diferencias.
13. Realizar conciliación.

Flujo:

```text
CREAR INVENTARIO
        ↓
SELECCIONAR UBICACIÓN
        ↓
VERIFICAR RECURSOS
        ↓
ESCANEAR / REGISTRAR
        ↓
CONFIRMAR EXISTENCIA
        ↓
REGISTRAR ESTADO
        ↓
FINALIZAR
        ↓
CONCILIACIÓN
```

---

# 25. Conciliación de Inventario

La conciliación deberá comparar:

```text
INVENTARIO REGISTRADO
        +
EXISTENCIA FÍSICA
        ↓
CONCILIACIÓN
```

Resultados posibles:

```text
ENCONTRADO
NO_LOCALIZADO
DAÑADO
NUEVO
NO_IDENTIFICADO
NO_CORRESPONDE
```

Ejemplo:

```text
INVENTARIO FÍSICO 2026

Recursos registrados: 150
Encontrados: 140
No localizados: 7
Dañados: 3
Nuevos: 2
```

---

# 26. Movimientos

El sistema deberá conservar el historial de movimientos de los recursos.

Tipos de movimiento:

```text
REGISTRO
ASIGNACIÓN
PRÉSTAMO
DEVOLUCIÓN
TRASLADO
MANTENIMIENTO
PÉRDIDA
RECUPERACIÓN
BAJA
```

Cada movimiento deberá registrar:

```text
Recurso
Tipo de movimiento
Fecha
Usuario
Ubicación anterior
Ubicación nueva
Responsable anterior
Responsable nuevo
Observaciones
```

El historial no deberá eliminarse mediante operaciones ordinarias.

---

# 27. Auditoría

El sistema deberá mantener una bitácora de operaciones relevantes.

Ejemplo:

```text
Usuario:
Administrador

Acción:
MODIFICAR_RECURSO

Recurso:
TEC-0001

Fecha:
2026-07-25

Campo:
Responsable

Valor anterior:
Juan Pérez

Valor nuevo:
Carlos Pérez
```

La auditoría deberá permitir conocer:

- Quién realizó la acción.
- Qué acción realizó.
- Cuándo la realizó.
- Sobre qué recurso.
- Qué información cambió.

Las operaciones críticas deberán ser auditables.

---

# 28. Actores del Sistema

## Administrador

- Gestionar usuarios.
- Gestionar roles.
- Gestionar permisos.
- Configurar categorías.
- Gestionar ubicaciones.
- Gestionar recursos.
- Consultar auditoría.

## Superintendente

- Administrar y custodiar los bienes.
- Registrar recursos.
- Gestionar inventarios.
- Gestionar préstamos.
- Registrar entregas y devoluciones.
- Gestionar mantenimientos.
- Consultar reportes.

## Jefe de Grupo

- Aprobar solicitudes de préstamo.
- Autorizar procesos determinados.
- Revisar procesos de baja.
- Consultar información institucional.

## Custodio / Responsable

- Consultar recursos asignados.
- Confirmar recepción.
- Reportar daños.
- Reportar pérdidas.

## Solicitante

- Solicitar préstamos.
- Consultar sus solicitudes.
- Consultar el estado de sus préstamos.

---

# 29. Módulos Funcionales

1. Autenticación y Autorización
2. Dashboard
3. Gestión de Recursos
4. Gestión de Categorías
5. Gestión de Ubicaciones
6. Gestión de Responsables
7. Gestión de Préstamos
8. Gestión de Devoluciones
9. Gestión de Mantenimiento
10. Gestión de Inventario Físico
11. Conciliación de Inventario
12. Gestión de Pérdidas
13. Gestión de Bajas
14. Gestión de Movimientos
15. Gestión de Auditoría
16. Reportes
17. Gestión de Documentos

---

# 30. Casos de Uso Principales

## Recursos

- Registrar recurso.
- Consultar recurso.
- Actualizar recurso.
- Asignar recurso.
- Trasladar recurso.
- Consultar historial.
- Generar código QR.

## Inventario

- Crear inventario físico.
- Registrar existencia.
- Registrar recurso no localizado.
- Registrar recurso dañado.
- Conciliar inventario.
- Generar diferencias.

## Préstamos

- Solicitar préstamo.
- Revisar solicitud.
- Aprobar préstamo.
- Rechazar préstamo.
- Registrar entrega.
- Registrar devolución.
- Consultar préstamos pendientes.

## Mantenimiento

- Programar mantenimiento.
- Registrar mantenimiento.
- Completar mantenimiento.
- Consultar mantenimientos vencidos.
- Consultar historial de mantenimiento.

## Pérdidas

- Registrar recurso no localizado.
- Registrar investigación.
- Confirmar pérdida.
- Registrar recuperación.

## Bajas

- Solicitar baja.
- Revisar solicitud.
- Autorizar baja.
- Registrar baja.

## Reportes

- Inventario general.
- Recursos por categoría.
- Recursos por ubicación.
- Recursos por estado.
- Recursos prestados.
- Préstamos vencidos.
- Mantenimientos pendientes.
- Recursos no localizados.
- Recursos perdidos.
- Recursos dados de baja.
- Historial de movimientos.

---

# 31. Reglas de Negocio

### RN-01 — Disponibilidad
Un recurso solo puede estar disponible si está físicamente localizado y en condiciones de uso.

### RN-02 — Préstamo
Un recurso no puede tener más de un préstamo activo simultáneamente.

### RN-03 — Estado
Un recurso dado de baja no puede volver a estar disponible mediante una operación ordinaria.

### RN-04 — Mantenimiento
Un recurso en mantenimiento no puede ser prestado.

### RN-05 — Pérdida
Un recurso no localizado deberá permanecer en investigación antes de ser marcado como perdido.

### RN-06 — Préstamo
Todo préstamo debe estar asociado a un solicitante.

### RN-07 — Autorización
Todo préstamo debe contar con una autorización registrada.

### RN-08 — Devolución
Toda devolución debe registrar el estado físico del recurso.

### RN-09 — Baja
Toda baja requiere autorización.

### RN-10 — Auditoría
Las operaciones críticas deben quedar registradas en la auditoría.

### RN-11 — Inventario
Un inventario físico debe mantener el resultado de la verificación realizada.

### RN-12 — Historial
Los movimientos históricos no deben eliminarse mediante operaciones ordinarias.

---

# 32. Arquitectura del Sistema

```text
                         ANGULAR
                         FRONTEND
                             │
                             │ REST / JSON
                             ▼
┌──────────────────────────────────────────────┐
│              PRESENTATION                    │
│  Controllers │ DTOs │ Middleware │ Validation │
└──────────────────────┬───────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────┐
│              APPLICATION                    │
│  Commands │ Queries │ Use Cases │ Interfaces  │
└──────────────────────┬───────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────┐
│                 DOMAIN                       │
│  Entities │ Rules │ Enums │ Value Objects     │
└──────────────────────▲───────────────────────┘
                       │
┌──────────────────────┴───────────────────────┐
│              INFRASTRUCTURE                  │
│  EF Core │ SQL Server │ Identity │ Storage   │
└──────────────────────────────────────────────┘
```

---

# 33. Arquitectura Backend

```text
ScoutInventory.Api
ScoutInventory.Application
ScoutInventory.Domain
ScoutInventory.Infrastructure
ScoutInventory.Tests
```

Dependencias:

```text
ScoutInventory.Api → ScoutInventory.Application → ScoutInventory.Domain
ScoutInventory.Infrastructure → ScoutInventory.Domain
```

---

# 34-42. (Ver documentos de requisitos y diseño)

---

# 43. Orden de Implementación

1. Requisitos
2. Actores y permisos
3. Casos de uso
4. Reglas de negocio
5. Modelo de dominio
6. Modelo entidad-relación
7. Configuración de infraestructura
8. Backend base
9. Autenticación
10. Gestión de recursos
11. Inventario físico
12. Conciliación
13. Préstamos
14. Devoluciones
15. Mantenimiento
16. Pérdidas
17. Bajas
18. Movimientos
19. Auditoría
20. Reportes
21. Dashboard
22. Pruebas
23. Despliegue

---

# 44. Resultado Esperado

Al finalizar el proyecto, la Oficina Scout de Leonardo Murialdo N.° 1 - Archidona deberá disponer de un sistema que permita conocer los recursos institucionales existentes, identificarlos de manera única, conocer su ubicación y responsable, gestionar préstamos, devoluciones, mantenimientos, pérdidas y bajas, realizar inventarios físicos, conciliar información, consultar historial de movimientos, generar reportes y mantener auditoría.

---

# 45. Principio General

El sistema no debe ser considerado únicamente como un CRUD para registrar bienes. Su propósito principal es proporcionar una plataforma para identificar, controlar, custodiar y dar seguimiento al ciclo de vida de los recursos institucionales.
