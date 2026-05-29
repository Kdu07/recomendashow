// data/mockEventDetailData.js

export const novicaRebeldeDetail = {
  id: 'trending-novica',
  title: 'A Noviça Rebelde',
  image: require('../assets/home/banner-novica.png'),
  isWishlisted: false,

  status: 'ended',
  dateLabel: 'Temporada Encerrada',
  dateSubtitle: 'Últimas apresentações: Julho de 2024',

  venue: 'Vibra São Paulo',
  address: 'Av. das Nações Unidas, 17955',
  city: 'São Paulo, SP',

  description:
    'Uma superprodução brasileira do clássico musical com músicas de Richard Rodgers e letras de Oscar Hammerstein II. Adaptação brasileira e supervisão musical de Claudio Botelho.\n\nA história emocionante de Maria, uma jovem que deixa o convento para se tornar governanta dos sete filhos do Capitão von Trapp. Com canções inesquecíveis e uma mensagem atemporal sobre amor, família e liberdade, este musical encantou gerações ao redor do mundo.',
  duration: '3h00 (180 minutos com intervalo)',

  cast: [
    {
      id: 'cast-1',
      name: 'Malu Rodrigues',
      role: 'Maria',
      avatar: null,
      isGroup: false,
    },
    {
      id: 'cast-2',
      name: 'Pierre Baitelli',
      role: 'Capitão Georg von Trapp',
      avatar: null,
      isGroup: false,
    },
    {
      id: 'cast-3',
      name: 'Larissa Manoela',
      role: 'Liesl',
      avatar: null,
      isGroup: false,
    },
    {
      id: 'cast-group',
      name: 'Elenco de 40+ atores',
      role: 'Incluindo 18 crianças',
      avatar: null,
      isGroup: true,
      groupLabel: '+',
    },
  ],

  creativeTeam: {
    direction: 'Charles Möeller & Claudio Botelho',
    musicalDirection: 'Marcelo Castro',
    choreography: 'Dalal Achcar',
  },

  minPrice: 120,

  similarEvents: [
    {
      id: 'similar-1',
      title: 'Espelho Mágico',
      image: require('../assets/home/disney-princesa.png'),
    },
    {
      id: 'similar-2',
      title: 'Mamma Mia',
      image: require('../assets/home/mamma-mia.png'),
    },
    {
      id: 'similar-3',
      title: 'PIAF',
      image: require('../assets/home/ballet.png'),
    },
  ],
};
