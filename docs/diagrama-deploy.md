# Diagrama de Deploy - Plataforma Aventura Teatros

## Código Mermaid

```mermaid
graph TD
    subgraph Users["👥 Usuários"]
        Web_User["🌐 Usuário<br/>(Browser)"]
        Mobile_User["📱 Usuário<br/>(App)"]
    end

    subgraph CDN_Global["🌍 Content Delivery Network"]
        CDN_Edge["📦 CDN Global<br/>(CloudFront / Akamai)<br/>Static Assets,<br/>JS, CSS, Images"]
    end

    subgraph Kubernetes_Cluster["☸️ Kubernetes Cluster (Production)"]
        Ingress["🚪 Ingress Controller<br/>(NGINX / AWS ALB)<br/>TLS Termination"]
        
        subgraph Frontend_Pool["🎨 Frontend Pods (x3-5 replicas)"]
            FE1["React SSR Pod 1"]
            FE2["React SSR Pod 2"]
            FE3["React SSR Pod 3"]
        end
        
        subgraph API_Pool["🔌 API Pods (x3-5 replicas)"]
            API1["BFF + Services Pod 1"]
            API2["BFF + Services Pod 2"]
            API3["BFF + Services Pod 3"]
        end
        
        subgraph Reco_Pool["⭐ Recommendation Pods (x2-3 replicas)"]
            Reco1["Recommendation Engine Pod 1"]
            Reco2["Recommendation Engine Pod 2"]
        end
        
        subgraph Worker_Pool["🔄 Background Workers (x2-4 replicas)"]
            Worker1["ETL / Stream Processor 1"]
            Worker2["ETL / Stream Processor 2"]
        end
        
        InCluster_DB[("📚 Redis In-Cluster<br/>(StatefulSet)<br/>Session Cache")]
        Broker["📨 Kafka / RabbitMQ<br/>(StatefulSet)<br/>Message Persistence")]
    end

    subgraph Managed_Services["☁️ Managed Cloud Services"]
        ManagedDB[("🗄️ Managed Postgres<br/>(AWS RDS / Azure DB)<br/>Multi-AZ, Backups")]
        ObjectStore["📂 Object Storage<br/>(S3 / Azure Blob)<br/>Data Lake")]
        DW["🏗️ Data Warehouse<br/>(BigQuery / Redshift)<br/>Analytics & ML")]
        Secrets["🔐 Secrets Manager<br/>(AWS Secrets /Azure Key Vault)<br/>DB credentials, API keys"]
    end

    subgraph External_Services["🔗 Serviços Externos"]
        Ticketeria["🎫 Ticketeras API<br/>(Sympla / Eventim)<br/>Purchase Redirect"]
        PushProvider["📲 Push Service<br/>(FCM / APNs)<br/>Notifications"]
    end

    subgraph Observability["📊 Observability & Logging"]
        Monitor["📈 Monitoring<br/>(Prometheus + Grafana)"]
        Logs["📝 Centralized Logs<br/>(ELK / CloudWatch<br/>+ Fluentd)"]
        Tracing["🔍 Distributed Tracing<br/>(Jaeger / Datadog)"]
    end

    subgraph CI_CD["🚀 CI/CD Pipeline"]
        Git["📦 Git Repository<br/>(GitHub / GitLab)"]
        Pipeline["⚙️ CI/CD Pipeline<br/>(GitHub Actions /<br/>GitLab CI)"]
        Registry["🐳 Container Registry<br/>(ECR / Azure ACR)"]
    end

    %% User to CDN
    Web_User -->|Static Assets| CDN_Edge
    Mobile_User -->|Static Assets| CDN_Edge

    %% User to K8s
    Web_User -->|HTTPS| Ingress
    Mobile_User -->|HTTPS| Ingress

    %% Ingress to Services
    Ingress --> FE1
    Ingress --> FE2
    Ingress --> FE3
    Ingress --> API1
    Ingress --> API2
    Ingress --> API3

    %% Frontend to API
    FE1 --> API1
    FE2 --> API2
    FE3 --> API3

    %% API to Recommendation
    API1 --> Reco1
    API2 --> Reco2

    %% Services to Persistence
    API1 --> ManagedDB
    API2 --> ManagedDB
    API3 --> ManagedDB
    Reco1 --> ManagedDB
    Reco2 --> ManagedDB
    API1 --> InCluster_DB
    Reco1 --> InCluster_DB

    %% Services to Messaging
    API1 --> Broker
    API2 --> Broker
    Reco1 --> Broker
    Worker1 --> Broker
    Worker2 --> Broker

    %% Workers to Data
    Worker1 --> ObjectStore
    Worker2 --> ObjectStore
    Worker1 --> DW
    Broker --> Worker1
    Broker --> Worker2

    %% Secrets Access
    API1 -.->|Get Credentials| Secrets
    Reco1 -.->|Get Credentials| Secrets
    ManagedDB -.->|Auth| Secrets

    %% External Integrations
    API1 --> Ticketeria
    API1 --> PushProvider

    %% Observability Collection
    API1 -.->|Metrics| Monitor
    Reco1 -.->|Metrics| Monitor
    API1 -.->|Logs| Logs
    Reco1 -.->|Logs| Logs
    API1 -.->|Traces| Tracing

    %% CI/CD Deployment
    Git --> Pipeline
    Pipeline --> Registry
    Registry -.->|Deploy| Ingress

    %% Styling
    classDef user fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px
    classDef k8s fill:#e3f2fd,stroke:#1565c0,stroke-width:2px
    classDef managed fill:#f3e5f5,stroke:#6a1b9a,stroke-width:2px
    classDef external fill:#fce4ec,stroke:#c2185b,stroke-width:2px
    classDef observability fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef cicd fill:#e0f2f1,stroke:#00695c,stroke-width:2px

    class Users user
    class Kubernetes_Cluster k8s
    class Managed_Services managed
    class External_Services external
    class Observability observability
    class CI_CD cicd
```

