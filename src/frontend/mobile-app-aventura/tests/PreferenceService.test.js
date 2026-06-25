import { fetchPreferences } from '../services/preferenceService';
import { apiFetch } from '../services/apiClient';

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
}));

function okJson(data) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(data),
  };
}

describe('preferenceService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(console, 'warn').mockImplementation(() => {});
  });

  afterEach(() => {
    console.warn.mockRestore();
  });

  describe('fetchPreferences', () => {
    test('returns null without calling the API when userId is falsy', async () => {
      await expect(fetchPreferences(null)).resolves.toBeNull();
      await expect(fetchPreferences(undefined)).resolves.toBeNull();
      await expect(fetchPreferences('')).resolves.toBeNull();

      expect(apiFetch).not.toHaveBeenCalled();
    });

    test('calls the preferencias endpoint with the user id', async () => {
      apiFetch.mockResolvedValue(okJson({ generos: ['Musical'] }));

      await fetchPreferences('user-1');

      expect(apiFetch).toHaveBeenCalledWith('/api/Preferencias/user-1');
    });

    test('returns the parsed preferences on success', async () => {
      const data = { generos: ['Musical'], cidade: 'São Paulo' };
      apiFetch.mockResolvedValue(okJson(data));

      await expect(fetchPreferences('user-1')).resolves.toEqual(data);
    });

    test('returns null when the response is a 404', async () => {
      apiFetch.mockResolvedValue({ ok: false, status: 404 });

      await expect(fetchPreferences('user-1')).resolves.toBeNull();
    });

    test('returns null and warns when the response is a non-404 error', async () => {
      apiFetch.mockResolvedValue({ ok: false, status: 500 });

      await expect(fetchPreferences('user-1')).resolves.toBeNull();
      expect(console.warn).toHaveBeenCalled();
    });

    test('returns null and warns when fetch rejects', async () => {
      apiFetch.mockRejectedValue(new Error('network down'));

      await expect(fetchPreferences('user-1')).resolves.toBeNull();
      expect(console.warn).toHaveBeenCalled();
    });
  });
});