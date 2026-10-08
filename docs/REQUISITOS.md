# Requisitos del Sistema
## Sistema de Gestión, Control y Trazabilidad de Recursos
### Oficina Scout de Leonardo Murialdo N.° 1 - Archidona

---

## 1. Información del Documento

| Campo | Descripción |
|---|---|
| **Proyecto** | Sistema de Gestión, Control y Trazabilidad de Recursos |
| **Documento** | Especificación de Requisitos |
| **Versión** | 1.0 |
| **Fecha** | 2026-07-25 |
| **Estado** | Borrador |

---

## 2. Actores del Sistema

| ID | Actor | Descripción | Responsabilidades |
|---|---|---|---|
| **ACT-01** | Administrador | Usuario con control total del sistema | Gestionar usuarios, roles, permisos, categorías, ubicaciones, recursos. Consultar auditoría. |
| **ACT-02** | Superintendente | Responsable de la administración y custodia de los bienes | Registrar recursos, gestionar inventarios, gestionar préstamos, gestionar mantenimientos, consultar reportes. |
| **ACT-03** | Jefe de Grupo | Autoridad que aprueba operaciones críticas | Aprobar solicitudes de préstamo, autorizar bajas, revisar procesos. |
| **ACT-04** | Custodio | Usuario responsable de recursos asignados | Consultar recursos asignados, confirmar recepción, reportar daños y pérdidas. |
| **ACT-05** | Solicitante | Usuario que solicita préstamos | Solicitar préstamos, consultar estado de solicitudes. |

---

## 3. Matriz de Permisos

| Módulo | Acción | ADMIN | SUPERINTENDENTE | JEFE_GRUPO | CUSTODIO | SOLICITANTE |
|---|---|---|---|---|---|---|
| **Usuarios** | Crear | ✓ | | | | |
| | Modificar | ✓ | | | | |
| | Eliminar | ✓ | | | | |
| | Consultar | ✓ | ✓ | | | |
| **Roles** | Gestionar | ✓ | | | | |
| **Categorías** | Crear | ✓ | ✓ | | | |
| | Modificar | ✓ | ✓ | | | |
| | Eliminar | ✓ | ✓ | | | |
| | Consultar | ✓ | ✓ | ✓ | ✓ | ✓ |
| **Ubicaciones** | Crear | ✓ | ✓ | | | |
| | Modificar | ✓ | ✓ | | | |
| | Eliminar | ✓ | ✓ | | | |
| | Consultar | ✓ | ✓ | ✓ | ✓ | ✓ |
| **Recursos** | Registrar | ✓ | ✓ | | | |
| | Modificar | ✓ | ✓ | | | |
| | Consultar | ✓ | ✓ | ✓ | ✓ | ✓ |
| | Asignar | ✓ | ✓ | | | |
| | Trasladar | ✓ | ✓ | | | |
| | Dar de baja | | ✓ | ✓ | | |
| **Préstamos** | Solicitar | ✓ | ✓ | ✓ | ✓ | ✓ |
| | Aprobar | | | ✓ | | |
| | Rechazar | | | ✓ | | |
| | Entregar | ✓ | ✓ | | | |
| | Recibir devolución | ✓ | ✓ | | | |
| | Consultar | ✓ | ✓ | ✓ | ✓ | ✓ |
| **Mantenimiento** | Programar | ✓ | ✓ | | | |
| | Registrar | ✓ | ✓ | | | |
| | Completar | ✓ | ✓ | | | |
| | Consultar | ✓ | ✓ | ✓ | ✓ | ✓ |
| **Inventario Físico** | Crear jornada | ✓ | ✓ | | | |
| | Registrar hallazgos | ✓ | ✓ | | | |
| | Conciliar | ✓ | ✓ | | | |
| | Consultar | ✓ | ✓ | ✓ | | |
| **Pérdidas** | Registrar | ✓ | ✓ | | | |
| | Confirmar | | ✓ | ✓ | | |
| | Recuperar | ✓ | ✓ | | | |
| **Bajas** | Solicitar | | ✓ | | | |
| | Autorizar | | | ✓ | | |
| | Registrar | ✓ | ✓ | | | |
| **Reportes** | Generar | ✓ | ✓ | ✓ | ✓ | |
| **Auditoría** | Consultar | ✓ | ✓ | | | |

---

## 4. Requisitos Funcionales

