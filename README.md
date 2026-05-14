# PaymentSim

## Overview

PaymentSim is a cloud-oriented distributed payment reliability simulation platform built with ASP.NET Core and Azure-oriented architecture concepts.

The project does NOT aim to become a real payment platform.

Its main goal is to practice and demonstrate:

- Idempotent payment processing
- Distributed tracing
- Webhook signature validation
- Retry strategies
- Event-driven architecture
- Queue-based asynchronous processing
- Eventual consistency
- High concurrency protection
- Rate limiting
- Observability
- Cloud-native Azure services
- Reliability engineering concepts

---

# Core Objectives

This project focuses on:

## Reliability

How to keep systems stable under:

- retries
- duplicate requests
- partial failures
- slow external services
- webhook failures
- temporary outages

---

## Event-Driven Architecture

Move from:

Synchronous direct API calls

to:

Asynchronous event-driven processing.

---

## Observability

Track requests across:

- API
- Queue
- Worker
- Webhook
- Blob Storage
- Azure Functions

using:

- TraceId
- OpenTelemetry
- Metrics
- Structured Logs
- Distributed Tracing

---

## Cloud-Native Azure Learning

Practice real Azure services:

- Azure Queue Storage
- Azure Service Bus
- Azure Functions
- Azure Blob Storage
- Azure Application Insights
- Azure Container Apps
- Azure API Management

---

# Main Architecture

```text
Client
   ↓
Payment API
   ↓
SQL Server
   ↓
Queue / Message Bus
   ↓
Payment Processor
   ↓
Fake Payment Provider
   ↓
Webhook Callback
   ↓
Merchant Webhook API
```

---

# Project Philosophy

The project intentionally avoids:

- overengineering
- huge ERP complexity
- heavy DDD ceremony
- unnecessary abstractions
- ultra-complex Clean Architecture

The goal is:

## Practical Distributed Reliability Engineering

---

# Repository Structure

```text
PaymentSim/
│
├── src/
│   ├── PaymentSim.Api
│   ├── PaymentSim.Application
│   ├── PaymentSim.Domain
│   ├── PaymentSim.Infrastructure
│   │
│   ├── PaymentSim.Worker
│   ├── PaymentSim.ProviderMock
│   ├── PaymentSim.WebhookReceiver
│   │
│   └── Shared
│       ├── Contracts
│       ├── Tracing
│       ├── Idempotency
│       └── Observability
│
├── docker/
├── docs/
├── scripts/
├── tests/
└── docker-compose.yml
```

---

# Architecture Style

## Lightweight Clean Architecture

The project uses:

- Domain
- Application
- Infrastructure

but avoids excessive abstraction.

The main complexity should live in:

- workflows
- reliability
- async processing
- distributed behaviors

NOT in:

- generic repositories
- factory chains
- unnecessary wrappers

---

# Main Technologies

## Backend

- ASP.NET Core 8
- C#
- Minimal API / Controllers

---

## Database

## Local Development

- SQL Server Docker Container

Reason:

- enterprise familiarity
- realistic transaction behavior
- EF Core support
- common enterprise backend usage

---

## Azure Cloud

Potential options:

- Azure SQL Database
- Azure SQL Managed Instance
- Azure PostgreSQL Flexible Server

Primary recommendation:

## Azure SQL Database

because it is very common in enterprise Azure environments.

---

## Messaging

### Local

- RabbitMQ

### Azure

- Azure Service Bus
- Azure Queue Storage

---

## Observability

- Serilog
- OpenTelemetry
- Application Insights
- Seq

---

## Cloud

- Azure Functions
- Azure Blob Storage
- Azure Container Apps
- Azure API Management

---

# Core Concepts

# 1. Idempotency

## Problem

Users may:

- double click payment button
- retry requests
- resend webhook calls

Without idempotency:

- duplicate charges happen

---

## Solution

Use:

```http
Idempotency-Key
```

Server stores:

