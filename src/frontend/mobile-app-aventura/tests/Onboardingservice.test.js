/**
 * @file onboardingService.test.js
 * Testes unitários para onboardingService
 *
 * AsyncStorage é mockado para simular persistência local.
 */

import { onboardingService } from '../services/onboardingService';

// ── Mock de AsyncStorage ──────────────────────────────────────────────────────

const storage = {};

jest.mock('@react-native-async-storage/async-storage', () => ({
  setItem: jest.fn((key, value) => {
    storage[key] = value;
    return Promise.resolve();
  }),
  getItem: jest.fn((key) => Promise.resolve(storage[key] ?? null)),
  removeItem: jest.fn((key) => {
    delete storage[key];
    return Promise.resolve();
  }),
}));

beforeEach(() => {
  Object.keys(storage).forEach((k) => delete storage[k]);
  jest.clearAllMocks();
});

const PREFS = { categories: ['teatro'], city: 'sp', frequency: 'monthly' };

// ── save ──────────────────────────────────────────────────────────────────────

describe('onboardingService.save', () => {
  test('persiste as preferências no AsyncStorage como JSON', async () => {
    await onboardingService.save(PREFS);
    const stored = storage['@onboarding_preferences'];
    expect(JSON.parse(stored)).toEqual(PREFS);
  });

  test('sobrescreve preferências anteriores', async () => {
    await onboardingService.save(PREFS);
    const updated = { ...PREFS, city: 'rj' };
    await onboardingService.save(updated);
    const stored = JSON.parse(storage['@onboarding_preferences']);
    expect(stored.city).toBe('rj');
  });
});

// ── load ──────────────────────────────────────────────────────────────────────

describe('onboardingService.load', () => {
  test('retorna null quando nada foi salvo', async () => {
    const result = await onboardingService.load();
    expect(result).toBeNull();
  });

  test('retorna as preferências salvas corretamente', async () => {
    await onboardingService.save(PREFS);
    const result = await onboardingService.load();
    expect(result).toEqual(PREFS);
  });
});

// ── clear ─────────────────────────────────────────────────────────────────────

describe('onboardingService.clear', () => {
  test('remove as preferências do storage', async () => {
    await onboardingService.save(PREFS);
    await onboardingService.clear();
    const result = await onboardingService.load();
    expect(result).toBeNull();
  });
});