### 4.1 Módulo de Autenticación y Autorización

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-001** | Inicio de sesión | El sistema debe permitir a los usuarios autenticarse mediante credenciales (usuario y contraseña) y generar un token JWT. | Todos |
| **RF-002** | Cierre de sesión | El sistema debe permitir a los usuarios cerrar sesión invalidando el token activo. | Todos |
| **RF-003** | Gestión de usuarios | El sistema debe permitir al Administrador crear, modificar, eliminar y consultar usuarios del sistema. | ADMIN |
| **RF-004** | Gestión de roles | El sistema debe permitir al Administrador crear y asignar roles a los usuarios. | ADMIN |
| **RF-005** | Control de acceso por rol | El sistema debe restringir el acceso a funcionalidades según el rol del usuario autenticado. | Todos |
| **RF-006** | Recuperación de contraseña | El sistema debe permitir a los usuarios solicitar un restablecimiento de contraseña. | Todos |

### 4.2 Módulo de Dashboard

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-007** | Dashboard principal | El sistema debe mostrar un panel resumen con indicadores clave: total de recursos, disponibles, prestados, en mantenimiento, dañados, no localizados, perdidos y dados de baja. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |

### 4.3 Módulo de Gestión de Recursos

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-008** | Registrar recurso | El sistema debe permitir registrar un nuevo recurso con código único, nombre, descripción, categoría, marca, modelo, número de serie, fecha de adquisición, tipo de adquisición, costo de adquisición, valor de avalúo, estado físico, estado administrativo, ubicación, responsable y observaciones. | ADMIN, SUPERINTENDENTE |
| **RF-009** | Generar código único | El sistema debe generar automáticamente un código único para cada recurso basado en el prefijo de la categoría y un correlativo (ej. MOB-0001, TEC-0001). | Sistema |
| **RF-010** | Consultar recurso | El sistema debe permitir consultar recursos por código, nombre, categoría, ubicación, estado, responsable y texto libre. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO, CUSTODIO, SOLICITANTE |
| **RF-011** | Modificar recurso | El sistema debe permitir modificar los datos de un recurso existente, manteniendo un registro de auditoría de los cambios. | ADMIN, SUPERINTENDENTE |
| **RF-012** | Asignar responsable | El sistema debe permitir asignar un responsable a un recurso, registrando la fecha y tipo de responsabilidad. | ADMIN, SUPERINTENDENTE |
| **RF-013** | Trasladar recurso | El sistema debe permitir registrar el traslado de un recurso entre ubicaciones, conservando el historial de ubicaciones anteriores. | ADMIN, SUPERINTENDENTE |
| **RF-014** | Consultar historial del recurso | El sistema debe permitir consultar el historial completo de movimientos, cambios de estado, responsables, ubicaciones y préstamos de un recurso. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-015** | Generar código QR | El sistema debe generar un código QR para cada recurso que permita acceder a su información básica. | ADMIN, SUPERINTENDENTE |

### 4.4 Módulo de Gestión de Categorías

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-016** | Crear categoría | El sistema debe permitir crear categorías de recursos con nombre, descripción y prefijo para código único. | ADMIN, SUPERINTENDENTE |
| **RF-017** | Modificar categoría | El sistema debe permitir modificar los datos de una categoría existente. | ADMIN, SUPERINTENDENTE |
| **RF-018** | Eliminar categoría | El sistema debe permitir eliminar una categoría siempre que no tenga recursos asociados. | ADMIN, SUPERINTENDENTE |
| **RF-019** | Consultar categorías | El sistema debe permitir consultar el listado de categorías. | Todos |

### 4.5 Módulo de Gestión de Ubicaciones

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-020** | Crear ubicación | El sistema debe permitir registrar ubicaciones físicas (oficina, bodega, sala de reuniones, etc.) con nombre y descripción. | ADMIN, SUPERINTENDENTE |
| **RF-021** | Modificar ubicación | El sistema debe permitir modificar los datos de una ubicación existente. | ADMIN, SUPERINTENDENTE |
| **RF-022** | Eliminar ubicación | El sistema debe permitir eliminar una ubicación siempre que no tenga recursos asociados. | ADMIN, SUPERINTENDENTE |
| **RF-023** | Consultar ubicaciones | El sistema debe permitir consultar el listado de ubicaciones. | Todos |