```text
Key
RequestHash
Response
StatusCode
CreatedAt
```

If the same request arrives again:

- return previous response
- do NOT process payment twice

---

# 2. TraceId Propagation

Every request generates:

```text
X-Trace-Id
```

The TraceId flows through:

- API
- Queue
- Worker
- Webhook
- Blob Storage
- Azure Function

---

# 3. Event-Driven Architecture

The system uses:

- queues
- events
- async processing

instead of:

- direct synchronous calls everywhere

---

# 4. Retry Strategy

Retry policies are implemented for:

- webhook failures
- temporary network issues
- external provider instability

Example:

```text
Retry #1 -> 1 min
Retry #2 -> 5 min
Retry #3 -> 30 min
```

After max retry:

```text
DLQ
```

---

# 5. Eventual Consistency

The system does NOT require strong consistency everywhere.

Example:

- payment completed
- webhook temporarily failed
- retry later
- eventually synchronized

---

# 6. Webhook Signature Validation

Webhook payloads are protected using:

```text
HMACSHA256
```

Header example:

```text
X-Signature
```

This simulates:

- Stripe
- Adyen
- payment provider security

---

# 7. High Concurrency Protection

Future phases include:

- rate limiting
- queue buffering
- backpressure
- concurrency control
- retry isolation

---

# Fake Payment Provider

The project includes:

```text
PaymentSim.ProviderMock
```

This simulates:

- Stripe-like provider
- delayed processing
- webhook callback
- timeout
- retry
- random failures
- duplicate webhook delivery

Purpose:

Practice real-world distributed payment behavior.

---

# Observability

# Logs

Structured logging with:

- TraceId
- PaymentId
- Correlation data

---

# Metrics

Examples:

```text
payment_success_total
payment_failure_total
webhook_retry_total
queue_backlog
```

---

# Traces

Distributed tracing across:

```text
API
→ Queue
→ Worker
→ Webhook
→ Blob Storage
```

---

# Local Development Stack

## Docker Compose

The local environment should support:

```bash
docker compose up
```

and automatically start:

- SQL Server
- RabbitMQ
- Seq
- Payment API
- Worker
- ProviderMock

---

# Recommended Local Services

## SQL Server

```text
mcr.microsoft.com/mssql/server
```

---

## RabbitMQ

```text
rabbitmq:management
```

---

## Seq

For local structured logs.

---

# Planned Development Phases

# Phase 1 — Local MVP

## Goal

Build reliable local async payment flow.

## Features

- Create Payment API
- SQL Server persistence
- RabbitMQ messaging
- BackgroundService worker
- Idempotency
- TraceId
- Structured logs

---

# Phase 2 — Webhook + Retry

## Features

- Fake payment provider
- webhook callback
- webhook retry
- DLQ
- signature validation
- eventual consistency

---

# Phase 3 — Observability

## Features

- OpenTelemetry
- Metrics
- Distributed tracing
- Seq dashboards
- Request correlation

---

# Recommended Azure MVP Architecture

```text
Client
   ↓
Azure API Management
   ↓
Payment API \(Azure Container Apps\)
   ↓
Azure SQL Database
   ↓
Azure Queue Storage
   ↓
Azure Function
   ↓
Fake Payment Provider
   ↓
Webhook Callback
```

---

# Why This Azure Architecture

## Azure Container Apps \(ACA\)

Used for:

- Payment API
- containerized cloud-native hosting
- autoscaling
- future KEDA scaling
- event-driven backend architecture

Reason:

ACA provides a modern cloud-native platform without full Kubernetes complexity.

---

## Azure Queue Storage

Used for:

- lightweight async buffering
- payment processing queue
- webhook retry queue
- background task decoupling

Reason:

Simple, low-cost, serverless queueing.

---

## Azure Functions

Used for:

- queue-triggered payment processing
- webhook retry execution
- async event processing
- scheduled cleanup jobs

Reason:

Serverless event-driven compute.

---

## Azure API Management

