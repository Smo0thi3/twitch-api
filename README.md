# Twitch API

Proyecto ASP.NET Core que proporciona una API para interactuar con la plataforma Twitch. Este proyecto implementa endpoints para obtener información de usuarios y streams, validación de datos, y manejo centralizado de la comunicación con la API de Twitch.

## Descripción del Proyecto

Este proyecto expone una API REST que actúa como intermediaria entre el cliente y la API de Twitch. Implementa patrones de arquitectura limpia con separación de responsabilidades en capas de endpoints, servicios, validadores y modelos de datos.

## Ejecutar la aplicación

Para no subir el Client_Id y el Client_secret, se han dejado vacíos, por lo que hay que añadirlos antes de ejecutar el proyecto.
Las credenciales y configuración de Twitch se deben definir en el archivo `appsettings.json` o `appsettings.Development.json`:

```json
{
  "Twitch": {
    "ClientId": "tu_client_id",
    "ClientSecret": "tu_client_secret"
  }
}
```

Una vez configurados podemos lanzarlo con el comando 'dotnet run'.

Por defecto está configurado el puerto 5192 para HTTP.

Llamadas de prueba:
- http://localhost:5192/analytics/user?id=36138196 (ID del usuario de alexelcapo)
- http://localhost:5192/analytics/streams
    Obtiene 20 resultados obtenidos de mayor a menor views, es el valor por defecto de la búsqueda.

## Decisiones técnicas
Para esta aplicaciñon he decidido usar .NET 10 con C# al ser la versión LTS más actualizada de la tecnología con la que suelo trabajar.

Por lo general trabajo en proyectos más grandes, así que, dada la simplicidad de este caso, he aprovechado esta ocasión para usar una tecnología que simplifica bastante la creación de APIs: FastEndpoints. Viene a ser un sutituto de Minimal APIs. Con esta forma, se puede simplificar mucho la estructura del proyecto.

Hablando de la estructura del proyecto. Dada la embergadura del proyecto, he optado por ordenar en carpetas en base a las funciones de los archivos, como se puede ver en el diagrama de arriba.
La otra opción que me planteé fue meter todas las cosas de un endpoint en una carpeta y sacar las cosas comunes, pero ya que estoy acostumbrado a trabajar con más división que esta, he optado por encontrar un punto medio. De esta forma el proyecto tiene una buena mantenibilidad y facilita la navegación por el mismo.

Para la validación, como FastEndpoint usa FluentValidation no había que darle muchas vueltas.

En el apartado de los tests he usado XUnit y Moq como era de esperar trabajando con C#. Es un clásico ya y no falla.

### Proceso seguido
1. Pensar en las técnologías y arquitectura que podía utilizar para adaptarme a este proyecto. Basandome en lo que suelo usar para mi trabajo del día a día, donde me muevo en proyectos de gran embergadura, con muchas llamadas y conexiones entre si, decidí adaptar eso al nivel del proyecto actual. No tenía sentido llenar esto de carpetas y archivos en un proyecto con dos endpoints, por lo que busqué el punto en el que estuviese bien organizado y dicha organización fuese limpia para poder moverse facilmente por el proyecto.
    Como comente en la entrevista, en el trabajo uso la IA, pero no a mucho nivel. Para este proyecto he escrito unas 3 lineas de código porque me costaba menos corregir alguna cosa a mano que pedirselo a la IA, el resto he utilizado un agente.

2. Preparar el prompt para el agente. En este caso he usado Claude. Sinceramente, esta es la parte que más tiempo me ha consumido, ya que he intentado sacarle el máximo provecho para crear el proyecto con un prompt.
    En la petición inicial, se crearon todos los archivos necesarios para que el proyecto hiciese lo que pedía.
    El resto de peticiones fueron para ir corrigiendo algún error que salió con la petición inicial y algún otro despiste que tuve con el endpoint de obtener los streams en vivo, que al principio no me fije de que había un parámetro para filtrar por los streams en directo.

    Sobre este punto quiero comentar que he quedado fascinado. No me había puesto tan en serio en el tema de programar con la IA y la verdad que he quedado asombrado. Que con un buen prompt te pueda ahorrar tanto trabajo es algo increible.

3. Una vez el proyecto estaba creado, hice unas cuantas pruebas para verificar que funcionaba acorde a lo que se pedía en los enunciados.

4. Tras probar que todo iba correctamente, pase a reestructurar el proyecto para dejarlo más limpio, eliminando objetos que realmente me eran innecesarios, como los response de la API de Twitch, que realmente podía utilizar los mismos que los de esta API.

5. Con todo listo, decidí añadir un proyecto de test, que nunca vienen mal.

