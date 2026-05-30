import { useMemo } from 'react';

import {
  getInstitutionalData,
} from '../repositories/institutionalRepository';

export default function useInstitutionalViewModel() {
  const data = useMemo(
    () => getInstitutionalData(),
    []
  );

  function handleSocialPress(link) {
    console.log(
      'Abrir rede social:',
      link.platform
    );
  }

  return {
    aboutText: data.aboutText,
    socialLinks: data.socialLinks,
    contactInfo: data.contactInfo,

    handleSocialPress,
  };
}