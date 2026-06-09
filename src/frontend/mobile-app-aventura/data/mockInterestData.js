import {
  InterestModel,
  CityModel,
} from '../models/interestModels';

export const interests = [
  {
    ...InterestModel,
    id: '0ac88c96-1f1d-48dd-bc9d-9b08c00bdc65',
    label: 'Musical',
    selected: true,
  },

  {
    ...InterestModel,
    id: '18090fea-dfe3-447e-bdae-f1daf377b7d5',
    label: 'Drama',
    selected: true,
  },

  {
    ...InterestModel,
    id: '7c6060fe-c0d6-4822-aa72-bf55073215c7',
    label: 'Ballet',
  },

  {
    ...InterestModel,
    id: '3f7d09eb-98fb-4a23-b3c5-95227c7f80b6',
    label: 'Infantil',
  },

  {
    ...InterestModel,
    id: 'e45fd58c-2567-4da5-98fa-5f11d17466d0',
    label: 'Comédia',
  },

  {
    ...InterestModel,
    id: 'acdb1941-91d2-4b63-9a45-74d737d91947',
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
