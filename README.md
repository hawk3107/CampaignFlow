# CampaignFlow — Advertising Campaign Analytics Dashboard

CampaignFlow is a full-stack advertising campaign management and analytics platform built to demonstrate REST API development, relational database management, frontend development, application logging, and centralized monitoring.

## Tech Stack

### Frontend
- Vue 3
- Vite
- JavaScript
- HTML
- CSS

### Backend
- C#
- ASP.NET Core Web API
- Entity Framework Core
- REST APIs

### Database
- Microsoft SQL Server

### Monitoring & Logging
- Serilog
- Elasticsearch
- Kibana
- Docker

### Development Tools
- Git
- GitHub
- Postman

## Features

- Create, view, update and delete advertising campaigns
- Search campaigns
- Filter campaigns by status
- Campaign performance dashboard
- CTR calculation
- Conversion rate calculation
- Cost-per-click calculation
- Cost-per-conversion calculation
- RESTful API architecture
- SQL Server persistence using Entity Framework Core
- Structured application logging
- Centralized log storage using Elasticsearch
- Application monitoring dashboard using Kibana

## System Architecture

```text
                 ┌──────────────────────┐
                 │     Vue 3 Frontend   │
                 │      Dashboard       │
                 └──────────┬───────────┘
                            │
                         REST API
                            │
                            ▼
                 ┌──────────────────────┐
                 │   ASP.NET Core API   │
                 │        C#            │
                 └──────────┬───────────┘
                            │
                     Entity Framework
                            │
                            ▼
                 ┌──────────────────────┐
                 │    MS SQL Server     │
                 │   CampaignFlowDb     │
                 └──────────────────────┘


                 Application Monitoring

                 ASP.NET Core
                       │
                    Serilog
                       │
                       ▼
                 Elasticsearch
                       │
                       ▼
                    Kibana
                  Monitoring
                  Dashboard