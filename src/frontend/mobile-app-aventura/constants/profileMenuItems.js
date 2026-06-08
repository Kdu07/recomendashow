import { ProfileMenuItemModel } from '../models/profileModels';

export const profileMenuItems = [
  {
    ...ProfileMenuItemModel,
    id: 'preferences',
    title: 'Preferências e cidade',
    subtitle: 'Musical, Teatro, Dança',
    icon: 'heart-outline',
  },
  {
    ...ProfileMenuItemModel,
    id: 'notifications',
    title: 'Notificações',
    subtitle: 'Gerenciar alertas e e-mails',
    icon: 'notifications-outline',
  },
  {
    ...ProfileMenuItemModel,
    id: 'institutional',
    title: 'Informações Institucionais',
    subtitle: 'Conheça a Aventura',
    icon: 'information-circle-outline',
  },
];
