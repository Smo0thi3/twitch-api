# Twitch API

Proyecto ASP.NET Core que proporciona una API para interactuar con la plataforma Twitch. Este proyecto implementa endpoints para obtener información de usuarios y streams, validación de datos, y manejo centralizado de la comunicación con la API de Twitch.

## Descripción del Proyecto

Este proyecto expone una API REST que actúa como intermediaria entre el cliente y la API de Twitch. Implementa patrones de arquitectura limpia con separación de responsabilidades en capas de endpoints, servicios, validadores y modelos de datos.

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

## Iniciar la aplicación

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