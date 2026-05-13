# Arquitetura Técnica - Plataforma Aventura Teatros

## 📋 Sumário Executivo

Este repositório contém **dois diagramas de arquitetura** em Mermaid criados com base nos requisitos técnicos, funcionais e não-funcionais documentados em `docs/Projeto.md`:

1. **Diagrama de Componentes** (`diagrama-componentes.md`) — Visão lógica de serviços, camadas e fluxo de dados
2. **Diagrama de Deploy** (`diagrama-deploy.md`) — Arquitetura de infraestrutura cloud-nativa com Kubernetes

Ambos os diagramas foram estruturados para suportar:
- ✅ **RNF01**: Recomendação em <3s via cache + streaming
- ✅ **RNF02**: Segurança (TLS, hash, Secrets Manager)
- ✅ **RNF03**: Conformidade LGPD (criptografia, direito de deletion)
- ✅ **RNF04**: Responsividade mobile (SSR + CDN)
- ✅ **RNF05**: Disponibilidade 99% (Multi-AZ, replicas, health checks)
- ✅ **RNF06**: Escalabilidade 1.000 usuários concurrent (auto-scaling K8s)
- ✅ **RNF07**: Geolocalização com cache indexado

---

## 🚀 Como Usar

### Opção 1: Visualizar Online (Recomendado)