### 4.6 Módulo de Gestión de Responsables

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-024** | Registrar responsable | El sistema debe permitir asignar un usuario como responsable de un recurso, registrando la fecha y tipo de responsabilidad. | ADMIN, SUPERINTENDENTE |
| **RF-025** | Cambiar responsable | El sistema debe permitir cambiar el responsable de un recurso, manteniendo el historial de responsables anteriores. | ADMIN, SUPERINTENDENTE |
| **RF-026** | Consultar responsables | El sistema debe permitir consultar los recursos asignados a un responsable específico. | ADMIN, SUPERINTENDENTE, CUSTODIO |

### 4.7 Módulo de Gestión de Préstamos

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-027** | Solicitar préstamo | El sistema debe permitir a un usuario solicitar el préstamo de uno o más recursos, indicando motivo, fecha prevista de salida y fecha estimada de devolución. | Todos |
| **RF-028** | Revisar solicitud | El sistema debe permitir al Jefe de Grupo revisar las solicitudes de préstamo pendientes. | JEFE_GRUPO |
| **RF-029** | Aprobar préstamo | El sistema debe permitir al Jefe de Grupo aprobar una solicitud de préstamo, registrando el tipo de aprobación (documentada o verbal) y observaciones. | JEFE_GRUPO |
| **RF-030** | Rechazar préstamo | El sistema debe permitir al Jefe de Grupo rechazar una solicitud de préstamo indicando el motivo. | JEFE_GRUPO |
| **RF-031** | Registrar entrega | El sistema debe permitir registrar la entrega física del recurso al solicitante, capturando el estado físico de salida y la fecha de entrega. Al entregar, el estado administrativo del recurso pasa a PRESTADO. | ADMIN, SUPERINTENDENTE |
| **RF-032** | Registrar devolución | El sistema debe permitir registrar la devolución del recurso, capturando el estado físico de devolución, daños detectados y observaciones. | ADMIN, SUPERINTENDENTE |
| **RF-033** | Control de préstamos activos | El sistema debe impedir que un recurso sea prestado si ya tiene un préstamo activo. | Sistema |
| **RF-034** | Consultar préstamos | El sistema debe permitir consultar préstamos por estado, solicitante, recurso y fechas. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-035** | Notificar préstamos vencidos | El sistema debe notificar cuando un préstamo excede la fecha estimada de devolución. | Sistema |

### 4.8 Módulo de Gestión de Mantenimiento

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-036** | Programar mantenimiento | El sistema debe permitir programar mantenimientos preventivos para un recurso, indicando tipo, descripción, fecha programada y responsable. | ADMIN, SUPERINTENDENTE |
| **RF-037** | Registrar mantenimiento | El sistema debe permitir registrar la ejecución de un mantenimiento, capturando fecha realizada, costo, resultado y observaciones. | ADMIN, SUPERINTENDENTE |
| **RF-038** | Completar mantenimiento | Al completar un mantenimiento, el sistema debe actualizar el estado administrativo del recurso según el resultado. | ADMIN, SUPERINTENDENTE |
| **RF-039** | Consultar mantenimientos | El sistema debe permitir consultar mantenimientos por estado, recurso, fechas y tipo. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-040** | Alertar mantenimientos vencidos | El sistema debe alertar cuando un mantenimiento programado no se haya realizado en la fecha prevista. | Sistema |
| **RF-041** | Definir frecuencia de mantenimiento | El sistema debe permitir definir la frecuencia de mantenimiento para un recurso y calcular automáticamente la próxima fecha. | ADMIN, SUPERINTENDENTE |

### 4.9 Módulo de Inventario Físico

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-042** | Crear jornada de inventario | El sistema debe permitir crear una jornada de inventario con fecha, ubicaciones a cubrir y responsables asignados. | ADMIN, SUPERINTENDENTE |
| **RF-043** | Consultar recursos esperados | El sistema debe mostrar los recursos que se espera encontrar en cada ubicación seleccionada para la jornada. | ADMIN, SUPERINTENDENTE |
| **RF-044** | Registrar existencia | El sistema debe permitir registrar qué recursos fueron encontrados durante el inventario, capturando su estado físico real y ubicación real. | ADMIN, SUPERINTENDENTE |
| **RF-045** | Registrar recurso no encontrado | El sistema debe permitir marcar un recurso como no encontrado durante el inventario. | ADMIN, SUPERINTENDENTE |
| **RF-046** | Registrar recurso no registrado | El sistema debe permitir registrar recursos encontrados físicamente que no estaban registrados previamente en el sistema. | ADMIN, SUPERINTENDENTE |
| **RF-047** | Finalizar jornada | El sistema debe permitir cerrar una jornada de inventario y generar las diferencias encontradas. | ADMIN, SUPERINTENDENTE |
| **RF-048** | Consultar inventarios | El sistema debe permitir consultar jornadas de inventario anteriores y sus resultados. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |

