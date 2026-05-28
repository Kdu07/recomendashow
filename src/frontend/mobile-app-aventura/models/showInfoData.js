export const MOCK_SHOWS = {
  '1': {
    id: '1',
    title: 'A Noviça Rebelde',
    heroImage: require('../assets/hero-novica-rebelde.png'),
    isFavorited: false,

    date: {
      label: 'DATA E HORÁRIO',
      status: 'Temporada Encerrada',
      detail: 'Últimas apresentações: Julho de 2024',
    },

    location: {
      label: 'LOCAL',
      venue: 'Vibra São Paulo',
      address: 'Av. das Nações Unidas, 17955',
      city: 'São Paulo, SP',
      mapImage: require('../assets/mapa-novica-rebelde.png'),
    },

    about: {
      sectionLabel: 'SOBRE O ESPETÁCULO',
      description:
        'Uma superprodução brasileira do clássico musical com músicas de Richard Rodgers e letras de Oscar Hammerstein II. Adaptação brasileira e supervisão musical de Claudio Botelho.\n\nA história emocionante de Maria, uma jovem que deixa o convento para se tornar governanta dos sete filhos do Capitão von Trapp. Com canções inesquecíveis e uma mensagem atemporal sobre amor, família e liberdade, este musical encantou gerações ao redor do mundo.',
      duration: 'Duração: 3h00 (180 minutos com intervalo)',
    },

    cast: {
      sectionLabel: 'ELENCO PRINCIPAL',
      members: [
        { id: '1', name: 'Malu Rodrigues', role: 'Maria', image: require('../assets/head-shot-maluRodrigues.png') },
        { id: '2', name: 'Pierre Baitelli', role: 'Capitão Georg von Trapp', image: require('../assets/head-shot-pierreBaitelli.png') },
        { id: '3', name: 'Larissa Manoela', role: 'Liesl', image: require('../assets/head-shot-larissaManoela.png') },
        { id: '4', name: 'Elenco de 40+ atores', role: 'Incluindo 18 crianças', image: null },
      ],
    },

    creativeTeam: {
      sectionLabel: 'EQUIPE CRIATIVA',
      members: [
        { role: 'Direção', name: 'Charles Möeller & Claudio Botelho' },
        { role: 'Direção Musical', name: 'Marcelo Castro' },
        { role: 'Coreografia', name: 'Dalal Achcar' },
      ],
    },

    recommendations: [
      { id: '2', title: 'Viva: A Vida é uma Festa', image: require('../assets/home/viva.png') },
      { id: '3', title: 'Mamma Mia!', image: require('../assets/home/mamma-mia.png') },
    ],

    ticket: {
      priceLabel: 'A partir de',
      price: 'R$ 120',
      buttonLabel: 'INGRESSOS',
      url: 'https://bileto.sympla.com.br/event/95022',
    },
  },

  '2': {
    id: '2',
    title: 'Viva: A Vida é uma Festa, In Concert',
    heroImage: require('../assets/header-viva.png'),
    isFavorited: false,

    date: {
      label: 'DATA E HORÁRIO',
      status: '30 de Maio a 7 de Junho',
      detail: 'Em cartaz de 30/05 à 07/06',
    },

    location: {
      label: 'LOCAL',
      venue: 'BTG Pactual Hall',
      address: 'R. Bento Branco de Andrade Filho, 722',
      city: 'São Paulo, SP',
      mapImage: require('../assets/home/btg-pactual-hall.png'),
    },

    about: {
      sectionLabel: 'SOBRE O ESPETÁCULO',
      description:
        'Uma experiência além da tela: enquanto o filme é projetado, uma orquestra ao vivo, cantores, atores e bailarinos ocupam o palco e interagem com a plateia.\n\nA história de Miguel, sua jornada à Terra dos Mortos e a celebração da cultura mexicana e dos valores familiares. Lembrar é um ato de amor.\n\nApresentado pelo Ministério da Cultura e BTG Pactual, via Lei de Incentivo à Cultura.',
      duration: 'Com intervalo',
    },

    creativeTeam: {
      sectionLabel: 'EQUIPE CRIATIVA',
      members: [
        { role: 'Direção', name: 'Diego Morais' },
        { role: 'Direção Musical e Regência', name: 'Laura Visconti' },
        { role: 'Coreografia', name: 'Natacha Travassos' },
        { role: 'Figurino', name: 'Karen Brusttolin' },
        { role: 'Cenografia', name: 'Jackson Tinoco' },
        { role: 'Direção Artística e Produção Geral', name: 'Aniela Jordan' },
      ],
    },

    recommendations: [
      { id: '1', title: 'A Noviça Rebelde', image: require('../assets/hero-novica-rebelde.png') },
      { id: '3', title: 'Mamma Mia!', image: require('../assets/home/mamma-mia.png') },
    ],

    ticket: {
      priceLabel: 'A partir de',
      price: 'R$ 80',
      buttonLabel: 'INGRESSOS',
      url: 'https://bileto.sympla.com.br/event/115142/d/359399',
    },
  },

  '3': {
    id: '3',
    title: 'Mamma Mia!',
    heroImage: require('../assets/header-show-info-mammaMia.jpg'),
    isFavorited: false,

    date: {
      label: 'DATA E HORÁRIO',
      status: '18 de Junho a 12 de Julho',
      detail: 'Em cartaz de 18/06 à 12/07',
    },

    location: {
      label: 'LOCAL',
      venue: 'BTG Pactual Hall',
      address: 'R. Bento Branco de Andrade Filho, 722',
      city: 'São Paulo, SP',
      mapImage: require('../assets/home/btg-pactual-hall.png'),
    },

    about: {
      sectionLabel: 'SOBRE O ESPETÁCULO',
      description:
        'O fenômeno da Broadway retorna ao Brasil em 2026 com uma superprodução recheada de hits do ABBA: Dancing Queen, Mamma Mia!, The Winner Takes it All, Money Money Money e muito mais.\n\nSophie está prestes a se casar e convida três ex-namorados de sua mãe Donna para descobrir quem é seu pai. Em uma ilha grega ensolarada, amor, amizade e liberdade se encontram em um espetáculo que celebra o empoderamento feminino e a alegria de viver.',
      duration: 'Com intervalo',
    },

    cast: {
      sectionLabel: 'ELENCO PRINCIPAL',
      members: [
        { id: '1', name: 'Claudia Netto', role: 'Donna', image: require('../assets/head-shot-claudiaNetto.png') },
        { id: '2', name: 'Totia Meireles', role: 'Tanya', image: require('../assets/head-shot-totiaMeireles.png') },
        { id: '3', name: 'Gottsha', role: 'Rosie', image: require('../assets/head-shot-gottsha.png') },
        { id: '4', name: 'Elenco de 20+ artistas', role: 'Sujeito a alteração sem aviso prévio', image: null },
      ],
    },

    creativeTeam: {
      sectionLabel: 'EQUIPE CRIATIVA',
      members: [
        { role: 'Direção, Cenário e Figurinos', name: 'Charles Möeller' },
        { role: 'Versão Brasileira e Supervisão Musical', name: 'Claudio Botelho' },
        { role: 'Direção Musical', name: 'Marcelo Castro' },
        { role: 'Coreografia', name: 'Mariana Barros' },
        { role: 'Direção Artística e Produção Geral', name: 'Aniela Jordan' },
      ],
    },

    recommendations: [
      { id: '1', title: 'A Noviça Rebelde', image: require('../assets/hero-novica-rebelde.png') },
      { id: '2', title: 'Viva: A Vida é uma Festa', image: require('../assets/home/viva.png') },
    ],

    ticket: {
      priceLabel: 'A partir de',
      price: 'R$ 100',
      buttonLabel: 'INGRESSOS',
      url: 'https://bileto.sympla.com.br/event/115150/d/359512',
    },
  },
};
