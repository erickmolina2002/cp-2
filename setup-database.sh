#!/bin/bash

echo "Aguardando SQL Server estar pronto..."
sleep 15

echo "Executando Setup.sql..."
docker cp Setup.sql sqlserver2022:/tmp/Setup.sql 2>/dev/null

for i in {1..10}; do
    echo "Tentativa $i de 10..."
    if docker exec sqlserver2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SqlServer2024!" -C -i /tmp/Setup.sql 2>&1 | grep -q "Commands completed successfully\|já existe\|already exists"; then
        echo "Banco de dados inicializado com sucesso!"
        exit 0
    fi
    sleep 5
done

echo "Não foi possível inicializar o banco automaticamente."
echo "Execute manualmente: docker exec -it sqlserver2022 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'SqlServer2024!' -C -i /tmp/Setup.sql"
exit 1
