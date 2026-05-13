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
- [12. Diagramação do banco](#12-diagramação-do-banco)
- [14. Diagrama de Componentes](#14-diagrama-de-componentes)
- [15. Termos e Abreviações](#15-termos-e-abreviações)
- [Referências](#referências)

---

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

**Característica ISO 25010:** Eficiência de Desempenho

**User story:** Como usuário, quero que as recomendações personalizadas e os carrosséis da home carreguem em até 3 segundos, para que eu possa explorar o catálogo sem frustração e manter o interesse na navegação.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuário autenticado com perfil de onboarding preenchido e wishlist contendo ao menos 1 item e conexão estável de no mínimo 10 Mbps. |
| Procedimento de teste | Acessar a Home do Portal e medir, o tempo entre o carregamento da página e a renderização completa das prateleiras de recomendação. Repetir 5 vezes em horários distintos. |
| Resultado esperado | O tempo médio de carregamento das recomendações deve ser ≤ 3 segundos em pelo menos 4 das 5 execuções. |
| Pós-condição | Carrosséis renderizados com itens personalizados e logs de performance armazenados para análise. |

---

### RNF02 — Segurança das credenciais de usuário

**Característica ISO 25010:** Segurança

**User story:** Como usuário cadastrado, quero que minha senha e dados pessoais (nome, idade, gênero, CEP) sejam armazenados e transmitidos de forma criptografada, para que minhas informações não sejam expostas em caso de tentativa de invasão ou interceptação.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuário cadastrado com e-mail e senha válidos e banco de dados de teste populado. |
| Procedimento de teste | Inspecionar o tráfego de rede no momento do login com ferramenta como DevTools, verificando uso de HTTPS/TLS. Consultar diretamente o banco de dados e verificar se a senha está armazenada com hash e não em texto plano. |
| Resultado esperado | Todo tráfego ocorre via HTTPS, nenhuma senha é armazenada em texto plano no banco e o hash não é reversível. |
| Pós-condição | Sessão autenticada com token seguro e nenhum dado sensível visível em logs ou no payload em texto plano. |

---

### RNF03 — Conformidade com a LGPD

**Característica ISO 25010:** Segurança / Conformidade

**User story:** Como usuário, quero poder consultar, editar e excluir meus dados pessoais e preferências a qualquer momento, para que a plataforma respeite meus direitos garantidos pela LGPD e eu mantenha controle sobre minhas informações.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuário autenticado com dados de perfil preenchidos no onboarding (idade, gênero, CEP, preferências). |
| Procedimento de teste | Acessar a área de Perfil → tentar editar cada campo de dado pessoal e preferências. Em seguida, solicitar exclusão da conta e verificar se os dados são efetivamente removidos ou anonimizados na base. |
| Resultado esperado | O usuário consegue visualizar, editar e excluir seus dados e após a exclusão, os registros pessoais não estão mais associados ao identificador do usuário na base. |
| Pós-condição | Processo de exclusão/edição concluído com sucesso. |

---

### RNF04 — Usabilidade da interface em dispositivos móveis

**Característica ISO 25010:** Usabilidade

**User story:** Como espectador que acessa o portal pelo celular, quero que toda a navegação (home, busca, wishlist, perfil e fluxo de onboarding) seja totalmente responsiva e intuitiva em telas a partir de 480px, para que eu possa descobrir espetáculos e gerenciar minhas preferências em qualquer dispositivo.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Aplicação publicada em ambiente de homologação, dispositivos disponíveis com larguras de 480px e 720px. |
| Procedimento de teste | Realizar teste de usabilidade com 5 usuários, solicitando: completar o onboarding, buscar um espetáculo, adicionar à wishlist, acessar perfil e seguir para a bilheteria. Medir taxa de conclusão e SUS (System Usability Scale). |
| Resultado esperado | Taxa de conclusão ≥ 90% das tarefas sem ajuda externa; pontuação SUS ≥ 70; nenhum elemento cortado ou inacessível nas resoluções testadas. |
| Pós-condição | Relatório de usabilidade documentado; eventuais ajustes de UI registrados como backlog. |

---

### RNF05 — Disponibilidade da plataforma

**Característica ISO 25010:** Confiabilidade

**User story:** Como usuário do Portal Aventura, quero que a plataforma esteja disponível 99% do tempo no horário comercial e em horários de pico, para que eu possa consultar a programação e ser direcionado à bilheteria sem indisponibilidades.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Plataforma publicada em ambiente de produção com monitoramento ativo. |
| Procedimento de teste | Monitorar a disponibilidade do portal por um período contínuo de 10 dias, registrando todas as interrupções e suas durações. |
| Resultado esperado | Disponibilidade mensal ≥ 99%. |
| Pós-condição | Relatório de uptime gerado e incidentes registrados com causa raiz. |

---

### RNF06 — Escalabilidade do sistema de recomendação

**Característica ISO 25010:** Escalabilidade

**User story:** Como gestor da Aventura, quero que a plataforma suporte ao menos 1.000 usuários simultâneos sem degradação perceptível das recomendações, para que campanhas de lançamento de espetáculos não comprometam a experiência dos espectadores.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Ambiente de homologação configurado com infraestrutura equivalente à produção. |
| Procedimento de teste | Executar teste de carga simulando 1.000 usuários concorrentes navegando pela home, acessando carrosséis de recomendação e adicionando itens à wishlist por 15 minutos. |
| Resultado esperado | Tempo médio de resposta ≤ 3s, taxa de erro < 1% e nenhuma queda do serviço durante o teste. |
| Pós-condição | Métricas de CPU, memória e latência registradas e eventuais gargalos documentados. |

---

### RNF07 — Precisão da geolocalização

**Característica ISO 25010:** Adequação Funcional

**User story:** Como usuário localizado no Rio de Janeiro ou em São Paulo, quero que o sistema identifique corretamente minha cidade a partir do CEP ou da geolocalização do dispositivo, para que as recomendações priorizem espetáculos da minha região com precisão.

**Testes de usuário:**

| Campo | Descrição |
|---|---|
| Pré-condição | Usuários de teste cadastrados com CEPs reais de SP e RJ, permissão de geolocalização concedida em parte dos casos. |
| Procedimento de teste | Acessar a home com 10 perfis distintos (5 SP e 5 RJ), verificar se as recomendações da seção regional priorizam espetáculos da cidade correta. Repetir o teste com geolocalização do navegador ativada. |
| Resultado esperado | A cidade correta é identificada em ≥ 95% dos casos e os espetáculos da região correspondente aparecem nas primeiras posições. |
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

### 12.1 - Diagrama conceitual


### 12.2 Diagrama Lógico

<div align="center">
<sub>Imagem 09 - Diagrama lógico</sub><br>
  <img src="../assets/diagrama-logico.png" width="100%" alt="Diagrama logico do banco"><br>
  <sup>Source: Material produced by the authors, 2026.</sup>
</div>



O modelo lógico de dados representa a estruturação dos dados seguindo as regras do modelo relacional, servindo como uma ponte entre a abstração do modelo conceitual e a implementação física no SGDB .

---

### 12.2.1 Dicionário de Dados e Mapeamento

A conversão do modelo conceitual para o modelo lógico foi realizada mapeando cada entidade conceitual para uma tabela relacional e transformando relacionamentos Muitos-para-Muitos (N:N) em tabelas associativas.

---

### A. Tabelas de Cadastro (Entidades Fortes)

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **USUARIOS** | `id_usuario` | INT | PK, AI | Identificador único do usuário. |
| | `nome` | VARCHAR(100) | NN | Nome completo do usuário. |
| | `email` | VARCHAR(100) | UNIQUE, NN | E-mail utilizado para autenticação. |
| | `senha` | VARCHAR(255) | NN | Hash da senha de acesso. |
| | `idade` | INT | - | Idade do usuário. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **GENEROS** | `id_genero` | INT | PK, AI | Identificador único do gênero. |
| | `nome` | VARCHAR(50) | NN | Nome do gênero artístico. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **TEATROS** | `id_teatro` | INT | PK, AI | Identificador único do teatro. |
| | `nome` | VARCHAR(100) | NN | Nome do teatro ou espaço cultural. |
| | `cidade` | VARCHAR(100) | NN | Cidade onde o teatro está localizado. |
| | `estado` | VARCHAR(50) | NN | Estado onde o teatro está localizado. |
| | `rua` | VARCHAR(150) | NN | Nome da rua do teatro. |
| | `numero` | VARCHAR(20) | NN | Número do endereço do teatro. |
| | `horario_func` | VARCHAR(100) | - | Horário de funcionamento do teatro. |
| | `descricao` | TEXT | - | Descrição geral do teatro. |
| | `imagem` | VARCHAR(255) | - | URL ou caminho da imagem do teatro. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **ESPETACULOS** | `id_espetaculo` | INT | PK, AI | Identificador único do espetáculo. |
| | `id_teatro` | INT | FK, NN | Teatro onde o espetáculo é apresentado. |
| | `nome` | VARCHAR(150) | NN | Nome do espetáculo. |
| | `link_ingresso` | VARCHAR(255) | - | Link para compra de ingressos. |
| | `descricao` | TEXT | - | Descrição detalhada do espetáculo. |
| | `imagem` | VARCHAR(255) | - | URL ou caminho da imagem do espetáculo. |

---

### B. Tabelas Associativas e Operacionais

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **ESP_GEN** | `id_espetaculo` | INT | FK, PK(1/2) | Espetáculo relacionado ao gênero. |
| | `id_genero` | INT | FK, PK(2/2) | Gênero associado ao espetáculo. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **GEN_FAV** | `id_usuario` | INT | FK, PK(1/2) | Usuário que favoritou o gênero. |
| | `id_genero` | INT | FK, PK(2/2) | Gênero marcado como preferência pelo usuário. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **CURT_WISH** | `id_usuario` | INT | FK, PK(1/2) | Usuário que realizou a interação. |
| | `id_espetaculo` | INT | FK, PK(2/2) | Espetáculo alvo da interação. |
| | `tipo` | ENUM('curtida', 'wishlist') | NN | Tipo de interação realizada pelo usuário. |

---

| Tabela | Coluna | Tipo | Restrição | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **INTERAÇÕES_USU** | `id_evento` | INT | PK, AI | Identificador único do evento registrado. |
| | `id_usuario` | INT | FK, NN | Usuário que realizou a interação. |
| | `id_espetaculo` | INT | FK, NN | Espetáculo relacionado à interação. |
| | `tipo` | VARCHAR(50) | NN | Tipo de evento/interação realizada. |
| | `tempo_pagina` | INT | - | Tempo de permanência do usuário na página do espetáculo em segundos. |

---

### 12.2.2 Definição de Chaves e Integridade Referencial

### Chaves Primárias (PK)

As chaves primárias foram definidas para garantir a unicidade de cada registro dentro das tabelas.

### Chaves Estrangeiras (FK)

As chaves estrangeiras garantem a integridade referencial entre as entidades relacionadas, impedindo registros órfãos e assegurando consistência entre os dados.

### Relacionamentos implementados

- Um usuário pode possuir vários registros de eventos.
- Um usuário pode favoritar múltiplos gêneros.
- Um usuário pode interagir com vários espetáculos.
- Um espetáculo pertence a um teatro.
- Um espetáculo pode possuir múltiplos gêneros.
- Um gênero pode estar associado a múltiplos espetáculos.

---

### 12.2.3 Regras de Normalização Aplicadas

O modelo lógico segue os princípios da Terceira Forma Normal (3FN).

### Primeira Forma Normal (1FN)

Todos os atributos armazenam valores atômicos, sem grupos repetitivos ou listas em uma mesma coluna.

### Segunda Forma Normal (2FN)

As tabelas associativas utilizam chaves compostas, garantindo que todos os atributos dependam integralmente da chave primária completa.

### Terceira Forma Normal (3FN)

Os atributos não-chave dependem exclusivamente da chave primária, eliminando dependências transitivas e reduzindo redundâncias no banco de dados.

---

### 12.2.4 Justificativa e Rastreabilidade

O modelo lógico foi estruturado para manter fidelidade ao modelo conceitual, garantindo consistência entre as regras de negócio e a implementação relacional.

As tabelas associativas permitem representar corretamente relacionamentos Muitos-para-Muitos, como:

- preferências de gêneros dos usuários;
- categorização de espetáculos;
- interações entre usuários e espetáculos.

A separação das entidades reduz redundâncias, melhora a escalabilidade do sistema e facilita futuras manutenções e expansões do banco de dados.

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

## 15. Termos e Abreviações

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