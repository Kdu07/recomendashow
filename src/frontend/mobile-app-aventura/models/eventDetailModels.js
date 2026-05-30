export const CastMemberModel = {
  id: '',
  name: '',
  role: '',
  avatar: null,
  isGroup: false,
  groupLabel: '',
};

export const CreativeTeamModel = {
  direction: '',
  musicalDirection: '',
  choreography: '',
};

export const EventDetailModel = {
  id: '',
  title: '',
  image: null,
  isWishlisted: false,

  status: '',        // 'active' | 'ended'
  dateLabel: '',     // ex: 'Temporada Encerrada'
  dateSubtitle: '',  // ex: 'Últimas apresentações: Julho de 2024'

  venue: '',
  address: '',
  city: '',

  description: '',
  duration: '',      // ex: '3h00 (180 minutos com intervalo)'

  cast: [],          // CastMemberModel[]
  creativeTeam: {},  // CreativeTeamModel

  minPrice: 0,
  similarEvents: [], // HomeEventModel[]
};