### 4.10 Módulo de Conciliación de Inventario

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-049** | Conciliar inventario | El sistema debe comparar el inventario registrado con la existencia física encontrada y generar un reporte de conciliación. | ADMIN, SUPERINTENDENTE |
| **RF-050** | Resultados de conciliación | El sistema debe clasificar cada recurso en: ENCONTRADO, NO_LOCALIZADO, DAÑADO, NUEVO, NO_IDENTIFICADO, NO_CORRESPONDE. | Sistema |
| **RF-051** | Aplicar conciliación | El sistema debe permitir aplicar los resultados de la conciliación, actualizando los estados de los recursos según corresponda. | ADMIN, SUPERINTENDENTE |

### 4.11 Módulo de Gestión de Pérdidas

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-052** | Registrar no localizado | El sistema debe permitir registrar un recurso como NO_LOCALIZADO cuando no sea encontrado durante un inventario. | ADMIN, SUPERINTENDENTE |
| **RF-053** | Registrar investigación | El sistema debe permitir registrar el proceso de investigación de un recurso no localizado. | ADMIN, SUPERINTENDENTE |
| **RF-054** | Confirmar pérdida | El sistema debe permitir confirmar un recurso como PERDIDO después del proceso de investigación, capturando circunstancias, responsable y documento de respaldo. | SUPERINTENDENTE, JEFE_GRUPO |
| **RF-055** | Registrar recuperación | El sistema debe permitir registrar la recuperación de un recurso previamente marcado como perdido o no localizado. | ADMIN, SUPERINTENDENTE |

### 4.12 Módulo de Gestión de Bajas

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-056** | Solicitar baja | El sistema debe permitir al Superintendente solicitar la baja de un recurso, indicando motivo, responsable y valor de avalúo. | SUPERINTENDENTE |
| **RF-057** | Revisar solicitud de baja | El sistema debe permitir al Jefe de Grupo revisar las solicitudes de baja pendientes. | JEFE_GRUPO |
| **RF-058** | Autorizar baja | El sistema debe permitir al Jefe de Grupo autorizar o rechazar una solicitud de baja. | JEFE_GRUPO |
| **RF-059** | Registrar baja | El sistema debe permitir registrar la baja definitiva de un recurso autorizado, cambiando su estado administrativo a DADO_DE_BAJA. | ADMIN, SUPERINTENDENTE |

### 4.13 Módulo de Movimientos

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-060** | Registrar movimiento | El sistema debe registrar automáticamente un movimiento cada vez que un recurso cambie de estado, ubicación, responsable o sea parte de un préstamo, mantenimiento, pérdida o baja. | Sistema |
| **RF-061** | Consultar movimientos | El sistema debe permitir consultar el historial de movimientos de un recurso específico o de todos los recursos por fechas y tipo de movimiento. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |

### 4.14 Módulo de Auditoría

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-062** | Registrar auditoría | El sistema debe registrar automáticamente en una bitácora las operaciones críticas: creación, modificación y eliminación de recursos, cambios de estado, préstamos, devoluciones, bajas y modificaciones de usuarios. | Sistema |
| **RF-063** | Consultar auditoría | El sistema debe permitir consultar la bitácora de auditoría filtrada por usuario, acción, recurso, fechas y campos modificados. | ADMIN, SUPERINTENDENTE |

### 4.15 Módulo de Reportes

| ID | Nombre | Descripción | Actor |
|---|---|---|---|
| **RF-064** | Reporte de inventario general | El sistema debe generar un reporte de todos los recursos con su información principal. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-065** | Reporte por categoría | El sistema debe generar un reporte de recursos filtrados por categoría. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-066** | Reporte por ubicación | El sistema debe generar un reporte de recursos filtrados por ubicación. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-067** | Reporte por estado | El sistema debe generar un reporte de recursos filtrados por estado físico o administrativo. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-068** | Reporte de préstamos activos | El sistema debe generar un reporte de todos los préstamos actualmente activos. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-069** | Reporte de préstamos vencidos | El sistema debe generar un reporte de préstamos cuya fecha de devolución ha excedido. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-070** | Reporte de mantenimientos | El sistema debe generar un reporte de mantenimientos pendientes, vencidos y completados. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |
| **RF-071** | Reporte de pérdidas y bajas | El sistema debe generar un reporte de recursos perdidos y dados de baja. | ADMIN, SUPERINTENDENTE, JEFE_GRUPO |

