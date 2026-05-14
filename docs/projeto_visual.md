# Projeto Visual (Mobile First)

---

## 1. Introdução

### 1.1 Objetivo desta documentação
Este documento registra as decisões visuais e de experiência do usuário do protótipo mobile, servindo como referência para stakeholders (comunicando a abordagem visual), guia de implementação para desenvolvedores (garantindo consistência), instrumento de validação (avaliando alinhamento com objetivos de negócio e experiência) e base para futuras iterações que mantenham a estratégia visual e comportamental desejada.

### 1.2 Contexto da solução

O Portal Aventura é uma plataforma de descoberta de espetáculos culturais que atualmente funciona como um catálogo estático. A solução proposta transforma isso em um hub dinâmico com recomendações personalizadas. O mobile é um ponto de entrada complementar  utilizado por usuários em momentos de navegação e consultas rápidas (5-15 min), exigindo experiência que priorize rapidez, clareza e simplicidade.

### 1.3 Objetivo da experiência Mobile First
A experiência mobile foi concebida com três objetivos: (1) descoberta rápida e intuitiva em poucos toques, com recomendação como centro da experiência; (2) leitura confortável em movimento com hierarquia clara, textos diretos, imagens de impacto e tamanhos de toque adequados para uso com uma mão; (3) conversão acelerada para venda removendo barreiras entre descoberta e compra através de botão sempre visível, wishlist persistente, redirecionamento direto para vendas e micro-animações confirmando ações.

### 1.4 Escopo do protótipo visual
O protótipo abrange 11 telas em nível mockup navegável, cobrindo todos os fluxos críticos: autenticação (Splash, Login, Cadastro), onboarding do usuário (Onboarding, Boas-Vindas), descoberta de conteúdo (Home, Buscar, Detalhes do Espetáculo), gestão de preferências (Wishlist, Perfil, Editar Interesses). As interações são plenamente navegáveis, permitindo validar tanto a jornada do novo usuário quanto os fluxos de exploração e conversão para compra. 

---

## 2. Abordagem Mobile First
A abordagem Mobile First consiste em iniciar o desenvolvimento pensando primeiro nos dispositivos de telas menores, como smartphones. Ele segue o conceito de Progressive Enhancement (melhoramento progressivo), que defende a criação inicial voltada para dispositivos mais simples, priorizando as funcionalidades essenciais e a boa experiência do usuário.

A partir disso, conforme o aumento do tamanho da tela e da capacidade de processamento do dispositivo, novas funcionalidades e melhorias visuais podem ser adicionadas gradualmente. Dessa forma, o sistema cresce de maneira progressiva sem comprometer a usabilidade nos dispositivos móveis.


### 2.1 Priorização da experiência mobile
O wireframe foi desenvolvido inicialmente para dispositivos móveis, priorizando as funcionalidades essenciais e a navegação rápida em telas pequenas. As principais ações permanecem visíveis sem excesso de rolagem, reduzindo etapas e facilitando o uso com apenas uma mão.

Os elementos mais importantes foram posicionados em áreas de maior destaque e fácil acesso, melhorando a usabilidade e a eficiência da navegação.

### 2.2 Adaptação para diferentes tamanhos de tela
O layout foi projetado de forma responsiva para funcionar corretamente em resoluções como 360×640, 390×844 e 414×896.

Foram aplicadas regras como:

- Organização vertical do conteúdo;
- Componentes flexíveis e redimensionáveis;
- Ajuste automático de espaçamentos e elementos;
- Redução de colunas em telas menores;
- Priorização das informações principais nas  primeiras áreas visíveis da tela.

Essas adaptações garantem boa visualização e usabilidade em diferentes dispositivos móveis.

### 2.3 Legibilidade
A hierarquia tipográfica foi definida para facilitar a leitura em dispositivos móveis e ambientes com baixa luminosidade. Títulos possuem maior destaque visual, enquanto textos secundários mantêm tamanhos adequados para leitura confortável.

Também foram mantidas regras de:

- Bom contraste entre texto e fundo;
- Espaçamento adequado entre elementos;
- Redução de blocos extensos de texto;
- Organização clara entre títulos e conteúdos.

### 2.4 Organização de conteúdo em telas pequenas
O conteúdo foi organizado em blocos verticais e hierárquicos para reduzir a carga cognitiva do usuário.

A interface prioriza:

- Separação clara das informações;
- Destaque para ações principais;
- Exibição gradual do conteúdo;
- Layout limpo e com pouca poluição visual.

Isso torna a navegação mais intuitiva e confortável em telas pequenas.

---

## 3. Estrutura de Navegação

A navegação do Portal Aventura foi desenhada para sustentar o pilar estratégico do produto — descoberta contínua e baixo atrito até a conversão — adotando um modelo *mobile-first* baseado em barra de abas inferior e hierarquia plana entre as áreas principais. Telas de configuração e detalhe operam em uma camada secundária, acessadas por *drill-down*, sem interromper o contexto da área raiz.

### 3.1 Fluxo principal do usuário

O fluxo é segmentado em três estados de jornada:

