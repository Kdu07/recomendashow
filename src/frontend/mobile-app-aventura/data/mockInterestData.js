import {
  InterestModel,
  CityModel,
} from '../models/interestModels';

export const interests = [
  {
    ...InterestModel,
    id: 'musical',
    label: 'Musical',
    selected: true,
  },

  {
    ...InterestModel,
    id: 'drama',
    label: 'Drama',
    selected: true,
  },

  {
    ...InterestModel,
    id: 'ballet',
    label: 'Ballet',
  },

  {
    ...InterestModel,
    id: 'infantil',
    label: 'Infantil',
  },

  {
    ...InterestModel,
    id: 'corporativo',
    label: 'Corporativo',
  },

  {
    ...InterestModel,
    id: 'comedia',
    label: 'Comédia',
  },

  {
    ...InterestModel,
    id: 'danca',
    label: 'Dança',
    selected: true,
  },
];

export const cities = [
  {
    ...CityModel,
    id: 'sp',
    label: 'São Paulo',
  },

  {
    ...CityModel,
    id: 'rj',
    label: 'Rio de Janeiro',
    selected: true,
  },

  {
    ...CityModel,
    id: 'other',
    label: 'Outra',
  },
];
