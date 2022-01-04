if docker exec test-db sh -c "psql -U postgres -lqt | cut -d \| -f 1 | grep -qw dvdrental" >/dev/null 2>&1; then
    echo "Dropping database"
    docker exec -it test-db psql -U postgres -d postgres -c "DROP DATABASE If EXISTS dvdrental;"

    echo "Creating database"
    docker exec -it test-db psql -U postgres -d postgres -c "CREATE DATABASE dvdrental;"

fi

echo "Restoring dvdrental"
docker exec test-db sh -c "pg_restore -U postgres -c -d dvdrental /var/lib/postgresql/backup/dvdrental.tar" >/dev/null 2>&1