**Primeiro acesso (não autenticado):**
Splash → Login → (Cadastro, se novo usuário) → Onboarding de Interesses (gêneros → cidade → frequência) → Bem-vinda → Explorar.

O caminho alternativo do "Pular" no onboarding leva diretamente a Bem-vinda → Explorar, com a seção "Para você" exibindo *empty state* até que o usuário visite Editar Interesses pelo Perfil.

**Uso recorrente (autenticado):**
Splash → Explorar. A partir desse ponto, o usuário transita livremente entre as quatro áreas principais (Explorar, Buscar, Wishlist, Perfil) via barra de abas inferior, sem hierarquia entre elas.

**Fluxo de conversão (objetivo de negócio):**
Qualquer área principal → *card* de espetáculo → Informações do Espetáculo → CTA "Ingressos" → *hand-off* para parceiro de bilheteria (saída do app). Em paralelo, o CTA "Salvar" alimenta a Wishlist, retendo o usuário no ecossistema para uma decisão futura.

### 3.2 Hierarquia entre telas

A arquitetura de informação se organiza em três níveis:

**Nível 0 — Telas de entrada (fora da navegação principal):**
Splash, Login, Cadastro, Onboarding (3 passos), Bem-vinda. São transicionais, não fazem parte do uso recorrente e não exibem a *tab bar*. Operam em um fluxo linear com saída única para a área principal.

**Nível 1 — Áreas principais (raízes da navegação):**
Explorar, Buscar, Wishlist e Perfil. Todas exibem a *tab bar* inferior e funcionam como pontos de partida para *drill-down*. São consideradas pares — nenhuma é "filha" de outra.

**Nível 2 — Telas de detalhe e configuração (sub-telas):**
- A partir de qualquer área principal: **Informações do Espetáculo** (detalhe de conteúdo, ponto de conversão).
- A partir do Perfil: **Editar Interesses**, **Notificações** e **Informações Institucionais** (telas de configuração).
- A partir da Buscar: o **modal de Filtros** (overlay, não consome rota).

Telas de nível 2 herdam o contexto da raiz pela qual foram acessadas e devolvem o usuário a esse mesmo ponto ao retornar.

### 3.3 Navegação principal

O padrão predominante é a **barra de abas inferior fixa** (*bottom tab bar*) com quatro itens, na ordem:

1. **Explorar** — ícone de casa (raiz da descoberta).
2. **Buscar** — ícone de lupa (descoberta ativa).
3. **Wishlist** — ícone de coração (retenção).
4. **Perfil** — ícone de pessoa (conta e configurações).

A *tab bar* aparece em todas as telas de nível 1 e é ocultada nas telas de nível 0 (entrada/onboarding) e em telas de nível 2 de configuração (Notificações, Informações Institucionais, Editar Interesses) — nestas, a navegação se dá pelo botão "voltar" no header. Em Informações do Espetáculo, a *tab bar* pode ser substituída pela barra de ação fixa ("Salvar" + "Ingressos"), priorizando a conversão sobre a troca de contexto.

Acima da *tab bar*, cada tela usa um header próprio com regras descritas em 3.4. Filtros na Buscar abrem como *bottom sheet* sobre a tela atual, sem alterar a rota — preservando o input e o contexto da busca.

### 3.4 Comportamento de menus e navegação inferior/superior

**Botão "voltar" (header):**
- Disponível em todas as telas de nível 2 (Informações do Espetáculo, Editar Interesses, Notificações, Informações Institucionais).
- Retorna o usuário à tela imediatamente anterior na pilha de navegação, preservando *scroll position* e estado de filtros quando aplicável.
- Não aparece nas áreas principais (nível 1) — a navegação entre elas se dá exclusivamente pela *tab bar*.
- Nas telas de onboarding, o avanço é unidirecional pelo CTA "Continuar"; o retorno entre passos pode ser oferecido como secundário, mas o "Pular" tem prioridade visual.

**Persistência e ocultação de barras ao scroll:**
- *Tab bar* inferior: **fixa**, sem ocultação ao scroll, garantindo acesso constante às áreas principais.
- Header superior: **fixo** nas áreas principais (logo + ações). Em Informações do Espetáculo, o header pode ter comportamento *sticky* com *parallax* sobre a imagem hero.
- Chips de categoria em Explorar: podem ser *sticky* ao scroll, mantendo o filtro visível durante a varredura do *feed*.
- Barra de ação fixa em Informações do Espetáculo ("Salvar" + "Ingressos"): persistente em todo o scroll, alinhada à heurística de "ação visível em qualquer ponto do scroll".

**Estados ativos e rotas:**
- O ícone da aba ativa é destacado por preenchimento e/ou peso de fonte mais forte no label.
- Tocar em uma aba já ativa **rola a tela ao topo**.
- Cada aba mantém sua própria pilha de navegação independente — sair de Wishlist para Perfil e voltar para Wishlist preserva o estado anterior (incluindo subtelas abertas).

