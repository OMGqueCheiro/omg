# 🚀 OMG Deployment Scripts

Este diretório contém scripts para configuração e deployment da aplicação OMG.

## 📁 Arquivos

- `setup-vm.sh` - Script de configuração inicial da VM

## 🛠️ Setup da VM

### Primeira Execução

```bash
# Conectar na VM
ssh ubuntu@201.23.75.91

# Executar setup
curl -fsSL https://raw.githubusercontent.com/OMGqueCheiro/omg/main/scripts/setup-vm.sh | bash
```

### O que o script faz

1. ✅ Atualiza o sistema Ubuntu
2. ✅ Instala Docker e Docker Compose
3. ✅ Configura usuário no grupo docker
4. ✅ Cria diretórios necessários (`/opt/omg`)
5. ✅ Configura firewall (UFW)
6. ✅ Otimiza parâmetros do sistema
7. ✅ Limpa packages desnecessários

### Após Execução

```bash
# Logout e login novamente
exit
ssh ubuntu@201.23.75.91

# Verificar instalação
docker --version
docker compose version
```

## 📚 Documentação Completa

Veja [DEPLOYMENT.md](../docs/DEPLOYMENT.md) para instruções completas de deploy.
