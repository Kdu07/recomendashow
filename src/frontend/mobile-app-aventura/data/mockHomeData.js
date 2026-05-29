// data/mockHomeData.js

export const heroBanners = [
  {
    id: 'banner-1',
    title: 'A Noviça Rebelde',
    showId: '1',
    image: require('../assets/home/banner-novica.png'),
  },
  {
    id: 'banner-2',
    title: 'Mamma Mia',
    showId: '3',
    image: require('../assets/home/mamma-mia.jpg'),
  },
  {
    id: 'banner-3',
    title: 'Hair',
    image: require('../assets/home/hair_banner.jpg'),
  },
];

export const categories = [
  { id: 'all', label: 'Todos' },
  { id: 'musical', label: 'Musical' },
  { id: 'drama', label: 'Drama' },
  { id: 'infantil', label: 'Infantil' },
  { id: 'ballet', label: 'Ballet' },
  { id: 'comédia', label: 'Comédia' },
];

export const trendingEvents = [
  {
    id: 'trending-1',
    title: 'Viva - In Concert',
    showId: '2',
    categoryId: 'musical',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/viva.jpeg'),
  },
  {
    id: 'trending-2',
    title: 'Vozes Negras – A Força do Canto Feminino',
    categoryId: 'teatro-negro',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/vozes-negras.jpeg'),
  },
  {
    id: 'trending-3',
    title: 'Ballet Dalal Achcar',
    categoryId: 'ballet',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/ballet.png'),
  },
  {
    id: 'trending-4',
    title: 'Hip Hop Hamlet',
    categoryId: 'urbano',
    city: 'São Paulo',
    location: 'Teatro btg pactual hall',
    image: require('../assets/home/hip-hop-hamlet.jpeg'),
  },
  {
    id: 'trending-5',
    title: 'Hair',
    categoryId: 'musical',
    city: 'São Paulo',
    location: 'Teatro Renault',
    image: require('../assets/home/hair.png'),
  },
];

export const recommendedEvents = [
  {
    id: 'rec-1',
    title: 'Mamma Mia',
    showId: '3',
    categoryId: 'musical',
    city: 'São Paulo',
    location: 'Teatro btg pactual hall',
    image: require('../assets/home/mamma-mia.png'),
  },
  {
    id: 'rec-2',
    title: 'Zaquim',
    categoryId: 'infantil',
    city: 'Rio de Janeiro',
    location: 'EcoVilla Riachuelo',
    image: require('../assets/home/zaquim.jpeg'),
  },
  {
    id: 'rec-3',
    title: 'Disney Princesa',
    categoryId: 'infantil',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/disney-princesa.png'),
  },
  {
    id: 'rec-4',
    title: 'Frankenstein',
    categoryId: 'terror',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/frankenstein.webp'),
  },
  {
    id: 'rec-5',
    title: 'PIAF',
    categoryId: 'drama',
    city: 'São Paulo',
    location: 'Teatro Renault',
    image: require('../assets/home/piaf.jpeg'),
  },
];

export const newEvents = [
  {
    id: 'new-1',
    title: 'Vozes Negras – A Força do Canto Feminino',
    categoryId: 'teatro-negro',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    tag: 'Estreia',
    image: require('../assets/home/vozes-negras.jpeg'),
  },
  {
    id: 'new-2',
    title: 'Zaquim',
    categoryId: 'infantil',
    city: 'Rio de Janeiro',
    location: 'EcoVilla Riachuelo',
    tag: 'Lançamento',
    image: require('../assets/home/zaquim.jpeg'),
  },
  {
    id: 'new-3',
    title: 'Viva - In Concert',
    showId: '2',
    categoryId: 'musical',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    tag: 'Lançamento',
    image: require('../assets/home/viva.jpeg'),
  },
  {
    id: 'new-4',
    title: 'Hip Hop Hamlet',
    categoryId: 'urbano',
    city: 'São Paulo',
    location: 'Teatro btg pactual hall',
    tag: 'Breve',
    image: require('../assets/home/hip-hop-hamlet.jpeg'),
  },
  {
    id: 'new-5',
    title: 'Frankenstein',
    categoryId: 'terror',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    tag: 'Estreia',
    image: require('../assets/home/frankenstein.webp'),
  },
  {
    id: 'new-6',
    title: 'Elis, a Musical',
    categoryId: 'musical',
    city: 'São Paulo',
    location: 'Teatro Santander',
    tag: 'Sucesso',
    image: require('../assets/home/elis.jpeg'),
  },
];

export const nearbyEvents = [
  {
    id: 'nearby-1',
    title: 'Viva - In Concert',
    showId: '2',
    categoryId: 'musical',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/viva.jpeg'),
  },
  {
    id: 'nearby-2',
    title: 'Vozes Negras – A Força do Canto Feminino',
    categoryId: 'teatro-negro',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/vozes-negras.jpeg'),
  },
  {
    id: 'nearby-3',
    title: 'Frankenstein',
    categoryId: 'terror',
    city: 'Rio de Janeiro',
    location: 'Teatro Riachuelo Rio',
    image: require('../assets/home/frankenstein.webp'),
  },
  {
    id: 'nearby-4',
    title: 'Zaquim',
    categoryId: 'infantil',
    city: 'Rio de Janeiro',
    location: 'EcoVilla Riachuelo',
    image: require('../assets/home/zaquim.jpeg'),
  },
  {
    id: 'nearby-5',
    title: 'Espelho Mágico',
    categoryId: 'infantil',
    city: 'Rio de Janeiro',
    location: 'EcoVilla Riachuelo',
    image: require('../assets/home/espelho.jpeg'),
  },
];

export const theaters = [
  {
    id: 'theater-1',
    name: 'Teatro Riachuelo Rio',
    location: 'Centro, Rio de Janeiro',
    image: require('../assets/home/teatro-riachuelo.png'),
  },
  {
    id: 'theater-2',
    name: 'EcoVilla Riachuelo',
    location: 'Jardim Botânico, Rio de Janeiro',
    image: require('../assets/home/ecovilla-riachuelo.webp'),
  },
  {
    id: 'theater-3',
    name: 'Teatro btg pactual hall',
    location: 'Vila Olímpia, São Paulo',
    image: require('../assets/home/btg-pactual-hall.png'),
  },
  {
    id: 'theater-4',
    name: 'Teatro Renault',
    location: 'Bela Vista, São Paulo',
    image: require('../assets/home/teatro-renault.png'),
  },
];