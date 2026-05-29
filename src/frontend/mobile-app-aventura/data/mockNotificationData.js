import {
  NotificationOptionModel,
  NotificationSectionModel,
} from '../models/notificationModels';

export const notificationSections = [
  {
    ...NotificationSectionModel,

    id: 'channels',

    title: 'CANAIS DE NOTIFICAÇÃO',

    options: [
      {
        ...NotificationOptionModel,

        id: 'push',

        title: 'Notificações Push',

        subtitle: 'Alertas no dispositivo',

        enabled: true,
      },

      {
        ...NotificationOptionModel,

        id: 'email',

        title: 'Notificações por Email',

        subtitle: 'Receber emails informativos',

        enabled: true,
      },
    ],
  },

  {
    ...NotificationSectionModel,

    id: 'events',

    title: 'ESPETÁCULOS E EVENTOS',

    options: [
      {
        ...NotificationOptionModel,

        id: 'new-events',

        title: 'Novos Espetáculos',

        subtitle: 'Estreias e lançamentos',

        enabled: true,
      },

      {
        ...NotificationOptionModel,

        id: 'recommendations',

        title: 'Recomendações Personalizadas',

        subtitle: 'Baseado nos seus gostos',

        enabled: true,
      },

      {
        ...NotificationOptionModel,

        id: 'wishlist',

        title: 'Espetáculos Salvos',

        subtitle: 'Atualizações da Wishlist',

        enabled: true,
      },
    ],
  },

  {
    ...NotificationSectionModel,

    id: 'marketing',

    title: 'MARKETING E NOVIDADES',

    options: [
      {
        ...NotificationOptionModel,

        id: 'newsletter',

        title: 'Newsletter',

        subtitle: 'Conteúdo semanal sobre cultura',

        enabled: false,
      },

      {
        ...NotificationOptionModel,

        id: 'updates',

        title: 'Atualizações do App',

        subtitle: 'Novos recursos e melhorias',

        enabled: true,
      },
    ],
  },
];