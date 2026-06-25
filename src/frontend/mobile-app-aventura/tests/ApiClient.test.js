import { apiFetch, API_BASE } from '../services/apiClient';
import { clearAuthStorage, getAuthToken } from '../services/tokenStorageService';
import { handleSessionExpired } from '../navigation/navigationRef';

jest.mock('../services/tokenStorageService', () => ({
  clearAuthStorage: jest.fn(),
  getAuthToken: jest.fn(),
}));

jest.mock('../navigation/navigationRef', () => ({
  handleSessionExpired: jest.fn(),
}));

function okResponse(status = 200) {
  return { ok: status >= 200 && status < 300, status };
}

describe('apiClient', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    global.fetch = jest.fn();
    getAuthToken.mockResolvedValue(null);
  });

  describe('API_BASE', () => {
    test('is a non-empty string URL', () => {
      expect(typeof API_BASE).toBe('string');
      expect(API_BASE.startsWith('http')).toBe(true);
    });

    test('resolves to the production URL when __DEV__ is false', () => {
      const originalDev = global.__DEV__;
      global.__DEV__ = false;

      let prodApiBase;
      jest.isolateModules(() => {
        prodApiBase = require('../services/apiClient').API_BASE;
      });

      expect(prodApiBase).toBe('http://localhost:5253');

      global.__DEV__ = originalDev;
    });
  });

  describe('apiFetch', () => {
    test('calls fetch with the API_BASE prefix and the given path', async () => {
      global.fetch.mockResolvedValue(okResponse(200));

      await apiFetch('/api/Espetaculo');

      expect(global.fetch).toHaveBeenCalledWith(
        `${API_BASE}/api/Espetaculo`,
        expect.objectContaining({ headers: expect.any(Object) })
      );
    });

    test('does not set an Authorization header when there is no token', async () => {
      global.fetch.mockResolvedValue(okResponse(200));
      getAuthToken.mockResolvedValue(null);

      await apiFetch('/api/Espetaculo');

      const [, options] = global.fetch.mock.calls[0];
      expect(options.headers.Authorization).toBeUndefined();
    });

    test('sets a Bearer Authorization header when a token is available', async () => {
      global.fetch.mockResolvedValue(okResponse(200));
      getAuthToken.mockResolvedValue('my-token');

      await apiFetch('/api/Espetaculo');

      const [, options] = global.fetch.mock.calls[0];
      expect(options.headers.Authorization).toBe('Bearer my-token');
    });

    test('sets Content-Type application/json when a body is provided and no Content-Type is set', async () => {
      global.fetch.mockResolvedValue(okResponse(200));

      await apiFetch('/api/Usuario/1', { method: 'PUT', body: JSON.stringify({ nome: 'Ana' }) });

      const [, options] = global.fetch.mock.calls[0];
      expect(options.headers['Content-Type']).toBe('application/json');
    });

    test('does not override an existing Content-Type header', async () => {
      global.fetch.mockResolvedValue(okResponse(200));

      await apiFetch('/api/Usuario/1', {
        method: 'PUT',
        body: 'raw',
        headers: { 'Content-Type': 'text/plain' },
      });

      const [, options] = global.fetch.mock.calls[0];
      expect(options.headers['Content-Type']).toBe('text/plain');
    });

    test('preserves custom headers passed through options', async () => {
      global.fetch.mockResolvedValue(okResponse(200));

      await apiFetch('/api/Espetaculo', { headers: { 'X-Custom': 'value' } });

      const [, options] = global.fetch.mock.calls[0];
      expect(options.headers['X-Custom']).toBe('value');
    });

    test('returns the fetch response', async () => {
      const response = okResponse(200);
      global.fetch.mockResolvedValue(response);

      await expect(apiFetch('/api/Espetaculo')).resolves.toBe(response);
    });

    test('does nothing on 401 when there was no token (already anonymous)', async () => {
      global.fetch.mockResolvedValue(okResponse(401));
      getAuthToken.mockResolvedValue(null);

      await apiFetch('/api/Espetaculo');

      expect(clearAuthStorage).not.toHaveBeenCalled();
      expect(handleSessionExpired).not.toHaveBeenCalled();
    });

    test('does nothing on 401 for auth paths even with a token', async () => {
      global.fetch.mockResolvedValue(okResponse(401));
      getAuthToken.mockResolvedValue('expired-token');

      await apiFetch('/api/auth/login', { method: 'POST' });

      expect(clearAuthStorage).not.toHaveBeenCalled();
      expect(handleSessionExpired).not.toHaveBeenCalled();
    });

    test('clears auth storage and redirects on 401 with an existing token on a protected path', async () => {
      global.fetch.mockResolvedValue(okResponse(401));
      getAuthToken.mockResolvedValue('expired-token');

      await apiFetch('/api/Usuario/1');

      expect(clearAuthStorage).toHaveBeenCalledTimes(1);
      expect(handleSessionExpired).toHaveBeenCalledTimes(1);
    });

    test('treats /api/auth/register as an auth path too', async () => {
      global.fetch.mockResolvedValue(okResponse(401));
      getAuthToken.mockResolvedValue('some-token');

      await apiFetch('/api/auth/register', { method: 'POST' });

      expect(clearAuthStorage).not.toHaveBeenCalled();
    });
  });
});