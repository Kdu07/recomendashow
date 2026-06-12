/**
 * @file onboardingService.test.js
 * Testes unitários para onboardingService.
 */

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn(),
}));

jest.mock('../services/categoryService', () => ({
  fetchCategories: jest.fn(),
}));

import { onboardingService } from '../services/onboardingService';
import { apiFetch } from '../services/apiClient';
import { getAuthSession } from '../services/tokenStorageService';
import { fetchCategories } from '../services/categoryService';
import { CATEGORIES } from '../models/onboardingData';

const USER_ID = 'user-123';
const PREFS = {
  categories: [CATEGORIES[0].id],
  city: 'sao_paulo',
  frequency: 'todo_mes',
};

function mockJsonResponse(body) {
  return {
    ok: true,
    json: jest.fn().mockResolvedValue(body),
    text: jest.fn().mockResolvedValue(''),
  };
}

beforeEach(() => {
  jest.clearAllMocks();
  getAuthSession.mockResolvedValue({ usuarioId: USER_ID });
  fetchCategories.mockResolvedValue([
    { id: 'all', label: 'Todos' },
    { id: CATEGORIES[0].id, label: CATEGORIES[0].label },
  ]);
});

describe('onboardingService.save', () => {
  test('envia preferências normalizadas para a API', async () => {
    apiFetch.mockResolvedValue(mockJsonResponse({ usuarioId: USER_ID }));

    await onboardingService.save(PREFS);

    expect(apiFetch).toHaveBeenCalledWith(`/api/Preferencias/${USER_ID}`, {
      method: 'PUT',
      body: JSON.stringify({
        Generos: [CATEGORIES[0].label],
        Cidade: 'São Paulo',
        Tags: ['todo_mes'],
        ReceberNotificacoes: true,
      }),
    });
  });

  test('lança erro quando a API retorna falha', async () => {
    apiFetch.mockResolvedValue({
      ok: false,
      status: 500,
      text: jest.fn().mockResolvedValue('erro'),
    });

    await expect(onboardingService.save(PREFS)).rejects.toThrow('Erro ao salvar onboarding');
  });
});

describe('onboardingService.load', () => {
  test('carrega preferências do usuário autenticado', async () => {
    const responseBody = { Generos: ['Musical'], Cidade: 'São Paulo' };
    apiFetch.mockResolvedValue(mockJsonResponse(responseBody));

    const result = await onboardingService.load();

    expect(apiFetch).toHaveBeenCalledWith(`/api/Preferencias/${USER_ID}`);
    expect(result).toEqual(responseBody);
  });
});

describe('onboardingService.clear', () => {
  test('mantém operação compatível sem persistência local', async () => {
    await expect(onboardingService.clear()).resolves.toBeNull();
  });
});
