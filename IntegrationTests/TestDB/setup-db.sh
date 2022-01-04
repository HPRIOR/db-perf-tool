
docker stop test-db >/dev/null 2>&1
docker rm test-db >/dev/null 2>&1

docker run -d --name test-db -p 5432:5432 dvd-test

while true; do
  echo "Waiting for db conn"
  if docker exec test-db sh -c "psql -U postgres -lqt | cut -d \| -f 1 | grep -qw dvdrental" >/dev/null 2>&1; then
    echo "Connection found"
    echo "Restoring dvdrental"
    docker exec test-db sh -c "pg_restore -U postgres -C -d dvdrental /var/lib/postgresql/backup/dvdrental.tar" >/dev/null 2>&1
    break
  fi
done