**Títulos e ações do topo:**
- **Áreas principais (Explorar, Wishlist, Perfil):** header com logo "Aventura" à esquerda.
- **Buscar:** header com logo + campo de busca compacto; o campo de busca da própria tela fica logo abaixo, em destaque.
- **Telas de configuração (Notificações, Informações Institucionais, Editar Interesses):** header simplificado com botão "voltar" à esquerda e título da seção em caixa-alta centralizado, sem ações à direita — reduzindo distrações em contexto de leitura/edição.
- **Informações do Espetáculo:** header com botão "voltar" e, opcionalmente, ação de compartilhar à direita.

**Overlays e modais:**
- O modal de Filtros (Buscar) abre como *bottom sheet* com transição *slide-up*, fundo escurecido e botão de fechar (X) no canto superior direito. Os CTAs "Aplicar Filtros" e "Limpar Filtros" são fixos no rodapé do *sheet*.
- Toque fora do *sheet* ou no X o fecha sem aplicar mudanças; "Aplicar" confirma e fecha, retornando à lista de resultados atualizada.

---

# 4. Wireframes

## Introdução

Esta seção documenta os wireframes de média fidelidade do aplicativo móvel do **Portal Aventura**, a camada de interface da plataforma de recomendação cultural desenvolvida para a Aventura Teatros. O escopo dos wireframes traduz para a UI o pilar estratégico do produto: adicionar um *hub* de descoberta personalizada para o espectador, concentrando-se em três jornadas críticas: **onboarding de interesses** (que alimenta o motor de recomendação), **descoberta** (Explorar, Buscar, Wishlist) e **conversão** (página de informações do espetáculo com *hand-off* para a bilheteria parceira).

A fidelidade média foi escolhida deliberadamente: o objetivo é validar hierarquia, fluxo e padrões de interação sem comprometer a discussão com decisões estéticas prematuras. Toda a documentação adota a abordagem *mobile-first*, dado que o aplicativo é o canal prioritário definido no Business Model Canvas do MVP. Componentes recorrentes (header com logo, *tab bar* de navegação inferior, *cards* de espetáculo) são tratados como padrões reutilizáveis, garantindo consistência visual e cognitiva entre as telas.

