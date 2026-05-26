// data/mockHomeData.js

export const heroBanners = [
  {
    id: 'banner-1',
    title: 'A Noviça Rebelde',
    image: require('../assets/home/banner-novica.png'),
  },
  {
    id: 'banner-2',
    title: 'Mamma Mia',
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
];

export const trendingEvents = [
  {
    id: 'trending-1',
    title: 'Viva - In Concert',
    categoryId: 'musical',
    location: 'Teatro Riachuelo',
    image: require('../assets/home/viva.png'),
  },
  {
    id: 'trending-2',
    title: 'Ballet Dalal Achcar',
    categoryId: 'ballet',
    location: 'Teatro Riachuelo',
    image: require('../assets/home/ballet.png'),
  },
  {
    id: 'trending-3',
    title: 'Hair',
    categoryId: 'musical',
    location: 'Teatro Riachuelo',
    image: require('../assets/home/hair.png'),
  },
];

export const newEvents = [
  {
    id: 'new-1',
    title: 'Viva - In Concert',
    categoryId: 'musical',
    location: 'Teatro Riachuelo',
    tag: 'Lançamento',
    image: require('../assets/home/viva.png'),
  },
  {
    id: 'new-2',
    title: 'Ballet Dalal Achcar',
    categoryId: 'ballet',
    location: 'Teatro Riachuelo',
    tag: 'Breve',
    image: require('../assets/home/ballet.png'),
  },
  {
    id: 'new-3',
    title: 'Hair',
    categoryId: 'musical',
    location: 'Teatro Riachuelo',
    image: require('../assets/home/hair.png'),
  },
];

export const recommendedEvents = [
  {
    id: 'rec-1',
    title: 'Mamma Mia',
    categoryId: 'musical',
    location: 'Aventura Teatros',
    image: require('../assets/home/mamma-mia.png'),
  },
  {
    id: 'rec-2',
    title: 'Disney Princesa',
    categoryId: 'infantil',
    location: 'Aventura Teatros',
    image: require('../assets/home/disney-princesa.png'),
  },
];

export const nearbyEvents = [];

export const theaters = [
  {
    id: 'theater-1',
    name: 'Teatro Riachuelo',
    location: 'Centro, Rio de Janeiro',
    image: require('../assets/home/teatro-riachuelo.png'),
  },
  {
    id: 'theater-2',
    name: 'BTG Pactual Hall',
    location: 'Vila Olímpia, São Paulo',
    image: require('../assets/home/btg-pactual-hall.png'),
  },
  {
    id: 'theater-3',
    name: 'Teatro Renault',
    location: 'Pinheiros, São Paulo',
    image: require('../assets/home/teatro-renault.png'),
  },
];