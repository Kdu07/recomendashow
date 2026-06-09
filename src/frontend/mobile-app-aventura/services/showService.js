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

function read(dto, pascal, camel) {
  return dto?.[pascal] ?? dto?.[camel];
}

function getShowImage(dto) {
  const name = `${read(dto, 'Nome', 'nome') ?? ''} ${read(dto, 'Genero', 'genero') ?? ''} ${read(dto, 'TipoEvento', 'tipoEvento') ?? ''}`;
  const match = IMAGE_MAP.find((item) => item.pattern.test(name));
  return match?.image ?? DEFAULT_IMAGE;
}

function getCategoryId(dto) {
  const genre = String(read(dto, 'Genero', 'genero') ?? read(dto, 'TipoEvento', 'tipoEvento') ?? '').toLowerCase();

  if (genre.includes('musical')) return 'musical';
  if (genre.includes('drama')) return 'drama';
  if (genre.includes('infantil')) return 'infantil';
  if (genre.includes('ballet')) return 'ballet';
  if (genre.includes('comedia') || genre.includes('comÃ©dia')) return 'comedia';

  return 'all';
}

function formatCurrency(value) {
  if (value === null || value === undefined) return '';
  return `R$ ${Number(value).toFixed(2).replace('.', ',')}`;
}

function formatDateStatus(startDate, endDate, emCartaz) {
  if (!startDate) return emCartaz ? 'Em cartaz' : 'Data indisponivel';

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
  if (!response.ok) throw new Error('Nao foi possivel carregar os espetaculos');
  return response.json();
}

export async function fetchShowById(showId) {
  const response = await apiFetch(`/api/Espetaculo/${showId}`);
  if (!response.ok) throw new Error('Espetaculo nao encontrado');
  return response.json();
}

export async function searchShows(nome, genero) {
  const params = new URLSearchParams();
  if (nome) params.append('nome', nome);
  if (genero && genero !== 'all') params.append('genero', genero);
  const query = params.toString() ? `?${params.toString()}` : '';
  const response = await apiFetch(`/api/Espetaculo/search${query}`);
  if (!response.ok) throw new Error('Erro ao buscar espetaculos');
  return response.json();
}

export function mapDtoToHomeEvent(dto) {
  const id = read(dto, 'Id', 'id');
  const title = read(dto, 'Nome', 'nome');
  const genero = read(dto, 'Genero', 'genero');
  const tipoEvento = read(dto, 'TipoEvento', 'tipoEvento');

  return {
    id,
    showId: id,
    title,
    categoryId: getCategoryId(dto),
    city: '',
    location: genero || tipoEvento || '',
    image: getShowImage(dto),
  };
}

export function mapDtoToShowInfo(dto) {
  const id = read(dto, 'Id', 'id');
  const title = read(dto, 'Nome', 'nome');
  const genero = read(dto, 'Genero', 'genero');
  const tipoEvento = read(dto, 'TipoEvento', 'tipoEvento');
  const dataInicio = read(dto, 'DataInicio', 'dataInicio');
  const dataFim = read(dto, 'DataFim', 'dataFim');
  const emCartaz = read(dto, 'EmCartaz', 'emCartaz');
  const descricao = read(dto, 'Descricao', 'descricao');
  const valorIngresso = read(dto, 'ValorIngresso', 'valorIngresso');
  const urlBilheteria = read(dto, 'UrlBilheteria', 'urlBilheteria');

  return {
    id,
    title,
    heroImage: getShowImage(dto),
    isFavorited: false,
    date: {
      label: 'DATA E HORARIO',
      status: formatDateStatus(dataInicio, dataFim, emCartaz),
      detail: dataInicio ? `Inicio: ${new Date(dataInicio).toLocaleDateString('pt-BR')}` : '',
    },
    location: {
      label: 'LOCAL',
      venue: tipoEvento || genero || 'Espetaculo',
      address: '',
      city: '',
      mapImage: DEFAULT_IMAGE,
      venueUrl: urlBilheteria || '',
    },
    about: {
      sectionLabel: 'SOBRE O ESPETACULO',
      description: descricao || 'Descricao nao disponivel.',
      duration: dataInicio ? `Inicio: ${new Date(dataInicio).toLocaleDateString('pt-BR')}` : '',
    },
    creativeTeam: {
      sectionLabel: 'EQUIPE CRIATIVA',
      members: [],
    },
    recommendations: [],
    ticket: {
      priceLabel: 'A partir de',
      price: formatCurrency(valorIngresso),
      buttonLabel: 'INGRESSOS',
      url: urlBilheteria || '',
    },
  };
}
