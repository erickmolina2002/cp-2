#!/bin/bash

echo "Aguardando SQL Server inicializar completamente..."
sleep 30

echo "Executando script de setup (Setup.sql)..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SqlServer2024!" -C -i /docker-entrypoint-initdb.d/Setup.sql

if [ $? -eq 0 ]; then
    echo "Banco de dados LojaDB configurado com sucesso!"
else
    echo "Erro ao executar script de setup!"
    exit 1
fi
