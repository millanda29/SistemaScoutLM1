#!/usr/bin/env bash

# ==============================================================================
# Script de Actualización y Despliegue Automatizado
# Flujo: fetch -> pull develop -> pull main -> merge develop en main -> push main -> docker compose
# ==============================================================================

set -e # Detener ejecución si ocurre un error

echo "=================================================="
echo "🚀 Iniciando proceso de actualización y merge..."
echo "=================================================="

# 1. Validar que no existan cambios locales sin commitear
if ! git diff-index --quiet HEAD --; then
    echo "⚠️  Error: Tienes cambios locales sin guardar en tu directorio de trabajo."
    echo "   Realiza un commit o 'git stash' antes de ejecutar la actualización."
    exit 1
fi

# 2. Descargar últimos cambios del repositorio remoto
echo "📥 [1/5] Obteniendo cambios del repositorio remoto (git fetch)..."
git fetch origin

# 3. Actualizar la rama develop
echo "🌿 [2/5] Actualizando rama develop..."
git checkout develop
git pull origin develop

# 4. Cambiar a rama main y sincronizarla con el remoto
echo "🌿 [3/5] Sincronizando rama main..."
git checkout main
git pull origin main

# 5. Fusionar develop en main
echo "🔀 [4/5] Fusionando cambios de 'develop' en 'main'..."
git merge develop -m "merge: actualizar main desde develop"
echo "⬆️  Subiendo main actualizado a origin..."
git push origin main

# 6. Actualización de contenedores Docker
echo "🐳 [5/5] Gestionando contenedores Docker..."
if command -v docker >/dev/null 2>&1; then
    # Verificar si el archivo .env existe
    if [ ! -f ".env" ]; then
        echo "⚠️  Aviso: No se encontró el archivo .env."
        if [ -f ".env.example" ]; then
            echo "   Generando .env desde .env.example..."
            cp .env.example .env
            echo "   ⚠️  Asegúrate de editar .env con tus credenciales reales."
        fi
    fi

    # Verificar existencia de la red interna 'infra'
    if ! docker network inspect infra >/dev/null 2>&1; then
        echo "🌐 Creando red de Docker 'infra'..."
        docker network create infra
    fi

    echo "🏗️  Reconstruyendo e iniciando servicios con compose.yaml..."
    docker compose up -d --build
    echo "✅ Contenedores actualizados correctamente."
else
    echo "ℹ️  Docker no está instalado o disponible en este entorno. Omitiendo reinicio de contenedores."
fi

echo "=================================================="
echo "🎉 ¡Actualización finalizada con éxito!"
echo "   Rama activa: $(git branch --show-current)"
echo "=================================================="
