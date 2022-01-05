echo "Settup up postgres..."
while true; do
  echo "Waiting for db conn"
  if docker exec test-db sh -c "psql -U postgres -lqt | cut -d \| -f 1 | grep -qw dvdrental" >/dev/null 2>&1; then
    echo "Connection found"
    echo "Restoring dvdrental"
    docker exec test-db sh -c "pg_restore -U postgres -C -d dvdrental /var/lib/postgresql/backup/dvdrental.tar" >/dev/null 2>&1
    break
  fi
done

echo "Setting up elastic..."

echo "Creating test index"
curl -XPUT "http://localhost:9200/test-index" 

echo "Inserting data into index"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field1\" : \"value1\"}"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field2\" : \"value2\"}"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field3\" : \"value3\"}"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field4\" : \"value4\"}"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field5\" : \"value5\"}"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field6\" : \"value6\"}"
curl -H "Content-Type: application/json" -X POST "http://localhost:9200/test-index/test-type" -d "{ \"field7\" : \"value7\"}"