## Que se ha quedado fuera
Hubo alguna idea que se me ocurrio implementar, pero sentía que si empezaba con algo de eso iba a empezar una rueda que no iba a parar.
Las dos cosas que he dejado fuera han sido un sistema de logs y la paginación del endpoint de obtener streams.

El sistema de logs lo he dejado fuera ya que para este proyecto realmente no me iba a aportar mucho, ya que el control de errores está bien manejado en los responses. Si fuese un proyecto que fuese a llegar a producción, lo hubiese metido para poder monitorizar mejor la aplicación.

Mirando la documentación del endpoint para obtener los streams vi que tenían un sistema de paginación. Le di vueltas a distintas formas de implementarlo, o podía implementar todas las que se me iban ocurriendo, pero hubiese sido ponerme a crear más variantes y a saber cuando hubiese parado.


## Dudas/Hipotesis enunciado
He tenido principalmente una duda con la gestión del error 401 al ser inválido el token JWT que se envía a la API de twitch. En la primera lectura entendí que cuando fallase, se debía realizar la llamada para obtener de nuevo el token.

Lo volví a leer y creía que la petición tenía que relanzarse una vez obtenido el token válido, pero no me cuadraba con el que se devolviese un 401 porque no habría caso de que saliese si falla y lo vuelvo a lanzar automáticamente con un token recién generado.

Al no cuadrarme este caso, hice una releida y sobretodo con la forma en la que pone en el "Endpoint 2: consultar streams en vivo" entendí que lo que se pedía es que si es posible, se generase el token antes de la llamada.

## Estructura del Proyecto

```
TwitchApi/
├── Configuration/              # Configuración de la aplicación
│   └── TwitchSettings.cs        # Configuración específica de Twitch
├── Constants/                  # Constantes de la aplicación
│   └── ErrorMessages.cs         # Mensajes de error predefinidos
├── Endpoints/                  # Definición de endpoints HTTP
│   ├── GetStreamsEndpoint.cs    # Endpoint para obtener información de streams
│   └── GetUserEndpoint.cs       # Endpoint para obtener información de usuarios
├── Models/                     # Modelos de datos
│   ├── Exceptions/              # Excepciones personalizadas
│   │   └── TwitchUnauthorizedException.cs
│   ├── Requests/                # Modelos de solicitud
│   │   └── GetUserRequest.cs
│   └── Responses/               # Modelos de respuesta
│       ├── ErrorResponse.cs
│       ├── StreamResponse.cs
│       ├── TwitchTokenResponse.cs
│       └── UserResponse.cs
├── Properties/                 # Configuración del proyecto
│   └── launchSettings.json      # Configuración de ejecución local
├── Services/                   # Servicios de negocio
│   └── TwitchService.cs         # Servicio centralizado para Twitch
├── Validators/                 # Validadores de datos
│   └── GetUserValidator.cs      # Validador para GetUserRequest
├── Program.cs                  # Punto de entrada de la aplicación
├── TwitchApi.csproj            # Archivo de proyecto
├── appsettings.json            # Configuración de aplicación
└── appsettings.Development.json # Configuración de desarrollo
```

## Componentes Principales

### Configuration (`Configuration/`)
- **TwitchSettings.cs**: Contiene la configuración de Twitch (credenciales, URLs, etc.) que se carga desde el archivo `appsettings.json`.

### Constants (`Constants/`)
- **ErrorMessages.cs**: Define mensajes de error estándar utilizados en toda la aplicación para mantener consistencia.

### Endpoints (`Endpoints/`)
- **GetUserEndpoint.cs**: Endpoint para obtener información de un usuario de Twitch.
- **GetStreamsEndpoint.cs**: Endpoint para obtener información de streams en vivo.

Estos endpoints utilizan validadores y servicios para procesar las solicitudes.

### Models (`Models/`)
- **Exceptions**: 
  - `TwitchUnauthorizedException`: Excepción lanzada cuando hay errores de autenticación con Twitch.
- **Requests**:
  - `GetUserRequest`: Modelo que define los parámetros para obtener información de un usuario.
- **Responses**:
  - `UserResponse`: Respuesta con datos del usuario.
  - `StreamResponse`: Respuesta con datos del stream.
  - `TwitchTokenResponse`: Respuesta con tokens de autenticación.
  - `ErrorResponse`: Respuesta estándar para errores.

### Services (`Services/`)
- **TwitchService.cs**: Servicio centralizado que encapsula la lógica de comunicación con la API de Twitch. Maneja las llamadas HTTP, autenticación y transformación de datos.

### Validators (`Validators/`)
- **GetUserValidator.cs**: Validador que asegura que los datos de `GetUserRequest` sean válidos antes de procesarlos.

## Tecnologías Utilizadas

- **.NET 10.0**: Framework de ejecución principal
- **ASP.NET Core**: Framework web para crear endpoints HTTP
- **Twitch API**: Plataforma de terceros para obtener datos de Twitch