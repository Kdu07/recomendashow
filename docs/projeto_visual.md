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

> **Instrução de preenchimento**: registre decisões que priorizam o uso em smartphone. Para cada tópico, descreva **o que foi decidido** e **por quê**, com base em leitura, toque e navegação.

### 2.1 Priorização da experiência mobile
Documente quais tarefas e informações foram priorizadas no mobile (ex.: ações principais visíveis sem excesso de rolagem; redução de etapas).

### 2.2 Adaptação para diferentes tamanhos de tela
Descreva como o layout se comporta em variações comuns (ex.: 360×640, 390×844, 414×896).  
Inclua critérios objetivos (ex.: regras de quebra, escalas, truncamento, grid).

### 2.3 Legibilidade
Registre decisões de hierarquia tipográfica e contraste para leitura em movimento/baixa luminosidade.  
Indique quais regras devem ser mantidas para consistência (ex.: tamanhos mínimos, densidade de texto, limites de linha).

### 2.4 Ergonomia para toque
Explique como as ações mais frequentes foram posicionadas para uso confortável (ex.: alcance do polegar, alvos de toque, espaçamentos).

### 2.5 Organização de conteúdo em telas pequenas
Descreva como o conteúdo foi estruturado para reduzir carga cognitiva (ex.: agrupamentos, progressão de informação, uso de cards, filtros).

---

## 3. Estrutura de Navegação

> **Instrução de preenchimento**: documente a navegação considerando o fluxo principal do usuário e a hierarquia entre telas. Se possível, inclua um diagrama/fluxo e explique regras de transição e retorno.

### 3.1 Fluxo principal do usuário
Descreva o caminho esperado do usuário do início ao uso recorrente (ex.: onboarding → explorar → detalhes/ações → wishlist → perfil).

### 3.2 Hierarquia entre telas
Explique a relação entre telas (ex.: telas de entrada vs. principais; telas de configuração; telas de apoio).

### 3.3 Navegação principal
Defina o padrão de navegação predominante (ex.: barra inferior, topo com ações, tabs) e **quando** ele aparece.

### 3.4 Comportamento de menus e navegação inferior/superior
Registre regras como:
- comportamento do botão “voltar”
- persistência/ocultação de barras ao scroll
- estados ativos e rotas
- títulos e ações do topo

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


---

## 5. Mockups Navegáveis

> **Instrução de preenchimento**: registre como a camada visual final materializa os wireframes, com foco em consistência e experiência mobile. Inclua link do protótipo e prints por tela/estado.

### 5.1 Objetivo visual do mockup
Descreva o objetivo da proposta visual (ex.: clareza, modernidade, destaque para descoberta, linguagem cultural) e o que deve ser percebido pelo usuário.

### 5.2 Coerência com os wireframes
Explique o que foi mantido e o que foi ajustado na passagem de wireframe → mockup (somente ajustes de UI/visual/hierarquia, sem inventar novas telas).

### 5.3 Fluxo navegável
Descreva como o usuário percorre as telas no protótipo (pontos de entrada, rotas principais, retornos).

### 5.4 Decisões visuais
Registre decisões-chave (ex.: hierarquia, uso de cor, ênfase em cards, densidade, contraste). Justificativas curtas e objetivas.

### 5.5 Experiência mobile
Descreva como o visual foi pensado para toque/legibilidade e como estados e feedbacks aparecem.

### 5.6 Link do protótipo navegável
- **Link do protótipo**:  

### 5.7 Imagens dos mockups (placeholders)
> Insira abaixo os prints/exportações dos mockups por tela (ou principais estados).

![Mockup - Splash Screen]()
![Mockup - Login]()
![Mockup - Cadastro]()
![Mockup - Onboarding]()
![Mockup - Boas-Vindas]()
![Mockup - Explorar / Home]()
![Mockup - Buscar]()
![Mockup - Wishlist]()
![Mockup - Perfil / Configurações]()
![Mockup - Detalhes do Espetáculo]()
![Mockup - Editar Interesses]()

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

> **Instrução de preenchimento**: descreva como o usuário interage com a interface no mobile. Registre comportamentos de navegação, feedbacks e estados, com foco em previsibilidade e acessibilidade.

### 7.1 Scroll
Descreva o que rola, como barras se comportam, e como preservar contexto (ex.: título fixo, retorno ao topo).

### 7.2 Swipe
Se aplicável, descreva onde existe swipe (ex.: onboarding), limites e feedback.

### 7.3 Transições
Documente transições entre telas/estados (ex.: padrões de entrada/saída, duração percebida, consistência).

### 7.4 Feedback visual
Explique como o sistema comunica ações e estados (ex.: toque, carregamento, sucesso/erro).

### 7.5 Microinterações
Liste microinterações relevantes (ex.: estados de botão, seleção, favoritos) e sua intenção.

### 7.6 Estados de navegação
Descreva estados como:
- tela ativa/inativa na navegação
- estado vazio
- carregando
- erro
- sem resultados

### 7.7 Comportamento responsivo
Documente regras de adaptação (ex.: colunas, tamanhos, truncamento, reorganização de blocos) sem sair do foco mobile.

---

## 8. Considerações Finais

### 8.1 Aderência aos objetivos do projeto
Explique como o projeto visual atende ao objetivo de recomendação e descoberta, de forma objetiva.

### 8.2 Alinhamento com a proposta Mobile First
Resuma como as decisões documentadas priorizam uso em smartphone (leitura, toque, fluxo).

### 8.3 Resultado esperado da experiência visual
Descreva o resultado esperado para o usuário (ex.: clareza, fluidez, confiança, facilidade de encontrar e salvar espetáculos).
