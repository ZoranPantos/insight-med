# Setup

<br>

## Containers startup

The simplest way to run _InsightMed_ is via docker, as the whole solution is containerized.
To start, first install docker for desktop from [here](https://www.docker.com/products/docker-desktop/) and enable WSL. Then simply navigate to cloned repository location, open the terminal
and run the command  
```sh
docker compose up -d --build
```

This should spin up the following containers:
- rabbitmq
- sqlserver-lab
- sqlserver
- elastic
- insightmed-api
- insightmed-labrpcserver
- kibana
- insightmed-web

<br>

If containers and volumes need to be removed, use
```sh
docker compose down -v
```
If cache needs to be removed, use
```sh
docker system prune
```
<br>

To access the required services, the following URLs are available. Host name and port numbers might vary depending on your configuration.
- InsightMed API Swagger UI: http://localhost:5000/swagger/index.html
- Angular home: http://localhost:4200
- Elasticsearch API: http://localhost:9200
  - Cluster health: http://localhost:9200/_cluster/health
- Kibana UI: http://localhost:5601/app/home
- RabbitMQ UI: http://localhost:15672

<br>

## Databases

We are connecting to two SQL Server instances in order to manage the **InsightMedDb** and **LabDb** databases. In our database management tool of choice, we create new connections:

**InsightMedDb**
| Setting                  | Value                        |
| ------------------------ | ---------------------------- |
| Server                   | localhost                    |
| Port                     | 1433                         |
| Schema                   | master                       |
| Authentication           | SQL Server Authentication    |
| Username                 | sa                           |
| Password                 | Password1!                   |
| Trust server certificate | Yes                          |

**LabDb**
| Setting                  | Value                     |
| ------------------------ | ------------------------- |
| Server                   | localhost                 |
| Port                     | 1434                      |
| Schema                   | master                    |
| Authentication           | SQL Server Authentication |
| Username                 | sa                        |
| Password                 | Password2!                |
| Trust server certificate | Yes                       |

<br>

When running the applications for the first time, all databases will be created automatically if they don't exist. Tables in **InsightMedDb** will be created empty by automatic execution of migration scripts. To populate them with seed data, we can execute the _[GET] api/AppManagement/SeedData_ endpoint.

**LabDb** will have only one table, `LabParameters`, which will be populated automatically, and it needs to be synchronized with the same table from **InsightMedDb** by columns `Id` and `Name`.

<br>

### Creating and applying migrations manually

To create a new migration, open terminal in _InsightMed.Infrastructure_ and run the following command
```sh
dotnet ef migrations add migration_name --startup-project ../InsightMed.API --output-dir Data/Migrations
```

To apply the migration, run
```sh
dotnet ef database update --startup-project ../InsightMed.API
```

<br>

## Running from IDE

Prerequisite for running the backend solution is to have **.NET 10 SDK** installed.
Set up a new startup profile in the IDE (Visual Studio or Rider) to run both _InsightMed.API_ and _InsightMed.LabRpcServer_ simultaneously.
SQL Server, RabbitMQ, Elasticsearch, and Kibana are required. They can either be installed locally or spun up in containers by running the following commands:

RabbitMQ  
```sh
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4.0-management
```

SQL Server (**InsightMedDb**)  
```sh
docker run -d --name sqlserver -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password1!" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

SQL Server (**LabDb**)  
```sh
docker run -d --name sqlserver-lab -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password2!" -p 1434:1433 mcr.microsoft.com/mssql/server:2022-latest
```

Elasticsearch  
```sh
docker run -d --name elastic -p 9200:9200 -e "discovery.type=single-node" -e "xpack.security.enabled=false" -e "ES_JAVA_OPTS=-Xms512m -Xmx512m" docker.elastic.co/elasticsearch/elasticsearch:8.15.2
```

Kibana  
```sh
docker run -d --name kibana -p 5601:5601 -e "ELASTICSEARCH_HOSTS=http://host.docker.internal:9200" docker.elastic.co/kibana/kibana:8.15.2
```

<br>

Prerequisites for the frontend:
- Node.js 24.11.1
- npm 11.6.2
- Angular CLI (global) @angular/cli@21.0.2

To run the frontend app, open terminal in _InsightMed.Web_ folder and execute the following command
```sh
npm ci
```
then
```sh
ng serve
```

<br>

## Kibana Data View

In order to inspect logs that are being sent to Elasticsearch, we need to create a data view, which is a one-time setup step. The Serilog sink will only create the data stream in Elasticsearch the very first time it successfully sends a log message. If no logs have been sent, the data stream doesn't exist, and therefore Kibana won't be able to find it.
To generate some logs beforehand, we can seed or truncate the **InsightMedDb** database through the corresponding API endpoints.

### 1. Create a Data View

1. Ensure our app has already sent at least one log to Elasticsearch  
   (the data stream is created only after the first log).

2. In Kibana, open the main menu → **Stack Management**.

3. Under **Kibana**, click **Data Views**.

4. Click **Create data view**.

5. Set:
   - **Name**: `logs-insightmed-development`
   - **Index pattern**: `logs-insightmed-development*`  
     (the `*` matches all time-based indices of this data stream).

6. Ensure **Timestamp field** is `@timestamp`.

7. Click **Save data view to Kibana**.

### 2. View Logs in Discover

1. Open the main menu → **Discover**.

2. In the top-left **data view** dropdown, select  
   `logs-insightmed-development`.

3. In the top-right **time range** picker, choose a range that covers when
   logs were written.

4. Logs should appear as:
   - A time histogram at the top.
   - A list of log entries below.

### 3. Customize Columns and Save the View

1. In **Discover**, find the **Available fields** panel on the left.

2. In the field search box, type `log.level`.

3. Hover `log.level` and click the **+** icon to add it as a column.

4. Repeat step 3 for these fields (if present):
   - `@timestamp` (usually already shown)
   - `message`
   - `labels.RequestName`
   - `metadata.CorrelationId`
   - `labels.Application`

5. Remove the `_source` column:
   - In the table header, hover `_source` and click the **−** icon.

6. Reorder columns by dragging their headers into your preferred order

7. Save this view:
   - Click **Save** at the top of Discover.
   - Enter a name, e.g. `API Logs - Clean View`.
   - Click **Save**.

8. To reuse it later, open **Discover** → click **Open** → select your
   saved search.

<br>

## Authentication

API supports basic authentication. Credentials for default seeded account are
| Email                    | Password                     |
| ------------------------ | ---------------------------- |
| default@test.com         | Default1!                    |

In order to test the endpoints via Swagger, we need to configure the Swagger UI client to automatically attach
an Authorization HTTP Header to every request we make.  
To do this, after performing the login via _[GET] api/Auth/Login_, copy the token from the response
and use it to authorize by clicking on **Authorize** button at the start of the page.