---

## 5. Requisitos No Funcionales

| ID | Nombre | Descripción |
|---|---|---|
| **RNF-001** | Disponibilidad | El sistema debe estar disponible al menos el 99% del tiempo en horario laboral. |
| **RNF-002** | Tiempo de respuesta | Las consultas de listados no deben superar los 3 segundos. Las operaciones de creación y modificación no deben superar los 2 segundos. |
| **RNF-003** | Seguridad - Autenticación | El sistema debe usar JWT con expiración configurable para la autenticación de usuarios. |
| **RNF-004** | Seguridad - Contraseñas | Las contraseñas deben almacenarse utilizando hashing seguro (ASP.NET Core Identity con PBKDF2). |
| **RNF-005** | Seguridad - Autorización | El sistema debe validar los permisos del usuario en cada operación del backend, no solo en el frontend. |
| **RNF-006** | Seguridad - HTTPS | Todo el tráfico debe ser cifrado mediante HTTPS. |
| **RNF-007** | Integridad de datos | El sistema debe usar transacciones de base de datos para operaciones que afecten múltiples entidades. |
| **RNF-008** | Auditoría | Las operaciones críticas deben registrarse automáticamente con información del usuario, acción, fecha, valores anteriores y nuevos. |
| **RNF-009** | Escalabilidad | La arquitectura debe permitir escalar horizontalmente añadiendo instancias del backend. |
| **RNF-010** | Mantenibilidad | El código debe organizarse en capas (Domain, Application, Infrastructure, Api) siguiendo principios SOLID. |
| **RNF-011** | Portabilidad | El sistema debe ejecutarse en contenedores Docker para facilitar su despliegue en diferentes entornos. |
| **RNF-012** | Navegadores | El frontend debe ser compatible con las versiones actuales de Chrome, Firefox, Edge y Safari. |
| **RNF-013** | Diseño responsive | La interfaz debe adaptarse a dispositivos móviles y de escritorio. |
| **RNF-014** | Backup | La base de datos debe contar con un plan de copias de seguridad automatizadas. |

---

## 6. Reglas de Negocio Formalizadas

| ID | Nombre | Descripción | Disparador |
|---|---|---|---|
| **RN-01** | Disponibilidad | Un recurso solo puede tener estado administrativo DISPONIBLE si: (a) está localizado, (b) está en una ubicación válida, (c) su estado físico es BUENO o REGULAR, (d) no tiene un préstamo activo, (e) no está en mantenimiento, (f) no está marcado como perdido, (g) no está dado de baja. | Al modificar estado del recurso |
| **RN-02** | Préstamo único activo | Un recurso no puede tener más de un préstamo en estado APROBADO, ENTREGADO o PRESTADO simultáneamente. | Al aprobar o entregar préstamo |
| **RN-03** | Baja irreversible | Un recurso con estado DADO_DE_BAJA no puede volver a DISPONIBLE mediante una operación ordinaria. Solo mediante una operación administrativa especial con auditoría. | Al modificar estado del recurso |
| **RN-04** | Mantenimiento bloquea préstamo | Un recurso con estado administrativo EN_MANTENIMIENTO no puede ser prestado. | Al solicitar préstamo |
| **RN-05** | Investigación previa a pérdida | Un recurso debe permanecer en estado NO_LOCALIZADO al menos 30 días antes de poder ser marcado como PERDIDO, a menos que el Jefe de Grupo autorice excepcionalmente el cambio. | Al confirmar pérdida |
| **RN-06** | Solicitante obligatorio | Todo préstamo debe estar asociado a un solicitante registrado en el sistema. | Al crear solicitud |
| **RN-07** | Autorización obligatoria | Todo préstamo debe contar con una autorización registrada (APROBACIÓN_DOCUMENTADA o APROBACIÓN_VERBAL) antes de la entrega del recurso. | Al entregar recurso |
| **RN-08** | Devolución con inspección | Toda devolución debe registrar el estado físico del recurso al momento de ser devuelto. | Al registrar devolución |
| **RN-09** | Baja requiere autorización | Toda baja debe ser autorizada por el Jefe de Grupo antes de su ejecución. | Al dar de baja |
| **RN-10** | Auditoría obligatoria | Las siguientes operaciones deben quedar registradas en la bitácora de auditoría: creación, modificación y eliminación de recursos; cambios de estado administrativo; creación y modificación de préstamos; cambios de responsable; creación y autorización de bajas. | Al ejecutar operación crítica |
| **RN-11** | Integridad del inventario | Un inventario físico debe mantener el resultado de la verificación de cada recurso, sin permitir eliminar registros de verificación una vez finalizada la jornada. | Al finalizar jornada |
| **RN-12** | Inmutabilidad del historial | Los movimientos históricos no pueden ser eliminados ni modificados mediante operaciones ordinarias del sistema. | Al intentar eliminar movimiento |
| **RN-13** | Código único | El código de recurso es único e irrepetible en todo el sistema. No puede reasignarse a otro recurso aunque el original sea dado de baja. | Al crear recurso |
| **RN-14** | Estado físico inicial | Al registrar un recurso, el estado físico es obligatorio. Al registrar una devolución o un hallazgo de inventario, el estado físico debe ser evaluado y registrado. | Al crear o modificar recurso |

