# SalesBack

API del flujo "crear venta + consultar detalle", hecha en .NET Framework 4.8 + Web API 2 + EF6 + ADO.NET.

## Stack

- .NET Framework 4.8 / Web API 2
- EF6 (para lecturas)
- ADO.NET + Stored Procedure (para crear la venta, con transacción)
- SQL Server

## Cómo correr

1. Ejecutar `Scripts.sql` en SSMS (crea la base, las 4 tablas, el SP y datos de prueba).
2. Ajustar la connection string en `Web.config` si tu SQL Server tiene otro nombre.
3. Abrir `SalesBack.sln` en Visual Studio y darle Iniciar (IIS Express).
4. Queda corriendo en `http://localhost:63013`, con Swagger en `http://localhost:63013/swagger/ui/index`.

## Endpoints

| Método | Ruta | Qué hace |
|---|---|---|
| GET | `/api/products` | Productos activos |
| GET | `/api/customers` | Clientes activos |
| POST | `/api/sales` | Crea una venta (Stored Procedure + ADO.NET + transacción) |
| GET | `/api/sales/{id}` | Detalle de una venta con sus items |

## Reglas

- No se puede vender a un cliente inactivo.
- No se puede vender un producto inactivo.
- Ambas se validan dentro del Stored Procedure, dentro de una transaccion.

## Arquitectura

Controller → Service → Data. El `POST /api/sales` usa ADO.NET puro (no EF6) para invocar el SP.
