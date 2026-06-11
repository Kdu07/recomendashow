import { apiFetch } from './apiClient';

const DEFAULT_IMAGE = require('../assets/home/teatro-riachuelo.png');

const THEATER_IMAGE_MAP = [
  { pattern: /riachuelo/i, image: require('../assets/home/teatro-riachuelo.png') },
  { pattern: /totalenergies|bloch/i, image: require('../assets/home/teatro-totalenergies.webp') },
  { pattern: /ecovilla|ri happy/i, image: require('../assets/home/ecovilla-riachuelo.webp') },
  { pattern: /arena b3/i, image: require('../assets/home/arena-b3.jpg') },
  { pattern: /btg pactual/i, image: require('../assets/home/btg-pactual-hall.png') },
  { pattern: /youtube|eva herz/i, image: require('../assets/home/teatro-youtube.avif') },
];

function read(dto, pascal, camel) {
  return dto?.[pascal] ?? dto?.[camel];
}

function getTheaterImage(name) {
  const match = THEATER_IMAGE_MAP.find((item) => item.pattern.test(name ?? ''));
  return match?.image ?? DEFAULT_IMAGE;
}

export async function fetchTheaters() {
  const response = await apiFetch('/api/Teatro');
  if (!response.ok) throw new Error('Nao foi possivel carregar os teatros');
  return response.json();
}

export async function fetchTheaterShows(theaterId) {
  const response = await apiFetch(`/api/Teatro/${theaterId}/espetaculos`);
  if (!response.ok) throw new Error('Nao foi possivel carregar os espetaculos do teatro');
  return response.json();
}

export function mapDtoToTheater(dto) {
  const id = read(dto, 'Id', 'id');
  const name = read(dto, 'Nome', 'nome');
  const city = read(dto, 'Cidade', 'cidade');
  const location = read(dto, 'Endereco', 'endereco');
  const openingHours = read(dto, 'HorarioFuncionamento', 'horarioFuncionamento');

  return {
    id,
    name,
    city,
    location,
    image: getTheaterImage(name),
    capacity: 'Consultar lotacao local',
    description: openingHours
      ? `Horario de funcionamento: ${openingHours}`
      : 'Detalhes do teatro indisponiveis no momento.',
    highlights: [
      city ? `Localizado em ${city}` : 'Cidade nao informada',
      location || 'Endereco nao informado',
      openingHours ? `Aberto: ${openingHours}` : 'Aberto ao publico',
    ],
  };
}