---

## 7. Flujos de Estado

### 7.1 Flujo de Estados del Recurso (Resource)

```
                ┌─────────────┐
                │  REGISTRADO  │
                └──────┬──────┘
                       │
                       ▼
                ┌─────────────┐
         ┌─────▶│  DISPONIBLE  │◀────┐
         │      └──────┬──────┘     │
         │             │            │
         │             ▼            │
         │      ┌───────────┐       │
         │      │  ASIGNADO  │       │
         │      └─────┬─────┘       │
         │            │             │
         │            ▼             │
         │      ┌───────────┐       │
         │      │  PRESTADO  │       │
         │      └─────┬─────┘       │
         │            │             │
         │            ▼             │
         │  ┌──────────────────┐    │
         │  │ EN_MANTENIMIENTO  │────┘
         │  └────────┬─────────┘
         │           │
         │           ▼
         │  ┌───────────────┐
         │  │ NO_LOCALIZADO  │
         │  └───────┬───────┘
         │          │
         │          ▼
         │  ┌───────────┐
         │  │  PERDIDO   │
         │  └─────┬─────┘
         │        │
         │        ▼
         │  ┌───────────────┐
         └──┤ DADO_DE_BAJA  │
            └───────────────┘
```

**Transiciones permitidas:**

| Desde | Hacia | Condición |
|---|---|---|
| REGISTRADO | DISPONIBLE | Automático al completar registro |
| DISPONIBLE | ASIGNADO | Se asigna responsable |
| DISPONIBLE | PRESTADO | Se entrega en préstamo |
| DISPONIBLE | EN_MANTENIMIENTO | Se inicia mantenimiento |
| DISPONIBLE | NO_LOCALIZADO | No encontrado en inventario |
| ASIGNADO | DISPONIBLE | Se desasigna responsable |
| ASIGNADO | PRESTADO | Se entrega en préstamo |
| PRESTADO | DISPONIBLE | Devuelto en buen estado |
| PRESTADO | EN_MANTENIMIENTO | Devuelto con daños |
| PRESTADO | NO_LOCALIZADO | No devuelto |
| EN_MANTENIMIENTO | DISPONIBLE | Mantenimiento completado |
| NO_LOCALIZADO | DISPONIBLE | Encontrado |
| NO_LOCALIZADO | PERDIDO | Confirmado perdido tras investigación |
| PERDIDO | DISPONIBLE | Recuperado |
| CUALQUIERA | DADO_DE_BAJA | Baja autorizada |

### 7.2 Flujo de Estados del Préstamo (Loan)

```
┌───────────┐
│ SOLICITADO │
└──────┬────┘
       │
       ├──────────────────────┐
       ▼                      ▼
┌───────────┐          ┌───────────┐
│ APROBADO   │          │ RECHAZADO  │
└──────┬────┘          └───────────┘
       │
       ▼
┌───────────┐
│ ENTREGADO  │
└──────┬────┘
       │
       ▼
┌───────────┐
│ PRESTADO   │
└──────┬────┘
       │
       ▼
┌───────────┐
│ DEVUELTO   │
└──────┬────┘
       │
       ▼
┌───────────┐
│ FINALIZADO │
└───────────┘
```

