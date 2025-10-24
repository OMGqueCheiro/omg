# ✅ Checklist de Deploy - OMG que Cheiro

Use este checklist para garantir que tudo está configurado corretamente antes do deploy.

## 📋 Pré-Deploy

### 1. Secrets do GitHub
Acesse: https://github.com/OMGqueCheiro/omg/settings/secrets/actions

- [ ] `VM_SSH_KEY` - Chave privada SSH criada e adicionada
- [ ] `VM_HOST` - `201.23.75.91` configurado
- [ ] `VM_USER` - `ubuntu` configurado
- [ ] `POSTGRES_PASSWORD` - Senha forte configurada
- [ ] `JWT_SECRET_KEY` - Chave de 32+ caracteres configurada

### 2. Chave SSH Local

```bash
# Gerar chave (se ainda não tiver)
ssh-keygen -t ed25519 -C "github-actions-omg" -f ~/.ssh/omg-deploy

# Copiar para VM
ssh-copy-id -i ~/.ssh/omg-deploy.pub ubuntu@201.23.75.91

# Testar conexão
ssh -i ~/.ssh/omg-deploy ubuntu@201.23.75.91

# Copiar conteúdo da chave privada para o secret VM_SSH_KEY
cat ~/.ssh/omg-deploy
```

- [ ] Chave SSH gerada
- [ ] Chave pública copiada para VM
- [ ] Conexão SSH testada e funcionando
- [ ] Chave privada adicionada ao secret `VM_SSH_KEY`

### 3. Setup da VM

```bash
# Conectar na VM
ssh ubuntu@201.23.75.91

# Executar script de setup
curl -fsSL https://raw.githubusercontent.com/OMGqueCheiro/omg/main/scripts/setup-vm.sh | bash

# Logout e login
exit
ssh ubuntu@201.23.75.91

# Verificar Docker
docker --version
docker compose version
```

- [ ] Conectado na VM via SSH
- [ ] Script de setup executado com sucesso
- [ ] Docker instalado (verificado com `docker --version`)
- [ ] Docker Compose instalado (verificado com `docker compose version`)
- [ ] Diretório `/opt/omg` criado
- [ ] Firewall configurado (portas 22, 80, 443)

## 🚀 Deploy

### 4. Commit e Push

```bash
# Verificar mudanças
git status

# Adicionar arquivos
git add .

# Commit
git commit -m "Setup complete deployment infrastructure"

# Push para main (vai disparar o deploy)
git push origin main
```

- [ ] Código commitado
- [ ] Push para branch `main` realizado
- [ ] GitHub Actions iniciado (verifique em Actions)

### 5. Acompanhar Deploy

Acesse: https://github.com/OMGqueCheiro/omg/actions

- [ ] Workflow "Deploy to Production" apareceu
- [ ] Job "build-and-push" - ✅ Completado
- [ ] Job "deploy" - ✅ Completado
- [ ] Sem erros nos logs

## ✅ Pós-Deploy

### 6. Verificar Containers na VM

```bash
# Conectar na VM
ssh ubuntu@201.23.75.91

# Ir para diretório
cd /opt/omg

# Verificar containers
docker compose ps

# Ver logs
docker compose logs
```

Containers esperados:
- [ ] `omg-postgres` - Status: Up (healthy)
- [ ] `omg-migration-worker` - Status: Exited (0)
- [ ] `omg-api` - Status: Up (healthy)
- [ ] `omg-blazorapp` - Status: Up (healthy)
- [ ] `omg-caddy` - Status: Up

### 7. Testar Aplicação

```bash
# Na VM, testar endpoints
curl http://localhost/health
curl http://201.23.75.91/health
```

No navegador:
- [ ] HTTP funciona: http://201.23.75.91
- [ ] HTTPS funciona: https://201.23.75.91 (aceitar certificado)
- [ ] BlazorApp carrega corretamente
- [ ] Consegue fazer login/criar conta

### 8. Verificar Logs

```bash
cd /opt/omg

# Ver logs gerais
docker compose logs

# Logs específicos
docker compose logs postgres
docker compose logs api
docker compose logs blazorapp
docker compose logs caddy

# Logs em tempo real
docker compose logs -f
```

- [ ] Sem erros críticos nos logs
- [ ] API respondendo
- [ ] BlazorApp conectando na API
- [ ] Migrations aplicadas com sucesso

### 9. Teste de Persistência

```bash
# Criar algo na aplicação (usuário, pedido, etc)
# Reiniciar containers
docker compose restart

# Verificar se dados persistiram
```

- [ ] Dados persistem após restart
- [ ] Volume PostgreSQL funcionando

## 🔧 Troubleshooting

Se algo der errado:

### Container não inicia
```bash
# Ver logs detalhados
docker compose logs nome-do-container

# Ver últimas 100 linhas
docker compose logs --tail=100 nome-do-container

# Inspecionar container
docker inspect nome-do-container
```

### Problemas de rede
```bash
# Testar conectividade
docker exec omg-blazorapp ping omg-api
docker exec omg-api ping postgres

# Ver networks
docker network ls
docker network inspect omg-network
```

### Banco de dados
```bash
# Conectar no PostgreSQL
docker exec -it omg-postgres psql -U postgres OMGdb

# Listar tabelas
\dt

# Ver usuários
SELECT * FROM "AspNetUsers";

# Sair
\q
```

### HTTPS/Caddy
```bash
# Logs do Caddy
docker compose logs caddy

# Validar Caddyfile
docker exec omg-caddy caddy validate --config /etc/caddy/Caddyfile

# Ver portas abertas
sudo netstat -tlnp | grep :443
```

## 📊 Métricas Pós-Deploy

```bash
# Uso de recursos
docker stats

# Espaço em disco
df -h
du -sh /var/lib/docker

# Memória
free -h
```

## 🎉 Deploy Completo!

Se todos os checkboxes acima estão marcados:
- ✅ Deploy realizado com sucesso!
- ✅ Aplicação rodando em produção
- ✅ Disponível em https://201.23.75.91

## 📚 Próximos Passos

- [ ] Configurar backup automático do banco
- [ ] Configurar monitoramento (opcional)
- [ ] Documentar processo de rollback
- [ ] Testar disaster recovery

## 🔗 Links Úteis

- **Aplicação**: https://201.23.75.91
- **GitHub Actions**: https://github.com/OMGqueCheiro/omg/actions
- **Secrets**: https://github.com/OMGqueCheiro/omg/settings/secrets/actions

## 📞 Suporte

Consulte a documentação completa:
- [QUICKSTART.md](QUICKSTART.md) - Início rápido
- [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) - Documentação completa
- [DEPLOY_SUMMARY.md](DEPLOY_SUMMARY.md) - Resumo técnico

---

**Boa sorte com o deploy! 🚀**
