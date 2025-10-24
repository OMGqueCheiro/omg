# 🚀 Guia de Deploy - OMG que Cheiro

Este documento contém todas as instruções para fazer o deploy da aplicação OMG na VM da Magalu Cloud.

## 📋 Informações da VM

- **IP Público**: 201.23.75.91
- **IP Privado**: 172.18.3.119
- **IPv6**: 2801:80:3ea0:d9df::367
- **Região**: br-se1
- **Recursos**: 4vCPU | 8GB RAM | 10GB Disco
- **SO**: Ubuntu 24.04 LTS
- **Usuário SSH**: ubuntu

## 🔐 Secrets Configurados no GitHub

Os seguintes secrets devem estar configurados em: `https://github.com/OMGqueCheiro/omg/settings/secrets/actions`

| Secret | Descrição | Exemplo |
|--------|-----------|---------|
| `VM_SSH_KEY` | Chave privada SSH para acesso à VM | Conteúdo do arquivo `~/.ssh/omg-deploy` |
| `VM_HOST` | IP público da VM | `201.23.75.91` |
| `VM_USER` | Usuário SSH | `ubuntu` |
| `POSTGRES_PASSWORD` | Senha do PostgreSQL | Senha forte gerada |
| `JWT_SECRET_KEY` | Chave secreta do JWT | Mínimo 32 caracteres |

## 🛠️ Setup Inicial da VM (Uma vez)

### Passo 1: Conectar na VM

```bash
ssh ubuntu@201.23.75.91
```

### Passo 2: Executar Script de Setup

```bash
# Baixar o script
curl -O https://raw.githubusercontent.com/OMGqueCheiro/omg/main/scripts/setup-vm.sh

# Dar permissão de execução
chmod +x setup-vm.sh

# Executar
./setup-vm.sh
```

### Passo 3: Logout e Login

```bash
# Fazer logout
exit

# Fazer login novamente para aplicar as permissões do Docker
ssh ubuntu@201.23.75.91

# Verificar instalação
docker --version
docker compose version
```

### Passo 4: Criar Estrutura de Diretórios

```bash
# Criar diretório principal (já criado pelo script)
sudo mkdir -p /opt/omg
sudo chown -R ubuntu:ubuntu /opt/omg

# Criar diretório para logs
sudo mkdir -p /var/log/caddy
sudo chown -R ubuntu:ubuntu /var/log/caddy
```

## 🚀 Deploy Automático (GitHub Actions)

O deploy é **totalmente automático** através do GitHub Actions.

### Como Funciona

1. **Trigger**: Push na branch `main` ou execução manual via workflow_dispatch
2. **Build**: Cria imagens Docker para API, BlazorApp e MigrationWorker
3. **Push**: Envia imagens para GitHub Container Registry (ghcr.io)
4. **Deploy**: SSH na VM, pull das imagens e restart dos containers

### Workflow

O arquivo `.github/workflows/deploy.yml` executa:

1. **Build and Push Job**
   - Build paralelo das 3 imagens Docker
   - Push para `ghcr.io/omgquecheiro/`
   - Cache de layers para builds mais rápidos

2. **Deploy Job**
   - Copia arquivos de configuração para VM
   - Cria arquivo `.env` com secrets
   - Executa `docker compose up -d`
   - Verifica status dos containers

### Deploy Manual

Para executar um deploy manual:

1. Acesse: `https://github.com/OMGqueCheiro/omg/actions`
2. Selecione o workflow "Deploy to Production"
3. Clique em "Run workflow"
4. Selecione a branch `main`
5. Clique em "Run workflow"

## 🐳 Arquitetura Docker

### Containers

| Container | Imagem | Porta Interna | Descrição |
|-----------|--------|---------------|-----------|
| `omg-postgres` | postgres:17-alpine | 5432 | Banco de dados PostgreSQL |
| `omg-migration-worker` | ghcr.io/.../omg-migrationworker | - | Executa migrations (run-once) |
| `omg-api` | ghcr.io/.../omg-api | 8080 | API REST backend |
| `omg-blazorapp` | ghcr.io/.../omg-blazorapp | 8080 | Aplicação Blazor frontend |
| `omg-caddy` | caddy:2-alpine | 80, 443 | Reverse proxy com HTTPS |

### Network

Todos os containers estão na network `omg-network` (bridge).

### Volumes

- `omg-postgres-data`: Persistência dos dados do PostgreSQL
- `omg-caddy-data`: Certificados SSL do Caddy
- `omg-caddy-config`: Configuração do Caddy

### Health Checks

- **PostgreSQL**: `pg_isready -U postgres`
- **API**: `curl -f http://localhost:8080/health`
- **BlazorApp**: `curl -f http://localhost:8080`

## 🌐 Acesso à Aplicação

### URLs

- **HTTP**: http://201.23.75.91
- **HTTPS**: https://201.23.75.91 ⚠️ Certificado self-signed (vai mostrar warning)

### Aceitar Certificado Self-Signed

No navegador:
1. Acesse https://201.23.75.91
2. Clique em "Avançado" ou "Advanced"
3. Clique em "Aceitar o risco e continuar" ou "Proceed to site"

