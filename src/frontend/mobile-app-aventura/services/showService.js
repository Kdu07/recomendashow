import { apiFetch } from './apiClient';

const DEFAULT_IMAGE = require('../assets/home/amazonica.jpeg');

const IMAGE_MAP = [
  { pattern: /mamma/i, image: require('../assets/home/mamma-mia.png') },
  { pattern: /viva/i, image: require('../assets/home/viva.png') },
  { pattern: /hair/i, image: require('../assets/home/hair.png') },
  { pattern: /frankenstein/i, image: require('../assets/home/frankenstein.webp') },
  { pattern: /zaquim/i, image: require('../assets/home/zaquim.jpeg') },
  { pattern: /disney/i, image: require('../assets/home/disney-princesa.png') },
  { pattern: /ballet/i, image: require('../assets/home/ballet.png') },
  { pattern: /hip hop|hip-hop|hamlet/i, image: require('../assets/home/hip-hop-hamlet.jpeg') },
];

function getShowImage(dto) {
  const name = `${dto?.Nome ?? ''} ${dto?.Genero ?? ''} ${dto?.TipoEvento ?? ''}`;

  const match = IMAGE_MAP.find((item) => item.pattern.test(name));
  return match?.image ?? DEFAULT_IMAGE;
}

function getCategoryId(dto) {
  const genre = (dto?.Genero ?? dto?.TipoEvento ?? '').toLowerCase();

  if (genre.includes('musical')) return 'musical';
  if (genre.includes('drama')) return 'drama';
  if (genre.includes('infantil')) return 'infantil';
  if (genre.includes('ballet')) return 'ballet';
  if (genre.includes('comédia') || genre.includes('comedia')) return 'comédia';

  return 'all';
}

function formatCurrency(value) {
  if (value === null || value === undefined) {
    return '';
  }

  return `R$ ${Number(value).toFixed(2).replace('.', ',')}`;
}

function formatDateStatus(startDate, endDate, emCartaz) {
  if (!startDate) {
    return emCartaz ? 'Em cartaz' : 'Data indisponível';
  }

  const start = new Date(startDate);
  const end = endDate ? new Date(endDate) : null;
  const formattedStart = start.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: 'long',
  });

  if (emCartaz && end) {
    const formattedEnd = end.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'long',
    });

    return `Em cartaz de ${formattedStart} a ${formattedEnd}`;
  }

  return `A partir de ${formattedStart}`;
}

export async function fetchShows() {
  const response = await apiFetch('/api/Espetaculo');

  if (!response.ok) {
    throw new Error('Não foi possível carregar os espetáculos');
  }

  return response.json();
}

export async function fetchShowById(showId) {
  const response = await apiFetch(`/api/Espetaculo/${showId}`);

  if (!response.ok) {
    throw new Error('Espetáculo não encontrado');
  }

  return response.json();
}

export function mapDtoToHomeEvent(dto) {
  return {
    id: dto.Id,
    showId: dto.Id,
    title: dto.Nome,
    categoryId: getCategoryId(dto),
    city: '',
    location: dto.Genero || dto.TipoEvento || '',
    image: getShowImage(dto),
  };
}

export function mapDtoToShowInfo(dto) {
  return {
    id: dto.Id,
    title: dto.Nome,
    heroImage: getShowImage(dto),
    isFavorited: false,
    date: {
      label: 'DATA E HORÁRIO',
      status: formatDateStatus(dto.DataInicio, dto.DataFim, dto.EmCartaz),
      detail: dto.DataInicio ? `Início: ${new Date(dto.DataInicio).toLocaleDateString('pt-BR')}` : '',
    },
    location: {
      label: 'LOCAL',
      venue: dto.TipoEvento || dto.Genero || 'Espetáculo',
      address: '',
      city: '',
      mapImage: DEFAULT_IMAGE,
      venueUrl: dto.UrlBilheteria || '',
    },
    about: {
      sectionLabel: 'SOBRE O ESPETÁCULO',
      description: dto.Descricao || 'Descrição não disponível.',
      duration: dto.DataInicio
        ? `Início: ${new Date(dto.DataInicio).toLocaleDateString('pt-BR')}`
        : '',
    },
    creativeTeam: {
      sectionLabel: 'EQUIPE CRIATIVA',
      members: [],
    },
    recommendations: [],
    ticket: {
      priceLabel: 'A partir de',
      price: formatCurrency(dto.ValorIngresso),
      buttonLabel: 'INGRESSOS',
      url: dto.UrlBilheteria || '',
    },
  };
}