## Legenda de Deploy

| Componente | Descrição | Replicação |
|---|---|---|
| **Ingress Controller** | Ponto de entrada único para tráfego HTTP/S com TLS termination e roteamento | 1-2 (HA) |
| **Frontend Pods** | React SSR/Next.js servindo HTML renderizado + interatividade | 3-5 (auto-scaling) |
| **API Pods** | BFF + Microserviços (Auth, Catalog, User, Notification) | 3-5 (auto-scaling) |
| **Recommendation Pods** | Motor de recomendação (KNN/SVD) com alto I/O de cache e DB | 2-3 (auto-scaling) |
| **Background Workers** | ETL, stream processing de eventos, jobs assíncrono | 2-4 (auto-scaling) |
| **Redis StatefulSet** | Cache distribuído em-cluster para sessões e dados quentes | 1 Primary + 2 Replicas |
| **Kafka/RabbitMQ StatefulSet** | Message broker persistente para event streaming | 3 brokers (Kafka) |
| **Managed PostgreSQL** | Database principal em Multi-AZ com backups automáticos | Master + 2 Standbys (HA) |
| **Object Storage** | Data Lake para dados brutos e arquivos (S3/Blob) | Replicated globalmente |
| **Data Warehouse** | OLAP para análises, BI e input para modelos de recomendação | Particionado por tempo |
| **Secrets Manager** | Armazenamento seguro de credenciais, chaves de API, certificados | Encrypted, rotação automática |
| **Monitoring** | Prometheus + Grafana para métricas de infraestrutura e aplicação | Retenção 30 dias |
| **Logs Centralizados** | ELK Stack ou CloudWatch para agregação e busca de logs | Retenção 90 dias |
| **Distributed Tracing** | Jaeger/Datadog para rastrear requisições cross-service | Retenção 72h |
| **CI/CD Pipeline** | GitHub Actions/GitLab CI — build, test, push image, deploy | Triggered by git |

