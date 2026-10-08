#!/usr/bin/env bash

# ==============================================================================
# Script de Actualización y Despliegue en Servidor
# Los cambios a main se realizan EXCLUSIVAMENTE mediante Pull Requests (PR) en GitHub.
# Este script descarga los cambios ya aprobados y reconstruye los contenedores Docker.
#
# Uso:
#   ./update.sh           # Actualiza la rama actual (por defecto main en producción)
#   ./update.sh develop   # Actualiza y despliega la rama develop (para staging/QA)
# ==============================================================================

set -e # Detener ejecución si ocurre un error

TARGET_BRANCH="${1:-$(git branch --show-current)}"
TARGET_BRANCH="${TARGET_BRANCH:-main}"

echo "=================================================="
echo "🚀 Iniciando despliegue de la rama: ${TARGET_BRANCH}"
echo "   (Política: Todos los cambios pasan por PRs en GitHub)"
echo "=================================================="

# 1. Validar que no existan cambios locales sin commitear
if ! git diff-index --quiet HEAD --; then
    echo "⚠️  Error: Tienes cambios locales sin guardar en tu directorio de trabajo."
    echo "   Realiza un commit o 'git stash' antes de ejecutar la actualización."
    exit 1
fi

# 2. Descargar últimos cambios y ramas del remoto
echo "📥 [1/4] Obteniendo cambios remotos (git fetch)..."
git fetch origin

# 3. Cambiar a la rama deseada y hacer pull del código aprobado vía PR
echo "🌿 [2/4] Sincronizando rama '${TARGET_BRANCH}'..."
git checkout "${TARGET_BRANCH}"
git pull origin "${TARGET_BRANCH}"

# 4. Verificar configuración de entorno y red Docker
echo "⚙️  [3/4] Verificando entorno y redes..."
if [ ! -f ".env" ]; then
    echo "⚠️  Aviso: No se encontró el archivo .env."
    if [ -f ".env.example" ]; then
        echo "   Generando .env desde .env.example..."
        cp .env.example .env
        echo "   ⚠️  Asegúrate de editar .env con tus credenciales reales antes de continuar."
    fi
fi

if command -v docker >/dev/null 2>&1; then
    # Verificar red 'infra'
    if ! docker network inspect infra >/dev/null 2>&1; then
        echo "🌐 Creando red de Docker 'infra'..."
        docker network create infra
    fi

    # 5. Reconstruir e iniciar servicios
    echo "🐳 [4/4] Reconstruyendo y levantando servicios con compose.yaml..."
    docker compose up -d --build
    echo "✅ Servicios desplegados y actualizados correctamente."
else
    echo "ℹ️  Docker no está disponible en este entorno. Se omitió la reconstrucción de contenedores."
fi

echo "=================================================="
echo "🎉 ¡Despliegue completado con éxito!"
echo "   Rama activa: $(git branch --show-current)"
echo "   Último commit: $(git log -1 --pretty=format:'%h - %s (%an)')"
echo "=================================================="