## 🔧 Comandos Úteis na VM

### Verificar Status

```bash
cd /opt/omg
docker compose ps
```

### Ver Logs

```bash
# Logs de todos os containers
docker compose logs

# Logs em tempo real
docker compose logs -f

# Logs de um container específico
docker compose logs blazorapp
docker compose logs api
docker compose logs postgres

# Últimas 50 linhas
docker compose logs --tail=50
```

### Restart de Containers

```bash
# Restart de todos
docker compose restart

# Restart de um específico
docker compose restart blazorapp
docker compose restart api
```

### Stop/Start

```bash
# Parar todos
docker compose down

# Iniciar todos
docker compose up -d

# Rebuild e restart (após mudanças locais)
docker compose up -d --build
```

### Limpeza

```bash
# Limpar imagens não utilizadas
docker image prune -af

# Limpar tudo (cuidado!)
docker system prune -af

# Ver uso de disco
docker system df
```

### Backup do Banco de Dados

```bash
# Backup
docker exec omg-postgres pg_dump -U postgres OMGdb > backup_$(date +%Y%m%d_%H%M%S).sql

# Restore
cat backup_YYYYMMDD_HHMMSS.sql | docker exec -i omg-postgres psql -U postgres OMGdb
```

## 🔍 Troubleshooting

### Container não inicia

```bash
# Ver logs detalhados
docker compose logs nome-do-container

# Ver eventos do Docker
docker events

# Inspecionar container
docker inspect nome-do-container
```

### Problemas de Rede

```bash
# Verificar networks
docker network ls
docker network inspect omg-network

# Testar conectividade entre containers
docker exec omg-blazorapp ping omg-api
docker exec omg-api ping postgres
```

### Banco de Dados

```bash
# Entrar no PostgreSQL
docker exec -it omg-postgres psql -U postgres OMGdb

# Verificar tabelas
\dt

# Sair
\q
```

### Caddy não responde

```bash
# Ver logs do Caddy
docker compose logs caddy

# Verificar se porta está aberta
sudo netstat -tlnp | grep :443
sudo netstat -tlnp | grep :80

# Testar Caddyfile
docker exec omg-caddy caddy validate --config /etc/caddy/Caddyfile
```

### Memory/CPU Issues

```bash
# Ver uso de recursos
docker stats

# Ver processos
htop
```

## 📊 Monitoramento

### Logs do Sistema

```bash
# Logs do Docker
sudo journalctl -u docker -f

# Logs do sistema
sudo journalctl -f

# Logs do Caddy
tail -f /var/log/caddy/access.log
```

### Métricas

```bash
# CPU, Memory, Network de cada container
docker stats

# Disco
df -h
du -sh /var/lib/docker
```

## 🔄 Rollback

Se algo der errado com um deploy:

```bash
cd /opt/omg

# Ver versões disponíveis
docker images | grep omg

# Editar docker-compose.yml para usar uma tag específica
# Exemplo: trocar 'latest' por uma SHA específica
vim docker-compose.yml

# Aplicar
docker compose up -d
```

## 🆘 Suporte

### Logs Completos para Debug

```bash
cd /opt/omg

# Salvar todos os logs
docker compose logs > /tmp/omg-full-logs.txt

# Comprimir
tar -czf omg-debug-$(date +%Y%m%d_%H%M%S).tar.gz \
  /tmp/omg-full-logs.txt \
  docker-compose.yml \
  Caddyfile \
  .env

# Download para análise
scp ubuntu@201.23.75.91:omg-debug-*.tar.gz .
```

## 📝 Checklist de Deploy

- [ ] Secrets configurados no GitHub
- [ ] VM provisionada e acessível via SSH
- [ ] Script de setup executado na VM
- [ ] Docker e Docker Compose instalados
- [ ] Diretórios criados em `/opt/omg`
- [ ] Firewall configurado (portas 80, 443, 22)
- [ ] Push para branch `main` ou execução manual do workflow
- [ ] Build das imagens concluído com sucesso
- [ ] Deploy executado sem erros
- [ ] Containers rodando: `docker compose ps`
- [ ] Aplicação acessível via HTTPS

## 🔐 Segurança

### Recomendações

1. **Trocar senhas padrão**: Use senhas fortes para PostgreSQL e JWT
2. **Firewall**: Manter apenas portas necessárias abertas
3. **Updates**: Manter SO e Docker atualizados
4. **Backup**: Fazer backup regular do banco de dados
5. **Logs**: Monitorar logs regularmente
6. **SSL**: Considerar domínio próprio com Let's Encrypt para SSL válido

### Hardening

```bash
# Configurar fail2ban
sudo apt-get install fail2ban -y
sudo systemctl enable fail2ban
sudo systemctl start fail2ban

# Desabilitar SSH com senha (apenas chave)
sudo vim /etc/ssh/sshd_config
# PasswordAuthentication no
sudo systemctl restart sshd
```

## 📚 Referências

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Caddy Documentation](https://caddyserver.com/docs/)
- [PostgreSQL Docker](https://hub.docker.com/_/postgres)
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)

---

**Última atualização**: 24 de outubro de 2025
**Versão**: 1.0.0
