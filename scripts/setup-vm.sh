#!/bin/bash
set -e

echo "==================================="
echo "OMG - Setup Inicial da VM"
echo "==================================="

# Atualizar sistema
echo "📦 Atualizando sistema..."
sudo apt-get update
sudo apt-get upgrade -y

# Instalar dependências básicas
echo "📦 Instalando dependências..."
sudo apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    git \
    htop \
    vim

# Instalar Docker
echo "🐳 Instalando Docker..."
if ! command -v docker &> /dev/null; then
    # Adicionar repositório do Docker
    sudo install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
    sudo chmod a+r /etc/apt/keyrings/docker.gpg
    
    echo \
      "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
      $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
    
    sudo apt-get update
    sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    
    # Adicionar usuário ao grupo docker
    sudo usermod -aG docker $USER
    
    echo "✅ Docker instalado com sucesso!"
else
    echo "✅ Docker já está instalado"
fi

# Verificar instalação do Docker Compose
echo "🐳 Verificando Docker Compose..."
if docker compose version &> /dev/null; then
    echo "✅ Docker Compose plugin instalado"
else
    echo "❌ Erro: Docker Compose não está instalado"
    exit 1
fi

# Criar diretório da aplicação
echo "📁 Criando diretórios..."
sudo mkdir -p /opt/omg
sudo chown -R $USER:$USER /opt/omg

# Criar diretório para logs do Caddy
sudo mkdir -p /var/log/caddy
sudo chown -R $USER:$USER /var/log/caddy

# Criar arquivo .env vazio (será preenchido pelo GitHub Actions)
touch /opt/omg/.env

# Configurar firewall (UFW)
echo "🔥 Configurando firewall..."
if command -v ufw &> /dev/null; then
    sudo ufw --force enable
    sudo ufw allow 22/tcp    # SSH
    sudo ufw allow 80/tcp    # HTTP
    sudo ufw allow 443/tcp   # HTTPS
    echo "✅ Firewall configurado"
fi

# Habilitar Docker para iniciar no boot
echo "🚀 Habilitando Docker no boot..."
sudo systemctl enable docker
sudo systemctl start docker

# Configurar limite de recursos
echo "⚙️  Configurando limites do sistema..."
sudo tee -a /etc/sysctl.conf > /dev/null <<EOF

# OMG Application Settings
vm.max_map_count=262144
fs.file-max=65536
EOF

sudo sysctl -p

# Limpar packages não utilizados
echo "🧹 Limpando sistema..."
sudo apt-get autoremove -y
sudo apt-get clean

echo ""
echo "==================================="
echo "✅ Setup concluído com sucesso!"
echo "==================================="
echo ""
echo "📝 Próximos passos:"
echo "1. Faça logout e login novamente para aplicar permissões do Docker"
echo "2. Execute: docker --version && docker compose version"
echo "3. O deploy automático via GitHub Actions irá:"
echo "   - Enviar os arquivos necessários"
echo "   - Configurar variáveis de ambiente"
echo "   - Iniciar os containers"
echo ""
echo "📍 Diretório da aplicação: /opt/omg"
echo "📍 Logs do Caddy: /var/log/caddy"
echo ""
