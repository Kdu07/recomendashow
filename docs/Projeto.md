# Projeto - Sistema de Recomendação - Aventura

## Sumário

- [1. Problema](#1-problema)
- [2. Contexto da Indústria do Parceiro](#2-contexto-da-indústria-do-parceiro)
- [3. Visão e Objetivos do Produto](#3-visão-e-objetivos-do-produto)
- [4. O que o Produto Faz e o que Não Faz](#4-o-que-o-produto-faz-e-o-que-não-faz)
- [5. Personas e Jornada do Usuário](#5-personas-e-jornada-do-usuário)
- [6. Fluxo do Negócio](#6-fluxo-do-negócio)
- [7. Brainstorming de Features](#7-brainstorming-de-features)
- [8. Business Model Canvas do MVP](#8-business-model-canvas-do-mvp)
- [9. Requisitos Funcionais](#9-requisitos-funcionais)
- [10. Requisitos Não Funcionais](#10-requisitos-não-funcionais)
- [11. Correlação entre RFs e RNFs](#11-correlação-entre-rfs-e-rnfs)
- [12. Termos e Abreviações](#12-termos-e-abreviações)
- [Referências](#referências)

---

## 1. Problema

> Descreva aqui o problema a ser enfrentado, evidenciando sua relevância no contexto do parceiro e os principais desafios e oportunidades associados.

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

> Apresente a visão do produto e seus objetivos, alinhados ao problema identificado, às necessidades do negócio e ao valor esperado para os usuários.

---

## 4. O que o Produto Faz e o que Não Faz

> Delimite o escopo da solução proposta, deixando claro o que está contemplado neste momento e o que está fora do foco do produto.

### 4.1 O que o produto faz

### 4.2 O que o produto não faz

---

## 5. Personas e Jornada do Usuário

> Caracterize os principais perfis de usuários e represente sua jornada, destacando necessidades, dores, expectativas e interações relevantes com a solução.

### 5.1 Personas

### 5.2 Jornada do usuário

---

## 6. Fluxo do Negócio

> Modele a cadeia de valor dos processos de negócio e o fluxo principal relacionado ao contexto do projeto, com uso de notação BPMN.

---

## 7. Brainstorming de Features

> Registre todas as ideias geradas durante o workshop, incluindo as features levantadas pelo grupo.

---

## 8. Business Model Canvas do MVP

> Consolide a visão final proposta pelo grupo para o projeto, evidenciando a proposta de valor, os segmentos envolvidos, a solução definida e os principais elementos que justificam o recorte inicial do MVP.

| Bloco | Descrição |
|---|---|
| Proposta de valor | |
| Segmentos de clientes | |
| Canais | |
| Relacionamento com clientes | |
| Fontes de receita | |
| Recursos principais | |
| Atividades principais | |
| Parcerias principais | |
| Estrutura de custos | |

---

## 9. Requisitos Funcionais

> Especificação dos requisitos funcionais em formato de user story: *"Como \<perfil\>, eu quero \<realizar uma ação\>"*.

### RF01 — \<Nome do requisito\>

**User story:** Como , eu quero .

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | |
| Procedimento de teste | |
| Resultado esperado | |
| Pós-condição | |

---

### RF02 — \<Nome do requisito\>

**User story:** Como , eu quero .

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | |
| Procedimento de teste | |
| Resultado esperado | |
| Pós-condição | |

---

## 10. Requisitos Não Funcionais

> Especificação dos requisitos não funcionais com base nas características de qualidade da **ISO 25010**, em formato de user story.

### RNF01 — \<Nome do requisito\>

**Característica ISO 25010:** \<ex: Desempenho, Usabilidade, Segurança...\>

**User story:** Como , eu quero .

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | |
| Procedimento de teste | |
| Resultado esperado | |
| Pós-condição | |

---

### RNF02 — \<Nome do requisito\>

**Característica ISO 25010:** \<ex: Desempenho, Usabilidade, Segurança...\>

**User story:** Como , eu quero .

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | |
| Procedimento de teste | |
| Resultado esperado | |
| Pós-condição | |

---

## 11. Correlação entre RFs e RNFs

> Tabela explicitando as relações diretas entre requisitos não funcionais e requisitos funcionais.

| RNF | RF relacionados | Justificativa |
|---|---|---|
| RNF01 | RF01, RF02 | |
| RNF02 | RF01 | |

---

## 12. Termos e Abreviações

> Glossário de termos e abreviações utilizados neste documento.

| Termo / Abreviação | Definição |
|---|---|
| RF | Requisito Funcional |
| RNF | Requisito Não Funcional |
| MVP | Minimum Viable Product (Produto Mínimo Viável) |
| BPMN | Business Process Model and Notation |

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