## Pressupostos de Arquitetura

### Suposição 1: **Cloud Provider**
**Assumido**: Multi-cloud ready (exemplos para AWS/Azure/GCP)
- **AWS**: EKS (Kubernetes gerenciado) + RDS (Postgres) + S3 (Data Lake) + BigQuery (analytics)
- **Azure**: AKS + Azure Database for PostgreSQL + Blob Storage + Azure Synapse
- **GCP**: GKE + Cloud SQL + GCS + BigQuery

**Alternativa**: On-premises Kubernetes + PostgreSQL self-managed (maior controle, mais overhead operacional)

### Suposição 2: **Número de Replicas (Auto-scaling)**
**Assumido**: 3-5 replicas para frontend/API em produção; 2-3 para recommendation
- Justificativa: Suporta ~1.000 usuários simultâneos (RNF06) com latência <3s (RNF01)

**Alternativa**: 2 replicas (custo menor, risco de downtimes); 6+ replicas (máxima HA para sites de alto tráfego)

### Suposição 3: **Message Broker**
**Assumido**: **Kafka** (production-grade streaming)
- Alternativa 1: **RabbitMQ** (setup mais simples, ideal para arquiteturas event-driven menores)
- Alternativa 2: **AWS SQS + SNS** (serverless, menos overhead operacional)

### Suposição 4: **Data Warehouse**
**Assumido**: **BigQuery** (GCP) para simplicidade; **Redshift** (AWS) como alternativa cloud-nativa
- Alternativa: **Snowflake** (multi-cloud, máxima flexibilidade)

### Suposição 5: **Monitoramento e Observability**
**Assumido**: Prometheus + Grafana (open-source) + ELK para logs
- Alternativa: **Datadog** ou **New Relic** (SaaS, mais integração pronta)

## Fluxo de Deployment (CI/CD)

```
1. Developer faz push em feature branch
2. GitHub Actions/GitLab CI roda:
   - Testes automatizados
   - Build de imagem Docker
   - Push para ECR/ACR/GCR
3. Pipeline triggered manual ou automático (para main/release)
4. Kubectl aplica manifests no K8s cluster
5. Ingress roteia tráfego para novos Pods
6. Observability coleta métricas + logs
7. Rollback automático se métricas de erro > threshold
```

## Considerações de Performance e Segurança

| Requisito | Implementação |
|---|---|
| **RNF01 (Latência ≤3s)** | Redis cache + DB indexes + CDN para assets; auto-scaling ajusta replicas conforme CPU |
| **RNF02 (Segurança)** | TLS 1.3 em transit; credentials em Secrets Manager; senhas hashed com bcrypt |
| **RNF03 (LGPD)** | Encryption at rest (managed keys); logs sem PII; deletion job no Worker Pool |
| **RNF04 (Mobile)** | CDN geográfico; SSR + CSR hybrid; responsive design em 480px+ |
| **RNF05 (Disponibilidade 99%)** | Multi-AZ DB; 3+ pod replicas; auto-healing no K8s |
| **RNF06 (1.000 concurrent users)** | Horizontal scaling; Message Broker desacopla trabalho; DW offline para analytics |
| **RNF07 (Geolocalização)** | Dados armazenados em Postgres indexed; cache Redis de lookup CEP→Cidade |

## Próximos Passos (Validação com Time)

1. **Qual provedor cloud principal?** (AWS / Azure / GCP / On-prem)
2. **Qual nível de SLA esperado?** (99% = 7.2h downtime/mês; 99.9% = 43min; 99.99% = 4min)
3. **Disaster Recovery obrigatório?** (multi-region, backup inter-cloud, RPO/RTO goals)
4. **Orçamento de infra mensal?** (impacta choice de managed services vs self-managed)
5. **Quer adicionar Security/API WAF, DDoS protection?** (AWS Shield, Cloudflare)

