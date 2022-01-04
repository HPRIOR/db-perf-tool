
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





