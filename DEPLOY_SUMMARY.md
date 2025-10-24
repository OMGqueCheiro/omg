# ✅ Deploy OMG - Configuração Completa

## 🎯 Resumo

Todos os arquivos de deploy foram criados e configurados! Aqui está o que foi feito:

## 📦 Arquivos Criados

### Docker & Containers
- ✅ `src/OMG.Api/Dockerfile` - Imagem Docker da API
- ✅ `src/OMG.BlazorApp/OMG.BlazorApp/Dockerfile` - Imagem Docker do BlazorApp
- ✅ `src/OMG.Worker/OMG.MigrationWorker/Dockerfile` - Imagem Docker do Migration Worker
- ✅ `.dockerignore` - Otimização do build Docker
- ✅ `docker-compose.prod.yml` - Orquestração dos containers em produção

### Configuração
- ✅ `src/OMG.Api/appsettings.Production.json` - Configurações de produção da API
- ✅ `src/OMG.BlazorApp/OMG.BlazorApp/appsettings.Production.json` - Configurações de produção do Blazor
- ✅ `Caddyfile` - Reverse proxy com HTTPS self-signed

### CI/CD
- ✅ `.github/workflows/deploy.yml` - Pipeline de deploy automático

### Scripts
- ✅ `scripts/setup-vm.sh` - Script de configuração inicial da VM
- ✅ `scripts/README.md` - Documentação dos scripts

### Documentação
- ✅ `docs/DEPLOYMENT.md` - Guia completo de deployment
- ✅ `QUICKSTART.md` - Guia rápido de início
- ✅ `DEPLOY_SUMMARY.md` - Este arquivo

### Modificações
- ✅ Health checks habilitados em produção (`OMG.ServiceDefaults/Extensions.cs`)

## 🚀 Próximos Passos

### 1. Executar Setup da VM (Uma Vez)

```bash
# Conectar na VM
ssh ubuntu@201.23.75.91

# Executar script de setup
curl -fsSL https://raw.githubusercontent.com/OMGqueCheiro/omg/main/scripts/setup-vm.sh | bash

# Logout e login novamente
exit
ssh ubuntu@201.23.75.91

# Verificar Docker
docker --version
docker compose version
```

### 2. Fazer o Deploy

#### Opção A: Push para GitHub (Recomendado)

```bash
# No seu repositório local
git add .
git commit -m "Setup complete deployment infrastructure"
git push origin main
```

O GitHub Actions vai:
1. Buildar as 3 imagens Docker
2. Publicar no GitHub Container Registry
3. Fazer deploy automático na VM

#### Opção B: Deploy Manual

1. Acesse https://github.com/OMGqueCheiro/omg/actions
2. Execute o workflow "Deploy to Production"

### 3. Verificar Deploy

Após o deploy:

```bash
# Conectar na VM
ssh ubuntu@201.23.75.91

# Verificar containers
cd /opt/omg
docker compose ps

# Ver logs
docker compose logs -f
```

### 4. Acessar Aplicação

- **HTTP**: http://201.23.75.91
- **HTTPS**: https://201.23.75.91 (aceitar certificado self-signed)

## 🔐 Secrets Configurados

Verifique que estes secrets estão configurados em:
https://github.com/OMGqueCheiro/omg/settings/secrets/actions

- ✅ `VM_SSH_KEY` - Chave SSH privada
- ✅ `VM_HOST` - `201.23.75.91`
- ✅ `VM_USER` - `ubuntu`
- ✅ `POSTGRES_PASSWORD` - Senha do PostgreSQL
- ✅ `JWT_SECRET_KEY` - Chave secreta JWT

## 🐳 Arquitetura

```
┌─────────────────────────────────────────────────┐
│              Internet / Users                    │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
        ┌─────────────────┐
        │  Caddy (Proxy)  │
        │  :80 / :443     │
        └────────┬────────┘
                 │
                 ▼
        ┌─────────────────┐
        │  BlazorApp      │
        │  :8080          │
        └────────┬────────┘
                 │
                 ▼
        ┌─────────────────┐
        │  API            │
        │  :8080          │
        └────────┬────────┘
                 │
                 ▼
        ┌─────────────────┐
        │  PostgreSQL     │
        │  :5432          │
        └─────────────────┘
                 ▲
                 │
        ┌────────┴────────┐
        │ Migration       │
        │ Worker          │
        │ (run once)      │
        └─────────────────┘
```

## 📊 Containers

| Nome | Função | Porta Externa | Status |
|------|--------|---------------|--------|
| omg-caddy | Reverse Proxy HTTPS | 80, 443 | Sempre rodando |
| omg-blazorapp | Frontend Blazor | - | Sempre rodando |
| omg-api | Backend API | - | Sempre rodando |
| omg-postgres | Banco de Dados | - | Sempre rodando |
| omg-migration-worker | Migrations | - | Executa e para |

## 📝 Comandos Úteis

```bash
# Ver status
cd /opt/omg
docker compose ps

# Ver logs
docker compose logs -f

# Restart
docker compose restart

# Parar
docker compose down

# Iniciar
docker compose up -d

# Backup do banco
docker exec omg-postgres pg_dump -U postgres OMGdb > backup.sql

# Ver recursos
docker stats
```

## 🔧 Troubleshooting

### Container não inicia
```bash
docker compose logs nome-do-container
```

### Verificar conectividade
```bash
docker exec omg-blazorapp ping omg-api
docker exec omg-api ping postgres
```

### Verificar banco
```bash
docker exec -it omg-postgres psql -U postgres OMGdb
```

### Problemas de HTTPS
```bash
docker compose logs caddy
docker exec omg-caddy caddy validate --config /etc/caddy/Caddyfile
```

## 📚 Documentação Completa

- **Quick Start**: [QUICKSTART.md](QUICKSTART.md)
- **Deploy Completo**: [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)
- **Scripts**: [scripts/README.md](scripts/README.md)

## ⚠️ Importante

1. **Certificado Self-Signed**: O HTTPS usa certificado auto-assinado. O navegador vai mostrar warning. Para produção real, use um domínio com Let's Encrypt.

2. **Backup**: Configure backups regulares do banco de dados PostgreSQL.

3. **Monitoramento**: Monitore logs e uso de recursos regularmente.

4. **Segurança**: As senhas estão em secrets. Nunca comite o arquivo `.env`.

5. **Primeiro Deploy**: No primeiro deploy, o migration worker vai criar o schema do banco.

## 🎉 Status

✅ Infraestrutura completa configurada
✅ CI/CD pipeline pronto
✅ Documentação completa
✅ Pronto para deploy!

---

**Data**: 24 de outubro de 2025
**Versão**: 1.0.0
