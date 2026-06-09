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

function getTheaterImage(name) {
  const match = THEATER_IMAGE_MAP.find((item) => item.pattern.test(name));
  return match?.image ?? DEFAULT_IMAGE;
}

export async function fetchTheaters() {
  const response = await apiFetch('/api/Teatro');

  if (!response.ok) {
    throw new Error('Não foi possível carregar os teatros');
  }

  return response.json();
}

export function mapDtoToTheater(dto) {
  return {
    id: dto.Id,
    name: dto.Nome,
    city: dto.Cidade,
    location: dto.Endereco,
    image: getTheaterImage(dto.Nome),
    capacity: 'Consultar lotação local', // Fallback, we don't have capacity in backend
    description: dto.HorarioFuncionamento ? `Horário de Funcionamento: ${dto.HorarioFuncionamento}` : 'Detalhes do teatro indisponíveis no momento.',
    highlights: [
      `Localizado em ${dto.Cidade}`,
      dto.Endereco,
      dto.HorarioFuncionamento ? `Aberto: ${dto.HorarioFuncionamento}` : 'Aberto ao público'
    ]
  };
}
