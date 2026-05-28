import {
  SocialLinkModel,
  ContactInfoModel,
} from '../models/institutionalModels';

export const socialLinks = [
  {
    ...SocialLinkModel,

    id: 'facebook',

    platform: 'Facebook',

    username: '@aventurateatros',

    icon: 'f',

    url: 'https://facebook.com',
  },

  {
    ...SocialLinkModel,

    id: 'instagram',

    platform: 'Instagram',

    username: '@aventurateatros',

    icon: '◎',

    url: 'https://instagram.com',
  },

  {
    ...SocialLinkModel,

    id: 'linkedin',

    platform: 'LinkedIn',

    username: 'Aventura Teatros',

    icon: 'in',

    url: 'https://linkedin.com',
  },
];

export const contactInfo = [
  {
    ...ContactInfoModel,

    id: 'email',

    label: 'Email de contato',

    value: 'comercial@aventurateatros.com.br',

    icon: '✉',
  },

  {
    ...ContactInfoModel,

    id: 'website',

    label: 'Site oficial',

    value: 'aventurateatros.com.br',

    icon: '◉',
  },

  {
    ...ContactInfoModel,

    id: 'location',

    label: 'Localização',

    value:
      'Rua México, nº 11 – 7º andar – Centro\nRio de Janeiro – RJ\nCEP: 20.031-144',

    icon: '⌖',
  },
];

export const aboutText = `
A Aventura é uma produtora de espetáculos culturais fundada em 2008. Somos referência na produção de musicais, danças e experiências teatrais de alta qualidade.

Nossa missão é democratizar o acesso à cultura e promover excelência artística no Brasil.
`;