<#
.SYNOPSIS
    Wrapper around `docker compose` for the hybrid local-dev workflow.

.DESCRIPTION
    Always merges docker-compose.yml + docker-compose.local.yml so the gateway
    container is created with the local-mode config (nginx.local.conf, port
    8080, host.docker.internal upstreams) and the api/labrpcserver/web
    container services stay disabled.

    Without this wrapper it is easy to accidentally start the gateway from
    just the base file (e.g. via `docker compose up gateway` or the "Start"
    button in Docker Desktop on a container previously created from the base
    file), which makes nginx crash with `host not found in upstream`.

.EXAMPLE
    .\dev-local.ps1 up -d gateway
    .\dev-local.ps1 up -d sqlserver sqlserver-lab rabbitmq elastic kibana
    .\dev-local.ps1 logs -f gateway
    .\dev-local.ps1 down
#>

$ErrorActionPreference = 'Stop'

$composeFiles = @(
    '-f', (Join-Path $PSScriptRoot 'docker-compose.yml'),
    '-f', (Join-Path $PSScriptRoot 'docker-compose.local.yml')
)

& docker compose @composeFiles @args
exit $LASTEXITCODE
