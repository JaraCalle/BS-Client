# BS-Client — Cliente Batalla Naval 🛳️⚓

Cliente de consola desarrollado en C# que permite a los jugadores conectarse a un servidor remoto y participar en partidas de Batalla Naval por turnos.

Este cliente implementa arquitectura MVC y utiliza `Spectre.Console` para brindar una experiencia visual mejorada dentro de la consola, facilitando la interacción del usuario con el juego.

---

## Características principales

- Arquitectura basada en MVC.
- Conexión y autenticación con servidor remoto.
- Sistema de turnos y gestión de partidas.
- Visualización de tableros (propio y enemigo).
- Colocación de barcos de manera automática.
- Realización de ataques por coordenadas.
- Visualización de historial de jugadas.
- Manejo de comunicación mediante protocolo de red personalizado.
- Sistema de logs.
- Interfaz de usuario mejorada en consola con colores y estilos.

---

## Requisitos

- .NET 8 SDK (solo para compilar desde código fuente).
- Consola compatible con UTF-8 y colores ANSI.

---

## Configuración

Antes de ejecutar el cliente es necesario configurar el archivo `config.json` con los parámetros de conexión al servidor:

```json
{
  "Host": "IP_O_DOMINIO_DEL_SERVIDOR",
  "Port": PUERTO_DEL_SERVIDOR
}
```

---

## Ejecución

Una vez configurado el cliente:

### Si descargaste el Release:

1. Descomprime el archivo `.zip`.
2. Configura `config.json`.
3. Ejecuta:

```bash
./BS-Client <logfile>
```

Ejemplo:

```bash
./BS-Client logs.txt
```

> Los logs de la partida se guardarán en el archivo indicado.

---

## Compilación desde código fuente

1. Clonar el repositorio:

```bash
git clone https://github.com/JaraCalle/BS-Client.git
cd BS-Client
```

2. Restaurar paquetes y compilar:

```bash
dotnet restore
dotnet build -c Release
```

3. Ejecutar:

```bash
dotnet run --project BS-Client <logfile>
```

---

## Tecnologías utilizadas

- C# 12
- .NET 9
- Spectre.Console
- Serilog (para logging)
- Arquitectura MVC
