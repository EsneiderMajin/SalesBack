# AI_LOG.md Venta rápida

## Herramientas usadas

- Claude (chat) — para dudas conceptuales, diagnóstico de errores.
- Claude Code (terminal) — para generar y editar código directamente en el proyecto con contexto.
- Swagger UI — para probar los endpoints del backend.
- Chrome DevTools (Network/Console) — para verificar respuestas JSON reales y depurar el frontend.

Usé IA en el proceso revisando lo que salía antes de aceptarlo, Varias veces tocó corregir o rechazar cosas, quedan documentadas abajo.

---

## Prompts usados (backend)

### 1. Estructura inicial
```
/plan Estoy desarrollando una Web API en .NET Framework 4.8 con Web API 2 y Entity Framework 6.

Necesito que crees la estructura de carpetas y archivos base del proyecto con esta arquitectura:

Carpetas requeridas:
- Controllers/
- Services/ (con interfaces)
- Data/
- Models/
- DTOs/

Modelos (mapean estas tablas de SQL Server):

Product: ProductId (int PK), Name (nvarchar), Price (decimal), Description (nvarchar nullable), IsActive (bit)
Customer: CustomerId (int PK), Name (nvarchar), Email (nvarchar), IsActive (bit)
Sale: SaleId (int PK), CustomerId (int FK), TotalAmount (decimal), CreatedAt (datetime)
SaleItem: SaleItemId (int PK), SaleId (int FK), ProductId (int FK), Quantity (int), UnitPrice (decimal), Subtotal (decimal)

DTOs requeridos:
- SaleItemRequest: ProductId (int), Quantity (int)
- CreateSaleRequest: CustomerId (int), Items (lista de SaleItemRequest)
- SaleDetailResponse: SaleId, CustomerName, TotalAmount, CreatedAt, Items (lista con ProductName, Quantity, UnitPrice, Subtotal)

Interfaces de servicios:
- IProductService: GetActiveProducts()
- ICustomerService: GetActiveCustomers()
- ISaleService: CreateSale(CreateSaleRequest), GetSaleById(int id)

No implementes nada todavia, solo crea los archivos con las clases y interfaces vacías bien definidas.
```
Sirvió para dejar el esqueleto del proyecto antes de meterle lógica.

### 2. Generación de .sln y .csproj legacy
```
Genera el archivo SalesBack.sln y SalesBack.csproj para un proyecto 
Web API 2 con .NET Framework 4.8, con las siguientes características:

- Tipo de proyecto: ASP.NET Web API 2
- Framework: .NET Framework 4.8
- Namespace raíz: SalesBack
- Incluir referencias a: System.Web, System.Web.Http, System.Web.Http.WebHost, 
  System.Net.Http.Formatting
- Incluir todos los archivos .cs existentes en las carpetas: 
  Controllers/, Data/, DTOs/, Models/, Services/
- Incluir App_Start/WebApiConfig.cs
- Incluir Global.asax y Global.asax.cs
- Incluir Web.config básico para Web API 2
- Usar packages.config para NuGet (formato legacy, no PackageReference)

No uses formato SDK-style (no uses <Project Sdk="...">). 
Usa el formato legacy MSBuild con ToolsVersion="15.0".
```
Acá tocó ser bien explícito de que NO quería el formato moderno de .csproj, porque Claude Code por defecto tiende a usar el formato moderno.

### 3. Base de los Controllers
```
Crea los siguientes controladores de Web API 2 en la carpeta Controllers/,
con los métodos vacíos (sin implementacion, solo la firma y un
throw new NotImplementedException(); en el cuerpo):

1. ProductsController
   GET /api/products = productos activos

2. CustomersController
   GET /api/customers = clientes activos

3. SalesController
   POST /api/sales = crear venta via Stored Procedure + ADO.NET
   GET /api/sales/{id} = detalle de venta con items

Usa [RoutePrefix] y [Route] de attribute routing (Web API 2).
Cada controlador debe inyectar su Service correspondiente
(IProductService, ICustomerService, ISaleService) por constructor.
No implementes la lógica interna, solo deja los cascarones listos
para que yo complete cada método.
```

### 4. Corrección de instanciación (constructor injection → new directo)
```
En los 3 controladores (ProductsController, CustomersController, SalesController),
cambia la forma de obtener el Service: en vez de recibirlo por constructor
(constructor injection), instancialo directo como campo privado usando new,
pero manteniendo el tipo de la interfaz correspondiente.

Ejemplo del patron a aplicar:

private readonly IProductService _service = new ProductService();

Aplica el mismo patrón en:
- ProductsController : IProductService _service = new ProductService();
- CustomersController : ICustomerService _service = new CustomerService();
- SalesController : ISaleService _service = new SalesService();

Elimina el constructor que recibia la interfaz por parametro, ya no es necesario.
No implementes la logica interna de los metodos, sigue dejando
throw new NotImplementedException(); en cada uno.
```
---

## Prompts usados (frontend)

