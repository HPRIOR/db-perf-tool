# DB performance tool

This tool will execute a series of SQL files and produce a table with their planning and/or execution times.

## Using the tool

The tool can be run natively with the .NET runtime/sdk or with a container. In either case several environment variables
need to be set - see below.

### Natively

```bash
dotnet run --project AutoDbPerf 
```

## Docker

To build the container a compressed clickhouse binary needs to be available to the docker file (it is not included in
the repo). Ensure that zip has been installed, then cd into AutoDbPerf and run:

```bash
curl -O 'https://builds.clickhouse.com/master/amd64/clickhouse' && chmod a+x clickhouse && zip clickhouse clickhouse
```

Then run:

```bash
docker build . -t auto-db-perf
```

### Postgres

```bash
docker run \ 
  -e TARGET=postgres \
  -e PGPASSWORD=topsecretpassword \
  -e PGUSER=postgresusername \
  -e PGNAME=databasename \
  -e PGHOST=databasehost \
  -e QUERYPATH="Volume/path/to/queries" \
  -e OUTPUTDIR="Volume/path/to/output/dir" \
  -v "/local/path/:/app/Volume" \
  auto-db-perf
```

Note: when running postgres benchmarks you *must* include explain analyse at the top of each query.

### BigQuery

```bash
docker run \
  -e TARGET=bq \
  -e GOOGLECREDPATH="/path/to/credentials/json.json" \
  -e GOOGLEPROJECTID="google-project-id" \
  -e QUERYPATH="Volume/path/to/queries" \
  -e OUTPUTDIR="Volume/path/to/output/dir" \
  -v "/local/path/:/app/Volume" \
  auto-db-perf
````

### Elastic

```bash
docker run \
  -e TARGET=elastic \
  -e QUERYPATH="Volume/query-path" \
  -e OUTPUTDIR="Volume/output-path" \
  -e ELASTICINDEX="versionprefix_countrycode_itemgroupcode" \
  -v "/local/path/:/app/Volume" \
   auto-db-perf 
```

### Clickhouse

```bash
docker run  \
  -e TARGET=clickhouse   \
  -e QUERYPATH="Volume/query-path" \
  -e OUTPUTDIR="Volume/output-path" \
  -v "/Users/harry.prior/Volume:/app/Volume"  \
  auto-db-perf 
```

A volume is used so that files containing queries can be accessed and the output can be saved on the host computer.

## Environment variables

Several environment variables need to be set otherwise the apps defaults will be run in appsettings.json.

#### General

- TARGET
    - The type of query to test (e.g. BigQuery (bq), Postgres (postgres), Elastic (elastic), ClickHouse (clickhouse))
- QUERYPATH
    - The path which contains the queries you want to benchmark (see section below for directory structure details)
- TIMEOUT
    - The maximum time a query should run for in minutes
- AVGPRECISION
    - The number of times each query will run to calculate an average
- OUTPUTDIR
    - The directory in which to save the result
- OUTPUTTYPE
    - The type of output produced (only csv supported currently)
- IGNOREFIRST
    - Ignores the first result of all the queries
- ORDER
    - The order in which queries are executed in each scenario (rr - round robin, seq - sequential)

#### Postgres

- PGHOST
- PGNAME
- PGPASSWORD
- PGPORT

#### BigQuery

- GOOGLECREDPATH
    - The path to a json file containing google credentials
- GOOGLEPROJECTID
    - e.g. gfk-eco-sandbox-red

#### ELASTIC

- ELASTICINDEX
    - The elastic indices to use in a given query (useful when testing a single scenario)
- INDEXV
    - The index version to prepend to each index of a matched scenario (see 'Notes on elastic indices' below)

## Query folder structure

QUERYPATH should point to a directory with the following structure:

```
-SQL
    - scenario1
        - query1.sql
        - query2.sql
        - query3.sql
    - scenario2
        - query1.sql
        - query2.sql
        - query3.sql
```

This will produce table with 2 columns (scenarios), and 3 rows (queries). If there is a mismatch in scenarios and
queries (for example, if `scenario3-query4.sql` is added to the above directory structure), the table will grow to
accomodate, but will leave blank "N/A" cells for empty parts of the table.

The intention is that `scenario1-query1` would be similar to `scenario2-query1` such that variations in SQL (different
scenarios) can be tested and compared.

## Running the integration tests

To run the integration tests, their dependencies will need to be setup. This can be done changing directory to
`IntegrationTests` and running:

```bash
docker-compose up --detach && ./TestDependencies/setup-dependencies.sh
```

The BigQuery benchmark tests rely on there being a google credentials json file located
in `Resources/gcreds/gcreds.json`

## Extending the tool

To extend the tool to include other databases the IQueryExecutor interface needs to be implemented and added to
the `BuildWith` method in `Program.cs`

## Notes

- The tool will try to produce *some* result for a single query even if most their executions timeout or error. For
  examples, given an average precision of 5, if 1/5 queries execute successfully then this single result will be shown.
  If all 5 fail then an error message will be displayed in the cell corresponding to why the query failed (error or
  timeout)