Cole o conteúdo Mermaid em qualquer editor online:
- [Mermaid Live](https://mermaid.live)
- [Excalidraw](https://excalidraw.com) — paste Mermaid syntax
- [Diagrams.net](https://diagrams.net) — File → Import → Paste Mermaid

### Opção 2: Renderizar PNG Localmente

Instale mermaid-cli:

```bash
npm install -g @mermaid-js/mermaid-cli
```

Renderize os diagramas:

```bash
mmdc -i docs/diagrama-componentes.md -o docs/diagrama-componentes.png
mmdc -i docs/diagrama-deploy.md -o docs/diagrama-deploy.png
```

Ou em lote:

```bash
for file in docs/diagrama-*.md; do 
  mmdc -i "$file" -o "${file%.md}.png"
done
```

### Opção 3: Integrar em Markdown/Documentação

Copie apenas o bloco Mermaid (entre ````mermaid` e `````) de cada arquivo e cole em um arquivo Markdown. GitHub renderiza automaticamente.

---

## 📐 Estrutura dos Diagramas

### Diagrama de Componentes (`diagrama-componentes.md`)

**O que mostra:**
- Camada de Apresentação (Web + Mobile)
- API Gateway & BFF (Backend for Frontend)
- Microserviços (Auth, Catalog, Recommendation, Notification, User)
- Persistência (PostgreSQL, Redis)
- Event Streaming (Kafka/RabbitMQ)
- Data Platform (ETL, Data Lake, Data Warehouse)
- Integrações externas (Ticketeras, Push providers)

**Fluxo crítico:**
```
Usuário interação → UserSvc → Broker → RecoSvc → Cache/DB → BFF → Cliente
                                ↓
                         ETL → DW (Analytics)
```

### Diagrama de Deploy (`diagrama-deploy.md`)

**O que mostra:**
- CDN global (Static assets)
- Kubernetes Cluster com Ingress + Pod Pools (Frontend, API, Recommendation, Workers)
- Managed Services (PostgreSQL RDS/Azure DB, Object Storage, Data Warehouse, Secrets Manager)
- Integrações externas (Ticketeras, Push providers)
- Observability (Monitoring, Logs, Tracing)
- CI/CD Pipeline (Git → Registry → Deploy)

**Características de Produção:**
- Multi-AZ para Database (HA)
- Auto-scaling de Pods (2-5 replicas por serviço)
- Secrets Manager para credenciais seguras
- Distributed tracing e centralized logging
- Health checks automáticos e rollback

---

## 🔧 Tecnologias Recomendadas

| Componente | Opções | Recomendação |
|---|---|---|
| **K8s** | EKS (AWS), AKS (Azure), GKE (GCP) | EKS para AWS; GKE para simplificar |
| **Database** | PostgreSQL managed (RDS, Azure DB, Cloud SQL) | RDS Multi-AZ (AWS) ou Azure Database |
| **Cache** | Redis (managed ou self) | ElastiCache (AWS) ou Azure Cache |
| **Broker** | Kafka, RabbitMQ, AWS SQS | Kafka (throughput alto) ou SQS (serverless) |
| **DW** | BigQuery, Redshift, Snowflake | BigQuery (simplici dade) ou Snowflake (portabilidade) |
| **Object Storage** | S3, Azure Blob, GCS | S3 (AWS) ou GCS (GCP) |
| **Secrets** | AWS Secrets Manager, Azure Key Vault, HashiCorp Vault | Secrets Manager nativo (AWS/Azure) |
| **Monitoring** | Prometheus + Grafana, Datadog, New Relic | Prometheus + Grafana (open-source) |
| **Logs** | ELK, CloudWatch, Splunk | ELK (on-prem) ou CloudWatch (AWS) |
| **Tracing** | Jaeger, Datadog, New Relic | Jaeger (open-source) |

---

## ✅ Decisões de Arquitetura Justificadas

### 1. Por que Microserviços?
- **Motivo**: Recommendation Engine é CPU/I/O intensivo; Notification é async; desacoplamento via Broker evita bottleneck
- **Alternativa**: Monólito (mais simples inicialmente, difícil de escalar selectively)

### 2. Por que Redis + PostgreSQL (não apenas Postgres)?
- **Motivo**: Postgres é excelente para durabilidade, mas Redis é 100x+ mais rápido para cache de recomendações (latência <10ms)
- **Alternativa**: Apenas PostgreSQL (latência 3-5s, viola RNF01)

### 3. Por que Kubernetes (não containers simples)?
- **Motivo**: Auto-scaling, self-healing, rolling updates, multi-node orchestration; essencial para atingir 99% uptime (RNF05)
- **Alternativa**: Docker Swarm (mais simples, menos recursos), EC2 instances (menos automação)

### 4. Por que Data Warehouse separado?
- **Motivo**: OLTP (Postgres) otimizado para transações rápidas; OLAP (DW) otimizado para queries analíticas pesadas; RecoSvc usa dados agregados do DW sem impactar database operacional
- **Alternativa**: Tudo em Postgres (query complexa mata performance)

### 5. Por que Message Broker (Kafka)?
- **Motivo**: Desacopla produtor/consumidor de eventos; permite replay (útil para debug/reprocessing); tolerância a falhas
- **Alternativa**: Direct calls (tight coupling, single point of failure), REST webhooks (lento, confiabilidade questionável)

---

## 🔍 Pressupostos e Aberturas para Discussão

### Aberto 1: Cloud Provider Principal
**Atual**: Agnóstico (exemplos para AWS/Azure/GCP)
**Questão para Time**: Qual cloud o Aventura Teatros já usa ou prefere?

### Aberto 2: Nível de Disponibilidade
**Atual**: 99% (7.2h downtime/mês)
**Questão para Time**: Precisa de 99.9% (43min) ou 99.99% (4min)?

### Aberto 3: Disaster Recovery
**Atual**: Multi-AZ (HA) dentro da região
**Questão para Time**: Precisa de multi-region (backup em region diferente)? RPO/RTO goals?

### Aberto 4: Orçamento Mensal
**Atual**: Não definido
**Questão para Time**: Qual é o orçamento aproximado para infra cloud? (impacta managed vs self-managed)

### Aberto 5: Segurança Adicional
**Atual**: HTTPS + Secrets Manager + LGPD
**Questão para Time**: Precisa de WAF (Web Application Firewall), DDoS protection, Scanning de vulnerabilidades?

---

## 📊 Benchmark de Performance Esperado

Com a arquitetura proposta e RNF06 (1.000 usuários simultâneos):

| Operação | P50 | P99 | P99.9 |
|---|---|---|---|
| Home (com recomendações) | 0.8s | 1.5s | 2.8s |
| Busca por espetáculo | 0.3s | 0.6s | 1.2s |
| Login (Auth) | 0.5s | 1.0s | 2.0s |
| Adicionar à Wishlist | 0.2s | 0.5s | 0.8s |
| Redirecionamento Ticketeria | 0.1s | 0.2s | 0.3s |

**Assunção**: Cache hit rate >90% para recomendações; DB indexes otimizados

---

## 🚨 Checklist de Implementação

- [ ] Escolher cloud provider principal
- [ ] Criar AWS/Azure/GCP account + tenants/regions
- [ ] Provisionar VPC, Security Groups, IAM roles
- [ ] Setup EKS/AKS/GKE cluster
- [ ] Provisionar managed PostgreSQL (Multi-AZ)
- [ ] Setup Redis cache (replicado)
- [ ] Configurar Kafka/Message Broker
- [ ] Criar CI/CD pipeline (GitHub Actions / GitLab CI)
- [ ] Setup container registry (ECR/ACR/GCR)
- [ ] Implementar Secrets Manager
- [ ] Configurar CDN para static assets
- [ ] Setup Prometheus + Grafana ou Datadog
- [ ] Centralizar logs (ELK / CloudWatch)
- [ ] Implementar Distributed Tracing (Jaeger)
- [ ] Load testing com 1.000 usuários (simular RNF06)
- [ ] Security scanning (SAST, dependency check)
- [ ] Audit logging para LGPD compliance

---

## 📞 Próximas Etapas

1. **Validar com Product/Business**: RNF06 (1.000 concurrent) é realista?
2. **Escolher Cloud Provider**: AWS, Azure, GCP ou híbrido?
3. **Estimativa de Custo**: Simular no pricing calculator do provider escolhido
4. **Definir SLA & RTO/RPO**: 99% é suficiente ou precisa 99.9%?
5. **Security Review**: WAF, DDoS protection, compliance adicional?
6. **Proof of Concept**: Deployar uma feature simples end-to-end (ex: Catalog Service)

---

## 📖 Referências e Leitura Adicional

- [12 Factor App](https://12factor.net) — Boas práticas para aplicações cloud-native
- [Google Cloud - Designing Scalable Applications](https://cloud.google.com/docs/tutorials/scalable-app-design)
- [Kubernetes Best Practices](https://kubernetes.io/docs/concepts/configuration/overview/)
- [Microservices Patterns](https://microservices.io/patterns/index.html)
- [Event Sourcing & CQRS](https://martinfowler.com/bliki/CQRS.html)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/) — Segurança de aplicações web

---

**Versão**: 1.0  
**Data**: Mai 2026  
**Autores**: Equipe de Arquitetura - Aventura Teatros  
**Status**: ✅ Pronto para discussão e validação

