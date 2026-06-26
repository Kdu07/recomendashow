import {
  saveAuthToken,
  getAuthToken,
  saveAuthSession,
  getAuthSession,
  clearAuthStorage,
  saveAuthData,
} from '../services/tokenStorageService';


const storage = {};

jest.mock('@react-native-async-storage/async-storage', () => ({
  setItem: jest.fn((key, value) => {
    storage[key] = value;
    return Promise.resolve();
  }),
  getItem: jest.fn((key) => Promise.resolve(storage[key] ?? null)),
  multiRemove: jest.fn((keys) => {
    keys.forEach((key) => delete storage[key]);
    return Promise.resolve();
  }),
}));

const AsyncStorage = require('@react-native-async-storage/async-storage');

describe('tokenStorageService', () => {
  beforeEach(() => {
    Object.keys(storage).forEach((key) => delete storage[key]);
    jest.clearAllMocks();
  });

  describe('saveAuthToken', () => {
    test('returns false and does not call AsyncStorage when token is falsy', async () => {
      await expect(saveAuthToken(null)).resolves.toBe(false);
      await expect(saveAuthToken('')).resolves.toBe(false);
      await expect(saveAuthToken(undefined)).resolves.toBe(false);

      expect(AsyncStorage.setItem).not.toHaveBeenCalled();
    });

    test('saves the token and returns true', async () => {
      await expect(saveAuthToken('abc123')).resolves.toBe(true);
      await expect(getAuthToken()).resolves.toBe('abc123');
    });

    test('returns false when AsyncStorage.setItem rejects', async () => {
      AsyncStorage.setItem.mockRejectedValueOnce(new Error('disk full'));

      await expect(saveAuthToken('abc123')).resolves.toBe(false);
    });
  });

  describe('getAuthToken', () => {
    test('returns null when there is nothing stored', async () => {
      await expect(getAuthToken()).resolves.toBeNull();
    });

    test('returns null when AsyncStorage.getItem rejects', async () => {
      AsyncStorage.getItem.mockRejectedValueOnce(new Error('read error'));

      await expect(getAuthToken()).resolves.toBeNull();
    });
  });

  describe('saveAuthSession', () => {
    test('returns false and does not call AsyncStorage when session is falsy', async () => {
      await expect(saveAuthSession(null)).resolves.toBe(false);

      expect(AsyncStorage.setItem).not.toHaveBeenCalled();
    });

    test('saves the session as JSON and returns true', async () => {
      const session = { usuarioId: '1', nome: 'Ana', email: 'ana@example.com' };

      await expect(saveAuthSession(session)).resolves.toBe(true);
      await expect(getAuthSession()).resolves.toEqual(session);
    });

    test('returns false when AsyncStorage.setItem rejects', async () => {
      AsyncStorage.setItem.mockRejectedValueOnce(new Error('disk full'));

      await expect(saveAuthSession({ usuarioId: '1' })).resolves.toBe(false);
    });
  });

  describe('getAuthSession', () => {
    test('returns null when there is nothing stored', async () => {
      await expect(getAuthSession()).resolves.toBeNull();
    });

    test('returns null when AsyncStorage.getItem rejects', async () => {
      AsyncStorage.getItem.mockRejectedValueOnce(new Error('read error'));

      await expect(getAuthSession()).resolves.toBeNull();
    });

    test('returns null when the stored value is not valid JSON', async () => {
      await saveAuthToken('whatever');
      storage['@aventura:auth_session'] = 'not-json{{{';

      await expect(getAuthSession()).resolves.toBeNull();
    });
  });

  describe('clearAuthStorage', () => {
    test('removes token and session and returns true', async () => {
      await saveAuthToken('abc123');
      await saveAuthSession({ usuarioId: '1' });

      await expect(clearAuthStorage()).resolves.toBe(true);
      await expect(getAuthToken()).resolves.toBeNull();
      await expect(getAuthSession()).resolves.toBeNull();
    });

    test('returns false when AsyncStorage.multiRemove rejects', async () => {
      AsyncStorage.multiRemove.mockRejectedValueOnce(new Error('remove failed'));

      await expect(clearAuthStorage()).resolves.toBe(false);
    });
  });

  describe('saveAuthData', () => {
    test('saves both token and session and returns true when both succeed', async () => {
      const result = await saveAuthData({
        token: 'abc123',
        usuarioId: '1',
        nome: 'Ana',
        email: 'ana@example.com',
      });

      expect(result).toBe(true);
      await expect(getAuthToken()).resolves.toBe('abc123');
      await expect(getAuthSession()).resolves.toEqual({
        usuarioId: '1',
        nome: 'Ana',
        email: 'ana@example.com',
      });
    });

    test('returns false when token is missing', async () => {
      const result = await saveAuthData({
        token: null,
        usuarioId: '1',
        nome: 'Ana',
        email: 'ana@example.com',
      });

      expect(result).toBe(false);
    });
  });
});