Used for:

- API key validation
- rate limiting
- centralized external boundary
- API governance
- request tracing

Reason:

Simulate enterprise-grade API gateway architecture.

---

## Azure SQL Database

Used for:

- payment persistence
- transaction consistency
- idempotency storage
- retry state tracking

Reason:

Common enterprise Azure database choice.

---

# MVP Definition

The MVP \(Minimum Viable Product\) should support the complete minimum payment lifecycle:

```text
POST /payments
↓
Idempotency validation
↓
Persist payment
↓
Send queue message
↓
Azure Function processing
↓
Call fake provider
↓
Webhook callback
↓
Update payment status
↓
Trace + Logs visible
```

If this full flow works reliably:

The project MVP is successful.

---

# Local Development Strategy

The project should first run fully locally using Docker Compose.

Local stack:

```text
SQL Server
RabbitMQ
Seq
Payment API
Worker
ProviderMock
```

Goal:

- fast development
- debugging simplicity
- understanding distributed behavior locally first

---

# Cloud Migration Strategy

After the local version becomes stable:

## Replace RabbitMQ

→ Azure Queue Storage / Azure Service Bus

## Replace BackgroundService

→ Azure Functions

## Replace local containers

→ Azure Container Apps

## Replace local logs

→ Application Insights

This staged migration avoids unnecessary cloud complexity too early.

---

# Phase 4 — Azure Cloud Migration

## Replace Local Infrastructure

### RabbitMQ

→ Azure Service Bus

### BackgroundService

→ Azure Functions

### Local Logs

→ Application Insights

---

# Phase 5 — Blob Storage

## Features

- PDF receipt generation
- Blob upload
- SAS URL generation

---

# Phase 6 — High Concurrency

## Features

- ASP.NET Rate Limiting
- Queue backpressure
- concurrent worker scaling
- ACA auto scaling
- KEDA-based scaling

---

# Phase 7 — Chaos Engineering

## Failure Simulation

- webhook 500 errors
- duplicate webhook delivery
- slow provider responses
- DB latency
- queue overload

Goal:

Validate system resilience.

---

# Phase 8 — API Gateway

Potential additions:

- YARP
- Azure API Management

Features:

- API key
- throttling
- routing
- centralized auth
- versioning

---

# Long-Term Learning Goals

This project aims to practice:

- distributed systems
- cloud-native architecture
- reliability engineering
- event-driven systems
- observability
- async workflows
- queue-based systems
- Azure cloud architecture
- backend scalability

---

# Important Non-Goals

This project intentionally avoids:

- massive ERP complexity
- frontend-heavy focus
- enterprise IAM complexity
- advanced Kubernetes management
- excessive DDD ceremony
- over-engineered abstractions

---

# Recommended AI-Assisted Workflow

The project is intentionally designed for AI-assisted development.

Recommended workflow:

1. Define feature goal
2. Generate initial structure with AI
3. Review architecture manually
4. Implement incrementally
5. Validate behavior with logs/traces
6. Add observability
7. Stress test
8. Refactor only when necessary

---

# Recommended First Milestone

The first milestone should already support:

```text
POST /payments
↓
Idempotency validation
↓
SQL persistence
↓
RabbitMQ event
↓
Worker processing
↓
Payment status update
↓
Structured logs + TraceId
```

If this works reliably:

The project foundation is already successful.

---

# Future Topics

Potential future additions:

- Redis distributed rate limiting
- circuit breaker with Polly
- bulkhead isolation
- outbox pattern
- inbox pattern
- saga concepts
- dead letter replay
- Grafana dashboards
- k6 load testing
- OpenTelemetry Collector
- distributed cache
- API throttling
- service mesh concepts

---

# Final Vision

PaymentSim is intended to become:

## A practical distributed payment reliability playground

focused on:

- reliability
- async processing
- cloud-native architecture
- observability
- scalability
- event-driven systems

rather than:

## a business-heavy ERP application.

