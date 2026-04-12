# Data Quest Tower Defense

<img src="https://img.shields.io/badge/engine-Unity 6.3.11f1 LTS-blue" alt="engine 6.3.11f1 LTS"/>

---

## 📋 Descripción

**Initial Template** proyecto inicial con configuraciones bases para desarrollo de juego.

---

## 💻 Requerimientos de sistema

- **Unity URP v6.3.11f1 LTS** — Motor de videjuegos
  Windows 10 versión 21H1 (build 19043) o posterior (X64), Windows 11 21H2 (build 22000) o posterior (Arm64).
  CPU de 64 bits x64 con con set de instrucciones SSE2, Arm64.
  GPU compatibl con DX10, DX11, DX12 o Vulkan.
  Minimo 8GB de RAM.
  Almacenamiento: NVMe M.2 o SSD SATA, no se recomienda HDD (disco mecánico)
- **Visual Studio 2022 Community** - IDE para desarrollo
  Windows 10 o Windows 11
  CPU de 64 bits x64, minimo 4 núcleos
  Minimo 8GB de RAM.
  Almacenamiento: NVMe M.2 o SSD SATA, no se recomienda HDD (disco mecánico)

---

## 🛠️ Prerequisitos para usar este proyecto

- Si no tienes Git instalado, descárgalo desde este <a href="https://git-scm.com/" target="_blank">link</a> y sigue el proceso de instalación.
- Durante la instalación, haz clic en **Next** hasta que encuentres la opción **Override the default branch name for new repositories**, actívala y continúa sin cambiar ninguna otra configuración.
- Al terminar la instalación, necesitas asociar tu nombre y correo a Git. En Windows, abre la terminal **CMD** y ejecuta los siguientes comandos:

**Configurar tu identidad:**
```bash
git config --global user.name "Tu Nombre"
git config --global user.email "tuemail@ejemplo.com"
```

- Cierra la terminal. Ahora crea una carpeta en el **disco C**, por ejemplo **proyectos_unity**. Es importante que el nombre no tenga tildes, caracteres especiales ni espacios, ya que en esta carpeta se guardarán los proyectos de Unity.
- Abre la carpeta y haz clic derecho dentro de ella; en el menú contextual selecciona **Abrir en terminal**. Si esa opción no aparece, abre **CMD** desde el menú de Windows, escribe `cd`, deja un espacio y arrastra la carpeta hacia la terminal. Presiona Enter. **Nota:** esto solo funciona si la carpeta está en el disco **C**.
- La terminal ahora debería mostrar algo como `c:\proyectos_unity>`. Desde ahí, clona el repositorio con el siguiente comando:

**Clonar repositorio:**
```bash
git clone https://github.com/cristhianr06/data-quest-tower-defense.git
```

El proyecto comenzará a descargarse. Cuando termine, la terminal volverá a mostrar `c:\proyectos_unity>`.

- Descarga **Visual Studio 2022 Community** desde este [link](https://aka.ms/vs/17/release/vs_community.exe). Al ejecutar el instalador, ve a la sección **Juegos**, activa **Desarrollo de juego con Unity** y haz clic en **Instalar**. Espera a que finalice.
- Descarga Unity Hub desde este <a href="https://unity.com/download" target="_blank">link</a> e instálalo.
- Abre Unity Hub e inicia sesión con tu cuenta Unity. Si aún no tienes una, créala en el momento.
- Una vez dentro, ve al apartado **Licenses**. Si no tienes una licencia activa, haz clic en **Add license**, selecciona **Get a free personal license** y acepta los términos con **Agree and get personal edition license**.
- En la sección **Projects**, haz clic en **Add** → **Add project from disk**. Navega hasta la carpeta que creaste y selecciona la carpeta del proyecto **data-quest-tower-defense**. Haz clic en **Open**.
- Aparecerá una ventana indicando que no se encuentra la versión de Unity requerida. Haz clic en **Install Version 6000.3.11.f1**.

##### En las opciones de instalación, selecciona únicamente:
- Android Build Support: OpenJDK y Android SDK & NDK Tools
- Web Build Support
- Windows Build Support (IL2CPP)
- Documentation

Acepta los términos y condiciones e instala. Puedes ver el progreso en la parte superior derecha de Unity Hub, en el ícono de descarga. Espera a que termine.

- Abre el proyecto y dale tiempo para que compile antes de comenzar a trabajar.
- Dentro de Unity, ve al menú superior izquierdo **Edit** → **Preferences**. En la sección **External Tools**, verifica que **External Script Editor** esté configurado como **Visual Studio 2022**.
- En la misma ventana, ve al apartado **Colors**, busca **Playmode tint** y elige un color llamativo, como verde. Esto te ayudará a identificar fácilmente cuándo el editor está en modo **Play**.