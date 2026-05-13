# Diagrama de Componentes - Plataforma Aventura Teatros

## Código Mermaid

```mermaid
graph LR
    subgraph Cliente["👥 Camada de Apresentação"]
        WebApp["🌐 Web App<br/>(React/NextJS)"]
        MobileApp["📱 Mobile App<br/>(React Native/Flutter)"]
    end

    subgraph CDN_Edge["🌍 Content Delivery"]
        CDN["📦 CDN<br/>(CloudFront/Akamai)"]
    end

    subgraph API_Layer["🔌 API & Gateway"]
        APIGateway["🚪 API Gateway<br/>(Kong/AWS APIGw)"]
        BFF["📋 Backend for Frontend<br/>(GraphQL/REST)"]
    end

    subgraph Services["🎯 Microserviços"]
        AuthSvc["🔐 Auth Service<br/>(OAuth 2.0/JWT)"]
        CatalogSvc["📚 Catalog Service<br/>(Espetáculos)"]
        RecoSvc["⭐ Recommendation<br/>Engine<br/>(KNN/SVD)"]
        NotifSvc["🔔 Notification Service<br/>(Push)"]
        UserSvc["👤 User Service<br/>(Perfil/Wishlist)"]
    end

    subgraph Persistence["💾 Persistência de Dados"]
        AppDB[("🗄️ PostgreSQL<br/>(Usuarios, Espetaculos,<br/>Teatros, Interacoes)")]
        Cache[("⚡ Redis<br/>(Cache Layer,<br/>Sessions)")]
    end

    subgraph Messaging["📤 Event Streaming"]
        Broker["📨 Message Broker<br/>(Kafka/RabbitMQ)<br/>user_events,<br/>catalog_updates"]
    end

    subgraph DataPlatform["📊 Data & Analytics"]
        ETL["🔄 ETL/Stream<br/>Processor"]
        DataLake["📂 Data Lake<br/>(S3/Azure Blob)"]
        DW["🏗️ Data Warehouse<br/>(BigQuery/Redshift)"]
    end

    subgraph External["🔗 Integrações Externas"]
        Ticketeria["🎫 Ticketeras<br/>(Sympla/Eventim)"]
        PushProvider["📲 Push Provider<br/>(FCM/APNs)"]
    end

    %% Client to CDN
    WebApp -->|Static| CDN
    MobileApp -->|Static| CDN

    %% Client to API
    WebApp -->|HTTPS| APIGateway
    MobileApp -->|HTTPS| APIGateway

    %% API to Services
    APIGateway --> BFF
    BFF --> AuthSvc
    BFF --> CatalogSvc
    BFF --> RecoSvc
    BFF --> UserSvc
    BFF --> NotifSvc

    %% Services to DB
    AuthSvc --> AppDB
    CatalogSvc --> AppDB
    RecoSvc --> AppDB
    UserSvc --> AppDB
    RecoSvc --> Cache

    %% Services to Messaging
    UserSvc --> Broker
    CatalogSvc --> Broker
    RecoSvc --> Broker
    NotifSvc --> Broker

    %% Broker to Data Pipeline
    Broker --> ETL
    AppDB -->|Batch| ETL

    %% Data Pipeline
    ETL --> DataLake
    ETL --> DW
    DW -->|Analytics| RecoSvc

    %% Notifications
    NotifSvc --> PushProvider

    %% External
    BFF --> Ticketeria

    %% Styling
    classDef clientLayer fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px
    classDef apiLayer fill:#e3f2fd,stroke:#1565c0,stroke-width:2px
    classDef serviceLayer fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef dataLayer fill:#f3e5f5,stroke:#6a1b9a,stroke-width:2px
    classDef externalLayer fill:#fce4ec,stroke:#c2185b,stroke-width:2px

    class Cliente clientLayer
    class API_Layer apiLayer
    class Services serviceLayer
    class Persistence dataLayer
    class Messaging dataLayer
    class DataPlatform dataLayer
    class External externalLayer
```

## Legenda de Componentes

| Componente | Descrição |
|---|---|
| **Web App & Mobile App** | Interfaces de usuário multiplataforma (React para web, React Native/Flutter para mobile) que consomem APIs REST/GraphQL |
| **API Gateway & BFF** | Camada de orquestração que roteia requisições e fornece uma interface unificada aos clientes |
| **Auth Service** | Autenticação e autorização (OAuth 2.0, JWT) — gerencia login, sessões e controle de acesso |
| **Catalog Service** | Gerencia catálogo de espetáculos, teatros, gêneros e metadados (informações fixas, disponibilidade) |
| **Recommendation Engine** | Motor de recomendação personalizadas usando algoritmos KNN e SVD sobre matriz usuário-item |
| **User Service** | Perfil de usuário, preferências, wishlist, histórico de interações |
| **Notification Service** | Envia push notifications (FCM/APNs) baseadas em eventos e preferências |
| **PostgreSQL** | Banco de dados relacional principal — armazena usuários, espetáculos, teatros, interações, histórico |
| **Redis** | Cache em memória — sessões, cache de recomendações, dados quentes para performance |
| **Message Broker** | Kafka/RabbitMQ — Event Streaming para desacoplamento entre serviços e processamento assíncrono |
| **ETL/Stream Processor** | Processa eventos do broker e dados de batch para alimentar o Data Warehouse |
| **Data Lake** | Armazenamento de dados brutos (S3/Azure Blob) para auditoria e processamento futuro |
| **Data Warehouse** | OLAP (BigQuery/Redshift) — análises, métricas, input para modelos de recomendação |
| **Ticketeras (Externa)** | Plataforma de bilheteria parceira — o app redireciona para purchase |
| **Push Provider (Externa)** | FCM (Firebase) ou APNs (Apple) — entrega final de notificações ao usuário |

## Pressupostos e Decisões de Design

1. **Message Broker**: Assumido **Kafka** para streaming de eventos; alternativa: **RabbitMQ** se preferir throughput menor ou setup mais simples.
2. **Data Warehouse**: Assumido **BigQuery** (GCP) ou **Redshift** (AWS); alternativa: **Snowflake** para portabilidade multi-cloud.
3. **Escalabilidade do Recommendation**: Motivo de separação em serviço dedicado — requer alto I/O e processamento de matriz grande.
4. **Geolocalização**: Capturada no frontend (CEP/GPS) → armazenada no perfil → usada para filtros de recomendação.
5. **LGPD Compliance**: Dados sensíveis (CEP, idade, gênero) criptografados em repouso e em trânsito (TLS 1.3+); direito de deletion implementado em User Service.

## Fluxo de Dados Crítico (Recomendação)

```
1. Usuário interage (clica, wishlist, busca) → evento → UserSvc
2. UserSvc publica evento no Broker
3. RecoSvc consome eventos, atualiza matriz usuário-item
4. RecoSvc calcula scores (KNN/SVD)
5. RecoSvc publica recomendações ao cache Redis e banco
6. BFF consulta cache → retorna ao cliente em <3s
7. Cliente exibe vitrines dinâmicas
8. Eventos alimentam Data Warehouse para análise futura
```