### 7.3 Flujo de Estados del Mantenimiento (Maintenance)

```
┌─────────────┐
│  PROGRAMADO  │
└──────┬──────┘
       │
       ▼
┌───────────┐
│  PENDIENTE │
└──────┬────┘
       │
       ▼
┌───────────┐
│ EN_PROCESO │
└──────┬────┘
       │
       ├──────────────────┐
       ▼                  ▼
┌───────────┐     ┌───────────┐
│ COMPLETADO │     │ CANCELADO │
└───────────┘     └───────────┘
       │
       ▼
┌───────────┐
│  VENCIDO   │ (si se pasa la fecha programada sin completar)
└───────────┘
```

### 7.4 Flujo de Estados de la Jornada de Inventario (InventorySession)

```
┌─────────────┐
│  PLANIFICADA │
└──────┬──────┘
       │
       ▼
┌───────────┐
│  EN_CURSO  │
└──────┬────┘
       │
       ▼
┌───────────┐
│ FINALIZADA │
└──────┬────┘
       │
       ▼
┌──────────────┐
│ CONCILIADA    │
└──────────────┘
```

---

## 8. Criterios de Aceptación

### 8.1 Gestión de Recursos

| CA | RF Asociado | Criterio |
|---|---|---|
| CA-001 | RF-008 | Dado un usuario autenticado con rol ADMIN o SUPERINTENDENTE, cuando complete el formulario de registro con todos los campos obligatorios y guarde, entonces el sistema debe crear el recurso, asignarle un código único y mostrar mensaje de éxito. |
| CA-002 | RF-009 | Dado un recurso recién creado en la categoría TECNOLOGÍA, cuando el sistema genere el código, entonces el código debe tener el formato TEC-XXXXX donde XXXXX es un correlativo numérico. |
| CA-003 | RF-010 | Dado un usuario autenticado, cuando busque por código "TEC-0001", entonces el sistema debe mostrar el recurso correspondiente. Cuando busque por texto "carpa", debe mostrar todos los recursos cuyo nombre contenga "carpa". |
| CA-004 | RF-012 | Dado un recurso existente, cuando se le asigne un responsable, entonces el sistema debe registrar la fecha de asignación y guardar el responsable anterior en el historial. |

### 8.2 Gestión de Préstamos

| CA | RF Asociado | Criterio |
|---|---|---|
| CA-005 | RF-027 | Dado un usuario autenticado, cuando solicite el préstamo de un recurso disponible, entonces el sistema debe crear la solicitud con estado SOLICITADO. |
| CA-006 | RF-029 | Dado un Jefe de Grupo autenticado, cuando apruebe una solicitud de préstamo, entonces el sistema debe cambiar el estado a APROBADO. |
| CA-007 | RF-031 | Dado un préstamo aprobado, cuando se registre la entrega del recurso, entonces el sistema debe cambiar el estado administrativo del recurso a PRESTADO. |
| CA-008 | RF-033 | Dado un recurso con préstamo activo, cuando un usuario intente solicitar un nuevo préstamo para el mismo recurso, entonces el sistema debe rechazar la operación con un mensaje indicando que el recurso ya está prestado. |

### 8.3 Inventario Físico

| CA | RF Asociado | Criterio |
|---|---|---|
| CA-009 | RF-042 | Dado un usuario autenticado, cuando cree una jornada de inventario seleccionando ubicaciones, entonces el sistema debe crear la jornada con estado PLANIFICADA y mostrar los recursos esperados para esas ubicaciones. |
| CA-010 | RF-047 | Dada una jornada de inventario en curso, cuando se finalice la jornada, entonces el sistema debe generar un resumen de recursos encontrados, no encontrados y dañados. |

### 8.4 Seguridad

| CA | RF Asociado | Criterio |
|---|---|---|
| CA-011 | RF-001 | Dado un usuario no autenticado, cuando intente acceder a cualquier ruta del sistema que requiera autenticación, entonces el sistema debe redirigir al login o devolver 401 Unauthorized. |
| CA-012 | RF-005 | Dado un usuario con rol SOLICITANTE, cuando intente acceder a la ruta de gestión de usuarios, entonces el sistema debe devolver 403 Forbidden. |

