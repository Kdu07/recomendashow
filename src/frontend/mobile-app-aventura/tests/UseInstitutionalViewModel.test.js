import { renderHook } from '@testing-library/react';

import useInstitutionalViewModel from '../viewmodels/useInstitutionalViewModel';
import {
  socialLinks,
  contactInfo,
  aboutText,
} from '../data/mockInstitutionalData';

describe('useInstitutionalViewModel', () => {
  beforeEach(() => {
    jest.spyOn(console, 'log').mockImplementation(() => {});
  });

  afterEach(() => {
    console.log.mockRestore();
  });

  test('exposes the institutional data as-is', () => {
    const { result } = renderHook(() => useInstitutionalViewModel());

    expect(result.current.aboutText).toBe(aboutText);
    expect(result.current.socialLinks).toBe(socialLinks);
    expect(result.current.contactInfo).toBe(contactInfo);
  });

  test('logs the platform when handleSocialPress is called', () => {
    const { result } = renderHook(() => useInstitutionalViewModel());

    result.current.handleSocialPress({ platform: 'Instagram' });

    expect(console.log).toHaveBeenCalledWith('Abrir rede social:', 'Instagram');
  });
});