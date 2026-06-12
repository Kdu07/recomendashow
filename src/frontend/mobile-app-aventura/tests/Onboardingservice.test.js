/**
 * @file onboardingService.test.js
 * Testes unitários para onboardingService
 *
 * O serviço persiste as preferências no backend (PUT /api/Preferencias/{usuarioId})
 * usando a sessão autenticada. apiClient, tokenStorageService e categoryService
 * são mockados.
 */

import { onboardingService } from '../services/onboardingService';
import { apiFetch } from '../services/apiClient';
import { getAuthSession } from '../services/tokenStorageService';
import { fetchCategories } from '../services/categoryService';
import { CATEGORIES } from '../models/onboardingData';

// ── Mocks ─────────────────────────────────────────────────────────────────────

jest.mock('../services/apiClient', () => ({
  apiFetch: jest.fn(),
}));

jest.mock('../services/tokenStorageService', () => ({
  getAuthSession: jest.fn(),
}));

jest.mock('../services/categoryService', () => ({
  fetchCategories: jest.fn(),
}));

const USUARIO_ID = 'b7e7a7d0-0000-4000-8000-000000000001';
const MUSICAL_ID = CATEGORIES.find((c) => c.label === 'Musical').id;
const DRAMA_ID = CATEGORIES.find((c) => c.label === 'Drama').id;

function mockResponse({ ok = true, status = 200, body = {} } = {}) {
  return {
    ok,
    status,
    json: jest.fn().mockResolvedValue(body),
    text: jest.fn().mockResolvedValue(JSON.stringify(body)),
  };
}

beforeEach(() => {
  jest.clearAllMocks();
  getAuthSession.mockResolvedValue({ usuarioId: USUARIO_ID });
  fetchCategories.mockResolvedValue([
    { id: 'all', label: 'Todos' },
    { id: MUSICAL_ID, label: 'Musical' },
    { id: DRAMA_ID, label: 'Drama' },
  ]);
});

const PREFS = { categories: [MUSICAL_ID], city: 'sao_paulo', frequency: 'todo_mes' };

// ── save ──────────────────────────────────────────────────────────────────────

describe('onboardingService.save', () => {
  test('envia PUT /api/Preferencias/{usuarioId} com gêneros, cidade e tags mapeados', async () => {
    apiFetch.mockResolvedValue(mockResponse({ body: { ok: true } }));

    await onboardingService.save(PREFS);

    expect(apiFetch).toHaveBeenCalledWith(
      `/api/Preferencias/${USUARIO_ID}`,
      expect.objectContaining({ method: 'PUT' })
    );
    const payload = JSON.parse(apiFetch.mock.calls[0][1].body);
    expect(payload).toEqual({
      Generos: ['Musical'],
      Cidade: 'São Paulo',
      Tags: ['todo_mes'],
      ReceberNotificacoes: true,
    });
  });

  test('descarta categorias que não existem no catálogo do backend', async () => {
    apiFetch.mockResolvedValue(mockResponse());
    fetchCategories.mockResolvedValue([{ id: DRAMA_ID, label: 'Drama' }]);

    await onboardingService.save(PREFS);

    const payload = JSON.parse(apiFetch.mock.calls[0][1].body);
    expect(payload.Generos).toEqual([]);
  });

  test('lança erro quando não há sessão de usuário', async () => {
    getAuthSession.mockResolvedValue(null);
    await expect(onboardingService.save(PREFS)).rejects.toThrow(
      'Sessao de usuario nao encontrada.'
    );
    expect(apiFetch).not.toHaveBeenCalled();
  });

  test('lança erro quando o backend responde com falha', async () => {
    apiFetch.mockResolvedValue(mockResponse({ ok: false, status: 500 }));
    await expect(onboardingService.save(PREFS)).rejects.toThrow(
      'Erro ao salvar onboarding: 500'
    );
  });
});

// ── load ──────────────────────────────────────────────────────────────────────

describe('onboardingService.load', () => {
  test('retorna as preferências do backend', async () => {
    const prefs = { Generos: ['Musical'], Cidade: 'São Paulo' };
    apiFetch.mockResolvedValue(mockResponse({ body: prefs }));

    const result = await onboardingService.load();

    expect(apiFetch).toHaveBeenCalledWith(`/api/Preferencias/${USUARIO_ID}`);
    expect(result).toEqual(prefs);
  });

  test('lança erro quando o backend responde com falha', async () => {
    apiFetch.mockResolvedValue(mockResponse({ ok: false, status: 404 }));
    await expect(onboardingService.load()).rejects.toThrow(
      'Erro ao carregar onboarding: 404'
    );
  });
});

// ── clear ─────────────────────────────────────────────────────────────────────

describe('onboardingService.clear', () => {
  test('retorna null (não há mais persistência local a limpar)', async () => {
    await expect(onboardingService.clear()).resolves.toBeNull();
  });
});