---

## 9. Priorización MVP

| Prioridad | Módulo | RFs | Justificación |
|---|---|---|---|
| **P0 - Imprescindible** | Autenticación y Autorización | RF-001 a RF-005 | Base de todo el sistema. Sin autenticación no hay control de acceso. |
| **P0 - Imprescindible** | Gestión de Recursos | RF-008 a RF-014 | Núcleo del sistema. Sin recursos no hay nada que gestionar. |
| **P0 - Imprescindible** | Gestión de Categorías | RF-016 a RF-019 | Las categorías son necesarias para clasificar recursos. |
| **P0 - Imprescindible** | Gestión de Ubicaciones | RF-020 a RF-023 | Las ubicaciones son necesarias para saber dónde están los recursos. |
| **P0 - Imprescindible** | Gestión de Responsables | RF-024 a RF-026 | Los responsables son necesarios para la custodia. |
| **P1 - Alta** | Gestión de Préstamos | RF-027 a RF-035 | Proceso crítico que actualmente no tiene registro formal. |
| **P1 - Alta** | Gestión de Movimientos | RF-060 a RF-061 | La trazabilidad depende del historial de movimientos. |
| **P1 - Alta** | Dashboard | RF-007 | Visibilidad del estado general del inventario. |
| **P2 - Media** | Inventario Físico | RF-042 a RF-048 | Necesario para validar el inventario real vs registrado. |
| **P2 - Media** | Conciliación | RF-049 a RF-051 | Complemento del inventario físico. |
| **P2 - Media** | Gestión de Mantenimiento | RF-036 a RF-041 | Importante pero no bloqueante para el inicio. |
| **P3 - Baja** | Gestión de Pérdidas | RF-052 a RF-055 | Depende de que existan inventarios físicos previos. |
| **P3 - Baja** | Gestión de Bajas | RF-056 a RF-059 | Proceso administrativo que puede esperar. |
| **P3 - Baja** | Auditoría | RF-062 a RF-063 | Puede implementarse después del MVP, pero debe registrarse desde el inicio. |
| **P3 - Baja** | Reportes | RF-064 a RF-071 | Pueden implementarse progresivamente. |
| **P3 - Baja** | Gestión de Documentos | — | Postergado para fases posteriores. |

### MVP (Minimum Viable Product)

El MVP incluye:

**Backend:**
1. Autenticación JWT + Roles (ADMIN, SUPERINTENDENTE, JEFE_GRUPO, CUSTODIO, SOLICITANTE)
2. CRUD completo de Recursos con generación automática de código único
3. CRUD de Categorías
4. CRUD de Ubicaciones
5. Asignación de responsables a recursos
6. Flujo completo de Préstamos (solicitar → aprobar → entregar → devolver)
7. Registro automático de movimientos
8. Dashboard con indicadores básicos

**Frontend:**
1. Login y gestión de sesión
2. Páginas CRUD para Recursos, Categorías, Ubicaciones
3. Flujo de Préstamos (solicitud, aprobación, entrega, devolución)
4. Dashboard
5. Angular Material como UI framework

---

## 10. Glosario

| Término | Definición |
|---|---|
| **Recurso** | Bien o activo institucional registrado en el sistema. |
| **Estado físico** | Condición material del recurso: BUENO, REGULAR, DAÑADO. |
| **Estado administrativo** | Situación del recurso en el ciclo de gestión: REGISTRADO, DISPONIBLE, ASIGNADO, PRESTADO, EN_MANTENIMIENTO, NO_LOCALIZADO, PERDIDO, DADO_DE_BAJA. |
| **Préstamo** | Proceso controlado de salida temporal de un recurso de la institución. |
| **Inventario físico** | Proceso de verificación presencial de los recursos institucionales. |
| **Conciliación** | Comparación entre el inventario registrado y la existencia física encontrada. |
| **Jornada de inventario** | Sesión planificada de verificación física de recursos. |
| **Código único** | Identificador alfanumérico irrepetible asignado a cada recurso. |
| **No localizado** | Estado temporal de un recurso registrado que no fue encontrado durante un inventario. |
| **Baja** | Proceso administrativo que elimina un recurso del inventario activo. |

---

## 11. Historial de Revisiones

| Versión | Fecha | Autor | Cambios |
|---|---|---|---|
| 1.0 | 2026-07-25 | — | Versión inicial del documento de requisitos |
