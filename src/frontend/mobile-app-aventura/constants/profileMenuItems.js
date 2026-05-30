import { ProfileMenuItemModel } from '../models/profileModels';

export const profileMenuItems = [
  {
    ...ProfileMenuItemModel,

    id: 'preferences',

    title: 'Preferências e cidade',

    subtitle: 'Musical, Teatro, Dança',

    icon: '♡',
  },

  {
    ...ProfileMenuItemModel,

    id: 'notifications',

    title: 'Notificações',

    subtitle: 'Gerenciar alertas e emails',

    icon: '◔',
  },

  {
    ...ProfileMenuItemModel,

    id: 'institutional',

    title: 'Informações Institucionais',

    subtitle: 'Conheça a Aventura',

    icon: 'ⓘ',
  },
];