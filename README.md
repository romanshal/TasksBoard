# TasksBoard
A task board system built as a modern microservices solution with Clean Architecture, CQRS, and event‑driven messaging. 
This project demonstrates enterprise‑grade patterns, observability, and integration across backend and frontend.

## 🚀 Features
- **Clean Architecture**: strict separation of concerns, modular layers, and testable boundaries.
- **Microservices**: independent services communicating via gRPC and RabbitMQ.
- **CQRS + Unit of Work**: explicit command/query segregation with transactional consistency.
- **Mediator Pattern**: decoupled request handling via MediatR.
- **AutoMapper**: streamlined DTO ↔ entity mapping.
- **Outbox Pattern**: reliable event publishing with transactional integrity.
- **Batch Processing**: concurrency‑safe background jobs for message handling.
- **Ocelot API Gateway**: unified entry point with routing, aggregation, and cross‑cutting concerns.
- **PostgreSQL**: relational persistence with EF Core.
- **Redis**: caching and distributed state.
- **SignalR**: real‑time updates to Angular frontend.
- **Angular Frontend**: SPA client with Dockerized deployment.
- **Observability**: Grafana, Prometheus, Jaeger, Serilog, and OpenSearch for metrics, tracing, logging, and search.

## 🏛️ Architecture Overview
Clean Architecture Layers:
- **Domain**: core business logic, aggregates, entities.
- **Application**: CQRS handlers, MediatR pipelines, AutoMapper profiles.
- **Infrastructure**: EF Core, PostgreSQL, Redis, RabbitMQ, Outbox.
- **Presentation**: gRPC endpoints, REST APIs, SignalR hubs, Angular UI.

## 📡 Communication
- **gRPC**: high‑performance inter‑service RPC.
- **RabbitMQ**: event bus for async messaging.
- **Outbox Pattern**: ensures events are published reliably after DB commits.

## 📊 Observability
- **Prometheus**: metrics scraping.
- **Grafana**: dashboards.
- **Jaeger**: distributed tracing.
- **Serilog + OpenSearch**: structured logging and search.

## 🧪 Testing
- Unit tests with **xUnit**, **Moq**, **FluentAssertions**, **TestContainers**.
- Integration tests for gRPC and RabbitMQ flows.
- Automated test case generation for CQRS handlers.

## 📦 Getting Started
Prerequisites:
- **[Docker](https://docs.docker.com/get-docker/)**
- **[Docker Compose](https://docs.docker.com/compose/)**
- **[Node.js](https://nodejs.org/)**
- **[.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)**

**Clone the repo**
```
git clone https://github.com/romanshal/TasksBoard.git
cd TasksBoard
```

**Run with Docker Compose**
```
docker-compose up --build
```

This will start:
- API Gateway
- Backend microservices
- PostgreSQL, Redis
- RabbitMQ
- Angular frontend
- Observability stack (Grafana, Prometheus, Jaeger, OpenSearch)
