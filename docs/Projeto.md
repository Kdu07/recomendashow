# Projeto - Sistema de Recomendação - Aventura

## Sumário

<details>
<summary><a href="#1-problema">1. Problema</a></summary>

- [Descrição do Problema](#descrição-do-problema)
- [Relevância no Contexto do Parceiro](#relevância-no-contexto-do-parceiro)
- [Desafios e Oportunidades](#desafios-e-oportunidades)

</details>

<details>
<summary><a href="#2-contexto-da-indústria-do-parceiro">2. Contexto da Indústria do Parceiro</a></summary>

- [2.1 Rivalidade entre concorrentes](#21-rivalidade-entre-concorrentes)
- [2.2 Ameaça de novos entrantes](#22-ameaça-de-novos-entrantes)
- [2.3 Poder de barganha dos fornecedores](#23-poder-de-barganha-dos-fornecedores)
- [2.4 Poder de barganha dos clientes](#24-poder-de-barganha-dos-clientes)
- [2.5 Ameaça de produtos substitutos](#25-ameaça-de-produtos-substitutos)
- [2.6 Análise SWOT](#26-análise-swot)

</details>

<details>
<summary><a href="#3-visão-e-objetivos-do-produto">3. Visão e Objetivos do Produto</a></summary>

- [3.1 Visão do Produto](#31-visão-do-produto)
- [3.2 Objetivos do Produto](#32-objetivos-do-produto)
- [3.3 Métricas de Sucesso (KPIs)](#33-métricas-de-sucesso-kpis)

</details>

<details>
<summary><a href="#4-o-que-o-produto-faz-e-o-que-não-faz">4. O que o Produto Faz e o que Não Faz</a></summary>

- [4.1 O Produto É](#41-o-produto-é)
- [4.2 O Produto Não É](#42-o-produto-não-é)
- [4.3 O Produto Faz](#43-o-produto-faz)
- [4.4 O Produto Não Faz](#44-o-produto-não-faz)

</details>

<details>
<summary><a href="#5-personas-e-jornada-do-usuário">5. Personas e Jornada do Usuário</a></summary>

- [5.1 Personas](#51-personas)
  - [5.1.1 Ana Luiza — Cliente em Formação](#511-ana-luiza--cliente-em-formação-descoberta)
  - [5.1.2 Marcos Abrantes — Cliente Ativo](#512-marcos-abrantes--cliente-ativo-decisão-e-consumo)
- [5.2 Jornada do Usuário](#52-jornada-do-usuário)
  - [5.2.1 Jornada de Ana Luiza](#521-jornada-de-ana-luiza--cliente-em-formação)
  - [5.2.2 Jornada de Marcos Abrantes](#522-jornada-de-marcos-abrantes--cliente-ativo)

</details>

<details>
<summary><a href="#6-fluxo-do-negócio">6. Fluxo do Negócio</a></summary>

- [Fluxo do Negócio](#6-fluxo-do-negócio)

</details>

<details>
<summary><a href="#7-brainstorming-de-features">7. Brainstorming de Features</a></summary>

- [1. Visão Geral do Modelo](#1-visão-geral-do-modelo-backend--inteligência)
- [2. Interface do Usuário](#2-interface-do-usuário-front-end)
- [2.1 Fluxo de Cadastro e Onboarding](#21-fluxo-de-cadastro-e-onboarding)
- [2.2 Estrutura de Navegação](#22-estrutura-de-navegação)
- [3. Funcionalidades Core](#3-funcionalidades-core)
- [4. Roadmap: Funcionalidades Adicionais](#4-roadmap-funcionalidades-adicionais-futuro)

</details>

<details>
<summary><a href="#8-business-model-canvas-do-mvp">8. Business Model Canvas do MVP</a></summary>

- [Business Model Canvas do MVP](#8-business-model-canvas-do-mvp)

</details>

<details>
<summary><a href="#9-requisitos-funcionais">9. Requisitos Funcionais</a></summary>

- [9.1 Elicitação e Análise de Requisitos](#91--elicitação-e-análise-de-requisitos)
- [9.2 Especificação e Validação de Requisitos Funcionais](#92-especificação-e-validação-de-requisitos-funcionais)
- [RF01 — Onboarding de Interesses](#rf01--onboarding-de-interesses)
- [RF02 — Recomendação por Geolocalização](#rf02--recomendação-por-geolocalização)
- [RF03 — Autenticação e Persistência](#rf03--autenticação-e-persistência)
- [RF04 — Catálogo Categorizado](#rf04--catálogo-categorizado)
- [RF05 — Busca Específica](#rf05--busca-específica)
- [RF06 — Gestão de Wishlist](#rf06--gestão-de-wishlist)
- [RF07 — Informações de Teatros](#rf07--informações-de-teatros)
- [RF08 — Gestão de Perfil e Preferências](#rf08--gestão-de-perfil-e-preferências)
- [RF09 — Redirecionamento de Compra](#rf09--redirecionamento-de-compra)
- [RF10 — Recomendação Inteligente](#rf10--recomendação-inteligente)

</details>

<details>
<summary><a href="#10-requisitos-não-funcionais">10. Requisitos Não Funcionais</a></summary>

- [RNF01 — Tempo de resposta das recomendações](#rnf01--tempo-de-resposta-das-recomendações)
- [RNF02 — Segurança das credenciais de usuário](#rnf02--segurança-das-credenciais-de-usuário)
- [RNF03 — Conformidade com a LGPD](#rnf03--conformidade-com-a-lgpd)
- [RNF04 — Usabilidade da interface em dispositivos móveis](#rnf04--usabilidade-da-interface-em-dispositivos-móveis)
- [RNF05 — Disponibilidade da plataforma](#rnf05--disponibilidade-da-plataforma)
- [RNF06 — Escalabilidade do sistema de recomendação](#rnf06--escalabilidade-do-sistema-de-recomendação)
- [RNF07 — Precisão da geolocalização](#rnf07--precisão-da-geolocalização)

</details>

<details>
<summary><a href="#11-correlação-entre-rfs-e-rnfs">11. Correlação entre RFs e RNFs</a></summary>

- [Correlação entre RFs e RNFs](#11-correlação-entre-rfs-e-rnfs)

</details>

<details>
<summary><a href="#12-diagramação-do-banco">12. Diagramação do banco</a></summary>

- [12.1 Diagrama Lógico](#121---diagrama-lógico)
- [12.2 Entidades e Relacionamentos](#122---entidades-e-relacionamentos)
- [12.3 Justificativa e Regras de Negócio](#123---justificativa-e-regras-de-negócio)
- [12.4 Diagrama Conceitual](#124-diagrama-conceitual)
- [12.4.1 Dicionário de Dados e Mapeamento](#1241-dicionário-de-dados-e-mapeamento)
- [12.4.2 Definição de Chaves e Integridade Referencial](#1242-definição-de-chaves-e-integridade-referencial)
- [12.4.3 Regras de Normalização Aplicadas](#1243-regras-de-normalização-aplicadas)
- [12.4.4 Justificativa e Rastreabilidade](#1244-justificativa-e-rastreabilidade)

</details>

<details>
<summary><a href="#13-diagrama-de-classes">13. Diagrama de Classes</a></summary>

- [Teatro](#teatro)
- [Espetaculo](#espetaculo)
- [Usuario](#usuario)
- [Wishlist](#wishlist)
- [Preferencias](#preferencias-value-object)
- [Interacao](#interacao)
- [Recomendacao](#recomendacao)
- [SistemaRecomendacao](#sistemarecomendacao)
- [TipoInteracao](#tipointeracao-enumeration)
- [Relacionamentos](#relacionamentos)
- [Observações e Decisões de Design](#observações-e-decisões-de-design)

</details>

<details>
<summary><a href="#14-diagrama-de-componentes">14. Diagrama de Componentes</a></summary>

- [Packages](#packages)
- [Componentes](#componentes)
- [Interfaces](#interfaces)
- [Responsabilidades do Storage Bucket](#responsabilidades-do-storage-bucket)
- [Responsabilidades do PostgreSQL](#responsabilidades-do-postgresql)
- [Decisões](#decisões)

</details>

<details>
<summary><a href="#15-diagrama-de-casos-de-uso">15. Diagrama de Casos de uso</a></summary>

- [Atores](#atores)
- [Casos de Uso](#casos-de-uso)
- [Relacionamentos do Diagrama](#relacionamentos-do-diagrama)
- [Associação entre Requisitos Funcionais e Casos de Uso](#associação-entre-requisitos-funcionais-e-casos-de-uso)

</details>

<details>
<summary><a href="#16-linguagens-e-tecnologias-utilizadas">16. Linguagens e Tecnologias Utilizadas</a></summary>

- [Linguagens e Tecnologias Utilizadas](#16-linguagens-e-tecnologias-utilizadas)

</details>

<details>
<summary><a href="#17-planejamento-de-testes">17. Planejamento de Testes</a></summary>

- [17.1 Objetivo do planejamento de testes](#171-objetivo-do-planejamento-de-testes)
- [17.2 Estratégia geral de testes](#172-estratégia-geral-de-testes)
- [17.3 Testes de funcionalidade](#173-testes-de-funcionalidade)
- [17.4 Testes de requisitos não funcionais](#174-testes-de-requisitos-não-funcionais)
- [17.5 Testes de usabilidade](#175-testes-de-usabilidade)
- [17.6 Testes de APIs externas](#176-testes-de-apis-externas)
- [17.7 Testes de integração](#177-testes-de-integração)
- [17.8 Discussão geral de abrangência dos testes](#178-discussão-geral-de-abrangência-dos-testes)
</details>

<details>
<summary><a href="#18-desenvolvimento-do-projeto--sprint-3">18. Desenvolvimento Do Projeto — Sprint 3</a></summary>

- [18.1 Objetivo do artefato](#181-objetivo-do-artefato)
- [18.2 Arquitetura de codificação e estrutura de diretórios](#182-arquitetura-de-codificação-e-estrutura-de-diretórios)
- [18.3 Desenvolvimento do Backend](#183-desenvolvimento-do-backend)
  - [18.3.1 Arquitetura do backend](#1831-arquitetura-do-backend)
- [18.4 Desenvolvimento do Frontend](#184-desenvolvimento-do-frontend)
  - [18.4.1 Arquitetura do frontend](#1841-arquitetura-do-frontend)
  - [18.4.2 Telas implementadas](#1842-telas-implementadas)
- [18.5 Padrões de codificação e padrões de projeto](#185-padrões-de-codificação-e-padrões-de-projeto)
  - [Padrões de codificação](#padrões-de-codificação)
  - [Padrões de projeto](#padrões-de-projeto)
- [18.6 Testes unitários e metodologia TDD](#186-testes-unitários-e-metodologia-tdd)
  - [Estratégia de testes](#estratégia-de-testes)
  - [Testes implementados](#testes-implementados)
  - [Execução dos testes](#execução-dos-testes)
  - [Resultado da cobertura](#resultado-da-cobertura)
  - [Link do vídeo demonstrativo](#link-do-vídeo-demonstrativo)

</details>

<details>
<summary><a href="#19-termos-e-abreviações">19. Termos e Abreviações</a></summary>

- [Termos e Abreviações](#19-termos-e-abreviações)

</details>

- [Referências](#referências)

## 1. Problema

A **Aventura Teatros**, fundada em 2008, consolidou-se como uma plataforma cultural completa e referência na economia criativa brasileira, acumulando mais de 50 produções de alto nível e um alcance de 5 milhões de espectadores. No entanto, esse sucesso no ambiente físico encontra um gargalo crítico em sua presença digital: atualmente a empresa só possui um site de informações de forma estática e linear, carecendo de um motor inteligente de recomendação de conteúdos e espetáculos.

### Descrição do Problema
O problema central reside na fragmentação da jornada do usuário dentro da plataforma. Atualmente, ao acessar a página de um espetáculo ou equipamento cultural específico, o visitante não é impactado por sugestões personalizadas que conectem outros ativos do ecossistema da empresa. Essa ausência de um sistema de recomendação, interrompe o fluxo de exploração, fazendo com que o potencial de descoberta de novos conteúdos seja subutilizado e, consequentemente, reduzindo as taxas de conversão em vendas.

### Relevância no Contexto do Parceiro
A resolução desta lacuna tecnológica é vital e estratégica para a Aventura por três pilares fundamentais:

* **Prioridade de Vendas:** Retenção e Aumento de Vendas: O fator de maior impacto esperado para o negócio é a retenção de vendas por meio de recomendações estratégicas. A ferramenta é vista como um motor essencial para impulsionar a venda de ingressos, pois direciona ativamente o espectador para novos espetáculos de seu interesse, reduzindo a evasão do site e facilitando a jornada até o momento da compra.
* **Fortalecimento do Rebranding:** O projeto atua diretamente na mitigação do baixo engajamento digital, servindo como uma ferramenta de suporte ao atual esforço de reposicionamento da marca no mercado.
* **Sinergia do Ecossistema:** A solução visa criar uma jornada conectada entre as diversas produções (teatro musical, festivais e ballet), incentivando o público a consumir a multiplicidade artística oferecida pela produtora, mostarndo de forma mais dinâmica e pessoal seus espetáculos.

### Desafios e Oportunidades

* **Desafios:** O principal desafio técnico consiste em projetar uma arquitetura orientada a serviços (SOA) que suporte o processamento de dados de interesse do usuário para gerar recomendações assertivas em tempo real, com os dados que foram disponibilizados. Além do desafio técnico, existe também um desafio de negócio: garantir que as recomendações realmente agreguem valor à experiência do usuário, evitando sugestões genéricas e contribuindo efetivamente para ampliar a descoberta de conteúdos e a conversão no portal.
* **Oportunidades:** Apesar dos desafios, o projeto apresenta uma oportunidade clara de transformação digital para o parceiro. Ao incorporar uma camada de recomendação inteligente, o Portal Aventura pode deixar de ser uma plataforma de navegação estática e passar a atuar como um hub dinâmico de entretenimento e descoberta cultural. Essa mudança tende a aumentar o tempo de permanência dos usuários no portal, melhorar a experiência de navegação, estimular o consumo cruzado entre diferentes produções e fortalecer o vínculo do público com o ecossistema da Aventura. 
Dessa forma, a solução proposta não apenas responde a uma lacuna tecnológica existente, mas também cria condições para que a experiência digital da Aventura se torne mais conectada, personalizada e estratégica para a geração de valor ao negócio.
---

## 2. Contexto da Indústria do Parceiro

Para compreender o ambiente competitivo em que a Aventura Teatros está inserida, é fundamental analisar a estrutura da indústria de produção teatral sob a ótica do modelo das 5 Forças de Porter. Essa abordagem permite avaliar não apenas a concorrência direta, mas também as dinâmicas que envolvem novos entrantes, fornecedores, clientes e produtos substitutos, oferecendo uma visão sistêmica do setor.

A figura a seguir apresenta uma síntese dessas forças aplicadas ao contexto da Aventura Teatros, servindo como base para a análise detalhada de cada dimensão competitiva nas seções seguintes. A partir dessa estrutura, torna-se possível identificar os principais fatores que influenciam a competitividade da empresa, bem como os riscos e oportunidades que impactam sua atuação nos mercados de São Paulo e Rio de Janeiro.

<div align="center">
<sub>Imagem 01 - Cinco Forças de Porter</sub><br>
  <img src="../assets/5-forcas-de-porter.png" width="100%" alt="Cinco Forças de Porter"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>


### 2.1 Rivalidade entre concorrentes

**Risco: Alto**

A Aventura Teatros é uma empresa de produção de espetáculos que atua principalmente nas cidades de São Paulo e Rio de Janeiro, os dois maiores polos culturais do país. Nesses mercados, a rivalidade competitiva é intensa: apenas em São Paulo, a APETESP, Associação de Produtores de Espetáculos Teatrais do Estado de São Paulo, representa cerca de 108 empresas produtoras, evidenciando a densa concentração de concorrentes na principal praça de atuação da Aventura. No Rio de Janeiro, a disputa é igualmente acirrada, com a APTR, Associação dos Produtores de Teatro do Rio de Janeiro, reunindo produtoras locais de expressiva atuação.

Além da quantidade de competidores, a rivalidade se manifesta na disputa por recursos escassos: patrocínios privados, editais de fomento cultural, datas em teatros de prestígio e elencos renomados. A produção teatral abrange diferentes formatos, como musicais, teatro de grupo, produções comerciais e independentes, além de apresentações em teatros de shopping, centros culturais, praças e parques, o que amplia o leque de competidores diretos e indiretos com os quais a Aventura precisa se posicionar. Some-se a isso a sazonalidade do setor, que concentra a disputa por público em períodos específicos do ano, como férias escolares e fim de ano, intensificando ainda mais a competição. 

Diante de um mercado com mais de uma centena de produtoras concorrentes nos principais centros culturais do país, recursos finitos e demanda sazonalmente concentrada, a rivalidade entre os players estabelecidos é estruturalmente alta. Para a Aventura Teatros, isso significa que crescimento e sustentabilidade dependem não apenas da qualidade dos espetáculos, mas da capacidade de construir uma identidade de marca reconhecível e de fidelizar parceiros estratégicos.

### 2.2 Ameaça de novos entrantes

**Risco: Médio**

No mercado de produção teatral, o principal obstáculo à entrada de novos competidores é o capital. Produções musicais de grande porte podem demandar captações superiores a R$7 milhões, e mesmo projetos de menor escala envolvem custos expressivos com elenco, cenografia, figurino, iluminação e equipe técnica. O financiamento do setor é fortemente dependente de incentivos fiscais como a Lei Rouanet, cujo acesso exige histórico, credibilidade e conhecimento burocrático, ativos que novos entrantes não possuem de imediato. Além disso, a disputa por elencos qualificados, teatros com boa localização e datas estratégicas favorece as produtoras já estabelecidas, que contam com relacionamentos consolidados no setor, criando uma vantagem competitiva difícil de replicar no curto prazo.

Por outro lado, as barreiras regulatórias são baixas: não há exigências de licenças especiais ou certificações específicas para atuar como produtora teatral, o que permite que novos agentes, inclusive produtores independentes e grupos artísticos, entrem no mercado com produções de menor escala. A digitalização e o crescimento das plataformas de financiamento coletivo também reduziram parte da barreira financeira, abrindo espaço para estreantes que não dependem exclusivamente de editais ou patrocínios corporativos. Dessa forma, o risco mais concreto para a Aventura Teatros não vem de grandes produtoras entrantes, mas de competidores de nicho que operam com estruturas enxutas, custos reduzidos e propostas artísticas diferenciadas, capazes de disputar públicos e espaços específicos sem precisar competir no mesmo patamar de investimento.


### 2.3 Poder de barganha dos fornecedores

**Risco: Médio-Alto**

O poder de barganha dos fornecedores da Aventura é classificado como médio-alto. A cadeia produtiva de seus espetáculos depende de atores, diretores, cenógrafos e músicos com reconhecimento público, profissionais escassos num mercado que comporta apenas dez a doze grandes produções musicais em cartaz simultaneamente (Infoteatro, 2025). Esse cenário eleva o custo de troca para a Aventura e é agravado pela dinâmica da Lei Rouanet, que ao injetar patrocínio privado via renúncia fiscal elevou os preços de cachês e serviços técnicos ao longo dos anos (Infoteatro, 2025). Adicionalmente, títulos licenciados da Broadway presentes no portfólio da empresa, como "Mamma Mia!" e "A Noviça Rebelde" (Inteli & Aventura, 2024), criam dependência de licenciadores estrangeiros com poder de negociação elevado.

### 2.4 Poder de barganha dos clientes

**Risco: Médio**

O poder de barganha dos compradores da Aventura é classificado como médio. O alto grau de diferenciação de suas produções, com mais de 5 milhões de espectadores históricos e parcerias exclusivas com Disney-Pixar (Inteli & Aventura, 2024), limita o poder de negociação do consumidor individual ao dificultar a substituição por concorrentes. Ainda assim, a sensibilidade ao preço é uma variável relevante: o ticket médio dos espetáculos ao vivo no Brasil atingiu R$ 432 em 2024 (Anafima, 2024), e pesquisa da Revista Diálogo com a Economia Criativa (ESPM, 2018) aponta a política de preços como um dos fatores mais determinantes na decisão de consumo de teatro musical no país.

### 2.5 Ameaça de produtos substitutos

**Risco: Médio**

A Aventura Teatros compete por um bem escasso: o tempo de lazer do consumidor. Nesse campo, os substitutos são numerosos e estão em expansão acelerada. O streaming consolidou-se como o principal meio de entretenimento doméstico no Brasil, com dezenas de milhões de lares assinando ao menos uma plataforma, oferecendo acesso praticamente ilimitado a conteúdo cultural por uma fração do valor de um ingresso teatral. O cinema, por sua vez, também avança com força, em 2024, os cinemas brasileiros venderam mais de 116 milhões de ingressos, crescimento de 6,5% em relação ao ano anterior, enquanto shows e eventos ao vivo disputam as mesmas datas e o mesmo orçamento do público. Em conjunto, essas alternativas formam um ecossistema de entretenimento cada vez mais acessível e conveniente, o que pressiona o teatro a justificar seu custo mais elevado.
No entanto, a substituição é apenas parcial. O teatro acontece com a presença viva dos artistas em cena, compartilhando o mesmo espaço com a plateia por um determinado tempo, uma dimensão que nenhuma tela consegue replicar. A singularidade de cada apresentação, a efemeridade do ao vivo e o pertencimento coletivo à experiência conferem ao espetáculo teatral um valor simbólico próprio, que fideliza seu público e dificulta a substituição plena. Dessa forma, a ameaça dos substitutos é real e crescente na disputa pelo tempo e pela renda do consumidor, mas limitada quando se trata de replicar a experiência teatral em si, o que posiciona essa força em um nível médio de pressão sobre a Aventura Teatros.

### 2.6 Análise SWOT
 
Complementar às 5 Forças de Porter, a análise SWOT permite consolidar os fatores internos e externos que influenciam o posicionamento estratégico da Aventura Teatros, conectando os elementos competitivos identificados anteriormente a uma visão integrada de forças, fraquezas, oportunidades e ameaças.
 
<div align="center">
<sub>Imagem 02 - Análise SWOT</sub><br>
  <img src="../assets/analise-swot.png" width="100%" alt="Análise SWOT - Aventura Teatros"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>

#### **Forças**
 
A principal força da Aventura Teatros está em seu posicionamento em produções de grande escala, especialmente musicais. Esse modelo se beneficia do uso de propriedades intelectuais conhecidas, como biografias ou marcas culturais, o que reduz incertezas de demanda. Esse fenômeno é discutido no relatório Global Entertainment & Media Outlook da PwC, que aponta que conteúdos baseados em marcas consolidadas tendem a apresentar maior previsibilidade de audiência no setor de entretenimento.
 
Além disso, a empresa apresenta alto grau de consolidação, refletido em seu histórico de produções e alcance de público. Essa trajetória favorece a captação de recursos por meio de mecanismos formais de incentivo. Segundo a página oficial da Lei Rouanet (Lei nº 8.313/1991), projetos culturais aprovados podem captar recursos junto a empresas via renúncia fiscal, sendo comum que produtores com histórico consolidado tenham maior taxa de sucesso nesse processo. Essa vantagem reforça a capacidade da Aventura Teatros de viabilizar projetos de maior escala.
 
#### **Fraquezas**
 
O modelo adotado pela empresa implica um alto custo estrutural de produção, característica típica do setor de artes performáticas. De acordo com o relatório Cultural Times: The First Global Map of Cultural and Creative Industries, publicado pela UNESCO, produções ao vivo possuem elevada dependência de custos fixos e menor escalabilidade em comparação a conteúdos digitais, o que aumenta sua exposição a riscos financeiros.
 
Esse fator contribui diretamente para o alto valor do ticket médio, necessário para garantir a viabilidade econômica dos espetáculos. No contexto brasileiro, essa limitação é reforçada por dados da pesquisa Desigualdades Sociais por Cor ou Raça no Brasil e levantamentos de consumo cultural do IBGE, que indicam que o acesso a atividades culturais pagas ainda é concentrado em parcelas da população com maior renda. Assim, o modelo de precificação restringe o público potencial e torna a demanda mais sensível a variações econômicas.
 
#### **Oportunidades**
 
Uma oportunidade relevante está na expansão para mercados fora do eixo tradicional. Dados da Associação Paulista dos Produtores de Teatro mostram que a maior parte das produções teatrais está concentrada em São Paulo, enquanto o Rio de Janeiro também se destaca como polo relevante. Essa concentração sugere a existência de demanda reprimida em outras capitais, onde a concorrência é menor e o acesso a grandes produções ainda é limitado.
 
Outra oportunidade está no crescimento do investimento privado em experiências culturais. O relatório Global Entertainment & Media Outlook da PwC destaca o aumento do interesse de marcas em experiências presenciais como forma de engajamento, especialmente em um cenário de saturação da publicidade digital. No Brasil, esse movimento é potencializado por mecanismos como a Lei Rouanet, que permite às empresas direcionar parte de seus impostos para projetos culturais.
 
Além disso, a diversificação de formatos representa uma oportunidade estratégica relevante. O crescimento do stand-up comedy e de produções menores reflete uma mudança no padrão de consumo, com maior valorização de experiências mais acessíveis e frequentes. Embora não exista um único relatório centralizador desse movimento, ele é amplamente observado na expansão de circuitos de comédia e na ocupação crescente de teatros por esse tipo de espetáculo, indicando uma tendência de flexibilização do setor.
 
#### **Ameaças**
 
Entre as ameaças, destaca-se a competição por espaços físicos, intensificada pela expansão de outros formatos de entretenimento ao vivo, como shows e apresentações de comédia. Esse fenômeno impacta diretamente a disponibilidade de teatros e pode elevar custos operacionais, especialmente em grandes centros urbanos.
 
Além disso, a concorrência com plataformas digitais representa uma transformação estrutural no setor. Segundo o relatório Global Entertainment & Media Outlook 2023–2027 da PwC, o consumo de streaming continua em crescimento global, com aumento significativo do tempo gasto em plataformas digitais. Serviços como Netflix e YouTube oferecem conteúdo sob demanda, com menor custo e maior conveniência, competindo diretamente pela atenção do consumidor e reduzindo a atratividade relativa de experiências presenciais.

## 3. Visão e Objetivos do Produto

### 3.1 Visão do Produto

A visão do produto é conceber e estabelecer um novo ecossistema digital da Aventura Teatros. Através de um motor de recomendação orientado a dados, o sistema conectará o público ao vasto catálogo da produtora, antecipando interesses e sugerindo espetáculos de forma fluida. O propósito é criar uma jornada digital que retenha o usuário no ecossistema da marca, convertendo esse engajamento contínuo em um aumento real na venda de ingressos.

### 3.2 Objetivos do Produto
O desenvolvimento desta solução tem metas claras, alinhadas diretamente às necessidades de negócio da Aventura Teatros e ao valor entregue aos usuários:

* **Maximizar a Retenção e Conversão de Vendas:** Atuar como o principal motor de direcionamento estratégico da empresa. Ao sugerir de forma inteligente novos espetáculos antes que o usuário abandone a página, o sistema visa reter o cliente no portal, transformando o tempo de navegação em aumento direto na venda de ingressos.
* **Personalizar a Jornada de Descoberta:** Transformar a experiência do usuário de reativa para proativa. O objetivo é que cada visitante sinta que o portal entende suas preferências, recebendo recomendações assertivas que reduzem o atrito na busca por entretenimento.
* **Integrar o Ecossistema Aventura:** Quebrar os silos entre as diferentes produções (teatros, musical, festivais, etc.). O sistema deve criar pontes fluidas, incentivando o público de um nicho a descobrir e consumir a multiplicidade artística da produtora.
* **Implementar Arquitetura Escalável:** Entregar uma base tecnológica moderna através de uma Arquitetura Orientada a Serviços (SOA), capaz de processar os dados de interesse do usuário em tempo real e fornecer as recomendações de forma ágil, escalável e com alto desempenho.

### 3.3 Métricas de Sucesso (KPIs)
Para garantir e monitorar que os objetivos acima sejam atingidos, o produto focará nos seguintes indicadores chave:
* **Taxa de Conversão:** Aumento na porcentagem de usuários que clicam em uma recomendação e são direcionados ao parceiro de bilheteria.
* **Engajamento e Retenção:** Aumento do tempo médio de sessão e do número de páginas/telas acessadas por visita.
* **Taxa de Cliques (CTR):** Volume de interação direta com os carrosséis de vitrines dinâmicas e notificações push.

---

## 4. O que o Produto Faz e o que Não Faz

Considerando o desafio de criar um hub digital dinâmico, é fundamental estabelecer fronteiras claras de atuação. Para garantir o alinhamento de expectativas e o foco na entrega de valor estratégico para a Aventura Teatros, utilizamos a matriz **"É / Não É / Faz / Não Faz"**. Esta estrutura delimita com precisão o escopo do projeto, definindo o papel do motor de recomendação e da aplicação multiplataforma (Web e Mobile) dentro do ecossistema da empresa, e evitando sobreposições com sistemas já consolidados, como as parcerias com a bilheteria.

### 4.1. O Produto É:
* **Uma solução multiplataforma:** Uma aplicação disponível tanto em versão Web quanto em aplicativo móvel híbrido (iOS e Android), garantindo uma experiência consistente em diferentes dispositivos.
* **Um motor de recomendação inteligente:** Um sistema (via API/SOA) que personaliza o conteúdo exibido no app com base no perfil do usuário.
* **Um hub de engajamento digital:** Uma ferramenta estratégica para reter o público da Aventura Teatros através de uma jornada conectada e fluida.

### 4.2. O Produto Não É:
* **Um sistema de e-commerce ou bilheteria (Ticketing):** O produto não é um gateway de pagamento e não processa transações financeiras internamente; ele atua como um canal de captação e direcionamento.
* **Um Sistema de Gestão de Conteúdo (CMS):** Não substitui o banco de dados ou painel onde a Aventura cadastra seus eventos.
* **Uma rede social:** O foco é a recomendação de ativos da Aventura, e não a interação social entre usuários.

### 4.3. O Produto Faz:
* **Notificações Personalizadas (Push):** Utiliza o motor de recomendação para enviar sugestões relevantes diretamente ao smartphone do usuário.
* **Exibe Vitrines Dinâmicas:** Apresenta carrosséis de espetáculos, teatros e conteúdos baseados no comportamento de navegação no app.
* **Integra-se com APIs da Aventura:** Consome os dados de eventos e peças para organizar as recomendações em tempo real.
* **Monitora Interesses do Usuário:** Mapeia quais categorias (musicais, festivais) o usuário mais acessa para refinar as sugestões.

### 4.4. O Produto Não Faz:
* **Não vende ingressos diretamente:** A finalização da compra ocorre via redirecionamento para o parceiro de ticketing.
* **Não funciona offline:** Por depender de um motor de recomendação em tempo real e atualização de catálogo, o app exige conexão com a internet.
* **Não gerencia a logística dos teatros:** O escopo é estritamente informativo e de recomendação, sem controle de assentos ou entrada física (QR Code de acesso pode estar fora de escopo, dependendo do que vocês decidiram).

---

## 5. Personas e Jornada do Usuário
 
---
 
## 5.1 Personas
 
### 5.1.1 Ana Luiza — Cliente em Formação (Descoberta)
 
Ana Luiza representa um perfil emergente de consumidores culturais em formação: jovens que ainda não possuem o hábito consolidado de frequentar teatro, mas demonstram interesse quando expostos a estímulos visuais, sociais e tendências digitais. Sua relação com o lazer é fortemente influenciada por redes sociais e pelo grupo de amigos, tornando a descoberta de experiências mais espontânea do que planejada.
 
Apesar de não ser uma consumidora ativa do setor, Ana exerce influência significativa na decisão de compra, especialmente em contextos familiares. No entanto, enfrenta barreiras como a falta de direcionamento, dificuldade em identificar conteúdos adequados para sua faixa etária e baixa atratividade das plataformas tradicionais. Esse cenário evidencia a necessidade de soluções que tornem a experiência mais visual, intuitiva e alinhada ao comportamento digital, incentivando o interesse e reduzindo a distância entre o público jovem e o consumo cultural.
 
<div align="center">
  <sub>Imagem 03 - Persona do cliente</sub><br>
  <img src="../assets/personas-ana.png" width="100%" alt="Persona do cliente"><br>
  <sup>Fonte: Material produzido pelos autores, 2026.</sup>
</div>
---
 
### 5.1.2 Marcos Abrantes — Cliente Ativo (Decisão e Consumo)
 
Marcos Abrantes representa um perfil crescente de consumidores culturais urbanos: jovens adultos com rotina intensa, que buscam no lazer uma forma de descanso e inspiração, mas ainda têm uma relação pouco estruturada com o consumo de teatro. Seu contato com espetáculos acontece de forma reativa, mediado por redes sociais e indicações, o que torna sua jornada fragmentada. Apesar do interesse genuíno e do potencial de consumo, Marcos enfrenta dificuldades para descobrir novas produções com confiança, revelando uma oportunidade clara para soluções que organizem, personalizem e qualifiquem sua experiência de decisão.
 
<div align="center">
  <sub>Imagem 04 - Persona do cliente</sub><br>
  <img src="../assets/Persona-marcos.png" width="100%" alt="Persona do cliente"><br>
  <sup>Fonte: Material produzido pelos autores, 2026.</sup>
</div>
---
 
## 5.2 Jornada do Usuário
 
### 5.2.1 Jornada de Ana Luiza — Cliente em Formação
 
<div align="center">
  <sub>Imagem 05 - Jornada do usuário: Ana Luiza</sub><br>
  <img src="../assets/jornadaAna.png" width="100%" alt="Jornada do usuário Ana Luiza"><br>
  <sup>Fonte: Material produzido pelos autores, 2026.</sup>
</div>

| Etapa | Sentimento | Ações e Percepções |
|---|---|---|
| **Necessidade** | Infeliz | **Desorientação Cultural:** Ana quer sair com as amigas, mas sente que o teatro é um "mundo distante". Não sabe por onde começar a procurar e as opções que encontra parecem chatas ou "fora da sua bolha". **Barreira de Interesse:** Perde o interesse rapidamente se a interface não for dinâmica. Sente a frustração de não encontrar algo que pareça "instagramável" ou que faça sentido para a idade dela. |
| **Procura** | Neutro | **Esperança Investigativa:** Começa a procurar por coisas para fazer aos finais de semana na internet. **Descoberta Visual:** Encontra o Portal Aventura, e se surpreende com uma interface que fala a língua dela. O sentimento é de alívio por encontrar um lugar que destaca o que está "em alta" de forma visual e rápida, sem textos cansativos. |
| **Encontro** | Feliz | **Ação:** O algoritmo recomenda espetáculos que pessoas semelhantes a ela também curtiram. **Sentimento:** Ela descobre que as recomendações são precisas com seus gostos pessoais e faixa etária. Sente o entusiasmo de quem encontrou um "atalho" para ser a pessoa que sugere ótimos roles para o grupo. |
| **Acordo** | feliz | **Validação Social:** Ela compartilha o link no grupo das amigas e a resposta é imediata. **Sentimento:** Como a plataforma mostra fotos, do que é popular, ela sente segurança de que o investimento vale a pena e que ela não vai "pagar mico" com uma escolha errada. |
| **Adaptação** | Neutro | **A Barreira do Real:** A linha de felicidade cai levemente aqui. É o momento de convencer os pais, organizar o horário do grupo e lidar com a ansiedade de sair do digital para o presencial. **Sentimento:** Uma leve insegurança sobre se o evento ao vivo será tão legal quanto as fotos que ela viu na plataforma. O esforço de deslocamento gera um pequeno cansaço pré-evento. |
| **Resultados** | Feliz | **Sentimento:** A felicidade explode porque ela viveu algo memorável com as amigas. O Portal Aventura agora é o app de referência dela para qualquer fim de semana, eliminando o medo de "ficar de fora" (FOMO) das melhores experiências. |
 
---
 
### 5.2.2 Jornada de Marcos Abrantes — Cliente Ativo
 
<div align="center">
  <sub>Imagem 06 - Jornada do usuário: Marcos Abrantes</sub><br>
  <img src="../assets/jorandaMarcos.png" width="100%" alt="Jornada do usuário Marcos Abrantes"><br>
  <sup>Fonte: Material produzido pelos autores, 2026.</sup>
</div>

| Etapa | Sentimento | Ações e Percepções |
|---|---|---|
| **Necessidade** |  Infeliz | **Fadiga de Decisão e Desconexão:** Se sente fadigado por conta da rotina, além disso sente dificuldade em encontrar espetáculos parecidos com os que ele gosta sem pesquisar em vários locais. |
| **Procura** |  Neutro | **Ação:** Começa a buscar proativamente em portais e redes sociais, e descobre a plataforma que centraliza os espetáculos. **Revelação Estratégica:** Sente alívio momentâneo por encontrar um hub, mas ainda com ressalvas. Ele se anima com a promessa de não precisar mais pesquisar em lugares diferentes. |
| **Exploração** | Neutro | **Ação:** Explora a plataforma e começa a mostrar seus gostos. **Identificação e Validação:** Começa colocando os espetáculos que ele gostaria de ou assistiu para testar o sistema, mas sentiu dúvida se o sistema recomendaria seu gosto corretamente. |
| **Descoberta** | Feliz | **Ação:** Observa as recomendações feitas pelo algoritmo. **Fim da Incerteza, o Match Perfeito:** Descobre que as recomendações foram precisas com seus gostos pessoais, mostrando espetáculos que o interessaram. |
| **Adaptação** | Feliz | **Ação:** Ele assiste a um musical recomendado e sai do teatro. **Encantamento e Fidelização:** O espetáculo supera as expectativas. Ele viveu algo memorável e sente que o tempo livre foi bem aproveitado. Ele compartilha a experiência com amigos e se torna um cliente leal a longo prazo. |

---

## 6. Fluxo do Negócio

Nesta seção, apresentamos o fluxo de negócios da Plataforma Aventura modelado em BPMN (Business Process Model and Notation). O diagrama mapeia as interações entre três participantes principais — o Cliente, o Sistema de Recomendação e a Gestão de Conteúdo — demonstrando como esses processos se integram para oferecer uma experiência personalizada ao usuário. A notação BPMN foi escolhida por permitir uma leitura clara e padronizada dos fluxos, facilitando tanto a comunicação entre equipes técnicas quanto a compreensão por stakeholders de negócio.

<div align="center">
<sub>Imagem 07 - Fluxo de Negócio (BPMN)</sub><br>
  <img src="../assets/fluxo-negocios-bpmn.png" width="100%" alt="Fluxo de Negócio (BPMN)"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>

O fluxo de negócios é composto por duas pools distintas. Na pool do Cliente, o processo tem início com o acesso ao portal Aventura, seguido da recepção de recomendações. O gateway exclusivo "Se interessa?" determina o caminho seguinte: caso haja interesse, o cliente é direcionado à ticketera, encerrando o processo; caso contrário, o fluxo retorna à etapa de recepção de recomendações.

Na pool da Plataforma Aventura, dois fluxos paralelos operam de forma independente. O Sistema de Recomendação é disparado a cada 24 horas, executando a captura de interações, o tratamento dos dados e o encaminhamento ao gateway inclusivo. A Gestão de Conteúdo, por sua vez, é disparada semanalmente, realizando a coleta de dados dos espetáculos e a população do banco de dados, finalizando com o evento "Catálogo pronto", que também alimenta o gateway inclusivo. Esse gateway OR garante que a geração de recomendações ocorra assim que ao menos uma das entradas estiver disponível, enviando o resultado ao cliente via evento de mensagem "Recomendações geradas".

---

## 7. Brainstorming de Features

Para o projeto de plataforma digital baseada em arquitetura orientada a serviços, desenvolvido em parceria com a **Aventura Teatros**, o desafio central identificado é a ausência de um sistema estruturado de recomendação no Portal Aventura. Isso limita o potencial de descoberta de novos conteúdos e impacta diretamente a conversão em vendas de ingressos.

Esta documentação detalha o brainstorm de *features* realizado em grupo.


### 1. Visão Geral do Modelo (Backend & Inteligência)

O motor de recomendação será alimentado por dados demográficos e comportamentais para personalizar a experiência do usuário no portal.

* **Geolocalização:** Filtragem baseada na localização do usuário.
* **Perfil Demográfico:** Segmentação por idade e gênero.
* **Rastreio de Ações:** Monitoramento do comportamento de navegação dentro da plataforma.
* **Indicadores de Interesse:** Gatilhos de conversão, como o clique no link da ticketeira (venda de ingressos).
* **Integração com Wishlist:** O sistema deve reconhecer se o espetáculo já está na lista de desejos para refinar as sugestões.


### 2. Interface do Usuário (Front-end)

### 2.1 Fluxo de Cadastro e Onboarding
Essencial para coletar os dados iniciais que alimentarão o sistema de recomendação inteligente.
* **Credenciais:** E-mail e senha.
* **Dados Pessoais:** Nome, Idade, Gênero e CEP.
* **Questionário de Preferências:**
    * Frequência de ida ao teatro.
    * Tipos de preferência (Show musical, stand-up, espetáculo, espetáculo musical).
    * Seleção de gêneros favoritos.

### 2.2 Estrutura de Navegação
* **Home:** Visão geral e destaques.
* **Componentes de Descoberta:** Carrosséis e "Prateleiras" de recomendações personalizadas.
* **Filtros Regionais:** Seleção rápida entre São Paulo (SP) e Rio de Janeiro (RJ).
* **Retenção:** Seção "Assista Novamente" e acesso rápido à Wishlist.
* **Perfil:** Gestão de dados e preferências do usuário.
* **Teatro:** Páginas com informações fixas sobre os equipamentos geridos pela Aventura.


### 3. Funcionalidades Core

* **Direcionamento para Compra:** Link direto e facilitado para a aquisição de ingressos.
* **Busca:** Sistema de pesquisa interna de conteúdos e espetáculos.
* **Gestão de Wishlist:** Botão para salvar experiências de interesse futuro.


### 4. Roadmap: Funcionalidades Adicionais (Futuro)

* **Sistema de Feedback:** Avaliação de espetáculos por meio de "Like/Dislike" para aprimorar o algoritmo.
* **Notificações Push:** Alertas automáticos quando um espetáculo salvo na Wishlist entrar em cartaz ou tiver novas datas disponíveis.


---

## 8. Business Model Canvas do MVP

O Business Model Canvas apresentado a seguir sintetiza a estratégia do MVP da Plataforma Aventura, estruturando em nove blocos os elementos centrais do modelo de negócio. A partir das definições de proposta de valor, segmentos de clientes, canais, recursos e parcerias, o canvas oferece uma visão integrada de como a solução se posiciona no mercado e de que forma cada componente contribui para a geração de valor, tanto para o usuário final quanto para a Aventura Teatros como negócio.

<div align="center">
<sub>Imagem 08 - Business Model Canvas do MVP</sub><br>
  <img src="../assets/Model-Canvas-MVP.png" width="100%" alt="Business Model Canvas do MVP"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>

| Bloco | Descrição |
|---|---|
| Proposta de valor | Motor de recomendação personalizado que transforma o portal da Aventura de uma navegação estática em um hub dinâmico de descoberta cultural, conectando o usuário ao catálogo completo de produções (musicais, ballet, festivais) e reduzindo o atrito na jornada até a compra do ingresso. |
| Segmentos de clientes | Frequentadores de teatro e espetáculos musicais nas praças de São Paulo e Rio de Janeiro; público digital com perfil de early adopter cultural; apreciadores de produções internacionais; e novos públicos a serem convertidos por meio das recomendações personalizadas. |
| Canais | Aplicativo móvel híbrido (iOS e Android), plataforma web responsiva e notificações push. O redirecionamento para a compra de ingressos ocorre via parceiros de bilheteria (ticketeras). |
| Relacionamento com clientes | Experiência proativa e personalizada por meio de vitrines dinâmicas no app e na web, além de notificações push baseadas no comportamento de navegação do usuário. A plataforma ainda não existe, sendo este o modelo-alvo da solução. |
| Fontes de receita | Receita indireta via aumento da taxa de conversão de ingressos, com o fluxo financeiro passando pelas ticketeras parceiras. Há potencial futuro de destaque pago para espetáculos internacionais e valorização da marca Aventura como referência digital no setor cultural. |
| Recursos principais | Base de dados de espetáculos da Aventura, equipe interna de engenharia e dados, infraestrutura SOA desenvolvida pelo time e catálogo histórico de produções. |
| Atividades principais | Desenvolvimento e manutenção do motor de recomendação, curadoria e atualização do catálogo, monitoramento de comportamento do usuário, envio de notificações push personalizadas e integração contínua com as APIs da Aventura. |
| Parcerias principais | Parceiros de bilheteria (ticketeras), licenciadores internacionais (Broadway, Disney-Pixar), patrocinadores via Lei Rouanet e teatros e equipamentos culturais parceiros da Aventura. |
| Estrutura de custos | Desenvolvimento e manutenção da plataforma pela equipe interna, infraestrutura de servidores e nuvem, evolução contínua do motor de recomendação e integração com APIs externas. Por ser desenvolvida internamente, a plataforma não exige investimento direto em licenças de software de terceiros. |

---

## 9. Requisitos Funcionais

A fim de garantir a excelência e o rigor técnico no desenvolvimento do ecossistema digital, o processo de levantamento e documentação seguiu as diretrizes da Área de Conhecimento de Requisitos de Software (KA 1 - Software Requirements) do guia internacional SWEBOK V3 (IEEE). Essa adoção assegura que o projeto não apenas atenda aos objetivos de negócio do parceiro, mas que seja estruturado de forma rastreável, modular e pronta para um ciclo de desenvolvimento de software de qualidade.

### 9.1.  Elicitação e Análise de Requisitos
A fase de elicitação buscou compreender profundamente a jornada do consumidor cultural. O foco principal foi alinhar o vasto catálogo de produções da Aventura Teatros às expectativas de um público diversificado e exigente. Utilizamos a técnica ágil de User Stories (Histórias de Usuário) para mapear o comportamento do espectador, desde a descoberta de seu perfil no primeiro contato (Onboarding) até o redirecionamento seguro para a compra do ingresso (Hand-off).

Durante a etapa de análise, delimitamos estrategicamente o escopo do sistema para focar 100% na experiência do usuário final. Concluiu-se que o maior valor do produto reside na capacidade de oferecer uma curadoria ativa e personalizada. Dessa forma, todos os esforços de definição de requisitos foram direcionados para desenhar uma plataforma de alta usabilidade, onde o sistema atue de forma inteligente e orgânica na sugestão de espetáculos, maximizando o engajamento e a conversão de vendas.

### 9.2. Especificação e Validação de Requisitos Funcionais
Conforme as premissas do SWEBOK, a especificação tem o papel de traduzir as necessidades do negócio em comportamentos claros e executáveis para o sistema, enquanto a validação garante que o produto construído seja plenamente testável.

Para a especificação, adotamos o padrão estruturado de User Stories ("Como perfil, eu quero ação, para motivação"), garantindo que o desenvolvimento seja continuamente guiado pelo valor entregue ao usuário. Paralelamente, para a etapa de validação, foram definidos Casos de Teste de Usuário para cada funcionalidade descrita. Esses testes estabelecem critérios de aceitação rigorosos, contemplando pré-condições, procedimentos de execução e os resultados sistêmicos e visuais esperados, assegurando que todas as ações e comportamentos do software possam ser homologados com precisão técnica antes de serem disponibilizados ao público. Os requisitos detalhados sob esta ótica são apresentados a seguir.

### RF01 — Onboarding de Interesses

**User story:** Como novo usuário, eu quero responder a um questionário de interesses (onboarding), para que o sistema recomende o catálogo de acordo com minhas preferências (gênero, frequência, localização).

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O usuário acabou de criar a conta e está no seu primeiro acesso (sem preferências salvas). |
| Procedimento de teste | 1. Acessar a tela de Onboarding. <br> 2. Selecionar o gênero "Musical" e a cidade "São Paulo". <br>3. Clicar no botão "Salvar e Continuar". |
| Resultado esperado | O sistema redireciona o usuário para a Home, exibindo primariamente espetáculos da categoria "Musical" localizados em "SP". |
| Pós-condição |As preferências de gênero e localização são gravadas no perfil do Usuário no banco de dados.|

---

### RF02 — Recomendação por Geolocalização

**User story:** Como usuário, eu quero receber sugestões baseadas na minha localização (Geolocalização), para priorizar eventos que acontecem na minha cidade (RJ ou SP).

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | O usuário está logado e possui a localização definida (ex: São Paulo).|
| Procedimento de teste |1. Acessar a página inicial (Home).<br>2. Rolar a página até a seção "Em Cartaz Perto de Você". <br> 3. Verificar os locais dos espetáculos exibidos. |
| Resultado esperado |A prateleira exibe exclusivamente espetáculos vinculados a teatros localizados na cidade de São Paulo. |
| Pós-condição |O sistema registra a visualização para refinar o algoritmo de engajamento local. |

---

### RF03 — Autenticação e Persistência

**User story:** Como usuário, eu quero me cadastrar e realizar login (E-mail/Senha), para que minhas preferências e minha Wishlist sejam salvas entre diferentes sessões e dispositivos.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O usuário possui um cadastro válido, tem itens salvos na Wishlist, mas está deslogado do sistema. |
| Procedimento de teste |1. Acessar a tela de Login.<br>2. Inserir E-mail e Senha e confirmar.<br> 3. Navegar até a aba "Minha Wishlist". |
| Resultado esperado |O sistema autentica o usuário com sucesso e carrega todos os espetáculos que haviam sido salvos anteriormente. |
| Pós-condição |Uma sessão ativa (token) é gerada e armazenada de forma segura para manter o acesso durante a navegação. |

---

### RF04 — Catálogo Categorizado

**User story:** Como usuário, eu quero visualizar o catálogo completo de espetáculos organizado por categorias, para que eu tenha uma visão geral da multiplicidade artística da Aventura de forma estruturada.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O sistema possui espetáculos ativos cadastrados e vinculados a diferentes categorias de Gênero. |
| Procedimento de teste | 1. Acessar a página principal de Catálogo.<br>2. Clicar no filtro ou aba da categoria "Comédia".|
| Resultado esperado |A interface é atualizada, exibindo apenas os cards de espetáculos que contêm a tag do gênero "Comédia". |
| Pós-condição | O sistema processa a filtragem sem realizar alterações nos dados salvos do usuário.|

---
### RF05 — Busca Específica

**User story:** Como usuário, eu quero pesquisar por termos específicos (teatro, título), para encontrar rapidamente informações sobre uma produção específica.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | O espetáculo "Mamma Mia" e o teatro "Riachuelo" estão devidamente cadastrados e ativos no banco de dados.|
| Procedimento de teste | 1. Clicar no ícone de pesquisa (lupa).<br>2. Digitar o termo "Riachuelo" e pressionar Enter.|
| Resultado esperado | A página de resultados exibe uma lista contendo o espetáculo "Mamma Mia" e outras peças em cartaz no referido teatro. |
| Pós-condição |O termo pesquisado é registrado temporariamente em log para métricas de interesse da plataforma. |

---
### RF06 — Gestão de Wishlist

**User story:** Como espectador, eu quero adicionar ou remover espetáculos da minha Wishlist, para sinalizar ao algoritmo meus interesses reais e salvar peças para consulta futura.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O usuário está logado e acessa a página de um espetáculo que não está em sua Wishlist. |
| Procedimento de teste |1. Clicar no botão "Adicionar à Wishlist".<br>2. Acessar a página "Minha Wishlist" para validar a inclusão.<br>3. Voltar e clicar no mesmo botão para remover. |
| Resultado esperado | O botão muda de estado visual. O espetáculo aparece na página Wishlist (passo 2) e desaparece ao ser removido (passo 3).|
| Pós-condição |O banco de dados insere (Create) ou deleta (Delete) o relacionamento entre as entidades Usuário e Espetáculo. |

---

### RF07 — Informações de Teatros

**User story:** Como espectador, eu quero encontrar informações de localização e horário de funcionamento sobre os teatros Aventura, para me preparar para assistir a um espetáculo.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O sistema possui o cadastro do teatro com metadados preenchidos (endereço, horários) vinculado ao espetáculo selecionado. |
| Procedimento de teste |1. Acessar a página de detalhes de um espetáculo qualquer.<br> 2. Rolar a página até a seção de informações do local. |
| Resultado esperado |O nome do teatro, o endereço físico completo e os dias/horários de funcionamento da bilheteria são exibidos corretamente. |
| Pós-condição |Nenhuma alteração no estado do sistema; os dados são consumidos em modo de apenas leitura. |

---

### RF08 — Gestão de Perfil e Preferências

**User story:** Como usuário, quero gerenciar e editar meus dados de perfil e preferências a qualquer momento, para garantir transparência e privacidade.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O usuário está logado no sistema e acessa a área de configurações do próprio perfil. |
| Procedimento de teste |1. Acessar "Meu Perfil" > "Gêneros Favoritos".<br> 2. Desmarcar o gênero "Comédia" e selecionar "Drama".<br> 3. Clicar em "Salvar Alterações". |
| Resultado esperado |O sistema exibe um alerta de sucesso e o motor de recomendação ajusta as próximas sugestões na Home para priorizar "Drama". |
| Pós-condição |O vetor de interesses atrelado ao Usuário é atualizado (Update) permanentemente no banco de dados.|

---

### RF09 — Redirecionamento de Compra

**User story:** Como usuário, quero ser direcionado para a bilheteria oficial via link externo, para realizar a compra do ingresso com segurança.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O espetáculo selecionado possui uma URL externa de bilheteria válida cadastrada no sistema (ex: link do Sympla). |
| Procedimento de teste | 1. Acessar a página de detalhes do espetáculo em cartaz.<br> 2. Clicar no botão principal de call-to-action "Comprar Ingressos".|
| Resultado esperado |O navegador abre uma nova janela ou aba carregando a página oficial de vendas do espetáculo na ticketeira externa. |
| Pós-condição |A interação de clique (hand-off) é registrada no sistema para geração de métricas de conversão. |

---

### RF10 — Recomendação Inteligente

**User story:** Como usuário, quero receber sugestões personalizadas com base no meu comportamento e preferências, para descobrir novos espetáculos relevantes de forma mais rápida e intuitiva.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição |O usuário possui preferências registradas no sistema (via Onboarding ou interações prévias). |
| Procedimento de teste | 1. Acessar a página inicial (Home).<br> 2. Localizar a seção de recomendações personalizadas.<br>3. Comparar o gênero e temática dos espetáculos sugeridos com o perfil do usuário.|
| Resultado esperado |As sugestões exibidas são condizentes com os interesses do usuário, apresentando espetáculos que correspondam ao seu perfil de consumo cultural. |
| Pós-condição |O sistema mantém a personalização da interface ativa para a conta do usuário. |

---



## 10. Requisitos Não Funcionais


### RNF01 — Tempo de resposta das recomendações

**Característica ISO 25010:** Eficiência de Desempenho → Time Behaviour

**User story:** Como usuário, quero que as recomendações personalizadas e os carrosséis da home carreguem em até 3 segundos, para que eu possa explorar o catálogo sem frustração e manter o interesse na navegação.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuário autenticado com perfil de onboarding preenchido e wishlist contendo ao menos 1 item e conexão estável de no mínimo 10 Mbps. |
| Procedimento de teste | Acessar a Home do Portal e medir o tempo entre o carregamento da página e a renderização completa das prateleiras de recomendação. Repetir 20 vezes em horários distintos para garantir base estatística confiável. |
| Resultado esperado | Tempo de carregamento das recomendações com p95 ≤ 3 segundos e média ≤ 2 segundos. |
| Pós-condição | Carrosséis renderizados com itens personalizados e logs de performance armazenados para análise. |

---

### RNF02 — Segurança das credenciais de usuário

**Característica ISO 25010:** Segurança → Confidentiality e Integrity

**User story:** Como usuário cadastrado, quero que minha senha e dados pessoais (nome, idade, gênero, CEP) sejam armazenados e transmitidos de forma criptografada, para que minhas informações não sejam expostas em caso de tentativa de invasão ou interceptação.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuário cadastrado com e-mail e senha válidos e banco de dados de teste populado. |
| Procedimento de teste | Inspecionar o tráfego de rede no momento do login com ferramenta como DevTools, verificando uso de HTTPS/TLS 1.2+. Consultar diretamente o banco de dados e verificar se a senha está armazenada com hash (ex.: bcrypt, Argon2) e não em texto plano. Verificar ausência de dados sensíveis em logs de aplicação. |
| Resultado esperado | 100% do tráfego ocorre via HTTPS/TLS 1.2+; nenhuma senha armazenada em texto plano; hash aplicado é não reversível; nenhum dado sensível visível em logs ou payload. |
| Pós-condição | Sessão autenticada com token seguro e nenhum dado sensível visível em logs ou no payload em texto plano. |

---

### RNF03 — Conformidade com a LGPD

**Característica ISO 25010:** Segurança → Privacy

**User story:** Como usuário, quero poder consultar, editar e excluir meus dados pessoais e preferências a qualquer momento, para que a plataforma respeite meus direitos garantidos pela LGPD e eu mantenha controle sobre minhas informações.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuário autenticado com dados de perfil preenchidos no onboarding (idade, gênero, CEP, preferências). |
| Procedimento de teste | Acessar a área de Perfil → tentar editar cada campo de dado pessoal e preferências. Em seguida, solicitar exclusão da conta e verificar se os dados são efetivamente removidos ou anonimizados na base em até 30 dias. Verificar existência de registro de consentimento (opt-in) para cada finalidade de tratamento de dados. |
| Resultado esperado | O usuário consegue visualizar, editar e excluir seus dados; após a exclusão, os registros pessoais não estão mais associados ao identificador do usuário na base. |
| Pós-condição | Processo de exclusão/edição concluído com sucesso e logs de auditoria gerados. |

---

### RNF04 — Usabilidade da interface em dispositivos móveis

**Característica ISO 25010:** Usabilidade → Learnability e Operability

**User story:** Como espectador que acessa o portal pelo celular, quero que toda a navegação (home, busca, wishlist, perfil e fluxo de onboarding) seja totalmente responsiva e intuitiva em telas a partir de 480px, para que eu possa descobrir espetáculos e gerenciar minhas preferências em qualquer dispositivo.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Aplicação publicada em ambiente de homologação; dispositivos disponíveis com larguras de 480px e 720px. |
| Procedimento de teste | Realizar teste de usabilidade com 5 usuários, solicitando: completar o onboarding, buscar um espetáculo, adicionar à wishlist, acessar perfil e seguir para a bilheteria. Medir taxa de conclusão e SUS (System Usability Scale). |
| Resultado esperado | Taxa de conclusão ≥ 90% das tarefas sem ajuda externa; pontuação SUS ≥ 70; nenhum elemento cortado ou inacessível nas resoluções testadas. |
| Pós-condição | Relatório de usabilidade documentado; eventuais ajustes de UI registrados como backlog. |

---

### RNF05 — Disponibilidade da plataforma

**Característica ISO 25010:** Confiabilidade → Availability

**User story:** Como usuário do Portal Aventura, quero que a plataforma esteja disponível 99% do tempo no horário comercial e em horários de pico, para que eu possa consultar a programação e ser direcionado à bilheteria sem indisponibilidades.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Plataforma publicada em ambiente de produção com monitoramento ativo. |
| Procedimento de teste | Monitorar a disponibilidade do portal por 24 horas contínuas, registrando todas as interrupções e suas durações. |
| Resultado esperado | Disponibilidade ≥ 99% na janela monitorada (downtime tolerado ≤ 14,4 minutos nas 24h); alertas disparados corretamente em caso de queda. |
| Pós-condição | Relatório de uptime gerado e incidentes registrados com causa raiz. |

---

### RNF06 — Escalabilidade do sistema de recomendação

**Característica ISO 25010:** Eficiência de Desempenho → Capacity

**User story:** Como gestor da Aventura, quero que a plataforma suporte ao menos 100 usuários simultâneos sem degradação perceptível das recomendações, para que campanhas de lançamento de espetáculos não comprometam a experiência dos espectadores.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Ambiente de homologação configurado com infraestrutura equivalente à produção. |
| Procedimento de teste | Executar teste de carga simulando 100 usuários concorrentes navegando pela home, acessando carrosséis de recomendação e adicionando itens à wishlist por 15 minutos contínuos (ex.: com k6 ou JMeter). |
| Resultado esperado | Tempo médio de resposta ≤ 3s com p95 ≤ 5s; taxa de erro < 1%; nenhuma queda do serviço durante o teste; consumo de CPU < 80% e memória < 85% nos servidores monitorados. |
| Pós-condição | Métricas de CPU, memória e latência registradas e eventuais gargalos documentados. |

---

### RNF07 — Precisão da geolocalização

**Característica ISO 25010:** Adequação Funcional → Functional Correctness

**User story:** Como usuário localizado no Rio de Janeiro ou em São Paulo, quero que o sistema identifique corretamente minha cidade a partir do CEP ou da geolocalização do dispositivo, para que as recomendações priorizem espetáculos da minha região com precisão.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuários de teste cadastrados com CEPs reais de SP e RJ; permissão de geolocalização concedida em parte dos casos. |
| Procedimento de teste | Acessar a home com 10 perfis distintos (5 SP e 5 RJ), verificar se as recomendações da seção regional priorizam espetáculos da cidade correta. Repetir o teste com geolocalização do navegador ativada. |
| Resultado esperado | A cidade correta é identificada em ≥ 95% dos casos (≥ 19 de 20 perfis); espetáculos da região correspondente aparecem nas 3 primeiras posições da seção regional. |
| Pós-condição | Logs de geolocalização registrados (sem armazenar coordenadas precisas, apenas cidade), respeitando a LGPD. |



## 11. Correlação entre RFs e RNFs

> Tabela explicitando as relações diretas entre requisitos não funcionais e requisitos funcionais.

| RNF | RF relacionados | Justificativa |
|---|---|---|
| RNF01 | RF01, RF02, RF04, RF06 | O tempo de resposta impacta diretamente as funcionalidades de descoberta e personalização: o questionário de onboarding (RF01), as sugestões por geolocalização (RF02), a navegação pelo catálogo de espetáculos (RF04) e a interação com a Wishlist (RF06) dependem de carregamento ágil para que o usuário não abandone o fluxo antes de receber as recomendações. |
| RNF02 | RF03, RF08 | A proteção criptográfica de credenciais e dados pessoais sustenta diretamente o cadastro/login (RF03), em que senhas trafegam e são armazenadas, e a gestão de perfil e preferências (RF08), que manipula dados sensíveis como idade, gênero e CEP coletados no onboarding. |
| RNF03 | RF03, RF08 | A conformidade com a LGPD é o que dá validade legal ao cadastro do usuário (RF03) e operacionaliza o direito de visualizar, editar e excluir dados pessoais e preferências previsto na gestão de perfil (RF08), garantindo transparência e controle sobre as informações coletadas. |
| RNF04 | RF01, RF04, RF09 | A responsividade mobile é crítica nos pontos de maior fricção da jornada: o onboarding (RF01), em que o usuário responde ao questionário inicial pelo celular; a exploração do catálogo (RF04); e o redirecionamento para a bilheteria externa (RF09), em que qualquer falha de layout pode interromper a conversão em venda. |
| RNF05 | RF02, RF04, RF07, RF09 | A disponibilidade da plataforma sustenta toda a jornada de descoberta e conversão: recomendações regionais (RF02), exploração do catálogo (RF04), consulta de informações dos teatros (RF07) e direcionamento à bilheteria (RF09) precisam estar ativos especialmente em horários de pico, como vésperas de estreia. |
| RNF06 | RF01, RF02, RF04, RF06 | A escalabilidade garante que o motor de recomendação mantenha o desempenho mesmo com muitos usuários simultâneos realizando onboarding (RF01), recebendo sugestões por geolocalização (RF02), navegando pelo catálogo (RF04) e atualizando a Wishlist (RF06) — funcionalidades que dependem de processamento personalizado por usuário. |
| RNF07 | RF02, RF04 | A precisão da geolocalização é pré-requisito direto da filtragem regional (RF02), que prioriza eventos da cidade do usuário (RJ ou SP), e influencia a organização do catálogo (RF04), em que prateleiras regionais aparecem como destaque na home. |

---

## 12. Diagramação do banco

### 12.1 - Diagrama Lógico

<div align="center">
<sub>Imagem 09 - Diagrama Lógico</sub><br>
  <img src="../assets/diagrama_logico.png" width="100%" alt="Diagrama lógico do banco"><br>
  <sup>Source: Material produzido pelos autores, 2026.</sup>
</div>

O Diagrama Lógico representa as entidades principais do sistema, os relacionamentos entre elas e as cardinalidades, utilizando notação Entidade-Relacionamento (ER) com **retângulos** para as entidades e **losangos** para os relacionamentos.

---

### 12.2 - Entidades e Relacionamentos

#### **Entidades (Retângulos)**

- **Onboarding**: Representa o processo inicial de cadastro e definição de preferências do usuário.
- **Usuário**: Entidade principal que representa os usuários da plataforma.
- **Gênero**: Categorias artísticas dos espetáculos (Drama, Musical, Comédia, etc.).
- **Espetáculo**: Peças teatrais, musicais e eventos culturais disponíveis na aplicação.
- **Teatro**: Locais físicos onde os espetáculos são realizados.
- **Wishlist**: Lista de espetáculos salvos pelo usuário.

#### **Relacionamentos (Losangos) e Cardinalidades**

| Relacionamento      | Entidades Envolvidas                  | Cardinalidade | Descrição |
|---------------------|---------------------------------------|---------------|---------|
| **Realiza**         | Usuário ↔ Onboarding                 | **1:1**       | Cada usuário realiza exatamente um onboarding. |
| **Seleciona**       | Onboarding ↔ Gênero                  | **1:N**       | O usuário seleciona um ou mais gêneros durante o onboarding. |
| **Prefere**         | Usuário ↔ Gênero                     | **N:N**       | Usuário pode preferir vários gêneros e um gênero pode ser preferido por vários usuários. |
| **Adiciona**        | Usuário ↔ Wishlist                   | **1:1**       | Cada usuário possui apenas uma wishlist. |
| **Possui**          | Wishlist ↔ Espetáculo                | **1:N**       | Uma wishlist pode conter vários espetáculos. |
| **Possui**          | Espetáculo ↔ Gênero                  | **N:N**       | Um espetáculo pode pertencer a múltiplos gêneros. |
| **Recomenda**       | Espetáculo ↔ Usuário                 | **N:N**       | O sistema recomenda espetáculos aos usuários com base em suas preferências. |
| **Apresentado em**  | Espetáculo ↔ Teatro                  | **N:1**       | Muitos espetáculos podem ser apresentados em um mesmo teatro. |

---

### 12.3 - Justificativa e Regras de Negócio

O Diagrama Lógico foi construído para atender diretamente às necessidades da aplicação:

- O usuário realiza um **onboarding** para informar suas preferências de gêneros e recebe recomendações personalizadas;
- O sistema utiliza as preferências para gerar **recomendações** dinâmicas;
- O usuário tem permissão apenas de **leitura** para Espetáculos, Teatros e Gêneros;
- A **Wishlist** é a única funcionalidade que permite ao usuário **adicionar e remover** itens;
- Um espetáculo pode ser classificado em **múltiplos gêneros**, enriquecendo os filtros e recomendações;
- Suporte a filtros por gênero e localidade (via Teatro).

Este modelo garante **clareza visual**, boa escalabilidade para consultas de recomendação e forte alinhamento com as regras de negócio da aplicação.

### 12.4 Diagrama Conceitual

<div align="center">
<sub>Imagem 09 - Diagrama conceitual</sub><br>
  <img src="../assets/diagrama-logico.png" width="100%" alt="Diagrama conceitual do banco"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>

O modelo conceitual de dados representa a estruturação dos dados seguindo as regras do modelo relacional, servindo como uma ponte entre a abstração do modelo conceitual e a implementação física no SGDB.

---

### 12.4.1 Dicionário de Dados e Mapeamento

A conversão do modelo lógico para o modelo conceitual foi realizada mapeando cada entidade conceitual para uma tabela relacional e transformando relacionamentos Muitos-para-Muitos (N:N) em tabelas associativas.

---

### A. Tabelas de Cadastro (Entidades Fortes)

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **USUARIOS** | `Id` | INT | PK, AI | Identificador único do usuário. |
| | `Nome` | VARCHAR(100) | NN | Nome completo do usuário. |
| | `Email` | VARCHAR(100) | UNIQUE, NN | E-mail utilizado para autenticação. |
| | `Senha` | VARCHAR(255) | NN | Hash da senha de acesso. |
| | `Idade` | INT | - | Idade do usuário. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **GENEROS** | `id` | INT | PK, AI | Identificador único do gênero. |
| | `Nome` | VARCHAR(50) | NN | Nome do gênero artístico. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **TEATROS** | `id` | INT | PK, AI | Identificador único do teatro. |
| | `Nome` | VARCHAR(100) | NN | Nome do teatro ou espaço cultural. |
| | `Cidade` | VARCHAR(100) | NN | Cidade onde o teatro está localizado. |
| | `Estado` | VARCHAR(50) | NN | Estado onde o teatro está localizado. |
| | `Rua` | VARCHAR(150) | NN | Nome da rua do teatro. |
| | `Numero` | VARCHAR(20) | NN | Número do endereço do teatro. |
| | `Horario_func` | VARCHAR(100) | - | Horário de funcionamento do teatro. |
| | `Descricao` | TEXT | - | Descrição geral do teatro. |
| | `Imagem` | VARCHAR(255) | - | URL ou caminho da imagem do teatro. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **ESPETACULOS** | `id` | INT | PK, AI | Identificador único do espetáculo. |
| | `Nome` | VARCHAR(150) | NN | Nome do espetáculo. |
| | `link_ingresso` | VARCHAR(255) | - | Link para compra de ingressos. |
| | `Descricao` | TEXT | - | Descrição detalhada do espetáculo. |
| | `Imagem` | VARCHAR(255) | - | URL ou caminho da imagem do espetáculo. |

---

### B. Tabelas Associativas e Operacionais

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **ESP_GEN** | `id_esp` | INT | FK, PK(1/2) | Espetáculo relacionado ao gênero. |
| | `id_gen` | INT | FK, PK(2/2) | Gênero associado ao espetáculo. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **GEN_FAV** | `id_usu` | INT | FK, PK(1/2) | Usuário que favoritou o gênero. |
| | `id_gen` | INT | FK, PK(2/2) | Gênero marcado como preferência pelo usuário. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **TEAT_ESPET** | `id_teat` | INT | FK, PK(1/2) | Teatro onde o espetáculo ocorre. |
| | `id_espet` | INT | FK, PK(2/2) | Espetáculo vinculado ao teatro. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **CURT_WISH** | `id_usu` | INT | FK, PK(1/2) | Usuário que realizou a interação. |
| | `id_esp` | INT | FK, PK(2/2) | Espetáculo alvo da interação. |
| | `tipo` | ENUM('curtida', 'wishlist') | NN | Tipo de interação realizada pelo usuário. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **INTERAÇÕES_USU** | `id` | INT | PK, AI | Identificador único do evento registrado. |
| | `id_usu` | INT | FK, NN | Usuário que realizou a interação. |
| | `id_esp` | INT | FK, NN | Espetáculo relacionado à interação. |
| | `tipo` | VARCHAR(50) | NN | Tipo de evento/interação realizada. |
| | `temp_pagi` | INT | - | Tempo de permanência na página (segundos). |

---

### 12.4.2 Definição de Chaves e Integridade Referencial

### Chaves Primárias (PK)
As chaves primárias foram definidas para garantir a unicidade de cada registro. Nas tabelas associativas (`ESP_GEN`, `GEN_FAV`, `TEAT_ESPET`, `CURT_WISH`), utilizam-se chaves compostas pelos IDs das entidades relacionadas para evitar duplicidade de associações.

### Chaves Estrangeiras (FK)
As chaves estrangeiras garantem a integridade referencial, assegurando que as associações entre usuários, espetáculos, teatros e gêneros apontem apenas para registros existentes.

### Relacionamentos implementados
- **Usuários e Interações**: Um usuário pode gerar múltiplos registros de interação e eventos de navegação (`1:N`).
- **Gêneros Favoritos**: Relação `N:N` que permite ao usuário filtrar sua experiência com base em múltiplos interesses.
- **Espetáculos e Teatros**: Relação `N:N` via `TEAT_ESPET`, permitindo que uma produção ocorra em diferentes locais e que um teatro receba diversos eventos.
- **Categorização Multifacetada**: Através da `ESP_GEN`, um espetáculo pode ser associado a diversos gêneros simultaneamente.

---

### 12.4.3 Regras de Normalização Aplicadas

O modelo conceitual segue os princípios da Terceira Forma Normal (3FN).

### Primeira Forma Normal (1FN)
Todos os atributos possuem valores atômicos. Colunas como `tipo` em `CURT_WISH` utilizam ENUM para padronização, evitando dados multivalorados.

### Segunda Forma Normal (2FN)
Não existem dependências parciais. Em tabelas com chaves compostas, os atributos dependem da junção completa das chaves estrangeiras.

### Terceira Forma Normal (3FN)
Não há dependências transitivas. Atributos descritivos (como os detalhes de endereço em `TEATROS`) dependem exclusivamente da PK daquela tabela.

---

### 12.4.4 Justificativa e Rastreabilidade

O modelo conceitual foi estruturado para manter fidelidade ao modelo lógico, garantindo consistência entre as regras de negócio e a implementação relacional.

As tabelas associativas permitem representar corretamente relacionamentos Muitos-para-Muitos, como:

- preferências de gêneros dos usuários;
- categorização de espetáculos;
- interações entre usuários e espetáculos.

A separação das entidades reduz redundâncias, melhora a escalabilidade do sistema e facilita futuras manutenções e expansões do banco de dados.

## 13. Diagrama de Classes

O diagrama de classes representa o modelo de domínio de um sistema de gerenciamento de teatro, espetáculos e recomendações personalizadas. O sistema registra interações, além de permitir que usuários descubram espetáculos, gerenciem listas de desejos e recebam recomendações baseadas em seu comportamento e preferências.

<div align="center">
<sub>Imagem 09 - Diagrama de Classes UML</sub><br>
  <img src="../assets/diagrama-classes-mod6.drawio.png" width="100%" alt="Diagrama de Classes UML"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>

### Teatro
 
Representa um teatro físico cadastrado no sistema. É responsável por gerenciar os espetáculos que estão em cartaz em sua estrutura.
 
| Atributo / Método | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador único do teatro |
| Nome | - (private) | string | Nome do teatro |
| Endereco | - (private) | string | Endereço completo do teatro |
| Cidade | - (private) | string | Cidade onde o teatro está localizado |
| HorarioFuncionamento | - (private) | string | Horários de funcionamento do teatro |
| **Métodos** | | | |
| AdicionarEspetaculo() | + (public) | : void | Adiciona um espetáculo ao teatro |
| RemoverEspetaculo(id) | + (public) | : void | Remove um espetáculo pelo seu id |
| BuscarEspetaculos() | + (public) | : List | Retorna a lista de espetáculos do teatro |
 
---
 
### Espetaculo
 
Representa um espetáculo teatral. Contém todas as informações necessárias para exibição, compra de ingressos e categorização por gênero e tipo de evento.
 
| Atributo / Método | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador único do espetáculo |
| Nome | - (private) | string | Nome do espetáculo |
| Genero | - (private) | string | Gênero artístico do espetáculo |
| TipoEvento | - (private) | string | Tipo de evento (ex: peça, musical, ópera) |
| Descricao | - (private) | string | Descrição detalhada do espetáculo |
| UrlBilheteria | - (private) | string | URL para compra de ingressos |
| DataInicio | - (private) | DateTime | Data de início das apresentações |
| DataFim | - (private) | DateTime | Data de encerramento das apresentações |
| ValorIngresso | - (private) | decimal | Valor do ingresso |
| **Métodos** | | | |
| EstaEmCartaz() | + (public) | : bool | Verifica se o espetáculo está em cartaz na data atual |
| ObterUrlCompra() | + (public) | : string | Retorna a URL de compra de ingresso |
| PertenceAoGenero(genero) | + (public) | : bool | Verifica se o espetáculo pertence a um determinado gênero |
 
---
 
### Usuario
 
Representa um usuário cadastrado na plataforma. Agrega as preferências e a wishlist do usuário, sendo o ponto central das interações com o sistema.
 
| Atributo / Método | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador único do usuário |
| Nome | - (private) | string | Nome completo do usuário |
| Email | - (private) | string | Endereço de e-mail do usuário |
| SenhaHash | - (private) | string | Hash da senha do usuário |
| DataCadastro | - (private) | DateTime | Data de cadastro na plataforma |
| **Métodos** | | | |
| AtualizarPreferencias(preferencias) | + (public) | : void | Atualiza as preferências do usuário |
| SalvarEspetaculo() | + (public) | : void | Salva um espetáculo na wishlist |
| RemoverDaWishlist(itemId) | + (public) | : void | Remove um espetáculo da wishlist |
| AlterarSenha(hash) | + (public) | : void | Atualiza o hash da senha |
| ExcluirConta() | + (public) | : void | Exclui a conta do usuário |
 
---
 
### Wishlist
 
Lista de espetáculos salvos pelo usuário. Possui composição com `Usuario` — não existe independentemente de um usuário.
 
| Atributo / Método | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador único da wishlist |
| UsuarioId | - (private) | Guid | Referência ao usuário dono da wishlist |
| DataAtualizacao | - (private) | DateTime | Data da última atualização da lista |
| **Métodos** | | | |
| Adicionar(itemId : UUID) | + (public) | : void | Adiciona um espetáculo à lista |
| Remover(itemId : UUID) | + (public) | : void | Remove um espetáculo da lista |
| Contem(itemId : UUID) | + (public) | : bool | Verifica se um espetáculo está na lista |
| Limpar() | + (public) | : void | Remove todos os itens da lista |
 
---
 
### Preferencias `«Value Object»`
 
Objeto de valor que representa as preferências culturais do usuário. Por ser um Value Object, não possui identidade própria — sua igualdade é definida pelo conjunto de seus atributos.
 
| Atributo | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Generos | - (private) | List\<string\> | Lista de gêneros de interesse do usuário |
| Cidade | - (private) | string | Cidade de preferência para eventos |
| TiposEvento | - (private) | List\<string\> | Tipos de evento de interesse do usuário |
| DataAtualizacao | - (private) | DateTime | Data da última atualização das preferências |
 
---
 
### Interacao
 
Registra uma interação do usuário com um espetáculo (clique, acesso ou tempo de tela). Utiliza a enumeração `TipoInteracao` para categorizar o tipo de interação registrada.
 
| Atributo / Método | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador único da interação |
| Tipo | - (private) | TipoInteracao | Tipo da interação (Clique, Acesso, TempoDeTela) |
| Pontuacao | - (private) | Float | Pontuação atribuída à interação para o sistema de recomendação |
| DataRegistro | - (private) | DateTime | Data e hora em que a interação foi registrada |
| DataExpiracao | - (private) | DateTime | Data e hora em que a interação expira |
| **Métodos** | | | |
| EstaExpirada() | + (public) | : bool | Verifica se a interação já está expirada |
| Renovar() | + (public) | : void | Renova a validade da interação |
 
---
 
### Recomendacao
 
Representa uma recomendação gerada pelo `SistemaRecomendacao` para um usuário específico sobre um espetáculo. Armazena a pontuação calculada e o motivo da recomendação.
 
| Atributo | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador único da recomendação |
| Score | - (private) | decimal | Pontuação de relevância da recomendação |
| DataGeracao | - (private) | DateTime | Data em que a recomendação foi gerada |
| Motivo | - (private) | string | Justificativa textual da recomendação |
 
---
 
### SistemaRecomendacao
 
Componente responsável por gerar e gerenciar recomendações personalizadas. Depende da classe `Interacao` para calcular scores e produzir sugestões relevantes para cada usuário.
 
| Atributo / Método | Visibilidade | Tipo | Descrição |
|---|---|---|---|
| Id | - (private) | UUID | Identificador do sistema |
| Interacoes | - (private) | List\<Interacao\> | Lista de interações usadas como base para recomendações |
| UltimaExecucao | - (private) | DateTime | Data da última execução do algoritmo |
| Versao | - (private) | string | Versão do algoritmo de recomendação |
| **Métodos** | | | |
| GerarRecomendacoes(userId) | + (public) | : void | Gera recomendações personalizadas para o usuário informado |
| RegistrarInteracao(i : Interacao) | + (public) | : void | Registra uma nova interação no sistema |
| LimparInteracoesExpiradas() | + (public) | : void | Remove interações cuja validade expirou |
| ObterRanking(userId, itemId) | + (public) | : void | Retorna o ranking de relevância de um item para o usuário |
| FiltrarPorCidade(cidade) | + (public) | : void | Filtra recomendações pela cidade do usuário |
 
---
 
### TipoInteracao `«enumeration»`
 
Enumeração que define os tipos possíveis de interação de um usuário com um espetáculo. Utilizada pela classe `Interacao`.
 
| Valor | Descrição |
|---|---|
| Clique | Usuário clicou no espetáculo |
| Acesso | Usuário acessou a página do espetáculo |
| TempoDeTela | Tempo que o usuário permaneceu na página do espetáculo |
 
---
 
### Relacionamentos
 
| Origem | Destino | Tipo | Descrição |
|---|---|---|---|
| Teatro | Espetaculo | Associação (1 → 0..*) | Um teatro possui zero ou mais espetáculos |
| Espetaculo | Interacao | Associação (1 → 0..*) | Um espetáculo pode ter múltiplas interações de usuários |
| Espetaculo | Recomendacao | Associação (1 → 0..*) | Um espetáculo pode aparecer em múltiplas recomendações |
| Usuario | Wishlist | Composição (1 ◆→ 1) | Usuário possui uma wishlist; a wishlist não existe sem o usuário |
| Usuario | Preferencias | Composição (1 ◆→ 1) | Usuário possui preferências; elas não existem sem o usuário |
| Usuario | Interacao | Associação (1 → 0..*) | Um usuário pode ter múltiplas interações registradas |
| Usuario | Recomendacao | Associação (1 → 0..*) | Um usuário pode receber múltiplas recomendações |
| SistemaRecomendacao | Interacao | Dependência (→→) | O sistema depende de Interacao para gerar recomendações |
| SistemaRecomendacao | Recomendacao | Associação (1 → 0..*) | O sistema gera e gerencia múltiplas recomendações |
| Interacao | TipoInteracao | Dependência (→→) | Interacao utiliza a enumeração TipoInteracao |
 
---
 
### Observações e Decisões de Design
 
**Preferencias como Value Object:** a classe `Preferencias` não possui identidade própria. Sua igualdade é definida pelo conjunto de seus valores, seguindo o padrão de Value Object do Domain-Driven Design (DDD).
 
**Composição em Wishlist e Preferencias:** ambas possuem relação de composição com `Usuario`, pois seu ciclo de vida está diretamente ligado ao do usuário — são criadas e destruídas junto com ele.
 
**Separação de responsabilidades:** o `SistemaRecomendacao` é um componente dedicado exclusivamente à lógica de recomendação, mantendo as demais classes do domínio focadas em suas responsabilidades específicas.
 
**Expiração de interações:** a classe `Interacao` possui controle de validade por meio de `DataExpiracao` e dos métodos `EstaExpirada()` e `Renovar()`, permitindo que interações antigas sejam descartadas pelo `SistemaRecomendacao`.

## 14. Diagrama de Componentes

O diagrama de componentes abaixo representa a estrutura da aplicação seguindo o padrão arquitetural MVVM no cliente (React Native) e SOA no servidor (ASP.NET Core), com dois mecanismos de persistência: um banco de dados relacional PostgreSQL para dados estruturados e transacionais e um Storage Bucket para armazenamento de eventos de comportamento, vetores latentes do modelo SVD e assets de mídia.

<div align="center">
  <sub>Imagem 10 - Diagrama de Componentes UML</sub><br>
  <img src="../assets/diagrama-de-componentes.drawio.png" width="100%" alt="Diagrama de Componentes UML"><br>
  <sup>Fonte: Material produzido pelos autores, 2026.</sup>
</div>

### Packages
 
Packages são representados por um retângulo com uma abinha no topo. Servem para agrupar componentes que pertencem à mesma camada ou responsabilidade.
 
O diagrama possui quatro packages:
 
- **Presentation**: agrupa os componentes da camada de apresentação do app React Native (MVVM)
- **Service | ASP.NET Core**: agrupa os serviços do backend
- **Data**: agrupa os componentes da camada de acesso a dados
- **External**: agrupa os serviços externos consumidos pelo backend
PostgreSQL e Storage Bucket ficam **fora de qualquer package** pois são infraestrutura externa — não fazem parte do código da aplicação.
 
---
 
### Componentes
 
Componentes são representados por um retângulo com o ícone de componente no canto superior direito (dois retângulos pequenos sobrepostos). Cada componente possui um estereótipo que descreve sua natureza.
 
| Componente | Estereótipo | Package |
|---|---|---|
| View | «component» | Presentation |
| ViewModel | «component» | Presentation |
| Repository | «component» | Presentation |
| API Gateway | «service» | Service \| ASP.NET Core |
| Backend Services | «service» | Service \| ASP.NET Core |
| Recommendation Service | «service» | Service \| ASP.NET Core |
| Training Job | «job» | Service \| ASP.NET Core |
| PostgresDataSource | «dataSource» | Data |
| BucketDataSource | «dataSource» | Data |
| CDN Imagens | «external» | External |
| ViaCEP | «external» | External |
| PostgreSQL | «database» | — (externo) |
| Storage Bucket | «artifact» | — (externo) |
 
**Sobre os estereótipos:**
 
- `«component»`: unidade de software com responsabilidade própria dentro da aplicação cliente.
- `«service»`: serviço do backend exposto via HTTP, com responsabilidade bem definida.
- `«job»`: processo executado periodicamente de forma assíncrona, sem exposição via HTTP.
- `«dataSource»`: adaptador que encapsula os detalhes de acesso a uma fonte de dados externa.
- `«external»`: serviço de terceiros consumido pelo backend via HTTP.
- `«database»`: banco de dados externo ao código da aplicação.
- `«artifact»`: recurso implantado externamente, como um bucket de storage.
---
 
### Interfaces
 
As interfaces são representadas pelo padrão **ball-and-socket**:
 
- **Bolinha (provided interface)** — linha reta saindo do componente com um círculo na ponta. Indica que o componente fornece aquele contrato.
- **Semicírculo aberto (required interface)** — linha reta saindo do componente com um semicírculo na ponta. Indica que o componente precisa daquele contrato para funcionar.
O encaixe entre o semicírculo e a bolinha representa a satisfação do contrato — o componente que precisa encontra o componente que fornece.
 
O diagrama possui cinco interfaces:
 
| Interface | Fornecida por | Consumida por |
|---|---|---|
| IRepository | Repository | ViewModel |
| IApiGateway | API Gateway | Repository |
| IBackendService | Backend Services | API Gateway |
| IRecommendationService | Recommendation Service | API Gateway |
| IPostgresDataSource | PostgresDataSource | Backend Services, Training Job, Recommendation Service |
| IBucketDataSource | BucketDataSource | Backend Services, Training Job, Recommendation Service |
 
O `Repository` está no centro do cliente: fornece a bolinha `IRepository` para o ViewModel e consome a bolinha `IApiGateway` do API Gateway por meio do seu semicírculo — abstraindo todas as chamadas de rede.
 
`IPostgresDataSource` e `IBucketDataSource` são consumidas por três componentes cada — `Backend Services`, `Training Job` e `Recommendation Service` — pois todos precisam acessar dados persistidos, cada um com finalidades distintas.

### Responsabilidades do Storage Bucket
 
O Storage Bucket possui responsabilidade tripla, organizada internamente por prefixos de path:
 
| Path | Conteúdo | Escrito por | Lido por |
|---|---|---|---|
| `/media/` | Assets de mídia (imagens de eventos e atrações) | Backend Services | CDN Imagens |
| `/events/` | Eventos brutos de comportamento (cliques, visualizações, compras) | Backend Services | Training Job |
| `/model/` | Vetores latentes do modelo SVD (matrizes usuário × fator, item × fator) | Training Job | Recommendation Service |
 
### Responsabilidades do PostgreSQL
 
O PostgreSQL armazena exclusivamente dados transacionais e os hiperparâmetros do modelo SVD:
 
| Conteúdo | Escrito por | Lido por |
|---|---|---|
| Dados transacionais (usuários, pedidos, produtos, eventos) | Backend Services | Backend Services |
| Hiperparâmetros SVD (n_fatores, learning_rate, epochs, versão, data_treino) | Training Job | Recommendation Service, Training Job |

### Decisões

**Por que o ball-and-socket foi usado entre ViewModel e Repository?**
O ViewModel não depende da implementação concreta do Repository — ele depende apenas do contrato `IRepository`. O ball-and-socket torna esse contrato visível no diagrama, o que uma seta tracejada simples não expressa.
 
**Por que o Repository passou a consumir IApiGateway em vez de IPostgresDataSource e IBucketDataSource?**
O app React Native roda no dispositivo do usuário — nunca acessa banco de dados diretamente. Todo acesso a dados passa pela API do servidor. O Repository no cliente abstrai as chamadas HTTP para o API Gateway, não o acesso direto a fontes de dados. `IPostgresDataSource` e `IBucketDataSource` existem apenas no servidor.
 
**Por que IPostgresDataSource e IBucketDataSource têm múltiplos consumidores?**
Backend Services, Training Job e Recommendation Service têm necessidades distintas de acesso a dados, mas compartilham as mesmas fontes. Centralizar o acesso nos DataSources evita duplicação de lógica de conexão e mantém um único ponto de configuração para cada tecnologia de persistência.
 
**Por que o Training Job usa seta tracejada simples em vez de ball-and-socket para o Recommendation Service?**
O Training Job não consome uma interface fornecida pelo Recommendation Service — ele é acionado pelo próprio Recommendation Service periodicamente. A seta tracejada representa uma dependência de acionamento, não um contrato de interface.
 
**Por que o SVD Model Store (.zip) foi removido?**
O modelo SVD é composto por duas partes com naturezas distintas: hiperparâmetros (dados estruturados, poucos campos) e vetores latentes (arrays densos e pesados). Separar essas partes nos storages já existentes — PostgreSQL e Storage Bucket respectivamente — elimina um componente desnecessário e aproveita a infraestrutura já presente.
 
**Por que PostgreSQL e Storage Bucket ficam fora dos packages?**
Packages representam agrupamentos do código da aplicação. Infraestrutura externa não pertence a nenhuma camada do sistema — colocá-la dentro de um package seria incorreto semanticamente.

## 15. Diagrama de Casos de Uso

![Diagrama de Casos de Uso](../assets/Diagrama-use-case.png)

### Atores

#### Usuário
Representa o espectador autenticado na plataforma, responsável por interagir com o sistema, visualizar espetáculos, gerenciar preferências e acessar recomendações personalizadas.

---

### Casos de Uso

#### UC01 — Cadastrar conta
Como usuário, quero criar uma conta na plataforma para salvar minhas preferências e manter minha wishlist sincronizada entre diferentes sessões e dispositivos.

#### UC02 — Realizar login
Como usuário, quero realizar login utilizando e-mail e senha para acessar minhas informações personalizadas e minhas recomendações.

#### UC03 — Responder onboarding
Como novo usuário, quero responder um questionário de interesses para que o sistema personalize minhas recomendações iniciais.

#### UC04 — Gerenciar perfil e preferências
Como usuário, quero editar meus dados de perfil e minhas preferências para manter as recomendações alinhadas aos meus interesses atuais.

#### UC05 — Visualizar catálogo de espetáculos
Como usuário, quero visualizar o catálogo de espetáculos organizado por categorias para explorar produções disponíveis na plataforma.

#### UC06 — Pesquisar espetáculos
Como usuário, quero pesquisar espetáculos e teatros específicos para encontrar rapidamente informações relevantes.

#### UC07 — Visualizar detalhes do espetáculo
Como usuário, quero visualizar os detalhes de um espetáculo para conhecer informações sobre a produção, teatro e disponibilidade.

#### UC08 — Visualizar informações do teatro
Como usuário, quero visualizar endereço e horários do teatro para me planejar antes de assistir ao espetáculo.

#### UC09 — Receber recomendações personalizadas
Como usuário, quero receber sugestões personalizadas baseadas em minhas preferências e interações para descobrir novos espetáculos relevantes.

#### UC10 — Visualizar wishlist
Como usuário, quero acessar minha wishlist para visualizar os espetáculos salvos anteriormente.

#### UC11 — Adicionar espetáculo à wishlist
Como usuário, quero adicionar espetáculos à minha wishlist para salvar produções de interesse e melhorar futuras recomendações.

#### UC12 — Remover espetáculo da wishlist
Como usuário, quero remover espetáculos da wishlist para atualizar meus interesses na plataforma.

#### UC13 — Acessar link de compra
Como usuário, quero acessar a bilheteria oficial do espetáculo para realizar a compra do ingresso com segurança.

---

### Relacionamentos do Diagrama

#### Include
O caso de uso "Visualizar detalhes do espetáculo" possui relação `<<include>>` com "Visualizar informações do teatro", pois as informações do local fazem parte da experiência de visualização dos detalhes do espetáculo.

#### Extend
O caso de uso "Acessar link de compra" possui relação `<<extend>>` com "Visualizar detalhes do espetáculo", uma vez que o redirecionamento para compra ocorre opcionalmente após a visualização do espetáculo.

---

### Associação entre Requisitos Funcionais e Casos de Uso

| Requisito Funcional | Caso de Uso Relacionado |
|---|---|
| RF01 — Onboarding de Interesses | UC03 — Responder onboarding |
| RF02 — Recomendação por Geolocalização | UC09 — Receber recomendações personalizadas |
| RF03 — Autenticação e Persistência | UC01 — Cadastrar conta / UC02 — Realizar login |
| RF04 — Catálogo Categorizado | UC05 — Visualizar catálogo de espetáculos |
| RF05 — Busca Específica | UC06 — Pesquisar espetáculos |
| RF06 — Gestão de Wishlist | UC10 — Visualizar wishlist / UC11 — Adicionar espetáculo à wishlist / UC12 — Remover espetáculo da wishlist |
| RF07 — Informações de Teatros | UC08 — Visualizar informações do teatro |
| RF08 — Gestão de Perfil e Preferências | UC04 — Gerenciar perfil e preferências |
| RF09 — Redirecionamento de Compra | UC13 — Acessar link de compra |
| RF10 — Recomendação Inteligente | UC09 — Receber recomendações personalizadas |

## 16. Linguagens e Tecnologias Utilizadas

O projeto segue uma Arquitetura Orientada a Serviços (SOA), com camadas bem definidas de apresentação, serviços e dados. As tecnologias foram escolhidas para atender aos requisitos funcionais e não funcionais do sistema, e são descritas a seguir.

**Supabase (banco de dados, autenticação e armazenamento)**

O Supabase é uma plataforma open-source construída sobre PostgreSQL que oferece banco de dados relacional, autenticação de usuários e armazenamento de arquivos de forma integrada. Ele será provisionado localmente via Docker, o que garante que todos da equipe trabalhem no mesmo ambiente sem depender de serviços externos pagos durante o desenvolvimento. Usar uma única plataforma para essas três responsabilidades reduz a complexidade da infraestrutura e facilita a manutenção.

O banco de dados PostgreSQL armazenará as entidades do sistema como usuários, espetáculos, teatros e wishlists. O módulo de autenticação cuidará do cadastro, login e geração de tokens JWT, dispensando a necessidade de implementar essa lógica do zero. O Storage Bucket será usado para guardar assets de mídia e arquivos gerados pelo modelo de recomendação.

**Docker**

O Docker é a ferramenta de conteinerização utilizada para rodar o Supabase localmente. Com ele, toda a infraestrutura de banco de dados, autenticação e storage é empacotada em containers, garantindo que o ambiente seja idêntico para todos os membros da equipe independentemente do sistema operacional. Isso evita problemas de configuração e torna o setup do projeto mais simples e reproduzível.

**Backend de APIs em C# com ASP.NET Core**

O backend de APIs será desenvolvido em C# com ASP.NET Core, expondo endpoints REST a serem consumidos pelo frontend. A arquitetura prevê um API Gateway, que receberá todas as requisições e roteará para os serviços corretos, e os Backend Services, que concentrarão a lógica de negócio. O C# é fortemente tipado, o que facilita modelar as entidades do domínio e reduz erros em tempo de execução. O ASP.NET Core oferece suporte nativo a injeção de dependência, autenticação JWT e ferramentas de escalabilidade, atendendo aos requisitos de segurança e disponibilidade do sistema.

**Modelo de recomendação em Python com scikit-surprise**

O modelo de recomendação está sendo desenvolvido em Python, com a biblioteca scikit-surprise para filtragem colaborativa. Dois algoritmos estão sendo implementados e serão comparados: KNN, que recomenda espetáculos com base em usuários de comportamento similar, e Funk SVD, que fatoriza a matriz de interações em representações latentes para prever o engajamento do usuário com espetáculos não vistos. Os modelos são treinados sobre um dataset de 109.916 interações entre aproximadamente 68 mil usuários e 571 espetáculos, com hiperparâmetros otimizados via GridSearchCV. O modelo final, após a comparação entre os dois algoritmos, será integrado ao backend de recomendação em C#.

Python foi escolhido para essa etapa porque seu ecossistema de ciência de dados (pandas, numpy, scikit-learn, scikit-surprise) não tem equivalente de mesma maturidade em C# para tarefas de filtragem colaborativa, tornando a experimentação muito mais ágil.

**Frontend em React Native**

O frontend será desenvolvido em React Native, framework que permite criar aplicações móveis para iOS e Android a partir de uma única base de código. Isso é importante porque o produto precisa estar disponível nas duas plataformas sem duplicar o esforço de desenvolvimento. O React Native tem bom suporte a geolocalização, notificações push e navegação entre telas, funcionalidades centrais para o app. A proximidade com o ecossistema JavaScript facilita o consumo das APIs REST do backend.

**Padrão MVVM no frontend**

O frontend seguirá o padrão MVVM (Model-View-ViewModel), separando a interface visual (View) da lógica de estado e apresentação (ViewModel) e do acesso a dados (Repository). Essa separação torna os componentes de tela mais simples, os ViewModels testáveis de forma isolada e facilita mudanças na camada de comunicação com o backend sem impactar a interface. Para um app com muitas telas de conteúdo dinâmico, como carrosséis de recomendação e catálogo filtrado, essa organização evita que lógica e interface se misturem e tornem o código difícil de manter.

**ViaCEP**

O ViaCEP é uma API pública e gratuita que retorna dados de endereço a partir de um CEP informado. No projeto, ela será usada durante o onboarding do usuário para identificar automaticamente a cidade a partir do CEP cadastrado, sem que o usuário precise informar isso manualmente. Essa informação é essencial para o sistema de recomendação, que filtra os espetáculos de acordo com a cidade do usuário, sendo São Paulo ou Rio de Janeiro.

---

## 17. Planejamento de Testes

Esta seção apresenta a estrutura de planejamento de testes do sistema de recomendação da Aventura. O objetivo é organizar a validação sistemática da solução, cobrindo testes funcionais, testes de requisitos não funcionais, testes de usabilidade, testes de APIs externas e testes de integração.

O planejamento foi estruturado para garantir que os principais fluxos do sistema sejam avaliados antes da entrega final, considerando tanto o comportamento esperado da aplicação quanto possíveis falhas, limitações técnicas e cenários negativos.

A validação do sistema deve considerar o contexto do produto: um aplicativo mobile com onboarding, catálogo de espetáculos, recomendações personalizadas, wishlist, perfil, notificações, integração com APIs externas e redirecionamento para ticketeiras parceiras.

---

### 17.1 Objetivo do planejamento de testes

O objetivo do planejamento de testes é definir como o sistema será validado ao longo da Sprint 3, garantindo que os requisitos funcionais, requisitos não funcionais, integrações e fluxos de uso sejam verificados de forma organizada.

Os testes devem permitir identificar falhas de comportamento, problemas de usabilidade, inconsistências de integração, erros de comunicação com APIs externas e possíveis limitações de desempenho, segurança ou disponibilidade.

Este planejamento também busca garantir que cada cenário de teste possua:

- propósito;
- pré-condições;
- instruções de execução;
- cenário positivo;
- cenário negativo;
- pós-condições;
- critérios de validação;
- discussão de abrangência.

---

### 17.2 Estratégia geral de testes

A estratégia de testes do projeto está organizada em cinco frentes principais:

1. **Testes de funcionalidade:** validam se os requisitos funcionais do sistema estão sendo cumpridos corretamente.
2. **Testes de requisitos não funcionais:** avaliam atributos de qualidade, como desempenho, segurança, disponibilidade, usabilidade e manutenibilidade.
3. **Testes de usabilidade:** verificam se usuários externos conseguem compreender e executar os principais fluxos da aplicação.
4. **Testes de APIs externas:** validam o comportamento esperado das integrações com serviços externos, como ViaCEP e ticketeiras.
5. **Testes de integração:** verificam a comunicação entre frontend, backend, banco de dados, motor de recomendação e sistemas externos.

Os testes deverão contemplar cenários positivos e negativos, garantindo que o sistema seja avaliado tanto em fluxos esperados quanto em situações de erro.

---

### 17.3 Testes de funcionalidade

Esta seção evolui os testes de usuário definidos junto a cada Requisito Funcional (RFs da seção 9.2) para casos de teste funcionais completos, com propósito explícito, pré-condições, cenários positivos e negativos, instruções de execução, pós-condições e critério objetivo de sucesso. A premissa adotada foi garantir cobertura mínima de **um caso de teste principal por RF** (TF-01 a TF-10), priorizando os fluxos da jornada crítica do usuário: descoberta → personalização → wishlist → conversão. Os cenários negativos foram desenhados para validar tratamento de erro, validações de entrada, indisponibilidade de serviços externos e estados-limite da aplicação.

| ID do teste | Requisito relacionado | Funcionalidade testada | Propósito | Pré-condições | Cenário positivo | Cenário negativo | Instruções de execução | Pós-condições | Critério de sucesso |
|---|---|---|---|---|---|---|---|---|---|
| TF-01 | RF01 | Onboarding de interesses não-bloqueante (gênero, cidade, frequência) com possibilidade de pular perguntas | Verificar que (i) as preferências respondidas são persistidas e direcionam a personalização das seções da tela Explorar e (ii) perguntas puladas resultam em placeholders/CTAs nas seções dependentes ("Escolhidos para você" e "Perto de você") sem bloquear a navegação. | Conta recém-criada sem preferências salvas; usuário autenticado na tela de Onboarding; base com espetáculos cadastrados em SP e RJ e em múltiplos gêneros. | Usuário responde gênero "Musical" e cidade "São Paulo", conclui o onboarding e chega à tela Explorar com as seções "Escolhidos para você" e "Perto de você" populadas com conteúdo aderente às respostas (musicais em SP). | (a) Usuário pula todas as perguntas — sistema permite avançar e a tela Explorar exibe placeholders/CTAs nas seções "Escolhidos para você" e "Perto de você" convidando o usuário a definir as preferências, sem bloquear a navegação. (b) Usuário responde parcialmente (ex.: informa apenas a cidade) — sistema persiste o que foi respondido; "Perto de você" carrega normalmente, mas "Escolhidos para você" exibe o CTA para definir gênero. | 1. Criar conta nova de teste. <br>2. Acessar a tela de Onboarding e responder gênero "Musical" e cidade "São Paulo". <br>3. Concluir o onboarding e conferir as seções "Escolhidos para você" e "Perto de você" na tela Explorar. <br>4. Repetir o fluxo com uma nova conta pulando todas as perguntas e conferir os placeholders/CTAs (cenário negativo a). <br>5. Repetir com resposta parcial (apenas cidade) e conferir o estado das duas seções (cenário negativo b). | Preferências respondidas (totais ou parciais) gravadas em `Usuario.preferencias` no banco; usuário marcado como "onboarding concluído" independentemente das respostas; nenhuma resposta é exigida para liberar o acesso à Explorar. | Preferências respondidas são persistidas corretamente no banco em 100% dos casos; as seções "Escolhidos para você" e "Perto de você" refletem corretamente o estado das preferências (conteúdo populado quando há resposta, placeholder/CTA quando não há); a navegação nunca é bloqueada por ausência de respostas. |
| TF-02 | RF02 | Curadoria automática do carrossel "Perto de você" na tela Explorar conforme a cidade definida pelo usuário no onboarding ou no perfil (cidades suportadas: Rio de Janeiro e São Paulo) | Verificar que (i) o carrossel "Perto de você" expõe exclusivamente espetáculos cujo teatro está localizado na cidade definida pelo usuário, (ii) alterações posteriores na cidade via Perfil refletem corretamente na próxima carga da tela Explorar e (iii) quando a cidade não é definida, o carrossel exibe o empty state correto convidando o usuário a informá-la. | Usuário logado; base populada com espetáculos ativos em SP e RJ vinculados a teatros com a cidade preenchida corretamente. | Usuário com cidade = SP definida (no onboarding ou no Perfil) acessa a tela Explorar e o carrossel "Perto de você" expõe exclusivamente cards de espetáculos cujo teatro está localizado em São Paulo. | (a) Usuário acessa o Perfil, altera a cidade de SP para RJ, salva e retorna à Explorar — o carrossel "Perto de você" recarrega exibindo apenas espetáculos do RJ, sem nenhum card de SP remanescente. (b) Usuário acessa a Explorar sem cidade definida (pulou a pergunta no onboarding e nunca preencheu no Perfil) — o carrossel "Perto de você" exibe o empty state com o título "Adicione sua localidade", o texto "Compartilhar conosco sua cidade nos ajuda a encontrar espetáculos perto de você." e o botão "Adicionar cidade" que direciona para a tela de Perfil. | 1. Logar com usuário que possui cidade = SP definida (via onboarding ou Perfil). <br>2. Acessar a tela Explorar e conferir, no carrossel "Perto de você", a cidade do teatro em cada card exibido. <br>3. Acessar Perfil, alterar a cidade para RJ, salvar e retornar à Explorar. <br>4. Conferir novamente o carrossel "Perto de você" e validar que todos os cards são de teatros do RJ (cenário negativo a). <br>5. Logar com uma conta sem cidade definida (criada pulando a pergunta no onboarding) e acessar a Explorar para conferir o empty state do carrossel "Perto de você", incluindo título, texto e botão (cenário negativo b). <br>6. Clicar em "Adicionar cidade" e validar que o app direciona o usuário para a tela de Perfil. | Cidade do perfil atualizada no banco em caso de alteração; nenhum dado de espetáculo é modificado pela navegação. | Em todos os carregamentos com cidade definida, 100% dos cards exibidos no carrossel "Perto de você" possuem teatro com cidade igual à cidade do usuário; alterações de cidade via Perfil refletem corretamente na próxima visita à Explorar; quando não há cidade definida, o empty state é renderizado com o título, o texto e o botão exatos, e o botão direciona corretamente para o Perfil. |
| TF-03 | RF03 | Autenticação do usuário (login via e-mail/senha ou via Auth0) e gestão da sessão persistente até o logout explícito pelo Perfil | Verificar que (i) o login com credenciais válidas — seja por e-mail/senha, seja via Auth0 — autentica o usuário e libera o acesso ao app, (ii) a sessão permanece ativa entre aberturas do app até que o usuário execute logout explícito pelo Perfil, (iii) o app exige login desde a abertura, sem permitir uso anônimo e (iv) tentativas de login com credenciais incorretas exibem feedback visual ao usuário e permitem nova tentativa. | Aplicativo instalado e iniciado; conta válida cadastrada na base, com pelo menos 2 itens previamente salvos na Wishlist para validação da persistência; conta com cidade e gênero já definidos para evitar interferência do Onboarding na execução do teste. | Usuário insere e-mail e senha válidos na tela de Login, é autenticado e acessa a tela Explorar; ao abrir "Minha Wishlist", os itens previamente salvos estão presentes e íntegros. | (a) Usuário escolhe entrar via Auth0 em vez de e-mail/senha — o fluxo do provedor externo conclui com sucesso e o usuário acessa a Explorar normalmente. (b) Usuário autenticado fecha o app completamente (encerrando a tarefa em background) e reabre — o app abre direto na Explorar, sem solicitar credenciais novamente, comprovando a persistência da sessão. (c) Usuário autenticado acessa Perfil → Logout — o app retorna à tela de Login e nenhuma feature fica acessível até uma nova autenticação. (d) Usuário insere e-mail e/ou senha incorretos e confirma — uma notificação com a mensagem "Credenciais de login inválidas" é exibida no topo da tela e os campos de e-mail e senha são reabertos para que o usuário tente novamente, sem bloqueio ou contador de tentativas. | 1. Abrir o app pela primeira vez no dispositivo (estado deslogado) e validar que a tela de Login é exibida antes de qualquer conteúdo, sem permitir acesso anônimo. <br>2. Inserir e-mail e senha válidos e confirmar. <br>3. Conferir o acesso à tela Explorar e validar, em "Minha Wishlist", que os itens previamente salvos estão presentes (cenário positivo). <br>4. Em uma segunda conta de teste, repetir o passo 1 e optar por entrar via Auth0; concluir o fluxo do provedor externo e validar o acesso ao app (cenário negativo a). <br>5. Com o app já autenticado, fechar a aplicação completamente (encerrar a tarefa em background) e reabrir; conferir que o app abre direto na Explorar (cenário negativo b). <br>6. Acessar Perfil → Logout e validar o retorno à tela de Login; tentar interagir com qualquer feature do app para confirmar que o acesso está bloqueado (cenário negativo c). <br>7. Na tela de Login, inserir e-mail e/ou senha incorretos e confirmar; validar a exibição da notificação "Credenciais de login inválidas" e a reabertura dos campos de e-mail e senha para nova tentativa (cenário negativo d). | Sessão ativa registrada para o usuário após login bem-sucedido; sessão encerrada após logout explícito pelo Perfil; itens da Wishlist mantidos íntegros no banco independentemente do estado da sessão. | Login com credenciais válidas via e-mail/senha e via Auth0 autentica o usuário em ambos os caminhos e libera o acesso à Explorar; a sessão persiste entre fechamentos e reaberturas do app; o logout pelo Perfil encerra a sessão e bloqueia o acesso a qualquer feature até nova autenticação; nenhum acesso ao app é permitido sem login prévio; tentativas com credenciais incorretas exibem a notificação "Credenciais de login inválidas" e permitem nova tentativa sem bloquear o usuário. |
| TF-04 | RF04 | Catálogo categorizado por gênero distribuído nos carrosséis da aba Explorar ("Em alta", "Pra você", "Novidades" e "Perto de você") e no carrossel "Espetáculos semelhantes" da página de detalhes do espetáculo, com filtro por categoria aplicável na Explorar | Verificar que (i) ao aplicar um filtro por gênero na aba Explorar, os carrosséis dessa aba passam a exibir apenas espetáculos da categoria selecionada, (ii) ao remover o filtro, os carrosséis voltam ao estado original sem restrição por gênero e (iii) cada espetáculo do catálogo está obrigatoriamente associado a uma categoria válida (Musical, Drama, Balé infantil, Corporativo, Comédia ou Dança). | Usuário logado com cidade e preferências definidas; base com espetáculos cadastrados nas seis categorias suportadas (Musical, Drama, Balé infantil, Corporativo, Comédia e Dança); cada espetáculo possui categoria obrigatoriamente associada no banco. | Usuário acessa a aba Explorar, aplica o filtro "Comédia" e os carrosséis "Em alta", "Pra você", "Novidades" e "Perto de você" passam a exibir apenas cards de espetáculos da categoria Comédia, mantendo a estrutura visual e o scroll horizontal de cada carrossel. | (a) Usuário remove o filtro "Comédia" — os quatro carrosséis voltam a exibir a curadoria completa, sem restrição por gênero, recuperando o estado original da Explorar. (b) Usuário aplica o filtro em uma categoria menos representada (ex.: "Balé infantil") e algum carrossel não possui espetáculos correspondentes naquela combinação (ex.: nenhum balé infantil em "Perto de você" para a cidade do perfil) — o carrossel afetado é exibido vazio ou ocultado, sem quebrar a tela nem misturar resultados de outras categorias. | 1. Logar com usuário que possui cidade e preferências definidas. <br>2. Acessar a aba Explorar e conferir o estado padrão dos quatro carrosséis (sem filtro aplicado). <br>3. Aplicar o filtro "Comédia" e validar, em cada carrossel da Explorar, que todos os cards exibidos pertencem à categoria Comédia. <br>4. Remover o filtro e conferir que os quatro carrosséis voltam ao estado anterior (cenário negativo a). <br>5. Aplicar o filtro "Balé infantil" e percorrer os quatro carrosséis, registrando o comportamento de cada um (carrossel populado, vazio ou ocultado), sem que a tela apresente erro (cenário negativo b). <br>6. Clicar em qualquer card exibido durante o teste para validar que o app navega corretamente para a página de detalhes do respectivo espetáculo. | Nenhuma alteração de dados do usuário; preferências, cidade do perfil e Wishlist permanecem intactas; o filtro aplicado é descartado ao sair da aba Explorar ou ao remover explicitamente. | Quando há filtro ativo, 100% dos cards exibidos nos carrosséis da Explorar pertencem à categoria selecionada; remover o filtro restaura o estado original; carrosséis sem espetáculos para a combinação filtro+regra são renderizados de forma consistente (vazios ou ocultados) sem quebrar a UI; cliques em cards encaminham para a página de detalhes correta. |
| TF-05 | RF05 | Busca de espetáculos por título ou por nome de teatro na aba "Buscar", com confirmação explícita (não em tempo real, sem histórico) e exibição de empty state quando não há resultados | Verificar que (i) a busca por um título de espetáculo cadastrado retorna o espetáculo correspondente nos resultados, (ii) a busca por um nome de teatro cadastrado retorna todos os espetáculos em cartaz naquele teatro, (iii) a busca por um termo (título ou teatro) não cadastrado exibe o empty state "Nenhum espetáculo encontrado" e (iv) clicar em um resultado encaminha o usuário à página de detalhes do respectivo espetáculo. | Usuário logado e na aba "Buscar"; base com pelo menos 3 espetáculos cadastrados com títulos conhecidos (ex.: "A Noviça Rebelde", "Hamlet Revisitado", "O Mágico de Oz") e seus respectivos teatros (ex.: "Vibra São Paulo", "Teatro Sérgio Cardoso"); campo de busca disponível com o placeholder "Buscar espetáculos, teatros...". | (i) Usuário digita "A Noviça Rebelde" no campo de busca, confirma a busca (Enter) e a lista de resultados exibe o card do espetáculo "A Noviça Rebelde", contendo título, teatro, cidade e categoria. (ii) Usuário digita "Vibra São Paulo" no campo de busca, confirma e a lista de resultados exibe todos os espetáculos em cartaz nesse teatro (incluindo "A Noviça Rebelde"), com os respectivos metadados. | (a) Usuário digita um termo inexistente (ex.: "Espetáculo XYZ123" ou "Teatro Inexistente") e confirma a busca — a tela exibe o empty state "Nenhum espetáculo encontrado", sem cards de resultado nem mistura com a seção "Recomendados para você". (b) Usuário clica em um dos cards retornados pela busca — o app navega corretamente para a página de detalhes do respectivo espetáculo, com título, teatro, cidade e categoria coerentes com o card. | 1. Logar e acessar a aba "Buscar" pela bottom nav. <br>2. Conferir o estado inicial da tela: campo de busca com placeholder "Buscar espetáculos, teatros...", ícone de filtro à direita e seção "Recomendados para você" abaixo. <br>3. Digitar "A Noviça Rebelde" no campo de busca e confirmar (Enter); validar que o resultado exibe o card do espetáculo correspondente (cenário positivo i). <br>4. Limpar o campo, digitar "Vibra São Paulo" e confirmar; validar que a lista exibe todos os espetáculos em cartaz nesse teatro, incluindo "A Noviça Rebelde" (cenário positivo ii). <br>5. Limpar o campo, digitar "Espetáculo XYZ123" e confirmar; validar a exibição do empty state "Nenhum espetáculo encontrado" (cenário negativo a). <br>6. Buscar novamente "A Noviça Rebelde", clicar no card resultante e validar que o app navega para a página de detalhes correspondente (cenário negativo b). | Nenhuma alteração de dados do usuário ou do catálogo; nenhuma persistência do termo buscado (não há histórico de buscas). | Busca por título de espetáculo cadastrado retorna o resultado correto com os metadados completos; busca por nome de teatro cadastrado retorna todos os espetáculos em cartaz nesse teatro; busca por termo inexistente exibe o empty state "Nenhum espetáculo encontrado"; cliques em cards encaminham corretamente para a página de detalhes do respectivo espetáculo. |
| TF-06 | RF06 | Gestão da Wishlist: adição apenas a partir da página de detalhes do espetáculo (coração no canto superior direito), remoção pela página de detalhes ou pela aba Wishlist, com feedback visual por notificação a cada operação | Verificar que (i) adicionar um espetáculo via página de detalhes persiste o vínculo na Wishlist do usuário e dispara notificação de confirmação, (ii) remover um espetáculo pela própria página de detalhes ou pela aba Wishlist encerra o vínculo e dispara notificação de confirmação, (iii) quando a Wishlist está vazia, a aba exibe o empty state correto convidando o usuário a explorar espetáculos e (iv) a aba Wishlist permite apenas visualizar e remover, não dispondo de mecanismo para adicionar novos espetáculos. | Usuário logado com Wishlist inicialmente vazia; base com espetáculos cadastrados (ex.: "A Noviça Rebelde" em cartaz no "Vibra São Paulo"); permissões de notificação concedidas no dispositivo de teste. | Usuário acessa a página de detalhes de "A Noviça Rebelde", clica no coraçãozinho no canto superior direito — o ícone passa ao estado preenchido (roxo), são exibidas as notificações in-app e push com a mensagem "item adicionado à wishlist" e, ao acessar a aba Wishlist, o espetáculo aparece listado com coraçãozinho preenchido, título, teatro, cidade e categoria. | (a) Usuário com Wishlist vazia acessa a aba Wishlist — visualiza o empty state com o título "Você ainda não salvou nenhum espetáculo", o texto "Explore e salve seus favoritos para acompanhá-los depois" e o botão "Explorar espetáculos" que direciona para a aba Explorar. (b) Usuário remove o espetáculo clicando no coraçãozinho — seja na página de detalhes, seja na aba Wishlist — são exibidas as notificações com a mensagem "item removido da wishlist", o vínculo é encerrado e, se era o único item, a aba volta ao empty state. (c) Usuário inspeciona a aba Wishlist e confirma que a única ação disponível por item é remover (clique no coraçãozinho), sem nenhum mecanismo para adicionar novos espetáculos a partir dessa tela; a adição exige navegar até a página de detalhes de um espetáculo. | 1. Logar com usuário que possui a Wishlist vazia e acessar a aba Wishlist; validar o empty state com título, texto e botão "Explorar espetáculos" (cenário negativo a). <br>2. Clicar em "Explorar espetáculos" e confirmar a navegação para a aba Explorar. <br>3. Abrir a página de detalhes de "A Noviça Rebelde" e clicar no coraçãozinho no canto superior direito; validar a mudança do ícone para o estado preenchido e a exibição das notificações "item adicionado à wishlist" (cenário positivo). <br>4. Acessar a aba Wishlist e conferir que o espetáculo está listado com coraçãozinho preenchido e metadados completos (título, teatro, cidade, categoria). <br>5. Clicar no coraçãozinho do item diretamente na aba Wishlist; validar as notificações "item removido da wishlist" e o retorno ao empty state (cenário negativo b — remoção via Wishlist). <br>6. Repetir os passos 3 e 4 para readicionar o espetáculo; em seguida, voltar à página de detalhes e clicar no coraçãozinho; validar a remoção e as notificações (cenário negativo b — remoção via detalhes). <br>7. Inspecionar a aba Wishlist e confirmar que não existe botão, atalho ou interação que permita adicionar novos espetáculos diretamente dessa tela (cenário negativo c). | Cada operação de adicionar ou remover persiste ou remove imediatamente o vínculo entre o usuário e o espetáculo no banco; nenhuma alteração nos dados do espetáculo, nas preferências do usuário ou na cidade do perfil. | Adicionar via página de detalhes persiste o espetáculo na Wishlist do usuário e dispara as notificações "item adicionado à wishlist"; remover (via página de detalhes ou via aba Wishlist) encerra o vínculo e dispara as notificações "item removido da wishlist"; quando vazia, a aba Wishlist exibe o empty state com título, texto e botão "Explorar espetáculos" funcional; a aba Wishlist não oferece, em nenhuma condição, mecanismo para adicionar novos espetáculos. |


**Discussão de abrangência:**

O plano de testes funcionais cobre **100% dos 10 RFs definidos na seção 9.2**, com um caso de teste principal por requisito. Cada caso contempla pelo menos um cenário positivo (caminho feliz validado em conjunto com a equipe de produto) e dois ou três cenários negativos, totalizando uma cobertura mínima de **três variações por funcionalidade**. A escolha por concentrar a evolução em um teste principal por RF — em vez de fragmentar em vários testes pequenos — foi feita para manter o planejamento legível dentro do tempo da Sprint 3 e por entender que os cenários negativos já garantem a exploração de estados-limite mais relevantes.

Os fluxos priorizados foram aqueles que sustentam a jornada crítica do produto: **descoberta inicial** (TF-01 Onboarding, TF-02 Geolocalização, TF-04 Catálogo, TF-05 Busca), **personalização contínua** (TF-08 Edição de perfil, TF-10 Recomendação), **engajamento** (TF-06 Wishlist) e **conversão** (TF-09 Redirecionamento de compra). Os testes TF-03 (Autenticação) e TF-07 (Informações de teatro) cobrem requisitos transversais que sustentam a confiança e a completude informacional do produto. Essa priorização está coerente com as personas Ana Luiza (descoberta) e Marcos Abrantes (decisão e consumo) descritas na seção 5.1.

Os cenários negativos foram desenhados a partir de três famílias de risco: **validação de entrada** (campos obrigatórios, strings vazias, caracteres especiais), **falhas de integração** (indisponibilidade de serviços externos, perda de rede, sessão expirada) e **regras de negócio em estados-limite** (categoria sem cadastros, usuário sem histórico, URLs fora da allowlist). Os critérios de sucesso foram escritos em termos objetivos e mensuráveis (percentuais de aderência, tempos máximos de resposta, integridade entre estado visual e banco), permitindo execução e avaliação independentes pelo time.

**Limitações reconhecidas:** (i) os testes funcionais não exploram combinatórias profundas entre múltiplos filtros simultâneos no catálogo — esse tipo de exploração será coberto na fase de execução por testes exploratórios manuais; (ii) cenários relacionados a desempenho, segurança, LGPD e usabilidade não são contemplados aqui por já existirem nas seções 17.4 (RNFs) e 17.5 (usabilidade); (iii) o critério de aderência ≥ 70% para o motor de recomendação (TF-10) é uma meta inicial e poderá ser recalibrado após a primeira rodada de execução, conforme dados reais de comportamento dos usuários de teste.

Para a fase de execução, recomenda-se priorizar **automação dos casos críticos de regressão** (TF-01, TF-03, TF-06, TF-09 e TF-10) por serem os fluxos com maior impacto direto na conversão e na retenção, mantendo os demais em execução manual durante a Sprint 3.

---

### 17.4 Testes de requisitos não funcionais

Nesta seção, devem ser planejados os testes relacionados aos RNFs, avaliando atributos de qualidade como desempenho, segurança, disponibilidade, usabilidade, compatibilidade e escalabilidade.

| ID do teste | RNF relacionado | Categoria de qualidade | Propósito | Pré-condições | Cenário positivo | Cenário negativo | Instruções de execução | Pós-condições | Critério de sucesso |
|---|---|---|---|---|---|---|---|---|---|
| TNF-01 | RNF-XX | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher |
| TNF-02 | RNF-XX | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher |
| TNF-03 | RNF-XX | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher |

**Discussão de abrangência:**  
A preencher pelo responsável, explicando quais RNFs foram cobertos e como os testes se relacionam com a arquitetura do sistema.

---

### 17.5 Testes de usabilidade

Nesta seção, deve ser planejado o roteiro de testes de usabilidade com pelo menos 5 usuários externos ao grupo, utilizando SUS ou outro método definido pelo professor.

#### Perfil dos participantes

| Participante | Perfil | Familiaridade com apps mobile | Frequência de consumo cultural | Observações |
|---|---|---|---|---|
| P1 | A preencher | A preencher | A preencher | A preencher |
| P2 | A preencher | A preencher | A preencher | A preencher |
| P3 | A preencher | A preencher | A preencher | A preencher |
| P4 | A preencher | A preencher | A preencher | A preencher |
| P5 | A preencher | A preencher | A preencher | A preencher |

#### Roteiro de testes de usabilidade

| ID do teste | Fluxo avaliado | Propósito | Pré-condições | Tarefa do usuário | Cenário positivo | Cenário negativo | Pós-condições | Critério de sucesso | Métricas observadas |
|---|---|---|---|---|---|---|---|---|---|
| TU-01 | Onboarding | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Tempo, erros, dúvidas |
| TU-02 | Home e recomendações | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Tempo, erros, dúvidas |
| TU-03 | Busca | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Tempo, erros, dúvidas |
| TU-04 | Wishlist | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Tempo, erros, dúvidas |
| TU-05 | Perfil e preferências | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Tempo, erros, dúvidas |

#### Questionário SUS

| Item | Afirmação | Nota de 1 a 5 |
|---|---|---|
| 1 | Eu acho que gostaria de usar este sistema com frequência. | A preencher |
| 2 | Eu achei o sistema desnecessariamente complexo. | A preencher |
| 3 | Eu achei o sistema fácil de usar. | A preencher |
| 4 | Eu acho que precisaria de ajuda de uma pessoa técnica para usar este sistema. | A preencher |
| 5 | Eu achei que as funcionalidades do sistema estavam bem integradas. | A preencher |
| 6 | Eu achei que havia muita inconsistência no sistema. | A preencher |
| 7 | Eu imagino que a maioria das pessoas aprenderia a usar este sistema rapidamente. | A preencher |
| 8 | Eu achei o sistema muito difícil de usar. | A preencher |
| 9 | Eu me senti confiante usando o sistema. | A preencher |
| 10 | Eu precisei aprender muitas coisas antes de conseguir usar o sistema. | A preencher |

**Discussão de abrangência:**  
A preencher pelo responsável, explicando quais fluxos da jornada mobile foram avaliados e quais limitações existem na amostra de usuários.

---

### 17.6 Testes de APIs externas

Nesta seção, devem ser planejados os testes das APIs externas utilizadas pelo sistema, como ViaCEP, ticketeiras externas ou outros serviços consumidos pela aplicação.

| ID do teste | API externa | Propósito | Pré-condições | Cenário positivo | Cenário negativo | Instruções de execução | Pós-condições | Critério de sucesso | Ferramentas |
|---|---|---|---|---|---|---|---|---|---|
| TAPI-01 | ViaCEP | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Postman/Insomnia |
| TAPI-02 | Ticketeira externa | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Postman/Insomnia |
| TAPI-03 | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher |

**Discussão de abrangência:**  
A preencher pelo responsável, explicando quais APIs foram cobertas e como o sistema deve lidar com falhas ou respostas inválidas.

---

### 17.7 Testes de integração

Nesta seção, devem ser planejados os testes das interações entre componentes internos e sistemas externos. Também deve ser previsto o uso do módulo VHS para cache temporário de respostas de APIs externas.

| ID do teste | Integração avaliada | Propósito | Pré-condições | Cenário positivo | Cenário negativo | Instruções de execução | Pós-condições | Critério de sucesso | Ferramentas | Uso do VHS |
|---|---|---|---|---|---|---|---|---|---|---|
| TI-01 | Frontend Mobile → Backend | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Não se aplica |
| TI-02 | Backend → Banco de Dados | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Não se aplica |
| TI-03 | Backend → Serviço de Recomendação | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Não se aplica |
| TI-04 | Backend → API externa | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Sim |
| TI-05 | Backend → Ticketeira externa | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | A preencher | Sim |

**Método de validação dos resultados:**  
A preencher pelo responsável, indicando como os resultados serão verificados, como status code, logs, resposta JSON, registros no banco ou comparação com respostas cacheadas pelo VHS.

**Discussão de abrangência:**  
A preencher pelo responsável, explicando quais integrações críticas foram priorizadas e como o VHS contribui para estabilidade e reprodutibilidade dos testes.

---

### 17.8 Discussão geral de abrangência dos testes

A preencher após a conclusão das subseções anteriores.

Nesta seção, o grupo deve discutir a cobertura geral do planejamento, destacando quais dimensões do sistema foram contempladas, quais riscos permanecem e quais seriam os próximos passos para execução dos testes.

----

## 18. Desenvolvimento Do Projeto  — Sprint 3

Esta seção documenta o progresso incremental da solução de software durante a Sprint 3. O objetivo é evidenciar a evolução do backend, frontend, integrações, padrões de desenvolvimento, testes unitários, metodologia e comprovações de execução da aplicação.

A documentação deve demonstrar que a implementação está alinhada ao planejamento da sprint, à arquitetura definida e aos padrões técnicos do projeto.

---

### 18.1 Objetivo do artefato

Descrever brevemente como esta seção evidencia o progresso incremental da solução, contemplando implementação de backend e frontend, adoção de padrões de codificação, arquitetura definida, testes unitários e documentação das evidências de execução.

---

### 18.2 Arquitetura de codificação e estrutura de diretórios

Nesta seção, deve ser apresentada a organização geral do projeto, separando backend, frontend, documentação, testes, configurações e arquivos auxiliares.

#### Estrutura geral do repositório

```text
A preencher com a estrutura atual do projeto.
```

### 18.3 Desenvolvimento do Backend

Esta seção deve documentar o progresso da implementação do backend durante a Sprint 3, incluindo arquitetura adotada, features desenvolvidas, rotas criadas, integração com banco de dados, autenticação externa e documentação Swagger.

#### 18.3.1 Arquitetura do backend

Descrever a arquitetura adotada no backend, indicando camadas, serviços, controllers, models, repositories, integração com banco de dados, autenticação e demais responsabilidades.

```text
A preencher com a estrutura de pastas do backend.
```

---

### 18.4 Desenvolvimento do Frontend

Esta seção deve documentar o progresso da implementação do frontend durante a Sprint 3, incluindo arquitetura adotada, telas desenvolvidas, integração com backend ou dados mockados e evidências visuais da aplicação.

#### 18.4.1 Arquitetura do frontend

Descrever a arquitetura adotada no frontend, indicando organização de telas, componentes, navegação, serviços, estados, hooks, ViewModels ou repositories, conforme o padrão definido pelo grupo.

```text
A preencher com a estrutura de pastas do frontend.
```

#### 18.4.2 Telas implementadas

---

### 18.5 Padrões de codificação e padrões de projeto

Nesta seção, devem ser documentados os padrões adotados pelo grupo no desenvolvimento da Sprint 3, considerando organização de código, nomenclatura, commits, tratamento de erros, separação de responsabilidades e padrões de projeto.

#### Padrões de codificação

A preencher com os padrões definidos no `GuiaPadroesDesenvolvimento.md`, como nomenclatura, organização de arquivos, padrão de commits, tratamento de erros, formatação, separação de responsabilidades e boas práticas de código.

#### Padrões de projeto

| Tipo de padrão | Nome do padrão | Onde aparece no projeto | Justificativa |
|---|---|---|---|
| Criacional | A preencher | A preencher | A preencher |
| Comportamental | A preencher | A preencher | A preencher |

---

### 18.6 Testes unitários e metodologia TDD

Esta seção deve documentar a estratégia de testes unitários adotada no projeto, com foco nas funcionalidades implementadas e na aplicação da metodologia TDD.

#### Estratégia de testes

A preencher explicando como o grupo aplicou TDD, quais camadas foram priorizadas, quais ferramentas foram utilizadas e como os testes se conectam às funcionalidades desenvolvidas.

#### Testes implementados

| Teste | Camada testada | Funcionalidade | Resultado | Evidência |
|---|---|---|---|---|
| A preencher | Service | A preencher | A preencher | A preencher |
| A preencher | Service | A preencher | A preencher | A preencher |
| A preencher | Service | A preencher | A preencher | A preencher |

#### Execução dos testes

```bash
A preencher com o comando utilizado para executar os testes.
```

#### Resultado da cobertura

A preencher com prints, logs ou resumo da cobertura obtida.

---

#### Link do vídeo demonstrativo

A preencher com o link do vídeo demonstrando a aplicação em execução, preferencialmente com tela dividida entre aplicação e console/VSCode.

---


## 19. Termos e Abreviações

| Termo / Abreviação | Definição |
|---|---|
| Gateway (BPMN) | Elemento de controle de fluxo em diagramas BPMN que determina a divisão ou convergência de caminhos no processo. |
| Gateway de Pagamento | Serviço que processa transações financeiras eletrônicas entre o cliente e o estabelecimento. |
| Hash | Função criptográfica que converte dados de entrada em uma sequência fixa e irreversível, utilizada para armazenamento seguro de senhas. |
| HTTPS | HyperText Transfer Protocol Secure — protocolo de comunicação seguro na web, utilizando criptografia TLS. |
| IEEE | Institute of Electrical and Electronics Engineers — organização internacional que define padrões técnicos em engenharia, incluindo o SWEBOK. |
| ISO 25010 | Norma internacional que define o modelo de qualidade de software, classificando características como desempenho, segurança, usabilidade e confiabilidade. |
| Lei Rouanet | Lei nº 8.313/1991 que institui o Programa Nacional de Apoio à Cultura (PRONAC), permitindo incentivos fiscais para projetos culturais no Brasil. |
| LGPD | Lei Geral de Proteção de Dados Pessoais (Lei nº 13.709/2018) — legislação brasileira que regula o tratamento de dados pessoais. |
| MVP | Minimum Viable Product (Produto Mínimo Viável) — versão inicial de um produto contendo as funcionalidades essenciais para validar a proposta de valor. |
| Pool (BPMN) | Elemento gráfico no BPMN que representa um participante ou organização dentro de um processo de negócio. |
| Prateleira | Componente visual da interface que agrupa um conjunto temático de espetáculos exibido em formato de carrossel. |
| SWEBOK | Software Engineering Body of Knowledge — guia internacional do IEEE que consolida o conhecimento da área de engenharia de software. |
| TLS | Transport Layer Security — protocolo criptográfico que garante a segurança das comunicações em redes de computadores. |
| UI | User Interface (Interface do Usuário) — conjunto de elementos visuais e interativos com os quais o usuário interage. |
| UNESCO | United Nations Educational, Scientific and Cultural Organization — Organização das Nações Unidas para a Educação, a Ciência e a Cultura. |
| Uptime | Período em que o sistema permanece disponível e operacional, geralmente expresso em percentual. |
| Vitrine Dinâmica | Componente de interface que exibe conteúdos personalizados de forma adaptativa, baseando-se no perfil e comportamento do usuário. |
| Wishlist | Lista de desejos — conjunto de espetáculos que o cliente deseja assistir e salva para consulta futura. |
| MVVM | Model-View-ViewModel — padrão arquitetural utilizado para separar interface, lógica e dados da aplicação. |
| Include | Relação UML utilizada quando um caso de uso obrigatoriamente incorpora outro comportamento. |
| Extend | Relação UML utilizada quando um caso de uso adiciona comportamentos opcionais a outro. |


## Referências

ASSOCIAÇÃO DOS PRODUTORES DE ESPETÁCULOS TEATRAIS DO ESTADO DE SÃO PAULO (APETESP). Wikipédia, a enciclopédia livre. Disponível em: https://pt.wikipedia.org/wiki/Associa%C3%A7%C3%A3o_dos_Produtores_de_Espet%C3%A1culos_Teatrais_do_Estado_de_S%C3%A3o_Paulo. Acesso em: 23 abr. 2026.

ASSOCIAÇÃO DOS PRODUTORES DE TEATRO DO RIO DE JANEIRO (APTR). Sites Nacionais de Associações e Entidades. Centro Brasileiro Teatro para a Infância e Juventude. Disponível em: https://cbtij.org.br/associacoes-e-entidades-nacionais/. Acesso em: 23 abr. 2026.

PRADO, Miguel Arcanjo. APTI, JLeiva e iBT fazem mapeamento do teatro em pesquisa inédita. Blog do Arcanjo, 12 mar. 2025. Disponível em: https://www.blogdoarcanjo.com/2025/03/12/apti-jleiva-e-ibt-fazem-mapeamento-do-teatro-em-pesquisa-inedita/. Acesso em: 23 abr. 2026.

AGÊNCIA BRASIL. Projetos da Lei Rouanet têm novos limites; ingressos custarão R$ 150 em média. Brasília, 21 mar. 2017. Disponível em: https://agenciabrasil.ebc.com.br/cultura/noticia/2017-03/projetos-da-lei-rouanet-passam-ter-limite-ingressos-custarao-r-150-em-media. Acesso em: 23 abr. 2026.

INCENTIV. Produção teatral e leis de incentivo: entenda o impacto social positivo. Disponível em: https://incentiv.me/blog/2024/03/28/leis-de-incentivo-na-producao-teatral/. Acesso em: 23 abr. 2026.

NOTICIAS DO BRASIL. Governo libera R$ 7,1 milhões via Lei Rouanet para musical inédito sobre Ziraldo. 24 fev. 2026. Disponível em: https://noticias-do-brasil.news/cultura/cultura-eventos/lei-rouanet-libera-r-71-mi-para-musical-sobre-ziraldo.html. Acesso em: 23 abr. 2026.

PORTAL EXIBIDOR. Balanço 2024: cinemas superam expectativas e público aumenta 6,5%; filmes brasileiros crescem quase 300%. Disponível em: https://www.exibidor.com.br/noticias/mercado/14480-balanco-2024-cinemas-superam-expectativas-e-publico-aumenta-65-filmes-brasileiros-crescem-quase-300. Acesso em: 23 abr. 2026.

COREDAÇÃO. O desafio de reduzir a desigualdade no acesso ao teatro no Brasil. Disponível em: https://coredacao.com/temas-de-redacao/o-desafio-de-reduzir-a-desigualdade-no-acesso-ao-teatro-no-brasil/. Acesso em: 23 abr. 2026.

ANAFIMA. *PIB da Música no Brasil*. Valor Cultural, 2024. Disponível em: https://valorcultural.com.br/mercado-da-musica-fatura-r-116-bilhoes-mas-carece-de-valorizacao-institucional/. Acesso em: 23 abr. 2026.

ESPM. Teatro Musical no Rio de Janeiro: Uma pesquisa com consumidores cariocas. *Diálogo com a Economia Criativa*, 2018. Disponível em: https://dialogo.espm.br/revistadcec-rj/article/view/155. Acesso em: 23 abr. 2026.

INFOTEATRO. O Impacto do Teatro Musical no Cenário Brasileiro. 2025. Disponível em: https://infoteatro.com.br/o-impacto-do-teatro-musical-no-cenario-brasileiro/. Acesso em: 23 abr. 2026.

INFOTEATRO. Afinal, o que é a Lei Rouanet e como funciona? 2025. Disponível em: https://infoteatro.com.br/afinal-o-que-e-a-lei-rouanet-e-como-funciona/. Acesso em: 23 abr. 2026.

INTELI; AVENTURA. TAPI — ESMD6: Plataforma digital baseada em arquitetura orientada a serviços. 2024.

PRICEWATERHOUSECOOPERS (PwC). Global Entertainment & Media Outlook 2023–2027. Disponível em: https://www.pwc.com/outlook. Acesso em: 26 abr. 2026.

UNESCO. Cultural Times: The First Global Map of Cultural and Creative Industries. Paris: UNESCO, 2015. Disponível em: https://unesdoc.unesco.org. Acesso em: 26 abr. 2026.

INSTITUTO BRASILEIRO DE GEOGRAFIA E ESTATÍSTICA (IBGE). Pesquisa de Orçamentos Familiares 2017–2018: análise do consumo cultural no Brasil. Rio de Janeiro: IBGE, 2020. Disponível em: https://www.ibge.gov.br. Acesso em: 26 abr. 2026.
Lei Rouanet.

BRASIL. Lei nº 8.313, de 23 de dezembro de 1991. Institui o Programa Nacional de Apoio à Cultura (PRONAC). Diário Oficial da União: seção 1, Brasília, DF, 24 dez. 1991. Disponível em: http://www.planalto.gov.br. Acesso em: 26 abr. 2026.

ASSOCIAÇÃO PAULISTA DOS PRODUTORES DE TEATRO (APPT). Dados e relatórios sobre produção teatral em São Paulo. Disponível em: https://www.appt.org.br. Acesso em: 26 abr. 2026.
Aventura Teatros.

AVENTURA TEATROS. Site oficial. Disponível em: https://aventurateatros.com.br. Acesso em: 26 abr. 2026.