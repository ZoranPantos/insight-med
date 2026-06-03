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
- insightmed-gateway

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

Application traffic flows through the API Gateway on port `4200`, which routes requests
to the appropriate service. Individual services are also accessible directly on their own ports.
- Angular app: http://localhost:4200
- InsightMed API (Swagger): http://localhost:5000/swagger/index.html
- LabRpcServer:
   - Health check: http://localhost:5100/health
   - Lab parameters: http://localhost:5100/lab-parameters
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

This is the **hybrid** development flow: _InsightMed.API_, _InsightMed.LabRpcServer_, and the Angular dev server (`ng serve`) run on the host (from Visual Studio / Rider / a terminal), while all supporting infrastructure - and optionally the API Gateway - run as containers.

### Prerequisites

Backend:
- **.NET 10 SDK**

Frontend:
- Node.js 24.11.1
- npm 11.6.2
- Angular CLI (global) @angular/cli@21.0.2

In the IDE, set up a startup profile that runs _InsightMed.API_ and _InsightMed.LabRpcServer_ simultaneously.

<br>

### The `dev-local.ps1` wrapper

The repository contains a PowerShell helper at the repo root, `dev-local.ps1`, that wraps `docker compose` and always merges two files: the base `docker-compose.yml` and the override `docker-compose.local.yml`. Every `docker compose <subcommand>` you'd normally type, type instead as `.\dev-local.ps1 <subcommand>` - all arguments are forwarded verbatim.

The override file does three things specific to the hybrid flow:
1. Excludes `api`, `labrpcserver`, and `web` from being containerized (they run on the host instead) by placing them in a `never` profile.
2. Re-publishes the gateway on host port **8080** so it doesn't collide with `ng serve` on `4200`.
3. Swaps the gateway's mounted nginx config to `gateway/nginx.local.conf`, whose upstreams target `host.docker.internal` so it can reach the host-bound services.

Using the wrapper for every hybrid-mode operation is important: a plain `docker compose up gateway` (or clicking "Start" in Docker Desktop on a gateway container that was created from the base file) will bake in the base config and nginx will crash with `[emerg] host not found in upstream "api:5000"`.

<br>

### Starting hybrid mode

From a fresh state (nothing running):

1. Bring up infrastructure + gateway in containers:
   ```powershell
   .\dev-local.ps1 up -d
   ```
   This creates and starts `sqlserver`, `sqlserver-lab`, `rabbitmq`, `elastic`, `kibana`, and `gateway`.
   `api`, `labrpcserver`, and `web` are intentionally skipped (they're in the `never` profile).

2. In the IDE, start _InsightMed.API_ and _InsightMed.LabRpcServer_.

3. In _InsightMed.Web_, run `npm ci` (first time only) followed by `ng serve`.

Endpoints in hybrid mode:
- API Gateway: http://localhost:8080
- Angular dev server (direct): http://localhost:4200
- InsightMed API (direct, Swagger): http://localhost:5000/swagger/index.html
- LabRpcServer (direct): http://localhost:5100
- Kibana / RabbitMQ UI / Elasticsearch: same ports as in full-docker mode

Either entry point works during development:
- `http://localhost:4200` - the Angular dev server's own `proxy.conf.json` routes `/api` and `/notifications` to the locally running API.
- `http://localhost:8080` - the same traffic goes through the gateway container, also covering `/lab/*` routes that the Angular dev proxy doesn't handle.

<br>

### Stopping hybrid mode

Stop the backends in the IDE and stop `ng serve` (Ctrl+C). For the containers:
```powershell
.\dev-local.ps1 down      # stop and remove containers (named volumes preserved)
```
or, to keep the containers around for a fast restart later:
```powershell
.\dev-local.ps1 stop
.\dev-local.ps1 start     # bring them back without re-creation
```

<br>

### Switching between full-docker and hybrid modes

A Docker container has its mounts and published ports **baked in at creation time** - they don't update when you change the compose files. The gateway therefore needs to be re-created with the correct configuration whenever you cross between modes. Always run `down` (not `stop`) for the mode you're leaving before bringing up the other one.

**Full-docker → Hybrid:**
```powershell
docker compose down
.\dev-local.ps1 up -d
```

**Hybrid → Full-docker:**
```powershell
.\dev-local.ps1 down
docker compose up -d --build
```

Named volumes (`sqlserver-data`, `sqlserver-lab-data`, `esdata`) survive `down`, so database state is preserved across mode switches. Only `down -v` deletes them.

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