> **Arquivo Figma:** [WireFrame — Portal Aventura](https://www.figma.com/design/1MhhzP1FCg9BETFkPsi8T3/WireFrame---Portal-aventura-app?node-id=0-1)

> **Nota sobre estados:** algumas telas possuem variantes de estado (vazio, filtros aplicados, fluxo alternativo). Quando aplicável, essas variantes são descritas dentro da subseção da tela-mãe.

## 4.1 Splash / Intro
- **Objetivo da tela:** estabelecer presença de marca e cobrir o tempo de inicialização do app, comunicando posicionamento da Aventura logo no primeiro contato.
- **Descrição funcional:** tela passiva, sem interação. Exibida automaticamente na abertura do app antes do roteamento para Login (não-autenticado) ou Explorar (sessão ativa).
- **Componentes principais:** logotipo "AVENTURA" centralizado, *slogan/tagline* da marca abaixo do logo, lista de categorias culturais separadas por pontos no rodapé (reforço de portfólio).
- **Organização do layout:** alinhamento central vertical e horizontal; fundo escuro para alto contraste com o logo; uso de espaço negativo para destacar a marca.
- **Comportamento mobile:** exibição de 3 segundos; transição por *fade* para a próxima tela; ocupa 100% da viewport (sem *status bar* customizada).
- **Observações de usabilidade:** o rodapé com categorias funciona como *micro-pitch* do que o app oferece, mitigando a expectativa de "tela vazia". 

## 4.2 Login
- **Objetivo da tela:** autenticar usuários recorrentes e devolvê-los ao estado de sessão com preferências e Wishlist sincronizadas (RF03).
- **Descrição funcional:** o usuário insere e-mail e senha e clica em "Entrar"; alternativamente, autentica via Google. Há *atalho* para "Criar conta".
- **Componentes principais:** logo + mensagem de boas-vindas, campo de e-mail, campo de senha (com toggle de visibilidade), botão primário "Entrar", divisor "ou continue com", botão de login social (Google), *links* de cadastro e recuperação no rodapé.
- **Organização do layout:** hierarquia em três blocos verticais: identidade da marca, formulário, opções alternativas. CTA primário tem peso visual diferenciado para guiar o caminho preferencial.
- **Comportamento mobile:** teclado virtual abre ao focar nos campos sem ocultar o botão "Entrar"; validação inline de formato de e-mail; teclado do tipo `email` no campo correspondente.
- **Observações de usabilidade:** posicionar "Criar conta" no rodapé prioriza o caminho de retorno (usuário recorrente). Login social reduz fricção para personas em formação (Ana Luiza), que tendem a abandonar formulários longos.

## 4.3 Cadastro
- **Objetivo da tela:** capturar credenciais e dados mínimos para criar conta, habilitando a persistência de preferências do onboarding (RF03).
- **Descrição funcional:** o usuário preenche nome completo, e-mail, senha e confirmação de senha, e clica em "Criar conta". *Link* secundário "Já tenho uma conta" retorna ao Login.
- **Componentes principais:** título "Criar conta" + subtítulo, quatro campos de formulário com *placeholders* descritivos (incluindo regra "mínimo 8 caracteres" inline), botão primário "Criar conta", *link* de retorno ao login.
- **Organização do layout:** formulário em coluna única com espaçamento consistente entre campos; *labels* externos para acessibilidade; CTA único e proeminente.
- **Comportamento mobile:** validação inline de força de senha e confirmação; campos crescem na vertical sem comprometer o CTA; teclado adapta-se ao tipo de campo (email, password).
- **Observações de usabilidade:** manter o formulário enxuto no cadastro inicial (apenas o essencial) e transferir a coleta de preferências para o onboarding evita abandono no primeiro passo. O *placeholder* "Mínimo 8 caracteres" antecipa erro de validação, reduzindo retrabalho.

## 4.4 Onboarding de Interesses
- **Objetivo da tela:** coletar os sinais iniciais que alimentam o motor de recomendação (RF01) — gêneros preferidos, cidade e frequência de consumo cultural.
- **Descrição funcional:** sequência de três passos com seleção guiada. O usuário avança via botão "Continuar" a cada passo; pode interromper o fluxo a qualquer momento pelo *link* "Pular" no canto superior direito.
- **Estados desta tela (três passos do mesmo fluxo):**
  1. **Gêneros:** seleção múltipla em grade 2×4 com instrução "Escolha pelo menos 3 categorias".
  2. **Cidade:** seleção única entre "São Paulo", "Rio de Janeiro" e "Outra" (RF02).
  3. **Frequência:** seleção única entre "Toda semana", "Todo mês", "Algumas vezes por ano" e "É minha primeira vez".
- **Componentes principais:** indicador de progresso (*step indicator* de três pontos), *link* "Pular", título da pergunta, descrição auxiliar, grade ou lista de opções selecionáveis, CTA "Continuar" fixo no rodapé.
- **Organização do layout:** topo dedicado à orientação (progresso + escape), centro à pergunta, base ao CTA. O padrão se repete nos três passos, reduzindo carga cognitiva entre etapas.
- **Comportamento mobile:** o estado das seleções é preservado ao navegar entre passos; "Pular" leva direto a Bem-vinda e desencadeia o estado de *empty state* em "Para você" em Explorar (ver 4.6).
- **Observações de usabilidade:** dividir a coleta em micro-tarefas (uma pergunta por tela) reduz percepção de esforço — particularmente importante para Ana Luiza, que abandona formulários densos. O "Pular" preserva agência do usuário, mas o sistema deve sinalizar a perda de personalização downstream.

## 4.5 Bem-vinda
- **Objetivo da tela:** marcar o encerramento do onboarding com reforço positivo e direcionar o usuário à Home com intenção clara de exploração.
- **Descrição funcional:** tela transicional com mensagem personalizada ("Bem-vinda, [nome]!") e CTA único "Explorar Espetáculos".
- **Componentes principais:** logo da marca, ilustração/imagem placeholder, saudação personalizada com nome do usuário, mensagem motivacional ou pergunta engajadora, botão primário de transição.
- **Organização do layout:** composição centralizada vertical com hierarquia clara — marca → imagem → mensagem → CTA. Bastante respiro entre elementos para reforçar o caráter celebratório.
- **Comportamento mobile:** transição em fade ou *slide* a partir da última tela de onboarding; ao clicar no CTA, navega para Explorar.
- **Observações de usabilidade:** personalizar com o nome do usuário cria *anchoring* emocional. O CTA único elimina ambiguidade — o caminho desejado é uma decisão de produto, não do usuário neste momento.


## 4.6 Explorar (Home)
- **Objetivo da tela:** materializar a proposta de valor do produto — entregar uma vitrine dinâmica personalizada que reduz o atrito da descoberta e direciona à conversão.
- **Descrição funcional:** *feed* vertical scrollável com múltiplas prateleiras (*carrosséis*) de espetáculos, contemplando recomendações personalizadas e curadoria editorial. O usuário pode filtrar a vitrine por categoria no topo, scrollar lateralmente em cada prateleira e abrir o detalhe ao tocar em um *card*.
- **Estados desta tela:**
  - **Estado padrão (usuário com preferências):** todas as prateleiras populadas, incluindo a seção **"Para você"** com recomendações baseadas no perfil capturado no onboarding.
  - **Estado alternativo (usuário pulou o onboarding):** a seção "Para você" exibe *empty state* dedicado, com ícone (lâmpada), mensagem orientando a personalização e CTA "Editar Interesses" que leva ao fluxo 4.11. Demais prateleiras (curadoria editorial não personalizada, "Em alta", etc.) permanecem populadas.
- **Componentes principais:** header com saudação/data, barra de filtros por categoria (chips horizontais), prateleiras temáticas ("Em alta", "Para você", "Novidades", "Perto de você"), seções de retenção ("Continue assistindo aos trailers"), *tab bar* fixa inferior com quatro itens (Explorar, Buscar, Wishlist, Perfil).
- **Organização do layout:** *scroll* vertical da página, *scroll* horizontal dentro de cada prateleira; densidade visual modulada por seções com títulos claros para facilitar varredura; *tab bar* sempre acessível.
- **Comportamento mobile:** *infinite scroll* opcional nas prateleiras; *cards* otimizados para *thumb zone*; chips de categoria fixos ou sticky ao rolar.
- **Observações de usabilidade:** o padrão "prateleiras" (familiar de plataformas de *streaming*) reduz curva de aprendizado para o público-alvo. A seção "Perto de você" responde ao RF02 (geolocalização) e é peça-chave da estratégia de relevância local. O *empty state* personalizado de "Para você" transforma uma fraqueza (falta de dados) em um *hook* de coleta, sem bloquear o restante da experiência.

## 4.7 Buscar
- **Objetivo da tela:** permitir busca ativa por espetáculos, teatros ou cidades, complementando a descoberta passiva do feed.
- **Descrição funcional:** campo de busca no topo, sugestões/recomendações no estado inicial, lista de resultados ao digitar ou aplicar filtros. Filtros avançados acessíveis via ícone dedicado.
- **Estados desta tela:**
  - **Inicial:** campo de busca vazio + lista "Recomendados para você" como conteúdo de fallback.
  - **Filtros abertos:** *bottom sheet* / modal sobreposto com filtros por categoria, data, local/cidade e CTAs "Aplicar Filtros" e "Limpar Filtros".
  - **Resultados com filtro aplicado:** lista de resultados, badge numérico no ícone de filtro indicando filtros ativos, *link* "Limpar" para reset rápido, texto descrevendo filtros aplicados.
- **Componentes principais:** campo de busca com ícone de lupa, ícone de filtros (com *badge* de contagem ativa), texto descritivo de filtros ativos, lista vertical de *cards* de resultado, *tab bar* inferior.
- **Organização do layout:** input no topo (alcance natural do polegar para digitação), resultados em lista vertical scrollável. O modal de filtros ocupa a metade inferior, preservando contexto da busca acima.
- **Comportamento mobile:** *autofocus* no campo ao abrir; teclado fica visível imediatamente; histórico de buscas pode ser exposto entre o estado inicial e o resultado; modal de filtros aplica-se com transição *slide-up*.
- **Observações de usabilidade:** o *empty state* inicial popular com recomendações evita a sensação de "tela em branco" e mantém o usuário em fluxo de descoberta. O *badge* no ícone de filtros torna o estado do sistema visível (heurística de Nielsen).

## 4.8 Wishlist
- **Objetivo da tela:** consolidar os espetáculos salvos pelo usuário, oferecendo um acesso rápido a interesses futuros (peça da estratégia de retenção descrita no brainstorm de features).
- **Descrição funcional:** lista vertical de *cards* de espetáculos salvos. Cada *card* expõe título, tag de categoria, local e data; ícone de coração permite remover do favoritos. Toque no *card* leva à página de informações.
- **Estados desta tela:**
  - **Com itens:** lista de espetáculos salvos com layout consistente; o coração preenchido indica estado de "salvo".
  - **Vazia:** ícone de coração outline centralizado, mensagem de *empty state* explicativa, CTA "Explorar Espetáculos" que leva à Home.
- **Componentes principais:** header com título "Wishlist" e subtítulo, lista de *cards* (ou *empty state*), *tab bar* inferior com o ícone de coração destacado.
- **Organização do layout:** densidade média de informação por *card*, otimizando varredura; *empty state* segue padrão central com mensagem + CTA.
- **Comportamento mobile:** *pull-to-refresh* para sincronizar; remoção via *tap* no coração com confirmação opcional via *toast*; navegação para detalhe via *tap* no *card*.
- **Observações de usabilidade:** o *empty state* converte um momento de "nada para mostrar" em um ponto de entrada para a descoberta, conectando-se à Explorar. Manter o estado visual do coração consistente em todas as superfícies (Explorar, Buscar, Informações, Wishlist) é crítico para coerência do modelo mental.

## 4.9 Informações do Espetáculo
- **Objetivo da tela:** apresentar todas as informações necessárias para a decisão de compra e fazer o *hand-off* para a bilheteria parceira (ponto de conversão central do produto).
- **Descrição funcional:** página de detalhe scrollável com imagem principal, metadados (categoria, data, local), sinopse, elenco, recomendações relacionadas, e botões de ação ("Salvar" e "Ingressos") fixos no rodapé.
- **Componentes principais:** logo + categorias no header, imagem principal do espetáculo, título + bloco de tags de categoria, dados estruturados (data, local, mapa miniatura), seção "Sobre o espetáculo" (descrição + duração), seção "Elenco principal", seção "Você pode gostar" (recomendações cruzadas), barra inferior fixa com preço "RS [XX]", botão "Salvar" e botão primário "Ingressos".
- **Organização do layout:** *scroll* vertical longo, hierarquizado por seções com títulos. Barra inferior fixa garante acesso constante às duas ações principais (salvar = retenção; ingressos = conversão).
- **Comportamento mobile:** *parallax* ou *sticky header* opcional na imagem; o botão "Ingressos" redireciona o usuário para o parceiro de bilheteria (fluxo de saída, alinhado à definição de "o que o produto não faz"); a ação "Salvar" alterna o estado do coração e atualiza a Wishlist.
- **Observações de usabilidade:** a barra inferior persistente respeita a heurística de "ação visível em qualquer ponto do scroll", reduzindo distância até a conversão. A seção "Você pode gostar" alavanca o motor de recomendação para evitar *dead-ends* (alinhado ao objetivo de "Sinergia do Ecossistema"). Exibir o preço junto ao CTA elimina surpresas e qualifica o clique antes do redirecionamento à ticketeira.

## 4.10 Perfil
- **Objetivo da tela:** dar ao usuário controle sobre dados pessoais, preferências e canais de comunicação, sustentando a confiança no produto e a longevidade do relacionamento.
- **Descrição funcional:** painel de conta com identidade do usuário e três grupos de configuração — preferências/cidade, notificações e informações institucionais.
- **Componentes principais:** *card* de identidade (avatar, nome, e-mail, *link* "Editar Perfil"), lista de itens de configuração (Preferências e cidade, Notificações, Informações Institucionais), *tab bar* inferior com "Perfil" destacado.
- **Organização do layout:** identidade no topo (referência clara de "quem está logado"), lista de ações em *cards* com *chevron* indicando navegação para subpáginas; espaçamento generoso para legibilidade.
- **Comportamento mobile:** cada item navega para uma subpágina dedicada (4.11, 4.12, 4.13); o *link* "Editar Perfil" abre o fluxo de edição de dados básicos.
- **Observações de usabilidade:** agrupar configurações por intenção (preferências = recomendação; notificações = comunicação; institucional = transparência) é mais escaneável que uma lista plana. A separação clara entre "editar perfil" e "preferências" evita confusão entre dados pessoais e sinais para o algoritmo.

## 4.11 Editar Interesses
- **Objetivo da tela:** permitir ao usuário ajustar a qualquer momento os sinais que alimentam o motor de recomendação, fechando o *loop* iniciado no onboarding.
- **Descrição funcional:** reedição dos mesmos campos coletados no onboarding (gêneros e cidade) em uma única tela, com CTAs explícitos de salvar ou cancelar e dica auxiliar ao final.
- **Componentes principais:** header com botão de voltar e título "Editar Interesses", *card* informativo "Ajuste suas recomendações", seção "Selecione seus interesses" (grade de tags multiseleção), contador "[N] interesses selecionados", seção "Selecione sua cidade" (lista de opções), botão primário "Salvar Alterações", botão secundário "Cancelar", dica final sobre flexibilidade de mudança.
- **Organização do layout:** estrutura de formulário verticalizada com agrupamento por tema; CTAs no fim da página em hierarquia clara (primário sobre secundário).
- **Comportamento mobile:** as seleções refletem o estado atual no carregamento; "Cancelar" descarta mudanças e retorna ao Perfil; "Salvar Alterações" persiste e dispara recálculo das recomendações na Explorar.
- **Observações de usabilidade:** consolidar tudo em uma tela (ao contrário do onboarding em três passos) é apropriado aqui — o usuário já conhece o sistema e busca eficiência. A "dica" final reforça que mudar interesses é seguro e reversível, reduzindo a fricção da edição. Esta tela é também o destino do CTA do *empty state* "Para você" na Explorar (ver 4.6), fechando o fluxo do usuário que pulou o onboarding.

## 4.12 Notificações
- **Objetivo da tela:** dar controle granular sobre os canais e tipos de comunicação, item crítico para conformidade e para manter relevância da estratégia de *push* (descrita no brainstorm).
- **Descrição funcional:** lista de *toggles* agrupados por categoria: canais (Push, E-mail), conteúdo (Novos espetáculos, Recomendações personalizadas, Espetáculos salvos) e marketing (Newsletter, Atualizações do app).
- **Componentes principais:** header com botão voltar e título "NOTIFICAÇÕES", *card* explicativo de topo, três blocos de toggles com título de seção, label e descrição auxiliar por item.
- **Organização do layout:** agrupamento semântico em três blocos verticais, separados visualmente por títulos de seção; cada item ocupa altura confortável para alvo de toque.
- **Comportamento mobile:** toggles aplicam mudança imediatamente (sem botão "Salvar"); alteração no canal Push pode disparar pedido de permissão nativa do sistema operacional.
- **Observações de usabilidade:** separar canais de tipos de conteúdo respeita a expectativa do usuário de modular comunicação em duas dimensões. Descrições auxiliares por item reduzem ambiguidade sobre o que cada toggle controla, evitando *opt-outs* indesejados.

## 4.13 Informações Institucionais
- **Objetivo da tela:** consolidar informações sobre a Aventura Teatros e canais oficiais, reforçando confiança e cumprindo expectativas de transparência institucional.
- **Descrição funcional:** página estática com descrição da empresa, links de redes sociais, contato, site oficial e localização.
- **Componentes principais:** header com botão voltar e título "INFORMAÇÕES INSTITUCIONAIS" + subtítulo, bloco "Sobre [Nome da Empresa]" com texto institucional e versão do app, bloco "Siga-nos" com lista de redes sociais, bloco com e-mail, site, e localização (endereço + CEP), rodapé com copyright e tagline.
- **Organização do layout:** estrutura informativa em blocos verticais por categoria, com ícones para reconhecimento visual rápido das redes sociais; rodapé fecha a página com identidade da marca.
- **Comportamento mobile:** *taps* em redes/e-mail/site/endereço abrem o app nativo correspondente (browser, mail, maps); versão do app é informativa para diagnóstico de suporte.
- **Observações de usabilidade:** página de baixo tráfego mas alta importância simbólica — sinaliza profissionalismo e oferece pontos de contato verificáveis, especialmente relevantes para o público sênior do catálogo da Aventura. A versão do app facilita troubleshooting quando o usuário entra em contato com suporte.

---

## 5. Mockups Navegáveis

## Introdução

Esta seção documenta os mockups de alta fidelidade do aplicativo móvel do **Portal Aventura**, traduzindo os wireframes de média fidelidade em uma camada visual que materializa identidade, hierarquia cromática, tipografia e componentes finais. O escopo mantém alinhamento com as mesmas jornadas críticas: **onboarding de interesses** (alimentando o motor de recomendação), **descoberta** (Explorar, Buscar, Wishlist) e **conversão** (informações do espetáculo com *hand-off* para a bilheteria), agora com o detalhe necessário para validação estética, implementação e testes com usuários.

> **Arquivo Figma:** [Mockup — Portal Aventura (visualização completa)](https://www.figma.com/design/0lPbGGZo6VHkp0eKkrQlWs/Mockup-din%C3%A2mico---aventura?node-id=0-1&t=BVSOfexMDGf7JtH6-1)

> **Nota:** o link acima aponta ao quadro com o **mockup completo** das telas para revisão visual no Figma; O protótipo **dinâmico** navegável (fluxo interativo entre telas), está referenciado ao fim dessa sessão

### 5.1 Objetivo visual do mockup
Descreva o objetivo da proposta visual (ex.: clareza, modernidade, destaque para descoberta, linguagem cultural) e o que deve ser percebido pelo usuário.

### 5.2 Coerência com os wireframes
Explique o que foi mantido e o que foi ajustado na passagem de wireframe → mockup

### 5.3 Fluxo navegável
Descreva como o usuário percorre as telas no protótipo (pontos de entrada, rotas principais, retornos).

### 5.4 Decisões visuais
Registre decisões-chave (ex.: hierarquia, uso de cor, ênfase em cards, densidade, contraste). Justificativas curtas e objetivas.

### 5.5 Experiência mobile
Descreva como o visual foi pensado para toque/legibilidade e como estados e feedbacks aparecem.

### 5.6 Link do protótipo navegável
- **Mockup completo (Figma — visualização das telas):** [Mockup — Portal Aventura](https://www.figma.com/design/0lPbGGZo6VHkp0eKkrQlWs/Mockup-din%C3%A2mico---aventura?node-id=0-1&t=BVSOfexMDGf7JtH6-1) — uso para revisão estática do layout; o protótipo dinâmico será adicionado aqui quando existir.
- **Link do protótipo dinâmico (navegável):** *(a preencher)*

---

## 6. Guia de Estilos (Componentes Visuais)

> **Instrução de preenchimento**: documente o “kit visual” utilizado no protótipo para garantir consistência. Para cada item, descreva regras de uso, variações e justificativas objetivas (quando necessário).

### 6.1 Tipografia
Descreva família(s), escala (títulos/corpo/legendas), pesos e uso por contexto (ex.: títulos de seção, cards, botões).


### 6.2 Paleta de cores
Liste cores (hex) e uso (primária, secundária, fundo, texto, feedback).

### 6.3 Botões
Documente variantes (primário/secundário/terciário), estados (normal/press/disabled/loading) e regras de espaçamento.

### 6.4 Ícones
Defina estilo (linha/sólido), tamanhos, alinhamento e consistência semântica (quando usar cada ícone).

### 6.5 Cards
Descreva estrutura padrão de card (imagem/título/metadados/ações) e regras de truncamento/linhas.

### 6.6 Inputs
Documente padrões de campo (label, placeholder, máscara), estados (foco/erro/sucesso/disabled) e mensagens.

### 6.7 Barra de navegação
Especifique padrão (inferior/superior), itens, estado ativo, comportamento com scroll e em telas específicas.

### 6.8 Espaçamentos
Defina a escala de espaçamento (ex.: 4/8/12/16/24) e onde aplicar (margens, gaps, paddings).

### 6.9 Consistência visual
Registre regras gerais que devem ser mantidas em todas as telas (ex.: alinhamentos, raios, sombras, densidade).

---

## 7. Interações e Comportamentos Mobile

### 7.1 Scroll

O scroll vertical é utilizado como principal forma de navegação entre conteúdos extensos da aplicação.

Comportamentos definidos:

- Rolagem vertical contínua em listas, feeds e páginas de conteúdo.
- Header superior permanece fixo em telas principais para preservar contexto de navegação.
- Navegação inferior permanece visível durante a rolagem para facilitar acesso rápido às funcionalidades principais.
- Conteúdos são carregados progressivamente para evitar excesso de informações simultâneas.
- Em páginas longas, o usuário pode retornar ao topo rapidamente através de interação simplificada.
- Barras de rolagem seguem comportamento nativo do sistema operacional do dispositivo.

---

### 7.2 Swipe

O gesto de swipe é utilizado em áreas específicas da interface para melhorar navegação e dinamismo.

Comportamentos definidos:

- Swipe horizontal em carrosséis de categorias e destaques.
- Navegação limitada ao conteúdo disponível, impedindo movimentações excessivas.
- Feedback visual suave acompanha o movimento do usuário.
- Swipe utilizado apenas em componentes que exigem navegação horizontal, evitando conflitos com o scroll vertical.

---

### 7.3 Transições

As transições foram planejadas para manter fluidez e previsibilidade durante a navegação entre telas.

Padrões definidos:

- Transições suaves entre páginas principais.
- Abertura e fechamento de menus com animações rápidas e discretas.
- Alterações de estado ocorrem sem interrupções bruscas.
- Tempo de transição reduzido para evitar sensação de lentidão.
- Componentes seguem comportamento visual consistente em toda a aplicação.
- Navegação de retorno respeita o comportamento padrão do dispositivo móvel.

---

### 7.4 Feedback visual

O sistema fornece respostas visuais imediatas para informar o usuário sobre ações e estados da interface.

Feedbacks implementados:

- Alteração visual ao pressionar botões e cards.
- Indicadores de carregamento durante buscas e carregamento de conteúdo.
- Destaque visual para itens selecionados.
- Mensagens de sucesso para ações concluídas corretamente.
- Mensagens de erro em situações de falha ou conexão indisponível.
- Estados desabilitados representados com menor contraste visual.
- Campos de formulário apresentam validação visual em tempo real.

---

### 7.5 Microinterações

As microinterações auxiliam na percepção de controle e melhoram a experiência do usuário.

Microinterações previstas:

- Efeito visual ao tocar em botões.
- Destaque em itens ativos da navegação.
- Animações sutis em abertura de cards e conteúdos.
- Alteração de estado em botões favoritos e interações rápidas.
- Feedback visual em campos preenchidos corretamente.
- Respostas visuais em ações concluídas pelo usuário.
- Mudança de aparência em componentes selecionados.

Objetivos das microinterações:

- Confirmar ações executadas.
- Melhorar percepção de responsividade.
- Facilitar entendimento da interface.
- Tornar a navegação mais fluida e intuitiva.

---

### 7.6 Estados de navegação

A interface contempla diferentes estados para garantir clareza durante o uso.

#### Tela ativa/inativa

- A seção ativa da navegação inferior permanece destacada visualmente.
- Itens inativos possuem menor contraste visual.

#### Estado vazio

- Exibição de mensagens informativas quando não houver conteúdo disponível.
- Sugestões de navegação ou novas ações são apresentadas ao usuário.

#### Carregando

- Indicadores de loading são exibidos durante carregamento de conteúdo.
- Estruturas visuais simplificadas evitam sensação de travamento.

#### Erro

- Mensagens claras comunicam falhas de carregamento ou conexão.
- Usuário recebe possibilidade de tentar novamente.

#### Sem resultados

- Interface informa ausência de resultados encontrados.
- Sugestões alternativas podem ser exibidas para continuidade da navegação.

---

### 7.7 Comportamento responsivo

A interface foi estruturada para adaptação eficiente em diferentes resoluções mobile.

Regras definidas:

- Componentes organizados prioritariamente em coluna.
- Cards e imagens ajustam largura automaticamente.
- Textos possuem truncamento controlado para evitar quebra excessiva.
- Botões mantêm tamanho mínimo adequado para interação por toque.
- Espaçamentos internos preservam legibilidade em telas menores.
- Blocos de conteúdo são reorganizados conforme dimensão disponível.
- Navegação permanece acessível independentemente da resolução utilizada.
- Elementos secundários possuem menor prioridade visual em telas reduzidas.

O comportamento responsivo busca garantir acessibilidade, legibilidade e consistência visual em toda a experiência mobile.
---

## 8. Considerações Finais

### 8.1 Aderência aos objetivos do projeto
Explique como o projeto visual atende ao objetivo de recomendação e descoberta, de forma objetiva.

### 8.2 Alinhamento com a proposta Mobile First
Resuma como as decisões documentadas priorizam uso em smartphone (leitura, toque, fluxo).

### 8.3 Resultado esperado da experiência visual
Descreva o resultado esperado para o usuário (ex.: clareza, fluidez, confiança, facilidade de encontrar e salvar espetáculos).
