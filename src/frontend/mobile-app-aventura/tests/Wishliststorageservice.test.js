/**
 * @file wishlistStorageService.test.js
 * Testes unitários para wishlistStorageService
 *
 * AsyncStorage é mockado via jest.mock para simular armazenamento
 * sem dependência de ambiente nativo.
 */

import {
  getStoredWishlist,
  saveStoredWishlist,
  addEventToWishlist,
  removeEventFromWishlist,
  toggleEventInWishlist,
  isEventInWishlist,
} from '../services/wishlistStorageService';

// ── Mock de AsyncStorage ──────────────────────────────────────────────────────

const storage = {};

jest.mock('@react-native-async-storage/async-storage', () => ({
  getItem: jest.fn((key) => Promise.resolve(storage[key] ?? null)),
  setItem: jest.fn((key, value) => {
    storage[key] = value;
    return Promise.resolve();
  }),
  removeItem: jest.fn((key) => {
    delete storage[key];
    return Promise.resolve();
  }),
}));

const EVENT_A = { id: 'a', title: 'Mamma Mia' };
const EVENT_B = { id: 'b', title: 'Hamlet' };

beforeEach(() => {
  // limpa storage in-memory e todos os mocks
  Object.keys(storage).forEach((k) => delete storage[k]);
  jest.clearAllMocks();
});

// ── getStoredWishlist ─────────────────────────────────────────────────────────

describe('getStoredWishlist', () => {
  test('retorna array vazio quando não há dados armazenados', async () => {
    const result = await getStoredWishlist();
    expect(result).toEqual([]);
  });

  test('retorna os eventos armazenados corretamente', async () => {
    storage['@aventura:wishlist'] = JSON.stringify([EVENT_A]);
    const result = await getStoredWishlist();
    expect(result).toEqual([EVENT_A]);
  });
});

// ── saveStoredWishlist ────────────────────────────────────────────────────────

describe('saveStoredWishlist', () => {
  test('persiste os eventos e retorna true', async () => {
    const ok = await saveStoredWishlist([EVENT_A, EVENT_B]);
    expect(ok).toBe(true);
    const stored = JSON.parse(storage['@aventura:wishlist']);
    expect(stored).toEqual([EVENT_A, EVENT_B]);
  });
});

// ── addEventToWishlist ────────────────────────────────────────────────────────

describe('addEventToWishlist', () => {
  test('adiciona evento à wishlist vazia', async () => {
    const result = await addEventToWishlist(EVENT_A);
    expect(result).toContainEqual(EVENT_A);
  });

  test('não duplica evento já existente', async () => {
    await addEventToWishlist(EVENT_A);
    const result = await addEventToWishlist(EVENT_A);
    expect(result.filter((e) => e.id === 'a')).toHaveLength(1);
  });

  test('adiciona múltiplos eventos distintos', async () => {
    await addEventToWishlist(EVENT_A);
    const result = await addEventToWishlist(EVENT_B);
    expect(result).toHaveLength(2);
  });
});

// ── removeEventFromWishlist ───────────────────────────────────────────────────

describe('removeEventFromWishlist', () => {
  test('remove evento existente pelo id', async () => {
    await addEventToWishlist(EVENT_A);
    await addEventToWishlist(EVENT_B);
    const result = await removeEventFromWishlist('a');
    expect(result.find((e) => e.id === 'a')).toBeUndefined();
    expect(result).toHaveLength(1);
  });

  test('não lança erro ao remover id inexistente', async () => {
    await addEventToWishlist(EVENT_A);
    const result = await removeEventFromWishlist('z');
    expect(result).toHaveLength(1);
  });
});

// ── toggleEventInWishlist ─────────────────────────────────────────────────────

describe('toggleEventInWishlist', () => {
  test('adiciona evento quando não está na wishlist', async () => {
    const result = await toggleEventInWishlist(EVENT_A);
    expect(result).toContainEqual(EVENT_A);
  });

  test('remove evento quando já está na wishlist', async () => {
    await addEventToWishlist(EVENT_A);
    const result = await toggleEventInWishlist(EVENT_A);
    expect(result.find((e) => e.id === 'a')).toBeUndefined();
  });

  test('toggle duas vezes restaura o estado original', async () => {
    await toggleEventInWishlist(EVENT_A);
    const result = await toggleEventInWishlist(EVENT_A);
    expect(result.find((e) => e.id === 'a')).toBeUndefined();
  });
});

// ── isEventInWishlist ─────────────────────────────────────────────────────────

describe('isEventInWishlist', () => {
  test('retorna true para evento salvo', async () => {
    await addEventToWishlist(EVENT_A);
    expect(await isEventInWishlist('a')).toBe(true);
  });

  test('retorna false para evento não salvo', async () => {
    expect(await isEventInWishlist('z')).toBe(false);
  });
});