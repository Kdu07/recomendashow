export const MOCK_SHOW = {
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
    { id: '1', title: 'Espelho Mágico', image: require('../assets/hero-novica-rebelde.png') },
    { id: '2', title: 'Mamma Mia', image: require('../assets/hero-novica-rebelde.png') },
    { id: '3', title: 'PIAF', image: require('../assets/hero-novica-rebelde.png') },
  ],

  ticket: {
    priceLabel: 'A partir de',
    price: 'R$ 120',
    buttonLabel: 'INGRESSOS',
  },
};