### 5. Prompt de implementación de servicios y componentes 
```
STACK: Angular 6.2.9 / Node 10.24.1 / RxJS 6. Sin sintaxis Angular 7+.

YA IMPLEMENTADO (no tocar):
- core/models/, core/services/ (CatalogService, SalesService), app.module.ts,
  app-routing.module.ts, app.component.html, proxy.conf.json

PENDIENTE (los 6 archivos a implementar):
- features/crear-venta/crear-venta.component.ts/.html/.css
- features/detalle-venta/detalle-venta.component.ts/.html/.css

CREAR VENTA (ruta ''):
- Combo clientes, selector producto + cantidad, botón "Agregar al carrito"
- Carrito con total calculado en cliente
- Botón "Guardar venta" → SalesService.create() → mostrar saleId retornado
- errorMsg visible en rojo si el backend responde 400/error de red
- 2 .subscribe() independientes en ngOnInit (no forkJoin)
- Getter total() recorre cart × precio del catálogo
- Link a /sales/{{savedSaleId}} con routerLink tras guardar

DETALLE VENTA (ruta 'sales/:id'):
- Lee :id con route.snapshot.paramMap.get('id')
- SalesService.getDetail(id) → muestra customerName, totalAmount, createdAt,
  tabla de items (productName, quantity, unitPrice, subtotal)
- errorMsg en rojo si 404 o error de red

CSS: plano, sin Bootstrap/Material, consistente entre ambas pantallas.
Sin NgRx, sin interceptores HTTP, sin tests.

Muéstrame primero el plan de los 6 archivos antes de generar código.
```
Este prompt fue clave porque le dejé bien claro a Claude Code las restricciones de versión, porque de entrada tiende a sugerir sintaxis de Angular moderno.


### 6. Prompt de estilos
```
/plan

Estoy en el proyecto Angular 6 "SalesFront" (carpeta actual). Los componentes
CrearVentaComponent y DetalleVentaComponent ya funcionan correctamente
(consumen la API real, guardan y muestran ventas). Solo necesito mejorar el
diseño visual de estos 2 archivos:
- src/app/features/crear-venta/crear-venta.component.css
- src/app/features/detalle-venta/detalle-venta.component.css

RESTRICCIÓN: NO toques ningún .ts ni .html, solo los .css. NO agregues
librerías externas (nada de Bootstrap/Material) - sigue siendo CSS plano,
según el enunciado de la prueba ("diseño básico, sin librerías complejas").
NO cambies clases existentes en el HTML, solo mejora los estilos ya usados.

Quiero una estética con toque MASCULINO pero en tono MEDIO - ni muy oscuro
(nada de fondos negros/carbón) ni muy claro (nada de blanco puro o pasteles):
fondo general gris claro neutro, tarjetas/paneles en blanco roto o gris muy
suave, acentos en azul petróleo o verde botella para botones y detalles,
tipografía robusta bien espaciada, botones sólidos con esquinas poco
redondeadas (no pill-shaped), sombras sutiles, sensación de panel serio y
ordenado, no "juguetón". Antes de tocar el CSS, muéstrame la paleta de
colores exacta que vas a usar (con sus valores hex).
```
### 6. Prompt para crear readme en frontend

```
Genera el README.md del frontend SalesFront (Angular 6 / Node 10.).

INCLUYE:
1. Stack
2. Cómo correrlo: npm install, npx ng serve → http://localhost:4200
3. Proxy: proxy.conf.json redirige /api/* a http://localhost:63013 (backend)
4. Pantallas: Crear venta (/) y Detalle venta (/sales/:id)
5. Decisiones de alcance: sin NgRx, sin interceptor, CSS plano, errorMsg por componente.
6. Flujo verificado de punta a punta.
```
---

## Decisiones donde corregí o rechacé una sugerencia de la IA

### 1. Generación de .sln y .csproj no funcionó (backend)
El prompt para generar el archivo SalesBack.sln y SalesBack.csproj para un proyecto Web API 2 con 
.NET Framework 4.8 no produjo un funciono. Se creo un proyecto manualmente.

### 2. Constructor injection rechazado en favor de instanciación directa (backend)
El primer prompt pedía que los controllers recibieran los Services por constructor. Claude Code lo hizo bien, pero avisó que Web API 2 iba a necesitar un `IDependencyResolver` 
Unity configurado para resolver eso en runtime, algo que el enunciado no pide y que metería complejidad de más, por lo cual rechacé esa parte y pedí que se cambiara a instanciación 
directa con `new()`, manteniendo el tipo de la interfaz. Después esto evolucionó otra vez a `ServiceFactory` (Simple Factory) para cumplir con los principios SOLID.

### 3. Casing JSON — corrección de integración detectada
El backend generado no traía configurado ningún resolver de camelCase. Lo detecté viendo la respuesta cruda del json en Swagger cuando las listas de Angular se veían vacías, 
y se corrigió agregando el `CamelCasePropertyNamesContractResolver` en `WebApiConfig.cs`.

---

## Notas finales

Todo el flujo quedó probado end-to-end contra el backend real: crear venta, guardar, ver el `saleId` retornado, navegar al detalle y ver cliente/total/items correctos. Los errores
 documentados arriba pasaron en el proceso y tocó resolverlos uno por uno antes de poder avanzar.
