if [[ $OSTYPE == "linux-gnu"* ]]; then
  CH="clickhouse-client"
else
  CH="clickhouse client"
fi

echo "Setting up postgres:"
while true; do
  echo "Waiting for postgres conn.\r\c"
  sleep 1
  echo "Waiting for postgres conn..\r\c"
  sleep 1
  echo "Waiting for postgres conn...\r\c"
  sleep 1
  if docker exec test-db sh -c "psql -U postgres -lqt | cut -d \| -f 1 | grep -qw dvdrental" >/dev/null 2>&1; then
    echo "\n"
    echo "Connection found"
    echo "Restoring dvdrental"
    echo "\n"
    docker exec test-db sh -c "pg_restore -U postgres -C -d dvdrental /var/lib/postgresql/backup/dvdrental.tar" >/dev/null 2>&1
    break
  fi
done

echo "Setting up clickhouse:"
while true; do
  echo "Waiting for clickhouse conn.\r\c"
  sleep 1
  echo "Waiting for clickhouse conn..\r\c"
  sleep 1
  echo "Waiting for clickhouse conn...\r\c"
  sleep 1
  if clickhouse client -q "show databases" | grep INFORMATION_SCHEMA >/dev/null 2>&1; then
    echo "\n"
    echo "Connection found"
    echo "Adding data to clickhouse"
    $CH -q "create database test_db ENGINE = Memory" >/dev/null 2>&1
    $CH -q "create table test_db.test_table(x String) Engine = Memory AS SELECT 1" >/dev/null 2>&1
    echo "\n"
    break
  fi
done

echo "Setting up elastic:"
while true; do
  echo "Waiting for elastic conn.\r\c"
  sleep 1
  echo "Waiting for elastic conn..\r\c"
  sleep 1
  echo "Waiting for elastic conn...\r\c"
  sleep 1
  if curl -s -XGET "http://localhost:9200" | grep "docker-cluster" &>/dev/null; then
    echo "\n"
    echo "Connection found"
    echo "Adding data to elastic"
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field1\" : \"value1\"}">/dev/null 2>&1
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field2\" : \"value2\"}">/dev/null 2>&1
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field3\" : \"value3\"}">/dev/null 2>&1
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field4\" : \"value4\"}">/dev/null 2>&1
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field5\" : \"value5\"}">/dev/null 2>&1
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field6\" : \"value6\"}">/dev/null 2>&1
    curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field7\" : \"value7\"}">/dev/null 2>&1
    break
  fi
done
