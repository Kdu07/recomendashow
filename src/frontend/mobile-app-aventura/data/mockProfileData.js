import {
  UserProfileModel,
  ProfileMenuItemModel,
} from '../models/profileModels';

export const userProfile = {
  ...UserProfileModel,

  id: '1',
  name: 'Ana Luiza',
  email: 'ana.luiza@gmail.com',
  birthDate: '01/01/2000',
  cep: '01310-100',
  street: 'Avenida Paulista',
  number: '1000',
  complement: 'Apto 12',
  neighborhood: 'Bela Vista',
  city: 'São Paulo',
};
